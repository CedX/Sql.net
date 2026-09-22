namespace Belin.Sql;

using System.Collections.Specialized;

/// <summary>
/// Tests the features of the <see cref="SqlOrderHint"/> class.
/// </summary>
[TestClass]
public sealed class SqlOrderHintTests {

	[TestMethod]
	public void ImplicitConversion() {
		// It should create an order hint from the specified column name.
		SqlOrderHint orderHint = "Name";
		Assert.AreEqual("Name", orderHint.Column);
		Assert.AreEqual(SortOrder.Ascending, orderHint.SortOrder);

		// It should create an order hint from the specified array.
		orderHint = new object[] { "ID", "Descending" };
		Assert.AreEqual("ID", orderHint.Column);
		Assert.AreEqual(SortOrder.Descending, orderHint.SortOrder);

		// It should create an order hint from the specified tuple.
		orderHint = ("ID", SortOrder.Descending);
		Assert.AreEqual("ID", orderHint.Column);
		Assert.AreEqual(SortOrder.Descending, orderHint.SortOrder);

		// It should create an order hint from the specified key/value pair.
		orderHint = new KeyValuePair<string, SortOrder>("Name", SortOrder.Ascending);
		Assert.AreEqual("Name", orderHint.Column);
		Assert.AreEqual(SortOrder.Ascending, orderHint.SortOrder);
	}
}

/// <summary>
/// Tests the features of the <see cref="SqlOrderHintCollection"/> class.
/// </summary>
[TestClass]
public sealed class SqlOrderHintCollectionTests {

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new SqlOrderHintCollection();
		Assert.IsEmpty(collection);

		// It should create a collection from a single order order hint.
		collection = new(new SqlOrderHint("ID", SortOrder.Descending));
		Assert.HasCount(1, collection);

		var orderHint = collection.First();
		Assert.AreEqual("ID", orderHint.Column);
		Assert.AreEqual(SortOrder.Descending, orderHint.SortOrder);

		// It should create a collection from a list of order hints.
		collection = new(new("ID", SortOrder.Descending), new("Name", SortOrder.Ascending));
		Assert.HasCount(2, collection);

		orderHint = collection.Last();
		Assert.AreEqual("Name", orderHint.Column);
		Assert.AreEqual(SortOrder.Ascending, orderHint.SortOrder);
	}

	[TestMethod]
	public void Contains() {
		var collection = new SqlOrderHintCollection(("Key", SortOrder.Ascending));
		Assert.IsTrue(collection.Contains("key"));
		Assert.IsTrue(collection.Contains("KEY"));
		Assert.IsFalse(collection.Contains("foo"));
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of column names.
		SqlOrderHintCollection collection = new object[] { "ID", "Name" };
		Assert.AreSequenceEqual(["ID", "Name"], collection.Select(parameter => parameter.Column));
		Assert.AreSequenceEqual([SortOrder.Ascending, SortOrder.Ascending], collection.Select(parameter => parameter.SortOrder));

		collection = new string[] { "ID", "Name" };
		Assert.AreSequenceEqual(["ID", "Name"], collection.Select(parameter => parameter.Column));
		Assert.AreSequenceEqual([SortOrder.Ascending, SortOrder.Ascending], collection.Select(parameter => parameter.SortOrder));

		// It should create a collection from the specified list of column names.
		collection = new List<string> { "ID", "Name" };
		Assert.AreSequenceEqual(["ID", "Name"], collection.Select(parameter => parameter.Column));
		Assert.AreSequenceEqual([SortOrder.Ascending, SortOrder.Ascending], collection.Select(parameter => parameter.SortOrder));

		// It should create a collection from the specified dictionary of column names and sort orders.
		collection = new OrderedDictionary { ["ID"] = "Descending", ["Name"] = "Ascending" };
		Assert.AreSequenceEqual(["ID", "Name"], collection.Select(parameter => parameter.Column));
		Assert.AreSequenceEqual([SortOrder.Descending, SortOrder.Ascending], collection.Select(parameter => parameter.SortOrder));

		collection = new OrderedDictionary<string, SortOrder> { ["ID"] = SortOrder.Descending, ["Name"] = SortOrder.Ascending };
		Assert.AreSequenceEqual(["ID", "Name"], collection.Select(parameter => parameter.Column));
		Assert.AreSequenceEqual([SortOrder.Descending, SortOrder.Ascending], collection.Select(parameter => parameter.SortOrder));
	}

	[TestMethod]
	public void Indexer() {
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));

		// It should return the order hint with the specified column name.
		var orderHint = collection["id"];
		Assert.AreEqual("ID", orderHint.Column);
		Assert.AreEqual(SortOrder.Descending, orderHint.SortOrder);
		Assert.AreEqual(orderHint, collection[0]);

		// It should throw an error if the specified column does not exist.
		Assert.Throws<KeyNotFoundException>(() => collection["foo"]);
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		Assert.AreEqual(0, collection.IndexOf("id"));
		Assert.AreEqual(1, collection.IndexOf("name"));
		Assert.AreEqual(-1, collection.IndexOf("foo"));
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the order hint with the specified column name.
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		Assert.HasCount(2, collection);
		collection.RemoveAt("name");
		Assert.HasCount(1, collection);
		collection.RemoveAt("id");
		Assert.IsEmpty(collection);

		// It should throw an error if the specified column does not exist.
		collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		Assert.Throws<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
