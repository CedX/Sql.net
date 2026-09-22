namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void ExecuteScalar() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE gender = @Gender";
		Assert.AreEqual(2, connection.ExecuteScalar<int>(sql, [("Gender", nameof(CharacterGender.Balrog))]));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		Assert.AreEqual("Characters", connection.ExecuteScalar<string>(sql, [("Name", "Characters"), ("Type", "table")]));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE name = @Name";
		Assert.IsNull(connection.ExecuteScalar<string>(sql, [("Name", "FooBarBazQux")]));
	}

	[TestMethod]
	public async Task ExecuteScalarAsync() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE gender = @Gender";
		var parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Balrog)));
		Assert.AreEqual(2, await connection.ExecuteScalarAsync<int>(sql, parameters, testContext.CancellationToken));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		parameters = new SqlParameterCollection(("Name", "Characters"), ("Type", "table"));
		Assert.AreEqual("Characters", await connection.ExecuteScalarAsync<string>(sql, parameters, testContext.CancellationToken));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE name = @Name";
		parameters = new SqlParameterCollection(("Name", "FooBarBazQux"));
		Assert.IsNull(await connection.ExecuteScalarAsync<string>(sql, parameters, testContext.CancellationToken));
	}
}
