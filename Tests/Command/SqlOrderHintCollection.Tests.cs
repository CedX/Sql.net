namespace Belin.Sql;

using System.Collections.Specialized;

/// <summary>
/// Tests the features of the <see cref="SqlOrderHint"/> class.
/// </summary>
[TestClass]
public class SqlOrderHintTests {

	[TestMethod]
	public void ImplicitConversion() {
		// It should create an order hint from the specified column name.
		SqlOrderHint orderHint = "Name";
		orderHint.Column.ShouldBe("Name");
		orderHint.SortOrder.ShouldBe(SortOrder.Ascending);

		// It should create an order hint from the specified array.
		orderHint = new object[] { "ID", "Descending" };
		orderHint.Column.ShouldBe("ID");
		orderHint.SortOrder.ShouldBe(SortOrder.Descending);

		// It should create an order hint from the specified tuple.
		orderHint = ("ID", SortOrder.Descending);
		orderHint.Column.ShouldBe("ID");
		orderHint.SortOrder.ShouldBe(SortOrder.Descending);

		// It should create an order hint from the specified key/value pair.
		orderHint = new KeyValuePair<string, SortOrder>("Name", SortOrder.Ascending);
		orderHint.Column.ShouldBe("Name");
		orderHint.SortOrder.ShouldBe(SortOrder.Ascending);
	}
}

/// <summary>
/// Tests the features of the <see cref="SqlOrderHintCollection"/> class.
/// </summary>
[TestClass]
public class SqlOrderHintCollectionTests {

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new SqlOrderHintCollection();
		collection.ShouldBeEmpty();

		// It should create a collection from a single order order hint.
		collection = new(new SqlOrderHint("ID", SortOrder.Descending));
		collection.Count.ShouldBe(1);

		var orderHint = collection.First();
		orderHint.Column.ShouldBe("ID");
		orderHint.SortOrder.ShouldBe(SortOrder.Descending);

		// It should create a collection from a list of order hints.
		collection = new(new("ID", SortOrder.Descending), new("Name", SortOrder.Ascending));
		collection.Count.ShouldBe(2);

		orderHint = collection.Last();
		orderHint.Column.ShouldBe("Name");
		orderHint.SortOrder.ShouldBe(SortOrder.Ascending);
	}

	[TestMethod]
	public void Contains() {
		var collection = new SqlOrderHintCollection(("Key", SortOrder.Ascending));
		collection.Contains("key").ShouldBeTrue();
		collection.Contains("KEY").ShouldBeTrue();
		collection.Contains("foo").ShouldBeFalse();
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of column names.
		SqlOrderHintCollection collection = new object[] { "ID", "Name" };
		collection.Select(parameter => parameter.Column).ShouldBe(["ID", "Name"]);
		collection.Select(parameter => parameter.SortOrder).ShouldBe([SortOrder.Ascending, SortOrder.Ascending]);

		collection = new string[] { "ID", "Name" };
		collection.Select(parameter => parameter.Column).ShouldBe(["ID", "Name"]);
		collection.Select(parameter => parameter.SortOrder).ShouldBe([SortOrder.Ascending, SortOrder.Ascending]);

		// It should create a collection from the specified list of column names.
		collection = new List<string> { "ID", "Name" };
		collection.Select(parameter => parameter.Column).ShouldBe(["ID", "Name"]);
		collection.Select(parameter => parameter.SortOrder).ShouldBe([SortOrder.Ascending, SortOrder.Ascending]);

		// It should create a collection from the specified dictionary of column names and sort orders.
		collection = new OrderedDictionary { ["ID"] = "Descending", ["Name"] = "Ascending" };
		collection.Select(parameter => parameter.Column).ShouldBe(["ID", "Name"]);
		collection.Select(parameter => parameter.SortOrder).ShouldBe([SortOrder.Descending, SortOrder.Ascending]);

		collection = new OrderedDictionary<string, SortOrder> { ["ID"] = SortOrder.Descending, ["Name"] = SortOrder.Ascending };
		collection.Select(parameter => parameter.Column).ShouldBe(["ID", "Name"]);
		collection.Select(parameter => parameter.SortOrder).ShouldBe([SortOrder.Descending, SortOrder.Ascending]);
	}

	[TestMethod]
	public void Indexer() {
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));

		// It should return the order hint with the specified column name.
		var orderHint = collection["id"];
		orderHint.Column.ShouldBe("ID");
		orderHint.SortOrder.ShouldBe(SortOrder.Descending);
		collection[0].ShouldBe(orderHint);

		// It should throw an error if the specified column does not exist.
		Should.Throw<KeyNotFoundException>(() => collection["foo"]);
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		collection.IndexOf("id").ShouldBe(0);
		collection.IndexOf("name").ShouldBe(1);
		collection.IndexOf("foo").ShouldBe(-1);
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the order hint with the specified column name.
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		collection.Count.ShouldBe(2);
		collection.RemoveAt("name");
		collection.Count.ShouldBe(1);
		collection.RemoveAt("id");
		collection.ShouldBeEmpty();

		// It should throw an error if the specified column does not exist.
		collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		Should.Throw<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
