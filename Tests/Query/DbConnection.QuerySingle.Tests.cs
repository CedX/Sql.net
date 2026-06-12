namespace Belin.Sql;

using System.Dynamic;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void QuerySingle() {
		// It should return the single record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = connection.QuerySingle<Character>(sql, [("FullName", "Saruman")]);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);

		// It should throw an error if the query produces no results.
		Throws<InvalidOperationException>(() => connection.QuerySingle<Character>(sql, [("FullName", "Cédric")]));

		// It should throw an error if the query produces more than one result.
		sql = "SELECT * FROM Characters WHERE gender = @Gender";
		Throws<InvalidOperationException>(() => connection.QuerySingle(sql, [("Gender", CharacterGender.Human.ToString())]));
	}

	[TestMethod]
	public async Task QuerySingleAsync() {
		// It should return the single record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = await connection.QuerySingleAsync<Character>(sql, [("FullName", "Saruman")], testContext.CancellationToken);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);

		// It should throw an error if the query produces no results.
		await ThrowsAsync<InvalidOperationException>(() => connection.QuerySingleAsync(sql, [("FullName", "Cédric")], testContext.CancellationToken));

		// It should throw an error if the query produces more than one result.
		sql = "SELECT * FROM Characters WHERE gender = @Gender";
		await ThrowsAsync<InvalidOperationException>(() => connection.QuerySingleAsync(sql, [("Gender", CharacterGender.Human.ToString())], testContext.CancellationToken));
	}
}
