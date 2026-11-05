namespace Belin.Sql;

using System.Data.Common;

/// <summary>
/// Tests the features of the <see cref="ObjectUpdateExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class ObjectUpdateExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

	[TestMethod]
	public void Update() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = connection.QuerySingle<Character>(sql);
		sauron.FullName.ShouldBe("Sauron");
		sauron.Gender.ShouldBe(CharacterGender.DarkLord);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		ObjectUpdateExtensions.Update(connection, sauron).ShouldBe(1); // SQLite already defines an `Update` method.

		sauron = connection.QuerySingle<Character>(sql);
		sauron.FullName.ShouldBe("Sauron The big bad guy");
		sauron.Gender.ShouldBe(CharacterGender.Istari);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = connection.QuerySingle<Character>(sql);
		saruman.FullName.ShouldBe("Saruman");
		saruman.Gender.ShouldBe(CharacterGender.Istari);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		ObjectUpdateExtensions.Update(connection, saruman, ["gender"]).ShouldBe(1); // SQLite already defines an `Update` method.

		saruman = connection.QuerySingle<Character>(sql);
		saruman.FullName.ShouldBe("Saruman");
		saruman.Gender.ShouldBe(CharacterGender.DarkLord);
	}

	[TestMethod]
	public async Task UpdateAsync() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		sauron.FullName.ShouldBe("Sauron");
		sauron.Gender.ShouldBe(CharacterGender.DarkLord);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		(await connection.UpdateAsync(sauron, cancellationToken: testContext.CancellationToken)).ShouldBe(1);

		sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		sauron.FullName.ShouldBe("Sauron The big bad guy");
		sauron.Gender.ShouldBe(CharacterGender.Istari);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		saruman.FullName.ShouldBe("Saruman");
		saruman.Gender.ShouldBe(CharacterGender.Istari);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		(await connection.UpdateAsync(saruman, ["gender"], cancellationToken: testContext.CancellationToken)).ShouldBe(1);

		saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		saruman.FullName.ShouldBe("Saruman");
		saruman.Gender.ShouldBe(CharacterGender.DarkLord);
	}
}
