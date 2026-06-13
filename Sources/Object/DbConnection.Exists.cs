namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Checks whether an entity with the specified primary key exists.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="id">The primary key value.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
		public bool Exists<T>(object id, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() =>
			connection.Exists(typeof(T), id, timeout, transaction, builder);

		/// <summary>
		/// Checks whether an entity with the specified primary key exists.
		/// </summary>
		/// <param name="type">The entity type.</param>
		/// <param name="id">The primary key value.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
		public bool Exists(Type type, object id, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetExistsCommand(type, id);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return connection.ExecuteScalar<bool>(command, parameters);
		}

		/// <summary>
		/// Checks whether an entity with the specified primary key exists.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="id">The primary key value.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
		public async Task<bool> ExistsAsync<T>(object id, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetExistsCommand<T>(id);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return await connection.ExecuteScalarAsync<bool>(command, parameters, cancellationToken);
		}
	}
}
