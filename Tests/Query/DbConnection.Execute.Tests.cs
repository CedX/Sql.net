namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Execute() {
		AreEqual(16, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));
		AreEqual(2, connection.Execute("DELETE FROM Characters WHERE gender = @Gender", [("Gender", nameof(CharacterGender.Balrog))]));
		AreEqual(14, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));

		AreEqual(3, connection.Execute("DELETE FROM Characters WHERE gender = @Gender", [("Gender", nameof(CharacterGender.Elf))]));
		AreEqual(11, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));
	}

	[TestMethod]
	public async Task ExecuteAsync() {
		var parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Balrog)));
		AreEqual(16, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));
		AreEqual(2, await connection.ExecuteAsync("DELETE FROM Characters WHERE gender = @Gender", parameters, testContext.CancellationToken));
		AreEqual(14, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));

		parameters = new SqlParameterCollection(("Gender", nameof(CharacterGender.Elf)));
		AreEqual(3, await connection.ExecuteAsync("DELETE FROM Characters WHERE gender = @Gender", parameters, testContext.CancellationToken));
		AreEqual(11, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));
	}
}
