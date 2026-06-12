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
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public ExpandoObject QuerySingle(SqlCommand command, SqlParameterCollection? parameters = null) =>
			connection.QuerySingle<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public async Task<ExpandoObject> QuerySingleAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QuerySingleAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		public T QuerySingle<T>(SqlCommand command, SqlParameterCollection? parameters = null) where T: new() =>
			(T) connection.QuerySingle(typeof(T), command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		public object QuerySingle(Type type, SqlCommand command, SqlParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();

			object? record = null;
			var rowCount = 0;
			while (reader.Read()) {
				if (++rowCount > 1) break;
				record = SqlMapper.Instance.CreateInstance(type, reader);
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
		/// <returns>The single row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
		public async Task<T> QuerySingleAsync<T>(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);

			T? record = default;
			var rowCount = 0;
			while (reader.Read()) {
				if (++rowCount > 1) break;
				record = SqlMapper.Instance.CreateInstance<T>(reader);
			}

			return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public ExpandoObject? QuerySingleOrDefault(SqlCommand command, SqlParameterCollection? parameters = null) =>
			connection.QuerySingleOrDefault<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public async Task<ExpandoObject?> QuerySingleOrDefaultAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QuerySingleOrDefaultAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		public T? QuerySingleOrDefault<T>(SqlCommand command, SqlParameterCollection? parameters = null) where T: new() =>
			(T?) connection.QuerySingleOrDefault(typeof(T), command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		public object? QuerySingleOrDefault(Type type, SqlCommand command, SqlParameterCollection? parameters = null) {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();

			object? record = null;
			var rowCount = 0;
			while (reader.Read()) {
				if (++rowCount > 1) break;
				record = SqlMapper.Instance.CreateInstance(type, reader);
			}

			return rowCount == 1 ? record : null;
		}


		/// <summary>
		/// Executes a parameterized SQL query and returns the single row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The single row, or <see langword="null"/> if not found.</returns>
		public async Task<T?> QuerySingleOrDefaultAsync<T>(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);

			T? record = default;
			var rowCount = 0;
			while (reader.Read()) {
				if (++rowCount > 1) break;
				record = SqlMapper.Instance.CreateInstance<T>(reader);
			}

			return rowCount == 1 ? record : default;
		}
	}
}
