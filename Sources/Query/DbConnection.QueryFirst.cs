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
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public ExpandoObject QueryFirst(SqlCommand command, SqlParameterCollection? parameters = null) =>
			connection.QueryFirst<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public async Task<ExpandoObject> QueryFirstAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QueryFirstAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row.</returns>
		/// <exception cref="InvalidOperationException">The result set is empty.</exception>
		public T QueryFirst<T>(SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
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
		public async Task<T> QueryFirstAsync<T>(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public ExpandoObject? QueryFirstOrDefault(SqlCommand command, SqlParameterCollection? parameters = null) =>
			connection.QueryFirstOrDefault<ExpandoObject>(command, parameters);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
		public async Task<ExpandoObject?> QueryFirstOrDefaultAsync(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
			await connection.QueryFirstOrDefaultAsync<ExpandoObject>(command, parameters, cancellationToken);

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		public T? QueryFirstOrDefault<T>(SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
			if (connection.State == ConnectionState.Closed) connection.Open();
			using var dbCommand = command.ToDbCommand(connection, parameters);
			using var reader = dbCommand.ExecuteReader();
			return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : default;
		}

		/// <summary>
		/// Executes a parameterized SQL query and returns the first row.
		/// </summary>
		/// <typeparam name="T">The type of objects to return.</typeparam>
		/// <param name="command">The command to be executed.</param>
		/// <param name="parameters">The parameters of the SQL statement.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The first row, or <see langword="null"/> if not found.</returns>
		public async Task<T?> QueryFirstOrDefaultAsync<T>(SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
			if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
			using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
			using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
			return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : default;
		}
	}
}
