namespace Belin.Sql;

using System.Collections;
using System.Data;
using System.Management.Automation;

/// <summary>
/// Tests the features of the <see cref="SqlParameter"/> class.
/// </summary>
[TestClass]
public sealed class SqlParameterTests {

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a parameter from the specified array.
		SqlParameter parameter = new object?[] { "", null };
		AreEqual("?", parameter.Name);
		AreEqual(DBNull.Value, parameter.Value);

		parameter = new object[] { ":foo", "bar" };
		AreEqual(":foo", parameter.Name);
		AreEqual("bar", parameter.Value);

		parameter = new object[] { "baz", 123 };
		AreEqual("@baz", parameter.Name);
		AreEqual(123, parameter.Value);

		// It should create a parameter from the specified tuple.
		parameter = ("", null);
		AreEqual("?", parameter.Name);
		AreEqual(DBNull.Value, parameter.Value);

		parameter = (":foo", "bar");
		AreEqual(":foo", parameter.Name);
		AreEqual("bar", parameter.Value);

		parameter = ("baz", 123);
		AreEqual("@baz", parameter.Name);
		AreEqual(123, parameter.Value);

		// It should create a parameter from the specified key/value pair.
		parameter = new KeyValuePair<string, object?>("foo", null);
		AreEqual("@foo", parameter.Name);
		AreEqual(DBNull.Value, parameter.Value);

		parameter = (":bar", "Baz");
		AreEqual(":bar", parameter.Name);
		AreEqual("Baz", parameter.Value);
	}

	[TestMethod]
	[DataRow("", "?")]
	[DataRow("?", "?")]
	[DataRow("?1", "?1")]
	[DataRow("foo", "@foo")]
	[DataRow("@bar", "@bar")]
	[DataRow(":baz", ":baz")]
	[DataRow("$qux", "$qux")]
	public void Name(string name, string expected) =>
		AreEqual(expected, new SqlParameter(name).Name);

	[TestMethod]
	public void Value() {
		// It should normalize the parameter value.
		AreEqual(DBNull.Value, new SqlParameter("Name", null).Value);
		AreEqual(DBNull.Value, new SqlParameter("Name", DBNull.Value).Value);
		AreEqual(123, new SqlParameter("Name", 123).Value);
		AreEqual(-123.456, new SqlParameter("Name", -123.456).Value);
		AreEqual("", new SqlParameter("Name", "").Value);
		AreEqual("Foo", new SqlParameter("Name", "Foo").Value);
		AreEqual(DateTime.UnixEpoch, new SqlParameter("Name", DateTime.UnixEpoch).Value);

		// It should support the values wrapped in a `PSObject` instance.
		AreEqual(DBNull.Value, new SqlParameter("Name", new PSObject(DBNull.Value)).Value);
		AreEqual("FooBar", new SqlParameter("Name", new PSObject("FooBar")).Value);
		AreEqual(DateTime.UnixEpoch, new SqlParameter("Name", new PSObject(DateTime.UnixEpoch)).Value);
	}
}

/// <summary>
/// Tests the features of the <see cref="SqlParameterCollection"/> class.
/// </summary>
[TestClass]
public sealed class SqlParameterCollectionTests {

	[TestMethod]
	public void AddWithValue() {
		var collection = new SqlParameterCollection();
		IsEmpty(collection);

		var parameter = collection.AddWithValue("Name", "Value1");
		HasCount(1, collection);
		AreEqual("@Name", parameter.Name);
		AreEqual("Value1", parameter.Value);

		parameter = collection.AddWithValue("Value2");
		HasCount(2, collection);
		AreEqual("?2", parameter.Name);
		AreEqual("Value2", parameter.Value);
	}

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new SqlParameterCollection();
		IsEmpty(collection);

		// It should create a collection from a single parameter.
		collection = new(new SqlParameter("?1", 123) { DbType = DbType.Int64 });
		HasCount(1, collection);

		var parameter = collection.First();
		AreEqual("?1", parameter.Name);
		AreEqual(123, parameter.Value);
		AreEqual(DbType.Int64, parameter.DbType);

		// It should create a collection from a list of parameters.
		collection = new(new("?1", 123), new("@Key", "Unique") { DbType = DbType.AnsiString });
		HasCount(2, collection);

		parameter = collection.Last();
		AreEqual("@Key", parameter.Name);
		AreEqual("Unique", parameter.Value);
		AreEqual(DbType.AnsiString, parameter.DbType);
	}

	[TestMethod]
	public void Contains() {
		var collection = new SqlParameterCollection(("@Key", null));
		IsTrue(collection.Contains("Key"));
		IsTrue(collection.Contains("@Key"));
		IsFalse(collection.Contains("Foo"));
		IsFalse(collection.Contains("@Foo"));
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of positional parameters.
		SqlParameterCollection collection = new object[] { "foo", "bar" };
		AreSequenceEqual(["?1", "?2"], collection.Select(parameter => parameter.Name));
		AreSequenceEqual(["foo", "bar"], collection.Select(parameter => parameter.Value));

		// It should create a collection from the specified list of positional parameters.
		collection = new List<object?> { "foo", "bar" };
		AreSequenceEqual(["?1", "?2"], collection.Select(parameter => parameter.Name));
		AreSequenceEqual(["foo", "bar"], collection.Select(parameter => parameter.Value));

		// It should create a collection from the specified dictionary of named parameters.
		collection = new Dictionary<string, object?> { ["foo"] = "bar", ["baz"] = "qux" };
		AreSequenceEqual(["@foo", "@baz"], collection.Select(parameter => parameter.Name));
		AreSequenceEqual(["bar", "qux"], collection.Select(parameter => parameter.Value));

		// It should create a collection from the specified hash table of named parameters.
		collection = new Hashtable { ["foo"] = "bar", ["baz"] = "qux" };
		AreSequenceEqual(["@foo", "@baz"], collection.Select(parameter => parameter.Name), SequenceOrder.InAnyOrder);
		AreSequenceEqual(["bar", "qux"], collection.Select(parameter => parameter.Value), SequenceOrder.InAnyOrder);
	}

	[TestMethod]
	public void Indexer() {
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));

		// It should return the parameter with the specified name.
		var parameter = collection["Key"];
		AreEqual("@Key", parameter.Name);
		AreEqual("Unique", parameter.Value);
		AreEqual(parameter, collection[1]);

		// It should throw an error if the specified name does not exist.
		Throws<KeyNotFoundException>(() => collection["@Foo"]);
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		AreEqual(1, collection.IndexOf("Key"));
		AreEqual(1, collection.IndexOf("@Key"));
		AreEqual(-1, collection.IndexOf("Foo"));
		AreEqual(-1, collection.IndexOf("@Foo"));
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the parameter with the specified name.
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		HasCount(2, collection);
		collection.RemoveAt("Key");
		HasCount(1, collection);
		collection.RemoveAt("?1");
		IsEmpty(collection);

		// It should throw an error if the specified name does not exist.
		collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		Throws<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
