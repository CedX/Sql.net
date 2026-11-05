namespace Belin.Sql;

using System.Data.Common;

/// <summary>
/// Tests the features of the <see cref="ObjectExistsExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public class ObjectExistsExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private DbConnection connection = null!;
	[TestInitialize] public void Initialize() => connection = Connection.Open();
	[TestCleanup] public void Cleanup() => connection.Dispose();

	[TestMethod]
	public void Exists() {
		connection.Exists<Character>(1).ShouldBeTrue();
		connection.Exists<Character>(666).ShouldBeFalse();
	}

	[TestMethod]
	public async Task ExistsAsync() {
		(await connection.ExistsAsync<Character>(1, cancellationToken: testContext.CancellationToken)).ShouldBeTrue();
		(await connection.ExistsAsync<Character>(666, cancellationToken: testContext.CancellationToken)).ShouldBeFalse();
	}
}
