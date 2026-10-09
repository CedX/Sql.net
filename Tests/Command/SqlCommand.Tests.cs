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
		Assert.StartsWith(@"DELETE FROM ""main"".""Characters""", command.Text);
		Assert.EndsWith(@"WHERE ""ID"" = @ID", command.Text);

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
		Assert.IsEmpty(parameters);
	}

	[TestMethod]
	public void GetExistsCommand() {
		// It should return the SQL command to check the existence of an entity.
		var (command, parameters) = SqlCommandBuilder.Create(connection).GetExistsCommand<Character>(character.Id);
		Assert.StartsWith("SELECT 1", command.Text);
		Assert.Contains(@"FROM ""main"".""Characters""", command.Text);
		Assert.EndsWith(@"WHERE ""ID"" = @ID", command.Text);

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
		Assert.StartsWith(@"SELECT """, command.Text);
		Assert.DoesNotContain("*", command.Text);
		Assert.Contains(@"FROM ""main"".""Characters""", command.Text);
		Assert.EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		parameter.Name.ShouldBe("@ID");
		parameter.Value.ShouldBe(1000);

		// It should allow selecting a specific set of columns.
		(command, _) = builder.GetFindCommand<Character>(character.Id, ["firstName"]);
		Assert.StartsWith(@"SELECT ""firstName""", command.Text);
		Assert.DoesNotContain("gender", command.Text);
		Assert.DoesNotContain("lastName", command.Text);
		Assert.EndsWith(@"WHERE ""ID"" = @ID", command.Text);
	}

	[TestMethod]
	public void GetFindAllCommand() {
		var builder = SqlCommandBuilder.Create(connection);

		// It should return the SQL command to find all entities.
		var (command, parameters) = builder.GetFindAllCommand<Character>();
		Assert.StartsWith(@"SELECT """, command.Text);
		Assert.DoesNotContain("*", command.Text);
		Assert.Contains(@"FROM ""main"".""Characters""", command.Text);
		Assert.EndsWith(@"ORDER BY ""ID"" ASC", command.Text);

		// It should also return an empty parameter collection.
		Assert.IsEmpty(parameters);

		// It should allow sorting the results by a specific set of columns.
		(command, _) = builder.GetFindAllCommand<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)]);
		Assert.StartsWith(@"SELECT """, command.Text);
		Assert.DoesNotContain("*", command.Text);
		Assert.Contains(@"FROM ""main"".""Characters""", command.Text);
		Assert.EndsWith(@"ORDER BY ""gender"" ASC, ""fullName"" DESC", command.Text);

		// It should allow selecting a specific set of columns.
		(command, _) = builder.GetFindAllCommand<Character>(columns: ["firstName"]);
		Assert.StartsWith(@"SELECT ""firstName""", command.Text);
		Assert.DoesNotContain("gender", command.Text);
		Assert.DoesNotContain("lastName", command.Text);
		Assert.EndsWith(@"ORDER BY ""ID"" ASC", command.Text);
	}

	[TestMethod]
	public void GetInsertCommand() {
		// It should return the SQL command to insert an entity.
		var (command, parameters) = SqlCommandBuilder.Create(connection).GetInsertCommand(character);
		Assert.StartsWith(@"INSERT INTO ""main"".""Characters"" (", command.Text);
		Assert.Contains("VALUES (", command.Text);

		// It should also return the parameters used by the SQL command.
		Assert.HasCount(3, parameters);
		parameters["firstName"].Value.ShouldBe("Cédric");
		parameters["gender"].Value.ShouldBe(nameof(CharacterGender.DarkLord));
		parameters["lastName"].Value.ShouldBe("");
	}

	[TestMethod]
	public void GetUpdateCommand() {
		var builder = SqlCommandBuilder.Create(connection);

		// It should return the SQL command to update an entity.
		var (command, parameters) = builder.GetUpdateCommand(character);
		Assert.StartsWith(@"UPDATE ""main"".""Characters""", command.Text);
		Assert.Contains(@"SET """, command.Text);
		Assert.EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		// It should also return the parameters used by the SQL command.
		Assert.HasCount(4, parameters);
		parameters["ID"].Value.ShouldBe(1000);
		parameters["firstName"].Value.ShouldBe("Cédric");
		parameters["gender"].Value.ShouldBe(nameof(CharacterGender.DarkLord));
		parameters["lastName"].Value.ShouldBe("");

		// It should allow updating a specific set of columns.
		(_, parameters) = builder.GetUpdateCommand(character, "firstName");
		Assert.HasCount(2, parameters);
		parameters["ID"].Value.ShouldBe(1000);
		parameters["firstName"].Value.ShouldBe("Cédric");
	}
}
