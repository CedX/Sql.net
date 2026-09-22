namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Delete() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = connection.QuerySingle<Character>(sql, [("Id", 1)]);
		Assert.IsTrue(connection.Delete(record));
		Assert.IsFalse(connection.Delete(record));
		Assert.IsNull(connection.QueryFirstOrDefault<Character>(sql, [("Id", 1)]));
	}

	[TestMethod]
	public async Task DeleteAsync() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = await connection.QuerySingleAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken);
		Assert.IsTrue(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		Assert.IsFalse(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		Assert.IsNull(await connection.QueryFirstOrDefaultAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken));
	}

	[TestMethod]
	public void DeleteAll() {
		var sql = "SELECT COUNT(*) FROM Characters";
		Assert.IsGreaterThan(0, connection.ExecuteScalar<int>(sql));
		connection.DeleteAll<Character>(truncate: true);
		Assert.AreEqual(0, connection.ExecuteScalar<int>(sql));
	}

	[TestMethod]
	public async Task DeleteAllAsync() {
		var sql = "SELECT COUNT(*) FROM Characters";
		Assert.IsGreaterThan(0, await connection.ExecuteScalarAsync<int>(sql, cancellationToken: testContext.CancellationToken));
		await connection.DeleteAllAsync<Character>(truncate: true, cancellationToken: testContext.CancellationToken);
		Assert.AreEqual(0, await connection.ExecuteScalarAsync<int>(sql, cancellationToken: testContext.CancellationToken));
	}
}
