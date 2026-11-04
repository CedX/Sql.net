namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="TableInfo"/> class.
/// </summary>
[TestClass]
public class TableInfoTests {

	[TestMethod]
	public void Columns() {
		new TableInfo(typeof(ConsoleKeyInfo)).Columns.ShouldBeEmpty();

		var columns = new TableInfo(typeof(Character)).Columns;
		columns.Count.ShouldBe(5);
		columns.Keys.ShouldBe(["firstName", "fullName", "gender", "ID", "lastName"]);
	}

	[TestMethod]
	public void IdentityColumn() {
		new TableInfo(typeof(ConsoleKeyInfo)).IdentityColumn.ShouldBeNull();

		var identityColumn = new TableInfo(typeof(Character)).IdentityColumn;
		identityColumn.ShouldNotBeNull();
		identityColumn.Name.ShouldBe("ID");
	}

	[TestMethod]
	public void Name() {
		// It should return the class name when there is no [Table] attribute.
		new TableInfo(typeof(ConsoleKeyInfo)).Name.ShouldBe(nameof(ConsoleKeyInfo));

		// It should return the value of the [Table] attribute when it is present.
		new TableInfo(typeof(Character)).Name.ShouldBe("Characters");
	}

	[TestMethod]
	public void Schema() {
		// It should return `null` when there is no [Table] attribute.
		new TableInfo(typeof(ConsoleKeyInfo)).Schema.ShouldBeNull();

		// It should return the value of the [Table] attribute when it is present.
		new TableInfo(typeof(Character)).Schema.ShouldBe("main");
	}
}
