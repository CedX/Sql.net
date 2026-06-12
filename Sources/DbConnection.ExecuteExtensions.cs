namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Executes a parameterized SQL statement.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The number of rows affected.</returns>
		public int Execute(SqlCommand command, SqlParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			return dbCommand.ExecuteNonQuery();
		}

		/// <summary>
		/// Executes a parameterized SQL statement.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The number of rows affected.</returns>
		public async Task<int> ExecuteAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			return await dbCommand.ExecuteNonQueryAsync(cancellationToken);
		}

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first column of the first row.</returns>
		public object? ExecuteScalar(SqlCommand command, SqlParameterCollection? parameters = null) =>
			ExecuteScalar<object>(connection, command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first column of the first row.</returns>
		public async Task<object?> ExecuteScalarAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await ExecuteScalarAsync<object>(connection, command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first column of the first row.</returns>
		public T? ExecuteScalar<T>(SqlCommand command, SqlParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			var value = dbCommand.ExecuteScalar();
			return value is null || value is DBNull ? default : (T?) SqlMapper.Instance.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Executes a parameterized SQL query that selects a single value.
		/// </summary>
		/// <typeparam name="T">The type of object to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first column of the first row.</returns>
		public async Task<T?> ExecuteScalarAsync<T>(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			var value = await dbCommand.ExecuteScalarAsync(cancellationToken);
			return value is null || value is DBNull ? default : (T?) SqlMapper.Instance.ChangeType(value, typeof(T));
		}
	}
}
