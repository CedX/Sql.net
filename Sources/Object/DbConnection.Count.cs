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
		/// <returns>The total number of entities.</returns>
		public int CountAll<T>(int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() =>
			connection.CountAll(typeof(T), timeout, transaction, builder);

		/// <summary>
		/// Counts all entities.
		/// </summary>
		/// <param name="type">The entity type.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns>The total number of entities.</returns>
		public int CountAll(Type type, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetCountAllCommand(type);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return connection.ExecuteScalar<int>(command, parameters);
		}

		/// <summary>
		/// Counts all entities.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The total number of entities.</returns>
		public async Task<int> CountAllAsync<T>(int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetCountAllCommand<T>();
			command.Timeout = timeout;
			command.Transaction = transaction;
			return await connection.ExecuteScalarAsync<int>(command, parameters, cancellationToken);
		}
	}
}
