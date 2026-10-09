namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Find() {
		// It should find the record with the specified identifier.
		var record = connection.Find<Character>(2);
		record.ShouldNotBeNull();
		record.Id.ShouldBe(2);
		record.FullName.ShouldBe("Balin");

		record = connection.Find<Character>(14);
		record.ShouldNotBeNull();
		record.Id.ShouldBe(14);
		record.FullName.ShouldBe("Sam Gamgee");

		// It should allow selecting a specific set of columns.
		record = connection.Find<Character>(2, ["gender"]);
		record.ShouldNotBeNull();
		record.FullName.ShouldBeNull();
		record.Gender.ShouldBe(CharacterGender.Dwarf);

		record = connection.Find<Character>(14, ["gender"]);
		record.ShouldNotBeNull();
		record.FullName.ShouldBeNull();
		record.Gender.ShouldBe(CharacterGender.Hobbit);

		// It should return `null` if the record is not found.
		connection.Find<Character>(666).ShouldBeNull();
	}

	[TestMethod]
	public async Task FindAsync() {
		// It should find the record with the specified identifier.
		var record = await connection.FindAsync<Character>(2, cancellationToken: testContext.CancellationToken);
		record.ShouldNotBeNull();
		record.Id.ShouldBe(2);
		record.FullName.ShouldBe("Balin");

		record = await connection.FindAsync<Character>(14, cancellationToken: testContext.CancellationToken);
		record.ShouldNotBeNull();
		record.Id.ShouldBe(14);
		record.FullName.ShouldBe("Sam Gamgee");

		// It should allow selecting a specific set of columns.
		record = await connection.FindAsync<Character>(2, ["gender"], cancellationToken: testContext.CancellationToken);
		record.ShouldNotBeNull();
		record.FullName.ShouldBeNull();
		record.Gender.ShouldBe(CharacterGender.Dwarf);

		record = await connection.FindAsync<Character>(14, ["gender"], cancellationToken: testContext.CancellationToken);
		record.ShouldNotBeNull();
		record.FullName.ShouldBeNull();
		record.Gender.ShouldBe(CharacterGender.Hobbit);

		// It should return `null` if the record is not found.
		(await connection.FindAsync<Character>(666, cancellationToken: testContext.CancellationToken)).ShouldBeNull();
	}

	[TestMethod]
	public void FindAll() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = connection.FindAll<Character>();
		records.Count.ShouldBe(16);
		records[0].Id.ShouldBe(1);
		records[0].FullName.ShouldBe("Aragorn");
		records[15].Id.ShouldBe(16);
		records[15].FullName.ShouldBe("Sauron");

		// It should allow sorting the results by a specific set of columns.
		records = connection.FindAll<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)]);
		records.Count.ShouldBe(16);
		records[0].Id.ShouldBe(11);
		records[0].FullName.ShouldBe("Gothmog");
		records[15].Id.ShouldBe(8);
		records[15].FullName.ShouldBe("Gandalf");

		// It should allow selecting a specific set of columns.
		records = connection.FindAll<Character>(columns: ["gender"]);
		records[0].Id.ShouldBe(1);
		records[0].Gender.ShouldBe(CharacterGender.Human);
		records[0].FullName.ShouldBeNull();
		records[15].Id.ShouldBe(16);
		records[15].Gender.ShouldBe(CharacterGender.DarkLord);
		records[15].FullName.ShouldBeNull();
	}

	[TestMethod]
	public async Task FindAllAsync() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = await connection.FindAllAsync<Character>(cancellationToken: testContext.CancellationToken);
		records.Count.ShouldBe(16);
		records[0].Id.ShouldBe(1);
		records[0].FullName.ShouldBe("Aragorn");
		records[15].Id.ShouldBe(16);
		records[15].FullName.ShouldBe("Sauron");

		// It should allow sorting the results by a specific set of columns.
		records = await connection.FindAllAsync<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)], cancellationToken: testContext.CancellationToken);
		records.Count.ShouldBe(16);
		records[0].Id.ShouldBe(11);
		records[0].FullName.ShouldBe("Gothmog");
		records[15].Id.ShouldBe(8);
		records[15].FullName.ShouldBe("Gandalf");

		// It should allow selecting a specific set of columns.
		records = await connection.FindAllAsync<Character>(columns: ["gender"], cancellationToken: testContext.CancellationToken);
		records[0].Id.ShouldBe(1);
		records[0].Gender.ShouldBe(CharacterGender.Human);
		records[0].FullName.ShouldBeNull();
		records[15].Id.ShouldBe(16);
		records[15].Gender.ShouldBe(CharacterGender.DarkLord);
		records[15].FullName.ShouldBeNull();
	}
}
