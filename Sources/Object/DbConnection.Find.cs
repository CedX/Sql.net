namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

		/// <summary>
		/// Finds an entity with the specified primary key.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="id">The primary key value.</param>
		/// <param name="columns">The list of columns to select. By default, all columns.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
		public T? Find<T>(object id, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetFindCommand<T>(id, columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return connection.QuerySingleOrDefault<T>(command, parameters);
		}

		/// <summary>
		/// Finds an entity with the specified primary key.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="id">The primary key value.</param>
		/// <param name="columns">The list of columns to select. By default, all columns.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
		public async Task<T?> FindAsync<T>(object id, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetFindCommand<T>(id, columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return await connection.QuerySingleOrDefaultAsync<T>(command, parameters, cancellationToken);
		}

		/// <summary>
		/// Finds all entities of the specified type.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="orderHints">The hints describing the sort order of columns.</param>
		/// <param name="columns">The list of columns to select. By default, all columns.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns>The list of all entities of the specified type.</returns>
		public IList<T> FindAll<T>(SqlOrderHintCollection? orderHints = null, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetFindAllCommand<T>(orderHints ?? new(), columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return connection.Query<T>(command, parameters).AsList();
		}

		/// <summary>
		/// Finds all entities of the specified type.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="orderHints">The hints describing the sort order of columns.</param>
		/// <param name="columns">The list of columns to select. By default, all columns.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The list of all entities of the specified type.</returns>
		public async Task<IList<T>> FindAllAsync<T>(SqlOrderHintCollection? orderHints = null, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetFindAllCommand<T>(orderHints ?? new(), columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return (await connection.QueryAsync<T>(command, parameters, cancellationToken)).AsList();
		}
	}
}
