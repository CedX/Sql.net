namespace Belin.Sql;

using System.Dynamic;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Query() {
		// It should return the records produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
		var records = connection.Query<Character>(sql, [("Gender", nameof(CharacterGender.Elf))]);
		records.Count.ShouldBe(3);

		var elrond = records[0];
		elrond.FullName.ShouldBe("Elrond");
		elrond.Gender.ShouldBe(CharacterGender.Elf);

		var galadriel = records[1];
		galadriel.FullName.ShouldBe("Galadriel");
		galadriel.Gender.ShouldBe(CharacterGender.Elf);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = connection.Query<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")]);
		objects.Count.ShouldBe(1);

		var item1 = new Dictionary<string, object?>(objects[0].Item1);
		item1.ShouldContainKeyAndValue("ID", 6L);
		item1.ShouldContainKeyAndValue("firstName", "Frodo");
		item1.ShouldContainKeyAndValue("lastName", "Baggins");
		item1.ShouldNotContainKey("fullName");

		var item2 = new Dictionary<string, object?>(objects[0].Item2);
		item2.ShouldContainKeyAndValue("ID", 6L);
		item2.ShouldContainKeyAndValue("fullName", "Frodo Baggins");
		item2.ShouldContainKeyAndValue("gender", "Hobbit");
		item2.ShouldNotContainKey("firstName");
	}

	[TestMethod]
	public async Task QueryAsync() {
		// It should return the records produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
		var parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Elf)));
		var records = await connection.QueryAsync<Character>(sql, parameters, testContext.CancellationToken);
		records.Count.ShouldBe(3);

		var elrond = records[0];
		elrond.FullName.ShouldBe("Elrond");
		elrond.Gender.ShouldBe(CharacterGender.Elf);

		var galadriel = records[1];
		galadriel.FullName.ShouldBe("Galadriel");
		galadriel.Gender.ShouldBe(CharacterGender.Elf);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = await connection.QueryAsync<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")], "id", testContext.CancellationToken);
		objects.Count.ShouldBe(1);

		var item1 = new Dictionary<string, object?>(objects[0].Item1);
		item1.ShouldContainKeyAndValue("ID", 6L);
		item1.ShouldContainKeyAndValue("firstName", "Frodo");
		item1.ShouldContainKeyAndValue("lastName", "Baggins");
		item1.ShouldNotContainKey("fullName");

		var item2 = new Dictionary<string, object?>(objects[0].Item2);
		item2.ShouldContainKeyAndValue("ID", 6L);
		item2.ShouldContainKeyAndValue("fullName", "Frodo Baggins");
		item2.ShouldContainKeyAndValue("gender", "Hobbit");
		item2.ShouldNotContainKey("firstName");
	}
}
