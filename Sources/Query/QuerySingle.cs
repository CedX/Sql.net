namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static class QuerySingleExtensions {

	extension(IDbConnection connection) {
		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <param name="type">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		public object QuerySingle(Type type, Command command, ParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();

			object? record = null;
			var rowCount = 0;
			while (reader.Read()) {
				if (++rowCount > 1) break;
				record = Mapper.Instance.CreateInstance(type, reader);
			}

			return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		public T QuerySingle<T>(Command command, ParameterCollection? parameters = null) =>
			(T) connection.QuerySingle(typeof(T), command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <param name="type">The type of objects to return.</param>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		public object? QuerySingleOrDefault(Type type, Command command, ParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();

			object? record = null;
			var rowCount = 0;
			while (reader.Read()) {
				if (++rowCount > 1) break;
				record = Mapper.Instance.CreateInstance(type, reader);
			}

			return rowCount == 1 ? record : null;
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		public T? QuerySingleOrDefault<T>(Command command, ParameterCollection? parameters = null) =>
			(T?) connection.QuerySingleOrDefault(typeof(T), command, parameters);
	}

	extension(DbConnection connection) {
		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		public async Task<T> QuerySingleAsync<T>(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);

			T? record = default;
			var rowCount = 0;
			while (await reader.ReadAsync(cancellationToken)) {
				if (++rowCount > 1) break;
				record = Mapper.Instance.CreateInstance<T>(reader);
			}

			return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		public async Task<T?> QuerySingleOrDefaultAsync<T>(Command command, ParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
			if (connection.State == ConnectionState.Closed) await connection.OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			await using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);

			T? record = default;
			var rowCount = 0;
			while (await reader.ReadAsync(cancellationToken)) {
				if (++rowCount > 1) break;
				record = Mapper.Instance.CreateInstance<T>(reader);
			}

			return rowCount == 1 ? record : default;
		}
	}
}
