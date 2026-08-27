namespace Belin.Sql;

using System.Globalization;

/// <summary>
/// Defines the sort order for a database column.
/// </summary>
/// <param name="column">The name of the column for which the hint is being provided.</param>
/// <param name="sortOrder">The sort order of the column.</param>
public sealed class SqlOrderHint(string column, SortOrder sortOrder = SortOrder.Ascending) {

	/// <summary>
	/// The name of the column for which the hint is being provided.
	/// </summary>
	public string Column { get; } = column;

	/// <summary>
	/// The sort order of the column.
	/// </summary>
	public SortOrder SortOrder { get; set; } = sortOrder;

	/// <summary>
	/// Creates a new order hint from the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The command corresponding to the specified column name.</returns>
	public static implicit operator SqlOrderHint(string column) => new(column);

	/// <summary>
	/// Creates a new order hint from the specified tuple.
	/// </summary>
	/// <param name="orderHint">The tuple providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified tuple.</returns>
	/// <exception cref="ArgumentException">The specified array does not contain a column name and a sort order.</param>
	public static implicit operator SqlOrderHint(object[] orderHint) => orderHint.Length == 2
		? new(Convert.ToString(orderHint[0], CultureInfo.InvariantCulture) ?? "", orderHint[1] is SortOrder sortOrder ? sortOrder : Enum.Parse<SortOrder>(Convert.ToString(orderHint[1], CultureInfo.InvariantCulture) ?? "", ignoreCase: true))
		: throw new ArgumentException("The specified array must contain a column name and a sort order.", nameof(orderHint));

	/// <summary>
	/// Creates a new order hint from the specified tuple.
	/// </summary>
	/// <param name="orderHint">The tuple providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified tuple.</returns>
	public static implicit operator SqlOrderHint((string Column, SortOrder SortOrder) orderHint) => new(orderHint.Column, orderHint.SortOrder);

	/// <summary>
	/// Creates a new order hint from the specified key/value pair.
	/// </summary>
	/// <param name="orderHint">The key/value pair providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified key/value pair.</returns>
	public static implicit operator SqlOrderHint(KeyValuePair<string, SortOrder> orderHint) => new(orderHint.Key, orderHint.Value);
}
