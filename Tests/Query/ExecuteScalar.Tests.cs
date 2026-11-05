namespace Belin.Sql;

using System.Data.Common;

/// <summary>
/// Tests the features of the <see cref="QueryExecuteScalarExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class QueryExecuteScalarExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

	[TestMethod]
	public void ExecuteScalar() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE gender = @Gender";
		connection.ExecuteScalar<int>(sql, [("Gender", nameof(CharacterGender.Balrog))]).ShouldBe(2);

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		connection.ExecuteScalar<string>(sql, [("Name", "Characters"), ("Type", "table")]).ShouldBe("Characters");

		sql = "SELECT tbl_name FROM sqlite_schema WHERE name = @Name";
		connection.ExecuteScalar<string>(sql, [("Name", "FooBarBazQux")]).ShouldBeNull();
	}

	[TestMethod]
	public async Task ExecuteScalarAsync() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE gender = @Gender";
		var parameters = new ParameterCollection(("Gender", nameof(CharacterGender.Balrog)));
		(await connection.ExecuteScalarAsync<int>(sql, parameters, testContext.CancellationToken)).ShouldBe(2);

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		parameters = new ParameterCollection(("Name", "Characters"), ("Type", "table"));
		(await connection.ExecuteScalarAsync<string>(sql, parameters, testContext.CancellationToken)).ShouldBe("Characters");

		sql = "SELECT tbl_name FROM sqlite_schema WHERE name = @Name";
		parameters = new ParameterCollection(("Name", "FooBarBazQux"));
		(await connection.ExecuteScalarAsync<string>(sql, parameters, testContext.CancellationToken)).ShouldBeNull();
	}
}
