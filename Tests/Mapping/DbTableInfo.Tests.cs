namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbTableInfo"/> class.
/// </summary>
[TestClass]
public class DbTableInfoTests {

	[TestMethod]
	public void Columns() {
		new DbTableInfo(typeof(ConsoleKeyInfo)).Columns.ShouldBeEmpty();

		var columns = new DbTableInfo(typeof(Character)).Columns;
		columns.Count.ShouldBe(5);
		columns.Keys.ShouldBe(["firstName", "fullName", "gender", "ID", "lastName"]);
	}

	[TestMethod]
	public void IdentityColumn() {
		new DbTableInfo(typeof(ConsoleKeyInfo)).IdentityColumn.ShouldBeNull();

		var identityColumn = new DbTableInfo(typeof(Character)).IdentityColumn;
		identityColumn.ShouldNotBeNull();
		identityColumn.Name.ShouldBe("ID");
	}

	[TestMethod]
	public void Name() {
		// It should return the class name when there is no [Table] attribute.
		new DbTableInfo(typeof(ConsoleKeyInfo)).Name.ShouldBe(nameof(ConsoleKeyInfo));

		// It should return the value of the [Table] attribute when it is present.
		new DbTableInfo(typeof(Character)).Name.ShouldBe("Characters");
	}

	[TestMethod]
	public void Schema() {
		// It should return `null` when there is no [Table] attribute.
		new DbTableInfo(typeof(ConsoleKeyInfo)).Schema.ShouldBeNull();

		// It should return the value of the [Table] attribute when it is present.
		new DbTableInfo(typeof(Character)).Schema.ShouldBe("main");
	}
}
