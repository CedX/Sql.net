namespace Belin.Sql;

using System.Data.Common;
using System.Data.SQLite;

/// <summary>
/// Manages the test data source.
/// </summary>
internal static class Connection {

	/// <summary>
	/// Opens a connection to the data source.
	/// </summary>
	/// <returns>The newly opened connection.</returns>
	public static DbConnection Open() {
		var connection = new SQLiteConnection("DataSource=:memory:");
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "../Resources/Schema.sql"));
		command.ExecuteNonQuery();

		return connection;
	}
}
