namespace Belin.Sql;

using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Runtime.CompilerServices;

/// <summary>
/// Maps data records to entity objects.
/// </summary>
public sealed class SqlMapper {

	/// <summary>
	/// The singleton instance of the data mapper.
	/// </summary>
	public static SqlMapper Instance { get; } = new();

	/// <summary>
	/// The mapping between the entity types and their associated database tables.
	/// </summary>
	private static readonly ConcurrentDictionary<Type, DbTableInfo> mapping = [];

	/// <summary>
	/// Creates a new data mapper.
	/// </summary>
	private SqlMapper() {}

	/// <summary>
	/// Converts the specified object into an equivalent value of the specified type.
	/// </summary>
	/// <param name="value">The object to convert.</param>
	/// <param name="column">The column providing the type of object to return.</param>
	/// <returns>The value of the given type corresponding to the specified object.</returns>
	public object? ChangeType(object? value, DbColumnInfo column) => ChangeType(value, column.PropertyType, column.IsNullable);

	/// <summary>
	/// Converts the specified object into an equivalent value of the specified type.
	/// </summary>
	/// <param name="value">The object to convert.</param>
	/// <param name="conversionType">The type of object to return.</param>
	/// <param name="isNullable">Value indicating whether the specified conversion type is nullable.</param>
	/// <returns>The value of the given type corresponding to the specified object.</returns>
	public object? ChangeType(object? value, Type conversionType, bool isNullable = false) {
		var nullableType = Nullable.GetUnderlyingType(conversionType);
		var targetType = nullableType ?? conversionType;

