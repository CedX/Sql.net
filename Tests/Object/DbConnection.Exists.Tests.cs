namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

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
