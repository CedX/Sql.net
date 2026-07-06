namespace Belin.Sql;

/// <summary>
/// Provides common types for interoperability with PowerShell.
/// </summary>
internal static class PowerShell {

	/// <summary>
	/// The type of the <c>PSNoteProperty</c> class.
	/// </summary>
	public static Type? PSNoteProperty { get; } = Type.GetType("System.Management.Automation.PSNoteProperty, System.Management.Automation");

	/// <summary>
	/// The type of the <c>PSObject</c> class.
	/// </summary>
	public static Type? PSObject { get; } = Type.GetType("System.Management.Automation.PSObject, System.Management.Automation");
}
