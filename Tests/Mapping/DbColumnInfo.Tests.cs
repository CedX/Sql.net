namespace Belin.Sql;

using DataType = System.Data.DbType;

/// <summary>
/// Tests the features of the <see cref="DbColumnInfo"/> class.
/// </summary>
[TestClass]
public class DbColumnInfoTests {

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void CanRead(string name) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).CanRead.ShouldBeTrue();

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void CanWrite(string name) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).CanWrite.ShouldBeTrue();

	[TestMethod]
	[DataRow("FirstName", DataType.String)]
	[DataRow("FullName", DataType.String)]
	[DataRow("Gender", DataType.AnsiString)]
	[DataRow("Id", DataType.Int32)]
	public void DbType(string name, DataType expected) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).DbType.ShouldBe(expected);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", true)]
	[DataRow("Gender", false)]
	[DataRow("Id", true)]
	public void IsComputed(string name, bool expected) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).IsComputed.ShouldBe(expected);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", false)]
	[DataRow("Gender", false)]
	[DataRow("Id", true)]
	public void IsIdentity(string name, bool expected) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).IsIdentity.ShouldBe(expected);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", true)]
	[DataRow("Gender", false)]
	[DataRow("Id", false)]
	public void IsNullable(string name, bool expected) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).IsNullable.ShouldBe(expected);

	[TestMethod]
	[DataRow("FirstName", "firstName")]
	[DataRow("FullName", "fullName")]
	[DataRow("Gender", "gender")]
	[DataRow("Id", "ID")]
	public void Name(string name, string expected) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).Name.ShouldBe(expected);

	[TestMethod]
	[DataRow("FirstName", typeof(string))]
	[DataRow("FullName", typeof(string))]
	[DataRow("Gender", typeof(CharacterGender))]
	[DataRow("Id", typeof(int))]
	public void PropertyType(string name, Type expected) =>
		new DbColumnInfo(typeof(Character).GetProperty(name)!).PropertyType.ShouldBe(expected);

	[TestMethod]
	public void GetValue() {
		var record = new Character { FirstName = "Cédric", LastName = "Belin" };
		new DbColumnInfo(typeof(Character).GetProperty("FirstName")!).GetValue(record).ShouldBe("Cédric");
		new DbColumnInfo(typeof(Character).GetProperty("LastName")!).GetValue(record).ShouldBe("Belin");
	}

	[TestMethod]
	public void SetValue() {
		var record = new Character { FirstName = "Cédric", LastName = "Belin" };
		new DbColumnInfo(typeof(Character).GetProperty("FirstName")!).SetValue(record, "Anders");
		new DbColumnInfo(typeof(Character).GetProperty("LastName")!).SetValue(record, "Hejlsberg");
		record.FirstName.ShouldBe("Anders");
		record.LastName.ShouldBe("Hejlsberg");
	}
}
