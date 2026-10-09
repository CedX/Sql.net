namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void QuerySingle() {
		// It should return the single record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = connection.QuerySingle<Character>(sql, [("FullName", "Saruman")]);
		record.FirstName.ShouldBe("Saruman");
		record.Gender.ShouldBe(CharacterGender.Istari);

		// It should throw an error if the query produces no results.
		Should.Throw<InvalidOperationException>(() => connection.QuerySingle<Character>(sql, [("FullName", "Cédric")]));

		// It should throw an error if the query produces more than one result.
		sql = "SELECT * FROM Characters WHERE gender = @Gender";
		Should.Throw<InvalidOperationException>(() => connection.QuerySingle(sql, [("Gender", nameof(CharacterGender.Human))]));
	}

	[TestMethod]
	public async Task QuerySingleAsync() {
		// It should return the single record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = await connection.QuerySingleAsync<Character>(sql, [("FullName", "Saruman")], testContext.CancellationToken);
		record.FirstName.ShouldBe("Saruman");
		record.Gender.ShouldBe(CharacterGender.Istari);

		// It should throw an error if the query produces no results.
		await Should.ThrowAsync<InvalidOperationException>(() => connection.QuerySingleAsync(sql, [("FullName", "Cédric")], testContext.CancellationToken));

		// It should throw an error if the query produces more than one result.
		sql = "SELECT * FROM Characters WHERE gender = @Gender";
		await Should.ThrowAsync<InvalidOperationException>(() => connection.QuerySingleAsync(sql, [("Gender", nameof(CharacterGender.Human))], testContext.CancellationToken));
	}
}
