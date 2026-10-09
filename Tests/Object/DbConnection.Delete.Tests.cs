namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Delete() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = connection.QuerySingle<Character>(sql, [("Id", 1)]);
		connection.Delete(record).ShouldBeTrue();
		connection.Delete(record).ShouldBeFalse();
		connection.QueryFirstOrDefault<Character>(sql, [("Id", 1)]).ShouldBeNull();
	}

	[TestMethod]
	public async Task DeleteAsync() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = await connection.QuerySingleAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken);
		(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken)).ShouldBeTrue();
		(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken)).ShouldBeFalse();
		(await connection.QueryFirstOrDefaultAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken)).ShouldBeNull();
	}

	[TestMethod]
	public void DeleteAll() {
		var sql = "SELECT COUNT(*) FROM Characters";
		connection.ExecuteScalar<int>(sql).ShouldBeGreaterThan(0);
		connection.DeleteAll<Character>(truncate: true);
		connection.ExecuteScalar<int>(sql).ShouldBe(0);
	}

	[TestMethod]
	public async Task DeleteAllAsync() {
		var sql = "SELECT COUNT(*) FROM Characters";
		(await connection.ExecuteScalarAsync<int>(sql, cancellationToken: testContext.CancellationToken)).ShouldBeGreaterThan(0);
		await connection.DeleteAllAsync<Character>(truncate: true, cancellationToken: testContext.CancellationToken);
		(await connection.ExecuteScalarAsync<int>(sql, cancellationToken: testContext.CancellationToken)).ShouldBe(0);
	}
}
