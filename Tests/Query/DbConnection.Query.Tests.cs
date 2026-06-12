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
		var records = connection.Query<Character>(sql, [("Gender", CharacterGender.Elf.ToString())]).AsList();
		HasCount(3, records);

		var elrond = records[0];
		AreEqual("Elrond", elrond.FullName);
		AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		AreEqual("Galadriel", galadriel.FullName);
		AreEqual(CharacterGender.Elf, galadriel.Gender);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = connection.Query<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")]).AsList();
		HasCount(1, objects);

		dynamic left = objects[0].Item1;
		AreEqual(6, left.ID);
		AreEqual("Frodo", left.firstName);
		AreEqual("Baggins", left.lastName);
		IsFalse(((IDictionary<string, object?>) left).ContainsKey("fullName"));

		dynamic right = objects[0].Item2;
		AreEqual(6, right.ID);
		AreEqual("Frodo Baggins", right.fullName);
		AreEqual("Hobbit", right.gender);
		IsFalse(((IDictionary<string, object?>) right).ContainsKey("firstName"));
	}

	[TestMethod]
	public async Task QueryAsync() {
		// It should return the records produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Elf.ToString()));
		var records = (await connection.QueryAsync<Character>(sql, parameters, testContext.CancellationToken)).AsList();
		HasCount(3, records);

		var elrond = records[0];
		AreEqual("Elrond", elrond.FullName);
		AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		AreEqual("Galadriel", galadriel.FullName);
		AreEqual(CharacterGender.Elf, galadriel.Gender);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = (await connection.QueryAsync<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")], "id", testContext.CancellationToken)).AsList();
		HasCount(1, objects);

		dynamic left = objects[0].Item1;
		AreEqual(6, left.ID);
		AreEqual("Frodo", left.firstName);
		AreEqual("Baggins", left.lastName);
		IsFalse(((IDictionary<string, object?>) left).ContainsKey("fullName"));

		dynamic right = objects[0].Item2;
		AreEqual(6, right.ID);
		AreEqual("Frodo Baggins", right.fullName);
		AreEqual("Hobbit", right.gender);
		IsFalse(((IDictionary<string, object?>) right).ContainsKey("firstName"));
	}
}
