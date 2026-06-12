namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Update() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = connection.QuerySingle<Character>(sql);
		AreEqual("Sauron", sauron.FullName);
		AreEqual(CharacterGender.DarkLord, sauron.Gender);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		AreEqual(1, DbConnectionExtensions.Update(connection, sauron));

		sauron = connection.QuerySingle<Character>(sql);
		AreEqual("Sauron The big bad guy", sauron.FullName);
		AreEqual(CharacterGender.Istari, sauron.Gender);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = connection.QuerySingle<Character>(sql);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.Istari, saruman.Gender);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		AreEqual(1, DbConnectionExtensions.Update(connection, saruman, ["gender"]));

		saruman = connection.QuerySingle<Character>(sql);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.DarkLord, saruman.Gender);
	}

	[TestMethod]
	public async Task UpdateAsync() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Sauron", sauron.FullName);
		AreEqual(CharacterGender.DarkLord, sauron.Gender);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		AreEqual(1, await connection.UpdateAsync(sauron, cancellationToken: testContext.CancellationToken));

		sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Sauron The big bad guy", sauron.FullName);
		AreEqual(CharacterGender.Istari, sauron.Gender);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.Istari, saruman.Gender);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		AreEqual(1, await connection.UpdateAsync(saruman, ["gender"], cancellationToken: testContext.CancellationToken));

		saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.DarkLord, saruman.Gender);
	}
}
