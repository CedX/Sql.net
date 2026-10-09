namespace Belin.Sql;

using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="SqlCommand"/> class.
/// </summary>
[TestClass]
public class SqlCommandTests {

	[TestMethod]
	public void ImplicitConversion() {
		SqlCommand command = "SELECT * FROM Characters";
		command.Text.ShouldBe("SELECT * FROM Characters");
	}
}

/// <summary>
/// Tests the features of the <see cref="SqlCommandBuilder"/> class.
/// </summary>
[TestClass]
public class SqlCommandBuilderTests {

	/// <summary>
	/// The test data.
	/// </summary>
	private readonly Character character = new() { Id = 1000, FirstName = "Cédric", Gender = CharacterGender.DarkLord };

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private readonly SQLiteConnection connection = new("DataSource=:memory:");

	[TestMethod]
	public void GetDeleteCommand() {
		// It should return the SQL command to delete an entity.
		var (command, parameters) = SqlCommandBuilder.Create(connection).GetDeleteCommand(character);
		command.Text.ShouldStartWith(@"DELETE FROM ""main"".""Characters""");
		command.Text.ShouldEndWith(@"WHERE ""ID"" = @ID");

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		parameter.Name.ShouldBe("@ID");
		parameter.Value.ShouldBe(1000);
	}

	[TestMethod]
	public void GetDeleteAllCommand() {
		// It should return the SQL command to delete all entities.
		var (command, parameters) = SqlCommandBuilder.Create(connection).GetDeleteAllCommand<Character>();
		command.Text.ShouldBe(@"DELETE FROM ""main"".""Characters""");

		// It should also return an empty parameter collection.
		parameters.ShouldBeEmpty();
	}

	[TestMethod]
	public void GetExistsCommand() {
		// It should return the SQL command to check the existence of an entity.
		var (command, parameters) = SqlCommandBuilder.Create(connection).GetExistsCommand<Character>(character.Id);
		command.Text.ShouldStartWith("SELECT 1");
		command.Text.ShouldContain(@"FROM ""main"".""Characters""");
		command.Text.ShouldEndWith(@"WHERE ""ID"" = @ID");

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		parameter.Name.ShouldBe("@ID");
		parameter.Value.ShouldBe(1000);
	}

	[TestMethod]
	public void GetFindCommand() {
		var builder = SqlCommandBuilder.Create(connection);

		// It should return the SQL command to find an entity.
		var (command, parameters) = builder.GetFindCommand<Character>(character.Id);
		command.Text.ShouldStartWith(@"SELECT """);
		command.Text.ShouldNotContain("*");
		command.Text.ShouldContain(@"FROM ""main"".""Characters""");
		command.Text.ShouldEndWith(@"WHERE ""ID"" = @ID");

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		parameter.Name.ShouldBe("@ID");
		parameter.Value.ShouldBe(1000);

		// It should allow selecting a specific set of columns.
		(command, _) = builder.GetFindCommand<Character>(character.Id, ["firstName"]);
		command.Text.ShouldStartWith(@"SELECT ""firstName""");
		command.Text.ShouldNotContain("gender");
		command.Text.ShouldNotContain("lastName");
		command.Text.ShouldEndWith(@"WHERE ""ID"" = @ID");
	}

	[TestMethod]
	public void GetFindAllCommand() {
		var builder = SqlCommandBuilder.Create(connection);

		// It should return the SQL command to find all entities.
		var (command, parameters) = builder.GetFindAllCommand<Character>();
		command.Text.ShouldStartWith(@"SELECT """);
		command.Text.ShouldNotContain("*");
		command.Text.ShouldContain(@"FROM ""main"".""Characters""");
		command.Text.ShouldEndWith(@"ORDER BY ""ID"" ASC");

		// It should also return an empty parameter collection.
		parameters.ShouldBeEmpty();

		// It should allow sorting the results by a specific set of columns.
		(command, _) = builder.GetFindAllCommand<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)]);
		command.Text.ShouldStartWith(@"SELECT """);
		command.Text.ShouldNotContain("*");
		command.Text.ShouldContain(@"FROM ""main"".""Characters""");
		command.Text.ShouldEndWith(@"ORDER BY ""gender"" ASC, ""fullName"" DESC");

		// It should allow selecting a specific set of columns.
		(command, _) = builder.GetFindAllCommand<Character>(columns: ["firstName"]);
		command.Text.ShouldStartWith(@"SELECT ""firstName""");
		command.Text.ShouldNotContain("gender");
		command.Text.ShouldNotContain("lastName");
		command.Text.ShouldEndWith(@"ORDER BY ""ID"" ASC");
	}

	[TestMethod]
	public void GetInsertCommand() {
		// It should return the SQL command to insert an entity.
		var (command, parameters) = SqlCommandBuilder.Create(connection).GetInsertCommand(character);
		command.Text.ShouldStartWith(@"INSERT INTO ""main"".""Characters"" (");
		command.Text.ShouldContain("VALUES (");

		// It should also return the parameters used by the SQL command.
		parameters.Count.ShouldBe(3);
		parameters["firstName"].Value.ShouldBe("Cédric");
		parameters["gender"].Value.ShouldBe(nameof(CharacterGender.DarkLord));
		parameters["lastName"].Value.ShouldBe("");
	}

	[TestMethod]
	public void GetUpdateCommand() {
		var builder = SqlCommandBuilder.Create(connection);

		// It should return the SQL command to update an entity.
		var (command, parameters) = builder.GetUpdateCommand(character);
		command.Text.ShouldStartWith(@"UPDATE ""main"".""Characters""");
		command.Text.ShouldContain(@"SET """);
		command.Text.ShouldEndWith(@"WHERE ""ID"" = @ID");

		// It should also return the parameters used by the SQL command.
		parameters.Count.ShouldBe(4);
		parameters["ID"].Value.ShouldBe(1000);
		parameters["firstName"].Value.ShouldBe("Cédric");
		parameters["gender"].Value.ShouldBe(nameof(CharacterGender.DarkLord));
		parameters["lastName"].Value.ShouldBe("");

		// It should allow updating a specific set of columns.
		(_, parameters) = builder.GetUpdateCommand(character, "firstName");
		parameters.Count.ShouldBe(2);
		parameters["ID"].Value.ShouldBe(1000);
		parameters["firstName"].Value.ShouldBe("Cédric");
	}
}
