namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static class QueryExecuteScalarExtensions {

	extension(IDbConnection connection) {
		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <param name="type">The type of object to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first column of the first row.</returns>
		public object? ExecuteScalar(Type type, Command command, ParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			return Mapper.Instance.ChangeType(dbCommand.ExecuteScalar(), type, isNullable: true);
		}

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first column of the first row.</returns>
		public T? ExecuteScalar<T>(Command command, ParameterCollection? parameters = null) =>
			(T?) connection.ExecuteScalar(typeof(T), command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first column of the first row.</returns>
		public object? ExecuteScalar(Command command, ParameterCollection? parameters = null) =>
			connection.ExecuteScalar<object>(command, parameters);
	}

	extension(DbConnection connection) {
		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first column of the first row.</returns>
		public async Task<T?> ExecuteScalarAsync<T>(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			return (T?) Mapper.Instance.ChangeType(await dbCommand.ExecuteScalarAsync(cancellationToken), typeof(T), isNullable: true);
		}

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first column of the first row.</returns>
		public async Task<object?> ExecuteScalarAsync(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.ExecuteScalarAsync<object>(command, parameters, cancellationToken);
	}
}
