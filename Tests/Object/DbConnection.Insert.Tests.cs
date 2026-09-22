namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Insert() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		Assert.IsEmpty(connection.Query<Character>(sql));

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		Assert.AreEqual(0, record.Id);
		Assert.IsNull(record.FullName);

		var id = connection.Insert(record);
		Assert.IsGreaterThan(16, id);
		Assert.AreEqual(id, record.Id);

		var records = connection.Query<Character>(sql);
		Assert.HasCount(1, records);

		var cedric = records[0];
		Assert.AreEqual(id, cedric.Id);
		Assert.AreEqual("Cédric Belin", cedric.FullName);
		Assert.AreEqual(record.Gender, cedric.Gender);
	}

	[TestMethod]
	public async Task InsertAsync() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		Assert.IsEmpty(await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken));

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		Assert.AreEqual(0, record.Id);
		Assert.IsNull(record.FullName);

		var id = await connection.InsertAsync(record, cancellationToken: testContext.CancellationToken);
		Assert.IsGreaterThan(16, id);
		Assert.AreEqual(id, record.Id);

		var records = (await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken));
		Assert.HasCount(1, records);

		var cedric = records[0];
		Assert.AreEqual(id, cedric.Id);
		Assert.AreEqual("Cédric Belin", cedric.FullName);
		Assert.AreEqual(record.Gender, cedric.Gender);
	}
}
