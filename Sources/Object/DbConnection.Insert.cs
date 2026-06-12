namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {
	extension(IDbConnection connection) {

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

			var id = connection.ExecuteScalar<long>(command, parameters);
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

			var id = await connection.ExecuteScalarAsync<long>(command, parameters, cancellationToken);
			if (SqlMapper.Instance.GetTable<T>().IdentityColumn is DbColumnInfo column) column.SetValue(entity, SqlMapper.Instance.ChangeType(id, column));
			return id;
		}
	}
}
