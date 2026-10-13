namespace Belin.Sql;

using System.Data.SQLite;

/// <summary>
/// Provides the base class for tests requiring a data source.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public partial class DbConnectionExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private SQLiteConnection connection = null!;

	/// <summary>
	/// Opens a connection to the data source.
	/// </summary>
	[TestInitialize]
	public void TestInitialize() {
		connection = new("DataSource=:memory:");
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "../Resources/Schema.sql"));
		command.ExecuteNonQuery();
	}

	/// <summary>
	/// Closes the connection to the data source.
	/// </summary>
	[TestCleanup]
	public void TestCleanup() => connection.Dispose();
}
