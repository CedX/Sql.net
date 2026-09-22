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
		Assert.AreEqual("?", parameter.Name);
		Assert.AreEqual(DBNull.Value, parameter.Value);

		parameter = new object[] { ":foo", "bar" };
		Assert.AreEqual(":foo", parameter.Name);
		Assert.AreEqual("bar", parameter.Value);

		parameter = new object[] { "baz", 123 };
		Assert.AreEqual("@baz", parameter.Name);
		Assert.AreEqual(123, parameter.Value);

		// It should create a parameter from the specified tuple.
		parameter = ("", null);
		Assert.AreEqual("?", parameter.Name);
		Assert.AreEqual(DBNull.Value, parameter.Value);

		parameter = (":foo", "bar");
		Assert.AreEqual(":foo", parameter.Name);
		Assert.AreEqual("bar", parameter.Value);

		parameter = ("baz", 123);
		Assert.AreEqual("@baz", parameter.Name);
		Assert.AreEqual(123, parameter.Value);

		// It should create a parameter from the specified key/value pair.
		parameter = new KeyValuePair<string, object?>("foo", null);
		Assert.AreEqual("@foo", parameter.Name);
		Assert.AreEqual(DBNull.Value, parameter.Value);

		parameter = (":bar", "Baz");
		Assert.AreEqual(":bar", parameter.Name);
		Assert.AreEqual("Baz", parameter.Value);
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
		Assert.AreEqual(expected, new SqlParameter(name).Name);

	[TestMethod]
	public void Value() {
		// It should normalize the parameter value.
		Assert.AreEqual(DBNull.Value, new SqlParameter("Name", null).Value);
		Assert.AreEqual(DBNull.Value, new SqlParameter("Name", DBNull.Value).Value);
		Assert.AreEqual(123, new SqlParameter("Name", 123).Value);
		Assert.AreEqual(-123.456, new SqlParameter("Name", -123.456).Value);
		Assert.AreEqual("", new SqlParameter("Name", "").Value);
		Assert.AreEqual("Foo", new SqlParameter("Name", "Foo").Value);
		Assert.AreEqual(DateTime.UnixEpoch, new SqlParameter("Name", DateTime.UnixEpoch).Value);

		// It should support the values wrapped in a `PSObject` instance.
		Assert.AreEqual(DBNull.Value, new SqlParameter("Name", new PSObject(DBNull.Value)).Value);
		Assert.AreEqual("FooBar", new SqlParameter("Name", new PSObject("FooBar")).Value);
		Assert.AreEqual(DateTime.UnixEpoch, new SqlParameter("Name", new PSObject(DateTime.UnixEpoch)).Value);
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
		Assert.IsEmpty(collection);

		var parameter = collection.AddWithValue("Name", "Value1");
		Assert.HasCount(1, collection);
		Assert.AreEqual("@Name", parameter.Name);
		Assert.AreEqual("Value1", parameter.Value);

		parameter = collection.AddWithValue("Value2");
		Assert.HasCount(2, collection);
		Assert.AreEqual("?2", parameter.Name);
		Assert.AreEqual("Value2", parameter.Value);
	}

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new SqlParameterCollection();
		Assert.IsEmpty(collection);

		// It should create a collection from a single parameter.
		collection = new(new SqlParameter("?1", 123) { DbType = DbType.Int64 });
		Assert.HasCount(1, collection);

		var parameter = collection.First();
		Assert.AreEqual("?1", parameter.Name);
		Assert.AreEqual(123, parameter.Value);
		Assert.AreEqual(DbType.Int64, parameter.DbType);

		// It should create a collection from a list of parameters.
		collection = new(new("?1", 123), new("@Key", "Unique") { DbType = DbType.AnsiString });
		Assert.HasCount(2, collection);

		parameter = collection.Last();
		Assert.AreEqual("@Key", parameter.Name);
		Assert.AreEqual("Unique", parameter.Value);
		Assert.AreEqual(DbType.AnsiString, parameter.DbType);
	}

	[TestMethod]
	public void Contains() {
		var collection = new SqlParameterCollection(("@Key", null));
		Assert.IsTrue(collection.Contains("Key"));
		Assert.IsTrue(collection.Contains("@Key"));
		Assert.IsFalse(collection.Contains("Foo"));
		Assert.IsFalse(collection.Contains("@Foo"));
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of positional parameters.
		SqlParameterCollection collection = new object[] { "foo", "bar" };
		Assert.AreSequenceEqual(["?1", "?2"], collection.Select(parameter => parameter.Name));
		Assert.AreSequenceEqual(["foo", "bar"], collection.Select(parameter => parameter.Value));

		// It should create a collection from the specified list of positional parameters.
		collection = new List<object?> { "foo", "bar" };
		Assert.AreSequenceEqual(["?1", "?2"], collection.Select(parameter => parameter.Name));
		Assert.AreSequenceEqual(["foo", "bar"], collection.Select(parameter => parameter.Value));

		// It should create a collection from the specified dictionary of named parameters.
		collection = new Dictionary<string, object?> { ["foo"] = "bar", ["baz"] = "qux" };
		Assert.AreSequenceEqual(["@foo", "@baz"], collection.Select(parameter => parameter.Name));
		Assert.AreSequenceEqual(["bar", "qux"], collection.Select(parameter => parameter.Value));

		// It should create a collection from the specified hash table of named parameters.
		collection = new Hashtable { ["foo"] = "bar", ["baz"] = "qux" };
		Assert.AreSequenceEqual(["@foo", "@baz"], collection.Select(parameter => parameter.Name), SequenceOrder.InAnyOrder);
		Assert.AreSequenceEqual(["bar", "qux"], collection.Select(parameter => parameter.Value), SequenceOrder.InAnyOrder);
	}

	[TestMethod]
	public void Indexer() {
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));

		// It should return the parameter with the specified name.
		var parameter = collection["Key"];
		Assert.AreEqual("@Key", parameter.Name);
		Assert.AreEqual("Unique", parameter.Value);
		Assert.AreEqual(parameter, collection[1]);

		// It should throw an error if the specified name does not exist.
		Assert.Throws<KeyNotFoundException>(() => collection["@Foo"]);
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		Assert.AreEqual(1, collection.IndexOf("Key"));
		Assert.AreEqual(1, collection.IndexOf("@Key"));
		Assert.AreEqual(-1, collection.IndexOf("Foo"));
		Assert.AreEqual(-1, collection.IndexOf("@Foo"));
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the parameter with the specified name.
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		Assert.HasCount(2, collection);
		collection.RemoveAt("Key");
		Assert.HasCount(1, collection);
		collection.RemoveAt("?1");
		Assert.IsEmpty(collection);

		// It should throw an error if the specified name does not exist.
		collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		Assert.Throws<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
