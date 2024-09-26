
namespace LemonKit.Processors.Attributes;

/// <summary>
/// Used to mark a class that should generator processor code
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ProcessorAttribute : Attribute {

    public bool UseAssemblyProcedures { get; }

    public ProcessorAttribute(
        bool useAssemblyProcedures = true) {

        UseAssemblyProcedures = useAssemblyProcedures;

    }

}
