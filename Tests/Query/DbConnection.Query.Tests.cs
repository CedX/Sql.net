namespace Belin.Sql;

using System.Dynamic;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Query() {
		// It should return the records produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
		var records = connection.Query<Character>(sql, [("Gender", nameof(CharacterGender.Elf))]);
		Assert.HasCount(3, records);

		var elrond = records[0];
		Assert.AreEqual("Elrond", elrond.FullName);
		Assert.AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		Assert.AreEqual("Galadriel", galadriel.FullName);
		Assert.AreEqual(CharacterGender.Elf, galadriel.Gender);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = connection.Query<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")]);
		Assert.HasCount(1, objects);

		dynamic left = objects[0].Item1;
		Assert.AreEqual(6, left.ID);
		Assert.AreEqual("Frodo", left.firstName);
		Assert.AreEqual("Baggins", left.lastName);
		Assert.IsFalse(((IDictionary<string, object?>) left).ContainsKey("fullName"));

		dynamic right = objects[0].Item2;
		Assert.AreEqual(6, right.ID);
		Assert.AreEqual("Frodo Baggins", right.fullName);
		Assert.AreEqual("Hobbit", right.gender);
		Assert.IsFalse(((IDictionary<string, object?>) right).ContainsKey("firstName"));
	}

	[TestMethod]
	public async Task QueryAsync() {
		// It should return the records produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
		var parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Elf)));
		var records = await connection.QueryAsync<Character>(sql, parameters, testContext.CancellationToken);
		Assert.HasCount(3, records);

		var elrond = records[0];
		Assert.AreEqual("Elrond", elrond.FullName);
		Assert.AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		Assert.AreEqual("Galadriel", galadriel.FullName);
		Assert.AreEqual(CharacterGender.Elf, galadriel.Gender);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = await connection.QueryAsync<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")], "id", testContext.CancellationToken);
		Assert.HasCount(1, objects);

		dynamic left = objects[0].Item1;
		Assert.AreEqual(6, left.ID);
		Assert.AreEqual("Frodo", left.firstName);
		Assert.AreEqual("Baggins", left.lastName);
		Assert.IsFalse(((IDictionary<string, object?>) left).ContainsKey("fullName"));

		dynamic right = objects[0].Item2;
		Assert.AreEqual(6, right.ID);
		Assert.AreEqual("Frodo Baggins", right.fullName);
		Assert.AreEqual("Hobbit", right.gender);
		Assert.IsFalse(((IDictionary<string, object?>) right).ContainsKey("firstName"));
	}
}
