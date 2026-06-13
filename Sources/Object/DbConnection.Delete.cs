namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity to delete.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public bool Delete<T>(T entity, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetDeleteCommand(entity);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return connection.Execute(command, parameters) > 0;
		}

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity to delete.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public async Task<bool> DeleteAsync<T>(T entity, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetDeleteCommand(entity);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return await connection.ExecuteAsync(command, parameters, cancellationToken) > 0;
		}

		/// <summary>
		/// Deletes all entities.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="truncate">Value indicating whether to truncate the underlying table.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public void DeleteAll<T>(bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() =>
			connection.DeleteAll(typeof(T), truncate, timeout, transaction, builder);

		/// <summary>
		/// Deletes all entities.
		/// </summary>
		/// <param name="type">The entity type.</type>
		/// <param name="truncate">Value indicating whether to truncate the underlying table.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public void DeleteAll(Type type, bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetDeleteAllCommand(type, truncate);
			command.Timeout = timeout;
			command.Transaction = transaction;
			connection.Execute(command, parameters);
		}

		/// <summary>
		/// Deletes all entities.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="truncate">Value indicating whether to truncate the underlying table.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public async Task DeleteAllAsync<T>(bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetDeleteAllCommand<T>(truncate);
			command.Timeout = timeout;
			command.Transaction = transaction;
			await connection.ExecuteAsync(command, parameters, cancellationToken);
		}
	}
}
