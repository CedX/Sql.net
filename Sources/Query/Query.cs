namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {

	extension(IDbConnection connection) {
		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="type">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public List<object> Query(Type type, Command command, ParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return [.. Mapper.Instance.CreateInstances(type, reader)];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public List<T> Query<T>(Command command, ParameterCollection? parameters = null) =>
			[.. connection.Query(typeof(T), command, parameters).Cast<T>()];

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{TKey, TValue}"/>.</remarks>
		public List<ExpandoObject> Query(Command command, ParameterCollection? parameters = null) =>
			connection.Query<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="types">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The fields from which to split and read the next objects.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public List<ITuple> Query(Type[] types, Command command, ParameterCollection? parameters = null, string[]? splitOn = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return [.. Mapper.Instance.CreateInstances(types, reader, splitOn ?? [])];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="TItem1">The type of the first objects.</typeparam>
		/// <typeparam name="TItem2">The type of the second objects.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The field from which to split and read the next object.</param>
		/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
		public List<(TItem1, TItem2)> Query<TItem1, TItem2>(Command command, ParameterCollection? parameters = null, string splitOn = "Id") {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return [.. Mapper.Instance.CreateInstances<TItem1, TItem2>(reader, splitOn)];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="TItem1">The type of the first objects.</typeparam>
		/// <typeparam name="TItem2">The type of the second objects.</typeparam>
		/// <typeparam name="TItem3">The type of the third objects.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The fields from which to split and read the next objects.</param>
		/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
		public List<(TItem1, TItem2, TItem3)> Query<TItem1, TItem2, TItem3>(Command command, ParameterCollection? parameters = null, (string, string)? splitOn = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return [.. Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(reader, splitOn)];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="TItem1">The type of the first objects.</typeparam>
		/// <typeparam name="TItem2">The type of the second objects.</typeparam>
		/// <typeparam name="TItem3">The type of the third objects.</typeparam>
		/// <typeparam name="TItem4">The type of the fourth objects.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The fields from which to split and read the next objects.</param>
		/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
		public List<(TItem1, TItem2, TItem3, TItem4)> Query<TItem1, TItem2, TItem3, TItem4>(Command command, ParameterCollection? parameters = null, (string, string, string)? splitOn = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return [.. Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(reader, splitOn)];
		}
	}

	extension(DbConnection connection) {
		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public async Task<List<T>> QueryAsync<T>(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return [.. Mapper.Instance.CreateInstances<T>(reader)];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{TKey, TValue}"/>.</remarks>
		public async Task<List<ExpandoObject>> QueryAsync(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QueryAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="TItem1">The type of the first objects.</typeparam>
		/// <typeparam name="TItem2">The type of the second objects.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The field from which to split and read the next object.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
		public async Task<List<(TItem1, TItem2)>> QueryAsync<TItem1, TItem2>(Command command, ParameterCollection? parameters = null, string splitOn = "Id", CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return [.. Mapper.Instance.CreateInstances<TItem1, TItem2>(reader, splitOn)];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="TItem1">The type of the first objects.</typeparam>
		/// <typeparam name="TItem2">The type of the second objects.</typeparam>
		/// <typeparam name="TItem3">The type of the third objects.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The fields from which to split and read the next objects.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
		public async Task<List<(TItem1, TItem2, TItem3)>> QueryAsync<TItem1, TItem2, TItem3>(Command command, ParameterCollection? parameters = null, (string, string)? splitOn = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return [.. Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(reader, splitOn)];
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="TItem1">The type of the first objects.</typeparam>
		/// <typeparam name="TItem2">The type of the second objects.</typeparam>
		/// <typeparam name="TItem3">The type of the third objects.</typeparam>
		/// <typeparam name="TItem4">The type of the fourth objects.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="splitOn">The fields from which to split and read the next objects.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
		public async Task<List<(TItem1, TItem2, TItem3, TItem4)>> QueryAsync<TItem1, TItem2, TItem3, TItem4>(Command command, ParameterCollection? parameters = null, (string, string, string)? splitOn = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return [.. Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(reader, splitOn)];
		}
	}
}
