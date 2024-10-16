
namespace LemonKit.Generators.Observe;

internal sealed partial class ObserveGenerator
{

    private static ObserveInfo? Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();

        if(GetObserveAttribute(symbol.GetAttributes()) is not { } attribute)
        {
            return null;
        }

        if(attribute.ConstructorArguments is not { Length: 3 } args)
        {
            return null;
        }

        token.ThrowIfCancellationRequested();

        if(args is not [{ IsNull: false, Value: not null } aSourceConstant, { } meterNameConstant, { } versionConstant])
        {
            return null;
        }

        string activitySourceName = aSourceConstant.Value.ToString();
        string? meterName = null;
        string? version = null;

        if(meterNameConstant is { IsNull: false, Value: { } val })
        {
            meterName = val.ToString();
        }

        if(versionConstant is { IsNull: false, Value: { } valVersion })
        {
            version = valVersion.ToString();
        }
        else
        {
            version = "1.0.0";
        }

        return new ObserveInfo(
            classInfo,
            activitySourceName,
            meterName,
            version);

    }

    private static AttributeData? GetObserveAttribute(ImmutableArray<AttributeData> attributes)
    {

        return attributes.FirstOrDefault(x => IsObserveAttribute(x.AttributeClass));

    }

    private static bool IsObserveAttribute(ITypeSymbol? symbol)
    {

        return symbol is
        {
            Name: "ObserveAttribute",
            ContainingNamespace:
            {
                Name: "Attributes",
                ContainingNamespace:
                {
                    Name: "Observe",
                    ContainingNamespace:
                    {
                        Name: "LemonKit",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };

    }

}
