namespace Belin.Sql;


/// <summary>
/// Tests the features of the <see cref="DbTableInfo"/> class.
/// </summary>
[TestClass]
public sealed class DbTableInfoTests {

	[TestMethod]
	public void Columns() {
		Assert.IsEmpty(new DbTableInfo(typeof(ConsoleKeyInfo)).Columns);

		var columns = new DbTableInfo(typeof(Character)).Columns;
		Assert.HasCount(5, columns);
		Assert.AreSequenceEqual(["firstName", "fullName", "gender", "ID", "lastName"], columns.Keys);
	}

	[TestMethod]
	public void IdentityColumn() {
		Assert.IsNull(new DbTableInfo(typeof(ConsoleKeyInfo)).IdentityColumn);

		var identityColumn = new DbTableInfo(typeof(Character)).IdentityColumn;
		Assert.IsNotNull(identityColumn);
		Assert.AreEqual("ID", identityColumn.Name);
	}

	[TestMethod]
	public void Name() {
		// It should return the class name when there is no [Table] attribute.
		Assert.AreEqual(nameof(ConsoleKeyInfo), new DbTableInfo(typeof(ConsoleKeyInfo)).Name);

		// It should return the value of the [Table] attribute when it is present.
		Assert.AreEqual("Characters", new DbTableInfo(typeof(Character)).Name);
	}

	[TestMethod]
	public void Schema() {
		// It should return `null` when there is no [Table] attribute.
		Assert.IsNull(new DbTableInfo(typeof(ConsoleKeyInfo)).Schema);

		// It should return the value of the [Table] attribute when it is present.
		Assert.AreEqual("main", new DbTableInfo(typeof(Character)).Schema);
	}
}
