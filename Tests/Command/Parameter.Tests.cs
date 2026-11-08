namespace Belin.Sql;

using System.Collections;
using System.Data;
using System.Management.Automation;

/// <summary>
/// Tests the features of the <see cref="Parameter"/> class.
/// </summary>
[TestClass]
public class ParameterTests {

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a parameter from the specified array.
		Parameter parameter = new object?[] { "", null };
		parameter.Name.ShouldBe("?");
		parameter.Value.ShouldBe(DBNull.Value);

		parameter = new object[] { ":foo", "bar" };
		parameter.Name.ShouldBe(":foo");
		parameter.Value.ShouldBe("bar");

		parameter = new object[] { "baz", 123 };
		parameter.Name.ShouldBe("@baz");
		parameter.Value.ShouldBe(123);

		// It should create a parameter from the specified tuple.
		parameter = ("", null);
		parameter.Name.ShouldBe("?");
		parameter.Value.ShouldBe(DBNull.Value);

		parameter = (":foo", "bar");
		parameter.Name.ShouldBe(":foo");
		parameter.Value.ShouldBe("bar");

		parameter = ("baz", 123);
		parameter.Name.ShouldBe("@baz");
		parameter.Value.ShouldBe(123);
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
		new Parameter(name).Name.ShouldBe(expected);

	[TestMethod]
	public void Value() {
		// It should normalize the parameter value.
		new Parameter("Name", null).Value.ShouldBe(DBNull.Value);
		new Parameter("Name", DBNull.Value).Value.ShouldBe(DBNull.Value);
		new Parameter("Name", 123).Value.ShouldBe(123);
		new Parameter("Name", -123.456).Value.ShouldBe(-123.456);
		new Parameter("Name", "").Value.ShouldBe("");
		new Parameter("Name", "Foo").Value.ShouldBe("Foo");
		new Parameter("Name", DateTime.UnixEpoch).Value.ShouldBe(DateTime.UnixEpoch);

		// It should support the values wrapped in a `PSObject` instance.
		new Parameter("Name", new PSObject(DBNull.Value)).Value.ShouldBe(DBNull.Value);
		new Parameter("Name", new PSObject("FooBar")).Value.ShouldBe("FooBar");
		new Parameter("Name", new PSObject(DateTime.UnixEpoch)).Value.ShouldBe(DateTime.UnixEpoch);
	}
}

/// <summary>
/// Tests the features of the <see cref="ParameterCollection"/> class.
/// </summary>
[TestClass]
public class ParameterCollectionTests {

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new ParameterCollection();
		collection.ShouldBeEmpty();

		// It should create a collection from a single parameter.
		collection = new(new Parameter("?1", 123) { DbType = DbType.Int64 });
		collection.Count.ShouldBe(1);

		var parameter = collection.First();
		parameter.Name.ShouldBe("?1");
		parameter.Value.ShouldBe(123);
		parameter.DbType.ShouldBe(DbType.Int64);

		// It should create a collection from a list of parameters.
		collection = new(new("?1", 123), new("@Key", "Unique") { DbType = DbType.AnsiString });
		collection.Count.ShouldBe(2);

		parameter = collection.Last();
		parameter.Name.ShouldBe("@Key");
		parameter.Value.ShouldBe("Unique");
		parameter.DbType.ShouldBe(DbType.AnsiString);
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of positional parameters.
		ParameterCollection collection = new object[] { "foo", "bar" };
		collection.Select(parameter => parameter.Name).ShouldBe(["?1", "?2"]);
		collection.Select(parameter => parameter.Value).ShouldBe(["foo", "bar"]);

		// It should create a collection from the specified list of positional parameters.
		collection = new List<object?> { "foo", "bar" };
		collection.Select(parameter => parameter.Name).ShouldBe(["?1", "?2"]);
		collection.Select(parameter => parameter.Value).ShouldBe(["foo", "bar"]);

		// It should create a collection from the specified dictionary of named parameters.
		collection = new Dictionary<string, object?> { ["foo"] = "bar", ["baz"] = "qux" };
		collection.Select(parameter => parameter.Name).ShouldBe(["@foo", "@baz"]);
		collection.Select(parameter => parameter.Value).ShouldBe(["bar", "qux"]);

		// It should create a collection from the specified hash table of named parameters.
		collection = new Hashtable { ["foo"] = "bar", ["baz"] = "qux" };
		collection.Select(parameter => parameter.Name).ShouldBe(["@foo", "@baz"], ignoreOrder: true);
		collection.Select(parameter => parameter.Value).ShouldBe(["bar", "qux"], ignoreOrder: true);
	}

	[TestMethod]
	public void Indexer() {
		var collection = new ParameterCollection(("?1", 123), ("@Key", "Unique"));

		// It should return the parameter with the specified name.
		var parameter = collection["Key"];
		parameter.Name.ShouldBe("@Key");
		parameter.Value.ShouldBe("Unique");
		collection[1].ShouldBe(parameter);

		// It should throw an error if the specified name does not exist.
		Should.Throw<KeyNotFoundException>(() => collection["@Foo"]);
	}

	[TestMethod]
	public void AddParameter() {
		var collection = new ParameterCollection();
		collection.ShouldBeEmpty();

		var parameter = collection.AddParameter("Name", "Value1");
		collection.Count.ShouldBe(1);
		parameter.Name.ShouldBe("@Name");
		parameter.Value.ShouldBe("Value1");

		parameter = collection.AddParameter("Value2");
		collection.Count.ShouldBe(2);
		parameter.Name.ShouldBe("?2");
		parameter.Value.ShouldBe("Value2");
	}

	[TestMethod]
	public void Contains() {
		var collection = new ParameterCollection(("@Key", null));
		collection.Contains("Key").ShouldBeTrue();
		collection.Contains("@Key").ShouldBeTrue();
		collection.Contains("Foo").ShouldBeFalse();
		collection.Contains("@Foo").ShouldBeFalse();
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new ParameterCollection(("?1", 123), ("@Key", "Unique"));
		collection.IndexOf("Key").ShouldBe(1);
		collection.IndexOf("@Key").ShouldBe(1);
		collection.IndexOf("Foo").ShouldBe(-1);
		collection.IndexOf("@Foo").ShouldBe(-1);
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the parameter with the specified name.
		var collection = new ParameterCollection(("?1", 123), ("@Key", "Unique"));
		collection.Count.ShouldBe(2);
		collection.RemoveAt("Key");
		collection.Count.ShouldBe(1);
		collection.RemoveAt("?1");
		collection.ShouldBeEmpty();

		// It should throw an error if the specified name does not exist.
		collection = new ParameterCollection(("?1", 123), ("@Key", "Unique"));
		Should.Throw<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
