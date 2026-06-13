namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public IEnumerable<ExpandoObject> Query(SqlCommand command, SqlParameterCollection? parameters = null) =>
			connection.Query<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public async Task<IEnumerable<ExpandoObject>> QueryAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QueryAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public IEnumerable<T> Query<T>(SqlCommand command, SqlParameterCollection? parameters = null) where T: new() =>
			connection.Query(typeof(T), command, parameters).Cast<T>();

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <param name="type">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public IEnumerable<object> Query(Type type, SqlCommand command, SqlParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			var records = SqlMapper.Instance.CreateInstances(type, reader);
			return command.NoEnumerate ? records : records.AsList();
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
		public async Task<IEnumerable<T>> QueryAsync<T>(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			var records = SqlMapper.Instance.CreateInstances<T>(reader);
			return command.NoEnumerate ? records : records.AsList();
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
		public IEnumerable<(TItem1, TItem2)> Query<TItem1, TItem2>(SqlCommand command, SqlParameterCollection? parameters = null, string splitOn = "Id") where TItem1: new() where TItem2: new() {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2>(reader, splitOn);
			return command.NoEnumerate ? records : records.AsList();
		}

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
		public async Task<IEnumerable<(TItem1, TItem2)>> QueryAsync<TItem1, TItem2>(SqlCommand command, SqlParameterCollection? parameters = null, string splitOn = "Id", CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2>(reader, splitOn);
			return command.NoEnumerate ? records : records.AsList();
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
		public IEnumerable<(TItem1, TItem2, TItem3)> Query<TItem1, TItem2, TItem3>(SqlCommand command, SqlParameterCollection? parameters = null, (string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(reader, splitOn);
			return command.NoEnumerate ? records : records.AsList();
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
		public async Task<IEnumerable<(TItem1, TItem2, TItem3)>> QueryAsync<TItem1, TItem2, TItem3>(SqlCommand command, SqlParameterCollection? parameters = null, (string, string)? splitOn = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() where TItem3: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(reader, splitOn);
			return command.NoEnumerate ? records : records.AsList();
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
		public IEnumerable<(TItem1, TItem2, TItem3, TItem4)> Query<TItem1, TItem2, TItem3, TItem4>(SqlCommand command, SqlParameterCollection? parameters = null, (string, string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(reader, splitOn);
			return command.NoEnumerate ? records : records.AsList();
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
		public async Task<IEnumerable<(TItem1, TItem2, TItem3, TItem4)>> QueryAsync<TItem1, TItem2, TItem3, TItem4>(SqlCommand command, SqlParameterCollection? parameters = null, (string, string, string)? splitOn = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(reader, splitOn);
			return command.NoEnumerate ? records : records.AsList();
		}
	}
}
