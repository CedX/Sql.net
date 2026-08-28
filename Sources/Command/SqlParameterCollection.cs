namespace Belin.Sql;

using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

/// <summary>
/// Represents a parameter of a parameterized SQL statement.
/// </summary>
public sealed class SqlParameter(string name = "?", object? value = null) {

	/// <summary>
	/// The prefixes used for parameter placeholders.
	/// </summary>
	private static readonly char[] prefixes = ['?', '@', ':', '$'];

	/// <summary>
	/// The database type of this parameter.
	/// </summary>
	public DbType? DbType { get; set; }

	/// <summary>
	/// Value indicating whether this parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.
	/// </summary>
	public ParameterDirection? Direction { get; set; }

	/// <summary>
	/// The parameter name.
	/// </summary>
	public string Name { get; set => field = NormalizeName(value); } = NormalizeName(name);

	/// <summary>
	/// Indicates the precision of numeric parameters.
	/// </summary>
	public byte? Precision { get; set; }

	/// <summary>
	/// Indicates the scale of numeric parameters.
	/// </summary>
	public byte? Scale { get; set; }

	/// <summary>
	/// The maximum size of this parameter, in bytes.
	/// </summary>
	public int? Size { get; set; }

	/// <summary>
	/// The parameter value.
	/// </summary>
	[NotNull]
	public object? Value { get; set => field = NormalizeValue(value); } = NormalizeValue(value);

	/// <summary>
	/// Creates a new parameter from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the parameter name and value.</param>
	/// <returns>The parameter corresponding to the specified tuple.</returns>
	/// <exception cref="ArgumentException">The specified array does not contain a parameter name and a value.</param>
	public static implicit operator SqlParameter(object?[] parameter) => parameter.Length == 2
		? new(Convert.ToString(parameter[0], CultureInfo.InvariantCulture) ?? "", parameter[1])
		: throw new ArgumentException("The specified array must contain a parameter name and a value.", nameof(parameter));

	/// <summary>
	/// Creates a new parameter from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the parameter name and value.</param>
	/// <returns>The parameter corresponding to the specified tuple.</returns>
	public static implicit operator SqlParameter((string Name, object? Value) parameter) => new(parameter.Name, parameter.Value);

	/// <summary>
	/// Creates a new parameter from the specified key/value pair.
	/// </summary>
	/// <param name="parameter">The key/value pair providing the parameter name and value.</param>
	/// <returns>The parameter corresponding to the specified key/value pair.</returns>
	public static implicit operator SqlParameter(KeyValuePair<string, object?> parameter) => new(parameter.Key, parameter.Value);

	/// <summary>
	/// Converts this parameter into an <see cref="IDbDataParameter"/> object.
	/// </summary>
	/// <param name="command">The command to associate with the created parameter.</param>
	/// <returns>The <see cref="IDbDataParameter"/> object corresponding to this parameter.</returns>
	public IDbDataParameter ToDbParameter(IDbCommand command) {
		var parameter = command.CreateParameter();
		parameter.ParameterName = Name;
		parameter.Value = Value;
		if (DbType is not null) parameter.DbType = DbType.Value;
		if (Direction is not null) parameter.Direction = Direction.Value;
		if (Precision is not null) parameter.Precision = Precision.Value;
		if (Scale is not null) parameter.Scale = Scale.Value;
		if (Size is not null) parameter.Size = Size.Value;
		return parameter;
	}

	/// <summary>
	/// Normalizes the specified parameter name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The normalized parameter name.</returns>
	internal static string NormalizeName(string name) =>
		name.Length == 0 ? "?" : (prefixes.Contains(name[0]) ? name : $"@{name}");

	/// <summary>
	/// Normalizes the specified parameter value.
	/// </summary>
	/// <param name="value">The parameter value.</param>
	/// <returns>The normalized parameter value.</returns>
	internal static object NormalizeValue(object? value) {
		if (value is null) return DBNull.Value;
		if (PowerShell.PSObject is null || value.GetType() != PowerShell.PSObject) return value;
		return PowerShell.PSObject.GetProperty("BaseObject")!.GetValue(value) ?? DBNull.Value;
	}
}

