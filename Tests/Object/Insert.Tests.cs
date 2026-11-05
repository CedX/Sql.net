namespace Belin.Sql;

using System.Data.Common;

/// <summary>
/// Tests the features of the <see cref="ObjectInsertExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class ObjectInsertExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

	[TestMethod]
	public void Insert() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		connection.Query<Character>(sql).ShouldBeEmpty();

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		record.Id.ShouldBe(0);
		record.FullName.ShouldBeNull();

		var id = (int) connection.Insert(record);
		id.ShouldBeGreaterThan(16);
		record.Id.ShouldBe(id);

		var records = connection.Query<Character>(sql);
		records.Count.ShouldBe(1);

		var cedric = records[0];
		cedric.Id.ShouldBe(id);
		cedric.FullName.ShouldBe("Cédric Belin");
		cedric.Gender.ShouldBe(record.Gender);
	}

	[TestMethod]
	public async Task InsertAsync() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		(await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken)).ShouldBeEmpty();

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		record.Id.ShouldBe(0);
		record.FullName.ShouldBeNull();

		var id = (int) await connection.InsertAsync(record, cancellationToken: testContext.CancellationToken);
		id.ShouldBeGreaterThan(16);
		record.Id.ShouldBe(id);

		var records = await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		records.Count.ShouldBe(1);

		var cedric = records[0];
		cedric.Id.ShouldBe(id);
		cedric.FullName.ShouldBe("Cédric Belin");
		cedric.Gender.ShouldBe(record.Gender);
	}
}
