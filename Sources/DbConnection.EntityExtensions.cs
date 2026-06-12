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
			return Execute(connection, command, parameters) > 0;
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
			return await ExecuteAsync(connection, command, parameters, cancellationToken) > 0;
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
		public void DeleteAll<T>(bool truncate = false, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetDeleteAllCommand<T>(truncate);
			command.Timeout = timeout;
			command.Transaction = transaction;
			Execute(connection, command, parameters);
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
			await ExecuteAsync(connection, command, parameters, cancellationToken);
		}

		/// <summary>
		/// Checks whether an entity with the specified primary key exists.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="id">The primary key value.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
		public bool Exists<T>(object id, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetExistsCommand<T>(id);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return ExecuteScalar<bool>(connection, command, parameters);
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
			return await ExecuteScalarAsync<bool>(connection, command, parameters, cancellationToken);
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
		/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
		public T? Find<T>(object id, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetFindCommand<T>(id, columns ?? []);
			command.Timeout = timeout;
			command.Transaction = transaction;
			return QuerySingleOrDefault<T>(connection, command, parameters);
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
			return await QuerySingleOrDefaultAsync<T>(connection, command, parameters, cancellationToken);
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
			return Query<T>(connection, command, parameters).AsList();
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
			return (await QueryAsync<T>(connection, command, parameters, cancellationToken)).AsList();
		}

		/// <summary>
		/// Inserts the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity to insert.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <returns>The generated primary key value.</returns>
		public long Insert<T>(T entity, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetInsertCommand(entity);
			command.Timeout = timeout;
			command.Transaction = transaction;

			var id = ExecuteScalar<long>(connection, command, parameters);
			if (SqlMapper.Instance.GetTable<T>().IdentityColumn is DbColumnInfo column) column.SetValue(entity, SqlMapper.Instance.ChangeType(id, column));
			return id;
		}

		/// <summary>
		/// Inserts the specified entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity to insert.</param>
		/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
		/// <param name="transaction">The transaction within which the command executes.</param>
		/// <param name="builder">An optional command builder used to build the SQL query to be executed.</param>
		/// <param name="cancellationToken">The token to cancel the operation.</param>
		/// <returns>The generated primary key value.</returns>
		public async Task<long> InsertAsync<T>(T entity, int timeout = 30, IDbTransaction? transaction = null, SqlCommandBuilder? builder = null, CancellationToken cancellationToken = default) where T: new() {
			var (command, parameters) = (builder ?? SqlCommandBuilder.Create(connection)).GetInsertCommand(entity);
			command.Timeout = timeout;
			command.Transaction = transaction;

			var id = await ExecuteScalarAsync<long>(connection, command, parameters, cancellationToken);
			if (SqlMapper.Instance.GetTable<T>().IdentityColumn is DbColumnInfo column) column.SetValue(entity, SqlMapper.Instance.ChangeType(id, column));
			return id;
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
