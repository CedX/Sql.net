namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Find() {
		// It should find the record with the specified identifier.
		var record = connection.Find<Character>(2);
		Assert.IsNotNull(record);
		Assert.AreEqual(2, record.Id);
		Assert.AreEqual("Balin", record.FullName);

		record = connection.Find<Character>(14);
		Assert.IsNotNull(record);
		Assert.AreEqual(14, record.Id);
		Assert.AreEqual("Sam Gamgee", record.FullName);

		// It should allow selecting a specific set of columns.
		record = connection.Find<Character>(2, ["gender"]);
		Assert.IsNotNull(record);
		Assert.IsNull(record.FullName);
		Assert.AreEqual(CharacterGender.Dwarf, record.Gender);

		record = connection.Find<Character>(14, ["gender"]);
		Assert.IsNotNull(record);
		Assert.IsNull(record.FullName);
		Assert.AreEqual(CharacterGender.Hobbit, record.Gender);

		// It should return `null` if the record is not found.
		Assert.IsNull(connection.Find<Character>(666));
	}

	[TestMethod]
	public async Task FindAsync() {
		// It should find the record with the specified identifier.
		var record = await connection.FindAsync<Character>(2, cancellationToken: testContext.CancellationToken);
		Assert.IsNotNull(record);
		Assert.AreEqual(2, record.Id);
		Assert.AreEqual("Balin", record.FullName);

		record = await connection.FindAsync<Character>(14, cancellationToken: testContext.CancellationToken);
		Assert.IsNotNull(record);
		Assert.AreEqual(14, record.Id);
		Assert.AreEqual("Sam Gamgee", record.FullName);

		// It should allow selecting a specific set of columns.
		record = await connection.FindAsync<Character>(2, ["gender"], cancellationToken: testContext.CancellationToken);
		Assert.IsNotNull(record);
		Assert.IsNull(record.FullName);
		Assert.AreEqual(CharacterGender.Dwarf, record.Gender);

		record = await connection.FindAsync<Character>(14, ["gender"], cancellationToken: testContext.CancellationToken);
		Assert.IsNotNull(record);
		Assert.IsNull(record.FullName);
		Assert.AreEqual(CharacterGender.Hobbit, record.Gender);

		// It should return `null` if the record is not found.
		Assert.IsNull(await connection.FindAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public void FindAll() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = connection.FindAll<Character>();
		Assert.HasCount(16, records);
		Assert.AreEqual(1, records[0].Id);
		Assert.AreEqual("Aragorn", records[0].FullName);
		Assert.AreEqual(16, records[15].Id);
		Assert.AreEqual("Sauron", records[15].FullName);

		// It should allow sorting the results by a specific set of columns.
		records = connection.FindAll<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)]);
		Assert.HasCount(16, records);
		Assert.AreEqual(11, records[0].Id);
		Assert.AreEqual("Gothmog", records[0].FullName);
		Assert.AreEqual(8, records[15].Id);
		Assert.AreEqual("Gandalf", records[15].FullName);

		// It should allow selecting a specific set of columns.
		records = connection.FindAll<Character>(columns: ["gender"]);
		Assert.AreEqual(1, records[0].Id);
		Assert.AreEqual(CharacterGender.Human, records[0].Gender);
		Assert.IsNull(records[0].FullName);
		Assert.AreEqual(16, records[15].Id);
		Assert.AreEqual(CharacterGender.DarkLord, records[15].Gender);
		Assert.IsNull(records[15].FullName);
	}

	[TestMethod]
	public async Task FindAllAsync() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = await connection.FindAllAsync<Character>(cancellationToken: testContext.CancellationToken);
		Assert.HasCount(16, records);
		Assert.AreEqual(1, records[0].Id);
		Assert.AreEqual("Aragorn", records[0].FullName);
		Assert.AreEqual(16, records[15].Id);
		Assert.AreEqual("Sauron", records[15].FullName);

		// It should allow sorting the results by a specific set of columns.
		records = await connection.FindAllAsync<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)], cancellationToken: testContext.CancellationToken);
		Assert.HasCount(16, records);
		Assert.AreEqual(11, records[0].Id);
		Assert.AreEqual("Gothmog", records[0].FullName);
		Assert.AreEqual(8, records[15].Id);
		Assert.AreEqual("Gandalf", records[15].FullName);

		// It should allow selecting a specific set of columns.
		records = await connection.FindAllAsync<Character>(columns: ["gender"], cancellationToken: testContext.CancellationToken);
		Assert.AreEqual(1, records[0].Id);
		Assert.AreEqual(CharacterGender.Human, records[0].Gender);
		Assert.IsNull(records[0].FullName);
		Assert.AreEqual(16, records[15].Id);
		Assert.AreEqual(CharacterGender.DarkLord, records[15].Gender);
		Assert.IsNull(records[15].FullName);
	}
}
