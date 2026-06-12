namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity to update.</param>
		/// <param name="columns">The list of columns to update. By default, all columns.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns>The number of rows affected.</returns>
		public int Update<T>(T entity, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetUpdateCommand(entity, columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return Execute(connection, command, parameters);
		}

		/// <summary>
		/// Updates the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity to update.</param>
		/// <param name="columns">The list of columns to update. By default, all columns.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The number of rows affected.</returns>
		public async Task<int> UpdateAsync<T>(T entity, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetUpdateCommand(entity, columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return await ExecuteAsync(connection, command, parameters, cancellationToken);
		}
	}
}
