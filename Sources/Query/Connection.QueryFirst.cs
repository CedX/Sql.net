namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{TKey, TValue}"/>.</remarks>
		public ExpandoObject QueryFirst(Command command, ParameterCollection? parameters = null) =>
			connection.QueryFirst<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{TKey, TValue}"/>.</remarks>
		public async Task<ExpandoObject> QueryFirstAsync(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QueryFirstAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		public T QueryFirst<T>(Command command, ParameterCollection? parameters = null) where T: new() =>
			(T) connection.QueryFirst(typeof(T), command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="type">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		public object QueryFirst(Type type, Command command, ParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return reader.Read() ? Mapper.Instance.CreateInstance(type, reader) : throw new InvalidOperationException("The result set is empty.");
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		public async Task<T> QueryFirstAsync<T>(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return await reader.ReadAsync(cancellationToken) ? Mapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{TKey, TValue}"/>.</remarks>
		public ExpandoObject? QueryFirstOrDefault(Command command, ParameterCollection? parameters = null) =>
			connection.QueryFirstOrDefault<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{TKey, TValue}"/>.</remarks>
		public async Task<ExpandoObject?> QueryFirstOrDefaultAsync(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QueryFirstOrDefaultAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		public T? QueryFirstOrDefault<T>(Command command, ParameterCollection? parameters = null) where T: new() =>
			(T?) connection.QueryFirstOrDefault(typeof(T), command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="type">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		public object? QueryFirstOrDefault(Type type, Command command, ParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return reader.Read() ? Mapper.Instance.CreateInstance(type, reader) : default;
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		public async Task<T?> QueryFirstOrDefaultAsync<T>(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return await reader.ReadAsync(cancellationToken) ? Mapper.Instance.CreateInstance<T>(reader) : default;
		}
	}
}
