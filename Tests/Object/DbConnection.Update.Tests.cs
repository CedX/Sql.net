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
		Assert.AreEqual("Sauron", sauron.FullName);
		Assert.AreEqual(CharacterGender.DarkLord, sauron.Gender);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		Assert.AreEqual(1, DbConnectionExtensions.Update(connection, sauron));

		sauron = connection.QuerySingle<Character>(sql);
		Assert.AreEqual("Sauron The big bad guy", sauron.FullName);
		Assert.AreEqual(CharacterGender.Istari, sauron.Gender);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = connection.QuerySingle<Character>(sql);
		Assert.AreEqual("Saruman", saruman.FullName);
		Assert.AreEqual(CharacterGender.Istari, saruman.Gender);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		Assert.AreEqual(1, DbConnectionExtensions.Update(connection, saruman, ["gender"]));

		saruman = connection.QuerySingle<Character>(sql);
		Assert.AreEqual("Saruman", saruman.FullName);
		Assert.AreEqual(CharacterGender.DarkLord, saruman.Gender);
	}

	[TestMethod]
	public async Task UpdateAsync() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		Assert.AreEqual("Sauron", sauron.FullName);
		Assert.AreEqual(CharacterGender.DarkLord, sauron.Gender);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		Assert.AreEqual(1, await connection.UpdateAsync(sauron, cancellationToken: testContext.CancellationToken));

		sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		Assert.AreEqual("Sauron The big bad guy", sauron.FullName);
		Assert.AreEqual(CharacterGender.Istari, sauron.Gender);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		Assert.AreEqual("Saruman", saruman.FullName);
		Assert.AreEqual(CharacterGender.Istari, saruman.Gender);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		Assert.AreEqual(1, await connection.UpdateAsync(saruman, ["gender"], cancellationToken: testContext.CancellationToken));

		saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		Assert.AreEqual("Saruman", saruman.FullName);
		Assert.AreEqual(CharacterGender.DarkLord, saruman.Gender);
	}
}
