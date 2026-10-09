namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Update() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = connection.QuerySingle<Character>(sql);
		sauron.FullName.ShouldBe("Sauron");
		sauron.Gender.ShouldBe(CharacterGender.DarkLord);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		DbConnectionExtensions.Update(connection, sauron).ShouldBe(1);

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
		DbConnectionExtensions.Update(connection, saruman, ["gender"]).ShouldBe(1);

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
