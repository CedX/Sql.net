namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void CountAll() {
		// It should return the total number of entities from the underlying table.
		AreEqual(16, connection.CountAll<Character>());
	}

	[TestMethod]
	public async Task CountAllAsync() {
		// It should return the total number of entities from the underlying table.
		AreEqual(16, await connection.CountAllAsync<Character>(cancellationToken: testContext.CancellationToken));
	}
}
