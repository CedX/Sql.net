namespace Belin.Sql;

using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

/// <summary>
/// Defines the sort order for a database column.
/// </summary>
/// <param name="column">The name of the column for which the hint is being provided.</param>
/// <param name="sortOrder">The sort order of the column.</param>
public class OrderHint(string column, SortOrder sortOrder = SortOrder.Ascending) {

	/// <summary>
	/// The name of the column for which the hint is being provided.
	/// </summary>
	public string Column { get; set; } = column;

	/// <summary>
	/// The sort order of the column.
	/// </summary>
	public SortOrder SortOrder { get; set; } = sortOrder;

	/// <summary>
	/// Creates a new order hint from the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The command corresponding to the specified column name.</returns>
	public static implicit operator OrderHint(string column) => new(column);

	/// <summary>
	/// Creates a new order hint from the specified tuple.
	/// </summary>
	/// <param name="orderHint">The tuple providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified tuple.</returns>
	/// <exception cref="ArgumentException">The specified array does not contain a column name and a sort order.</exception>
	public static implicit operator OrderHint(object[] orderHint) => orderHint.Length == 2
		? new(Convert.ToString(orderHint[0], CultureInfo.InvariantCulture) ?? "", orderHint[1] is SortOrder sortOrder ? sortOrder : Enum.Parse<SortOrder>(Convert.ToString(orderHint[1], CultureInfo.InvariantCulture) ?? "", ignoreCase: true))
		: throw new ArgumentException("The specified array must contain a column name and a sort order.", nameof(orderHint));

	/// <summary>
	/// Creates a new order hint from the specified tuple.
	/// </summary>
	/// <param name="orderHint">The tuple providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified tuple.</returns>
	public static implicit operator OrderHint((string Column, SortOrder SortOrder) orderHint) => new(orderHint.Column, orderHint.SortOrder);
}

/// <summary>
/// A collection of hints describing the sort order of columns.
/// </summary>
/// <param name="orderHints">The collection whose elements are copied to the order hint collection.</param>
public class OrderHintCollection(params IEnumerable<OrderHint> orderHints): List<OrderHint>(orderHints) {

	/// <summary>
	/// Gets the order hint with the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The order hint with the specified column name.</returns>
	/// <exception cref="KeyNotFoundException">The specified column name does not exist.</exception>
	public OrderHint this[string column] =>
		Find(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase)) ?? throw new KeyNotFoundException(column);

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator OrderHintCollection(object[] columns) =>
		[.. columns.Select(value => new OrderHint(Convert.ToString(value, CultureInfo.InvariantCulture) ?? "", SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator OrderHintCollection(string[] columns) =>
		[.. columns.Select(value => new OrderHint(value, SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator OrderHintCollection(List<string> columns) =>
		[.. columns.Select(value => new OrderHint(value, SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified dictionary of column names and sort orders.
	/// </summary>
	/// <param name="orderHints">The dictionary whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified dictionary of column names and sort orders.</returns>
	public static implicit operator OrderHintCollection(OrderedDictionary orderHints) => [.. orderHints.Cast<DictionaryEntry>().Select(entry => {
		var value = entry.Value is SortOrder sortOrder ? sortOrder : Enum.Parse<SortOrder>(Convert.ToString(entry.Value, CultureInfo.InvariantCulture) ?? "", ignoreCase: true);
		return new OrderHint(Convert.ToString(entry.Key, CultureInfo.InvariantCulture) ?? "", value);
	})];

	/// <summary>
	/// Creates a new order hint collection from the specified dictionary of column names and sort orders.
	/// </summary>
	/// <param name="orderHints">The dictionary whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified dictionary of column names and sort orders.</returns>
	public static implicit operator OrderHintCollection(OrderedDictionary<string, SortOrder> orderHints) =>
		[.. orderHints.Select(entry => new OrderHint(entry.Key, entry.Value))];

	/// <summary>
	/// Gets a value indicating whether an order hint in this collection has the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns><see langword="true"/> if this collection contains an order hint with the specified column name, otherwise <see langword="false"/>.</returns>
	public bool Contains(string column) => Exists(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase));

	/// <summary>
	/// Returns the index of the order hint with the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The index of the order hint with the specified column name, or <c>-1</c> if not found.</returns>
	public int IndexOf(string column) => FindIndex(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase));

	/// <summary>
	/// Removes the order hint with the specified column name from this collection.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <exception cref="KeyNotFoundException">The specified column name does not exist.</exception>
	public void RemoveAt(string column) {
		try { RemoveAt(IndexOf(column)); }
		catch (ArgumentOutOfRangeException e) { throw new KeyNotFoundException(column, e); }
	}
}

/// <summary>
/// Specifies how rows of data are sorted.
/// </summary>
public enum SortOrder {

	/// <summary>
	/// The rows are sorted in ascending order.
	/// </summary>
	Ascending,

	/// <summary>
	/// The rows are sorted in descending order.
	/// </summary>
	Descending
}
