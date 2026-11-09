namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static class ObjectDeleteExtensions {

	extension(IDbConnection connection) {
		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public bool Delete<T>(T entity, int timeout = 30, IDbTransaction? transaction = null, CommandBuilder? builder = null) where T: notnull {
			var (command, parameters) = (builder ?? CommandBuilder.Create(connection)).GetDeleteCommand(entity);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return connection.Execute(command, parameters) > 0;
		}

		/// <summary>
		/// Deletes all entities.
		/// </summary>
		/// <param name="type">The entity type.</param>
		/// <param name="truncate">Value indicating whether to truncate the underlying table.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		public void DeleteAll(Type type, bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, CommandBuilder? builder = null) {
			var (command, parameters) = (builder ?? CommandBuilder.Create(connection)).GetDeleteAllCommand(type, truncate);
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
		public void DeleteAll<T>(bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, CommandBuilder? builder = null) =>
			connection.DeleteAll(typeof(T), truncate, timeout, transaction, builder);
	}

	extension(DbConnection connection) {
		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public async Task<bool> DeleteAsync<T>(T entity, int timeout = 30, IDbTransaction? transaction = null, CommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: notnull {
			var (command, parameters) = (builder ?? CommandBuilder.Create(connection)).GetDeleteCommand(entity);
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
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public async Task DeleteAllAsync<T>(bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, CommandBuilder? builder = null, CancellationToken cancellationToken = default) {
			var (command, parameters) = (builder ?? CommandBuilder.Create(connection)).GetDeleteAllCommand<T>(truncate);
			command.Timeout = timeout;
			command.Transaction = transaction;
			await connection.ExecuteAsync(command, parameters, cancellationToken);
		}
	}
}