		if (value is not null && !Convert.IsDBNull(value)) return true switch {
			true when targetType.IsEnum && value is string stringValue => Enum.Parse(targetType, stringValue, ignoreCase: true),
			true when targetType.IsEnum => Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType), CultureInfo.InvariantCulture)),
			_ => targetType.IsInstanceOfType(value) ? value : Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture),
		};

		return true switch {
			true when nullableType is not null => null,
			true when targetType.IsValueType => RuntimeHelpers.GetUninitializedObject(targetType),
			true when targetType == typeof(string) => isNullable ? null : string.Empty,
			_ => isNullable ? null : Activator.CreateInstance(targetType)
		};
	}

	/// <summary>
	/// Creates a new dyamic object from the specified data record.
	/// </summary>
	/// <param name="record">A data record providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public ExpandoObject CreateInstance(IDataRecord record) => CreateInstance<ExpandoObject>(record);

	/// <summary>
	/// Creates a new object of the given type from the specified data record.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="record">A data record providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public T CreateInstance<T>(IDataRecord record) where T: new() => CreateInstance<T>(SplitOn(record).First());

	/// <summary>
	/// Creates a new object of the given type from the specified data record.
	/// </summary>
	/// <param name="type">The object type.</param>
	/// <param name="record">A data record providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public object CreateInstance(Type type, IDataRecord record) => CreateInstance(type, SplitOn(record).First());

	/// <summary>
	/// Creates a new object tuple of the given types from the specified data record.
	/// </summary>
	/// <param name="types">The object types.</param>
	/// <param name="record">A data record providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>The newly created object tuple.</returns>
	public ITuple CreateInstance(Type[] types, IDataRecord record, params string[] splitOn) {
		if (types.Length < 2 || types.Length > 7) throw new ArgumentException("The number of object types is invalid.", nameof(types));
		if (splitOn.Length == 0) splitOn = [.. types.Skip(1).Select(_ => "Id")];
		else if (splitOn.Length != types.Length - 1) throw new ArgumentException("The number of split fields is invalid.", nameof(splitOn));

		var records = SplitOn(record, splitOn);
		var objects = new List<object?>(records.Count);
		for (var index = 0; index < types.Length; index++)
			objects.Add(records.Count <= index || IsNullObject(records[index]) ? null : CreateInstance(types[index], records[index]));

		return objects.Count switch {
			2 => (objects[0], objects[1]),
			3 => (objects[0], objects[1], objects[2]),
			4 => (objects[0], objects[1], objects[2], objects[3]),
			5 => (objects[0], objects[1], objects[2], objects[3], objects[4]),
			6 => (objects[0], objects[1], objects[2], objects[3], objects[4], objects[5]),
			_ => (objects[0], objects[1], objects[2], objects[3], objects[4], objects[5], objects[6])
		};
	}

	/// <summary>
	/// Creates a new object pair of the given types from the specified data record.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first object.</typeparam>
	/// <typeparam name="TItem2">The type of the second object.</typeparam>
	/// <param name="record">A data record providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The field from which to split and read the next object.</param>
	/// <returns>The newly created object pair.</returns>
	public (TItem1, TItem2) CreateInstance<TItem1, TItem2>(IDataRecord record, string splitOn = "Id") where TItem1: new() where TItem2: new() {
		var records = SplitOn(record, splitOn);
		return (
			IsNullObject(records[0]) ? default! : CreateInstance<TItem1>(records[0]),
			records.Count <= 1 || IsNullObject(records[1]) ? default! : CreateInstance<TItem2>(records[1])
		);
	}

	/// <summary>
	/// Creates a new object tuple of the given types from the specified data record.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first object.</typeparam>
	/// <typeparam name="TItem2">The type of the second object.</typeparam>
	/// <typeparam name="TItem3">The type of the third object.</typeparam>
	/// <param name="record">A data record providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>The newly created object tuple.</returns>
	public (TItem1, TItem2, TItem3) CreateInstance<TItem1, TItem2, TItem3>(IDataRecord record, (string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() {
		var (firstField, secondField) = splitOn ?? ("Id", "Id");
		var records = SplitOn(record, firstField, secondField);
		return (
			IsNullObject(records[0]) ? default! : CreateInstance<TItem1>(records[0]),
			records.Count <= 1 || IsNullObject(records[1]) ? default! : CreateInstance<TItem2>(records[1]),
			records.Count <= 2 || IsNullObject(records[2]) ? default! : CreateInstance<TItem3>(records[2])
		);
	}

	/// <summary>
	/// Creates a new object tuple of the given types from the specified data record.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first object.</typeparam>
	/// <typeparam name="TItem2">The type of the second object.</typeparam>
	/// <typeparam name="TItem3">The type of the third object.</typeparam>
	/// <typeparam name="TItem4">The type of the fourth object.</typeparam>
	/// <param name="record">A data record providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>The newly created object tuple.</returns>
	public (TItem1, TItem2, TItem3, TItem4) CreateInstance<TItem1, TItem2, TItem3, TItem4>(IDataRecord record, (string, string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
		var (firstField, secondField, thirdField) = splitOn ?? ("Id", "Id", "Id");
		var records = SplitOn(record, firstField, secondField, thirdField);
		return (
			IsNullObject(records[0]) ? default! : CreateInstance<TItem1>(records[0]),
			records.Count <= 1 || IsNullObject(records[1]) ? default! : CreateInstance<TItem2>(records[1]),
			records.Count <= 2 || IsNullObject(records[2]) ? default! : CreateInstance<TItem3>(records[2]),
			records.Count <= 3 || IsNullObject(records[3]) ? default! : CreateInstance<TItem4>(records[3])
		);
	}

	/// <summary>
	/// Creates a new dynamic object from the specified hash table.
	/// </summary>
	/// <param name="properties">A hash table providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public ExpandoObject CreateInstance(Hashtable properties) =>
		CreateInstance<ExpandoObject>(properties.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString() ?? "", entry => entry.Value));

	/// <summary>
	/// Creates a new dynamic object from the specified dictionary.
	/// </summary>
	/// <param name="properties">A dictionary providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public ExpandoObject CreateInstance(IDictionary<string, object?> properties) =>
		CreateInstance<ExpandoObject>(properties);

	/// <summary>
	/// Creates a new object of a given type from the specified hash table.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="properties">A hash table providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public T CreateInstance<T>(Hashtable properties) where T: new() =>
		CreateInstance<T>(properties.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString() ?? "", entry => entry.Value));

	/// <summary>
	/// Creates a new object of a given type from the specified dictionary.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="properties">A dictionary providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public T CreateInstance<T>(IDictionary<string, object?> properties) where T: new() =>
		(T) CreateInstance(typeof(T), properties);

	/// <summary>
	/// Creates a new object of a given type from the specified hash table.
	/// </summary>
	/// <param name="type">The object type.</param>
	/// <param name="properties">A hash table providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public object CreateInstance(Type type, Hashtable properties) =>
		CreateInstance(type, properties.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString() ?? "", entry => entry.Value));

	/// <summary>
	/// Creates a new object of a given type from the specified dictionary.
	/// </summary>
	/// <param name="type">The object type.</param>
	/// <param name="properties">A dictionary providing the properties to be set on the created object.</param>
	/// <returns>The newly created object.</returns>
	public object CreateInstance(Type type, IDictionary<string, object?> properties) {
		if (type == typeof(ExpandoObject)) {
			var expandoObject = (IDictionary<string, object?>) new ExpandoObject();
			foreach (var (key, value) in properties) expandoObject.Add(key, value);
			return expandoObject;
		}

		if (PowerShell.PSObject is not null && PowerShell.PSNoteProperty is not null && type == PowerShell.PSObject) {
			var psCustomObject = Activator.CreateInstance(PowerShell.PSObject)!;
			dynamic psProperties = PowerShell.PSObject.GetProperty("Properties")!.GetValue(psCustomObject)!;
			foreach (var (key, value) in properties) psProperties.Add((dynamic) Activator.CreateInstance(PowerShell.PSNoteProperty, [key, value])!);
			return psCustomObject;
		}

		var instance = Activator.CreateInstance(type)!;
		var table = GetTable(type);
		foreach (var name in properties.Keys.Where(table.Columns.ContainsKey)) {
			var column = table.Columns[name];
			if (column.CanWrite) column.SetValue(instance, ChangeType(properties[name], column));
		}

		return instance;
	}

	/// <summary>
	/// Creates new dynamic objects from the specified data reader.
	/// </summary>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <returns>An enumerable of newly created objects.</returns>
	public IEnumerable<ExpandoObject> CreateInstances(IDataReader reader) => CreateInstances<ExpandoObject>(reader);

	/// <summary>
	/// Creates new objects of the given type from the specified data reader.
	/// </summary>
	/// <typeparam name="T">The object type.</typeparam>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <returns>An enumerable of newly created objects.</returns>
	public IEnumerable<T> CreateInstances<T>(IDataReader reader) where T: new() {
		while (reader.Read()) yield return CreateInstance<T>(reader);
		reader.Close();
	}

	/// <summary>
	/// Creates new objects of the given type from the specified data reader.
	/// </summary>
	/// <param name="type">The object type.</param>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <returns>An enumerable of newly created objects.</returns>
	public IEnumerable<object> CreateInstances(Type type, IDataReader reader) {
		while (reader.Read()) yield return CreateInstance(type, reader);
		reader.Close();
	}

	/// <summary>
	/// Creates new object tuples of the given types from the specified data reader.
	/// </summary>
	/// <param name="types">The object types.</param>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>An enumerable of newly created object tuples.</returns>
	public IEnumerable<ITuple> CreateInstances(Type[] types, IDataReader reader, params string[] splitOn) {
		while (reader.Read()) yield return CreateInstance(types, reader, splitOn);
		reader.Close();
	}

	/// <summary>
	/// Creates new object pairs of the given types from the specified data reader.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first object.</typeparam>
	/// <typeparam name="TItem2">The type of the second object.</typeparam>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The field from which to split and read the next object.</param>
	/// <returns>An enumerable of newly created object pairs.</returns>
	public IEnumerable<(TItem1, TItem2)> CreateInstances<TItem1, TItem2>(IDataReader reader, string splitOn = "Id") where TItem1: new() where TItem2: new() {
		while (reader.Read()) yield return CreateInstance<TItem1, TItem2>(reader, splitOn);
		reader.Close();
	}

	/// <summary>
	/// Creates new object tuples of the given types from the specified data reader.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first object.</typeparam>
	/// <typeparam name="TItem2">The type of the second object.</typeparam>
	/// <typeparam name="TItem3">The type of the third object.</typeparam>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>An enumerable of newly created object tuples.</returns>
	public IEnumerable<(TItem1, TItem2, TItem3)> CreateInstances<TItem1, TItem2, TItem3>(IDataReader reader, (string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() {
		while (reader.Read()) yield return CreateInstance<TItem1, TItem2, TItem3>(reader, splitOn);
		reader.Close();
	}

	/// <summary>
	/// Creates new object tuples of the given types from the specified data reader.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first object.</typeparam>
	/// <typeparam name="TItem2">The type of the second object.</typeparam>
	/// <typeparam name="TItem3">The type of the third object.</typeparam>
	/// <typeparam name="TItem4">The type of the fourth object.</typeparam>
	/// <param name="reader">A data reader providing the properties to be set on the created objects.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>An enumerable of newly created object tuples.</returns>
	public IEnumerable<(TItem1, TItem2, TItem3, TItem4)> CreateInstances<TItem1, TItem2, TItem3, TItem4>(IDataReader reader, (string, string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
		while (reader.Read()) yield return CreateInstance<TItem1, TItem2, TItem3, TItem4>(reader, splitOn);
		reader.Close();
	}

	/// <summary>
	/// Gets the table information associated with the specified type.
	/// </summary>
	/// <typeparam name="T">The type to inspect.</typeparam>
	/// <returns>The table information associated with the specified type.</returns>
	public DbTableInfo GetTable<T>() where T: new() => GetTable(typeof(T));

	/// <summary>
	/// Gets the table information associated with the specified type.
	/// </summary>
	/// <param name="type">The type to inspect.</param>
	/// <returns>The table information associated with the specified type.</returns>
	public DbTableInfo GetTable(Type type) => mapping.GetOrAdd(type, type => new DbTableInfo(type));

	/// <summary>
	/// Returns a value indicating whether all values of the specified dictionary are <see langword="null"/>.
	/// </summary>
	/// <param name="dictionary">The dictionary to inspect.</param>
	/// <returns><see langword="true"/> if all values of the specified dictionary are <see langword="null"/>, otherwise <see langword="false"/>.</returns>
	internal static bool IsNullObject(Dictionary<string, object?> dictionary) => dictionary.Values.All(value => value is null);

	/// <summary>
	/// Splits the specified data record according to the specified fields.
	/// </summary>
	/// <param name="record">The data record to split.</param>
	/// <param name="fields">The fields from which to split and read the next objects.</param>
	/// <returns>A list of dictionaries representing the objects extracted from the data record.</returns>
	internal static List<Dictionary<string, object?>> SplitOn(IDataRecord record, params string[] fields) {
		var properties = new List<KeyValuePair<string, object?>>(record.FieldCount);
		for (var index = 0; index < record.FieldCount; index++) {
			var value = record[index];
			properties.Add(new(record.GetName(index), Convert.IsDBNull(value) ? null : value));
		}

		return SplitOn(properties, fields);
	}

	/// <summary>
	/// Splits the specified data record according to the specified fields.
	/// </summary>
	/// <param name="record">The data record to split.</param>
	/// <param name="fields">The fields from which to split and read the next objects.</param>
	/// <returns>A list of dictionaries representing the objects extracted from the data record.</returns>
	internal static List<Dictionary<string, object?>> SplitOn(List<KeyValuePair<string, object?>> record, params string[] fields) {
		var properties = new Dictionary<string, object?>(record.Count);
		if (fields.Length == 0) {
			foreach (var (key, value) in record) properties.TryAdd(key, value);
			return [properties];
		}

		var fieldQueue = new Queue<string>(fields);
		var records = new List<Dictionary<string, object?>>(fields.Length + 1);
		var splitOn = fieldQueue.Dequeue();

		foreach (var (index, (key, value)) in record.Index()) {
			if (index > 0 && key.Equals(splitOn, StringComparison.OrdinalIgnoreCase)) {
				records.Add(properties);
				properties = new Dictionary<string, object?>(record.Count - index);
				if (fieldQueue.TryDequeue(out var field)) splitOn = field;
			}

			properties.TryAdd(key, value);
		}

		records.Add(properties);
		return records;
	}
}
