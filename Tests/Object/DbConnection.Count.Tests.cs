namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void CountAll() {
		// It should return the total number of entities from the underlying table.
		connection.CountAll<Character>().ShouldBe(16);
	}

	[TestMethod]
	public async Task CountAllAsync() {
		// It should return the total number of entities from the underlying table.
		(await connection.CountAllAsync<Character>(cancellationToken: testContext.CancellationToken)).ShouldBe(16);
	}
}
