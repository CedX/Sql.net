namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Insert() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		IsEmpty(connection.Query<Character>(sql));

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		AreEqual(0, record.Id);
		IsNull(record.FullName);

		var id = connection.Insert(record);
		IsGreaterThan(16, id);
		AreEqual(id, record.Id);

		var records = connection.Query<Character>(sql);
		HasCount(1, records);

		var cedric = records[0];
		AreEqual(id, cedric.Id);
		AreEqual("Cédric Belin", cedric.FullName);
		AreEqual(record.Gender, cedric.Gender);
	}

	[TestMethod]
	public async Task InsertAsync() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		IsEmpty(await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken));

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		AreEqual(0, record.Id);
		IsNull(record.FullName);

		var id = await connection.InsertAsync(record, cancellationToken: testContext.CancellationToken);
		IsGreaterThan(16, id);
		AreEqual(id, record.Id);

		var records = (await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken));
		HasCount(1, records);

		var cedric = records[0];
		AreEqual(id, cedric.Id);
		AreEqual("Cédric Belin", cedric.FullName);
		AreEqual(record.Gender, cedric.Gender);
	}
}
