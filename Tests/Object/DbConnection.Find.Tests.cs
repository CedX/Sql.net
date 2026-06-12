namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Find() {
		// It should find the record with the specified identifier.
		var record = connection.Find<Character>(2);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = connection.Find<Character>(14);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		// It should allow selecting a specific set of columns.
		record = connection.Find<Character>(2, ["gender"]);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = connection.Find<Character>(14, ["gender"]);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		// It should return `null` if the record is not found.
		IsNull(connection.Find<Character>(666));
	}

	[TestMethod]
	public async Task FindAsync() {
		// It should find the record with the specified identifier.
		var record = await connection.FindAsync<Character>(2, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = await connection.FindAsync<Character>(14, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		// It should allow selecting a specific set of columns.
		record = await connection.FindAsync<Character>(2, ["gender"], cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = await connection.FindAsync<Character>(14, ["gender"], cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		// It should return `null` if the record is not found.
		IsNull(await connection.FindAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public void FindAll() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = connection.FindAll<Character>();
		HasCount(16, records);
		AreEqual(1, records[0].Id);
		AreEqual("Aragorn", records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual("Sauron", records[15].FullName);

		// It should allow sorting the results by a specific set of columns.
		records = connection.FindAll<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)]);
		HasCount(16, records);
		AreEqual(11, records[0].Id);
		AreEqual("Gothmog", records[0].FullName);
		AreEqual(8, records[15].Id);
		AreEqual("Gandalf", records[15].FullName);

		// It should allow selecting a specific set of columns.
		records = connection.FindAll<Character>(columns: ["gender"]);
		AreEqual(1, records[0].Id);
		AreEqual(CharacterGender.Human, records[0].Gender);
		IsNull(records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual(CharacterGender.DarkLord, records[15].Gender);
		IsNull(records[15].FullName);
	}

	[TestMethod]
	public async Task FindAllAsync() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = await connection.FindAllAsync<Character>(cancellationToken: testContext.CancellationToken);
		HasCount(16, records);
		AreEqual(1, records[0].Id);
		AreEqual("Aragorn", records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual("Sauron", records[15].FullName);

		// It should allow sorting the results by a specific set of columns.
		records = await connection.FindAllAsync<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)], cancellationToken: testContext.CancellationToken);
		HasCount(16, records);
		AreEqual(11, records[0].Id);
		AreEqual("Gothmog", records[0].FullName);
		AreEqual(8, records[15].Id);
		AreEqual("Gandalf", records[15].FullName);

		// It should allow selecting a specific set of columns.
		records = await connection.FindAllAsync<Character>(columns: ["gender"], cancellationToken: testContext.CancellationToken);
		AreEqual(1, records[0].Id);
		AreEqual(CharacterGender.Human, records[0].Gender);
		IsNull(records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual(CharacterGender.DarkLord, records[15].Gender);
		IsNull(records[15].FullName);
	}
}
