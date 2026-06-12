namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="SqlCommand"/> class.
/// </summary>
[TestClass]
public sealed class SqlCommandTests {

	[TestMethod]
	public void ImplicitConversion() {
		SqlCommand command = "SELECT * FROM Characters";
		AreEqual("SELECT * FROM Characters", command.Text);
	}
}
