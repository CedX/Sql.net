namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Execute() {
		connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters").ShouldBe(16);
		connection.Execute("DELETE FROM Characters WHERE gender = @Gender", [("Gender", nameof(CharacterGender.Balrog))]).ShouldBe(2);
		connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters").ShouldBe(14);

		connection.Execute("DELETE FROM Characters WHERE gender = @Gender", [("Gender", nameof(CharacterGender.Elf))]).ShouldBe(3);
		connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters").ShouldBe(11);
	}

	[TestMethod]
	public async Task ExecuteAsync() {
		var parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Balrog)));
		(await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken)).ShouldBe(16);
		(await connection.ExecuteAsync("DELETE FROM Characters WHERE gender = @Gender", parameters, testContext.CancellationToken)).ShouldBe(2);
		(await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken)).ShouldBe(14);

		parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Elf)));
		(await connection.ExecuteAsync("DELETE FROM Characters WHERE gender = @Gender", parameters, testContext.CancellationToken)).ShouldBe(3);
		(await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken)).ShouldBe(11);
	}
}
