namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Exists() {
		Assert.IsTrue(connection.Exists<Character>(1));
		Assert.IsFalse(connection.Exists<Character>(666));
	}

	[TestMethod]
	public async Task ExistsAsync() {
		Assert.IsTrue(await connection.ExistsAsync<Character>(1, cancellationToken: testContext.CancellationToken));
		Assert.IsFalse(await connection.ExistsAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}
}