/// <summary>
/// Collects all parameters relevant to a parameterized SQL statement.
/// </summary>
/// <param name="parameters">The collection whose elements are copied to the parameter collection.</param>
public class SqlParameterCollection(params IEnumerable<SqlParameter> parameters): List<SqlParameter>(parameters) {

	/// <summary>
	/// Gets the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The parameter with the specified name.</returns>
	/// <exception cref="KeyNotFoundException">The specified parameter name does not exist.</exception>
	public SqlParameter this[string name] {
		get {
			var normalizedName = SqlParameter.NormalizeName(name);
			return Find(parameter => parameter.Name == normalizedName) ?? throw new KeyNotFoundException(normalizedName);
		}
	}

	/// <summary>
	/// Creates a new parameter collection from the specified array of positional parameters.
	/// </summary>
	/// <param name="parameters">The array whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified array of positional parameters.</returns>
	public static implicit operator SqlParameterCollection(object?[] parameters) => [.. parameters.Select((value, index) =>
		value is SqlParameter parameter ? parameter : new SqlParameter($"?{index + 1}", value)
	)];

	/// <summary>
	/// Creates a new parameter collection from the specified list of positional parameters.
	/// </summary>
	/// <param name="parameters">The list whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified list of positional parameters.</returns>
	public static implicit operator SqlParameterCollection(List<object?> parameters) => [.. parameters.Select((value, index) =>
		value is SqlParameter parameter ? parameter : new SqlParameter($"?{index + 1}", value)
	)];

	/// <summary>
	/// Creates a new parameter collection from the specified dictionary of named parameters.
	/// </summary>
	/// <param name="parameters">The dictionary whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified dictionary of named parameters.</returns>
	public static implicit operator SqlParameterCollection(Dictionary<string, object?> parameters) => [.. parameters.Select(entry =>
		entry.Value is SqlParameter parameter ? parameter : new SqlParameter(entry.Key, entry.Value)
	)];

	/// <summary>
	/// Creates a new parameter collection from the specified hash table of named parameters.
	/// </summary>
	/// <param name="parameters">The hash table whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified hash table of named parameters.</returns>
	public static implicit operator SqlParameterCollection(Hashtable parameters) => parameters.Cast<DictionaryEntry>().ToDictionary(
		entry => Convert.ToString(entry.Key, CultureInfo.InvariantCulture) ?? "",
		entry => entry.Value
	);

	/// <summary>
	/// Adds a new positional parameter to the end of this collection.
	/// </summary>
	/// <param name="value">The parameter value.</param>
	/// <returns>The newly added parameter.</returns>
	public SqlParameter AddWithValue(object? value) => AddWithValue($"?{Count + 1}", value);

	/// <summary>
	/// Adds a new named parameter to the end of this collection.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="value">The parameter value.</param>
	/// <returns>The newly added parameter.</returns>
	public SqlParameter AddWithValue(string name, object? value) {
		var parameter = new SqlParameter(name, value);
		Add(parameter);
		return parameter;
	}

	/// <summary>
	/// Gets a value indicating whether a parameter in this collection has the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns><see langword="true"/> if this collection contains a parameter with the specified name, otherwise <see langword="false"/>.</returns>
	public bool Contains(string name) {
		var normalizedName = SqlParameter.NormalizeName(name);
		return Exists(parameter => parameter.Name == normalizedName);
	}

	/// <summary>
	/// Returns the index of the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The index of the parameter with the specified name, or <c>-1</c> if not found.</returns>
	public int IndexOf(string name) {
		var normalizedName = SqlParameter.NormalizeName(name);
		return FindIndex(parameter => parameter.Name == normalizedName);
	}

	/// <summary>
	/// Removes the parameter with the specified name from this collection.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <exception cref="KeyNotFoundException">The specified parameter name does not exist.</exception>
	public void RemoveAt(string name) {
		try { RemoveAt(IndexOf(name)); }
		catch (ArgumentOutOfRangeException e) { throw new KeyNotFoundException(SqlParameter.NormalizeName(name), e); }
	}
}
