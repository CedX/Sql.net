namespace Belin.Sql;

using System.Data.Common;

/// <summary>
/// Tests the features of the <see cref="ObjectDeleteExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class ObjectDeleteExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

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
