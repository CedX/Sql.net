namespace Belin.Sql;

using System.Data.Common;

/// <summary>
/// Tests the features of the <see cref="ObjectCountExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class ObjectCountExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

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
