namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Counts all entities.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public int CountAll<T>(int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetCountAllCommand<T>();
			command.Timeout = timeout;
			command.Transaction = transaction;
			return ExecuteScalar<int>(connection, command, parameters);
		}

		/// <summary>
		/// Counts all entities.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
		public async Task<int> CountAllAsync<T>(int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetCountAllCommand<T>();
			command.Timeout = timeout;
			command.Transaction = transaction;
			return await ExecuteScalarAsync<int>(connection, command, parameters, cancellationToken);
		}
	}
}
