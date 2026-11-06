namespace Belin.Sql;

using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Tests the features of the <see cref="QueryFirstExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class QueryFirstExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

	[TestMethod]
	public void QueryFirst() {
		// It should return the first record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = connection.QueryFirst<Character>(sql, [("FullName", "Sauron")]);
		record.FirstName.ShouldBe("Sauron");
		record.Gender.ShouldBe(CharacterGender.DarkLord);

		// It should throw an error if the query produces no results.
		Should.Throw<InvalidOperationException>(() => connection.QueryFirst<ExpandoObject>(sql, [("FullName", "Cédric")]));
	}

	[TestMethod]
	public async Task QueryFirstAsync() {
		// It should return the first record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = await connection.QueryFirstAsync<Character>(sql, [("FullName", "Sauron")], testContext.CancellationToken);
		record.FirstName.ShouldBe("Sauron");
		record.Gender.ShouldBe(CharacterGender.DarkLord);

		// It should throw an error if the query produces no results.
		await Should.ThrowAsync<InvalidOperationException>(() => connection.QueryFirstAsync<ExpandoObject>(sql, [("FullName", "Cédric")], testContext.CancellationToken));
	}
}
