
namespace LemonKit.Processors.Attributes;

/// <summary>
/// Used to indicate what procedures are used
/// <para>
/// If used for a class that is a processor, then this procedure is added to the logic
/// </para>
/// <para>
/// If used with the assembly (<c>[assembly: Procedures()]</c>), then this procedure is added to all matching processors
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
public sealed class ProceduresAttribute : Attribute {

    public Type[] Types { get; }

    public ProceduresAttribute(params Type[] types) {

        Types = types;

    }

}
