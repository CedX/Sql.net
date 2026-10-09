namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void QueryFirst() {
		// It should return the first record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = connection.QueryFirst<Character>(sql, [("FullName", "Sauron")]);
		record.FirstName.ShouldBe("Sauron");
		record.Gender.ShouldBe(CharacterGender.DarkLord);

		// It should throw an error if the query produces no results.
		Should.Throw<InvalidOperationException>(() => connection.QueryFirst(sql, [("FullName", "Cédric")]));
	}

	[TestMethod]
	public async Task QueryFirstAsync() {
		// It should return the first record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = await connection.QueryFirstAsync<Character>(sql, [("FullName", "Sauron")], testContext.CancellationToken);
		record.FirstName.ShouldBe("Sauron");
		record.Gender.ShouldBe(CharacterGender.DarkLord);

		// It should throw an error if the query produces no results.
		await Should.ThrowAsync<InvalidOperationException>(() => connection.QueryFirstAsync(sql, [("FullName", "Cédric")], testContext.CancellationToken));
	}
}
