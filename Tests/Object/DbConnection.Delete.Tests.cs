namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Delete() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = connection.QuerySingle<Character>(sql, [("Id", 1)]);
		IsTrue(connection.Delete(record));
		IsFalse(connection.Delete(record));
		IsNull(connection.QueryFirstOrDefault<Character>(sql, [("Id", 1)]));
	}

	[TestMethod]
	public async Task DeleteAsync() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = await connection.QuerySingleAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken);
		IsTrue(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsFalse(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsNull(await connection.QueryFirstOrDefaultAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken));
	}

	[TestMethod]
	public void DeleteAll() {
		var sql = "SELECT COUNT(*) FROM Characters";
		IsGreaterThan(0, connection.ExecuteScalar<int>(sql));
		connection.DeleteAll<Character>(truncate: true);
		AreEqual(0, connection.ExecuteScalar<int>(sql));
	}

	[TestMethod]
	public async Task DeleteAllAsync() {
		var sql = "SELECT COUNT(*) FROM Characters";
		IsGreaterThan(0, await connection.ExecuteScalarAsync<int>(sql, cancellationToken: testContext.CancellationToken));
		await connection.DeleteAllAsync<Character>(truncate: true, cancellationToken: testContext.CancellationToken);
		AreEqual(0, await connection.ExecuteScalarAsync<int>(sql, cancellationToken: testContext.CancellationToken));
	}
}
