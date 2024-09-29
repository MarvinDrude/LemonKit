
using Microsoft.CodeAnalysis;

namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private static ProcessorInfo? TransformProcessor(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token) {

        token.ThrowIfCancellationRequested();

        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var classInfo = ClassInfoBuilder.GetInfo(symbol);

        token.ThrowIfCancellationRequested();

        if(symbol.ContainingType is not null) {
            return null;
        }

        if(symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(static method => !method.IsStatic)
            .Where(static method => method.Name is "Execute")
            .ToList() is not [var execute]) {
            return null;
        }

        if(execute.Parameters is { Length: 0 }) {
            return null;
        }

        if(execute.ReturnsVoid) {
            return null;
        }

        token.ThrowIfCancellationRequested();

        var parameters = execute.Parameters.GetParameterInfos();
        token.ThrowIfCancellationRequested();

        ParameterInfo? inputParameter = null;

        foreach(var parameter in parameters) {
            foreach(var attr in parameter.Attributes) {
                if(attr.FullName is "global::LemonKit.Processors.Attributes.InputAttribute") {
                    inputParameter = parameter;
                    break;
                }
            }
            if(inputParameter is { }) {
                break;
            }
        }

        if(inputParameter is not { } input) {
            return null;
        }

        token.ThrowIfCancellationRequested();
        var originInput = execute.Parameters[input.Index];
        var inputType = originInput.Type.GetRTypeInfo();

        token.ThrowIfCancellationRequested();

        if(execute.GetTaskInnerReturnType() is not { } outputSymbol) {
            return null;
        }

        var outputType = outputSymbol.GetRTypeInfo();
        token.ThrowIfCancellationRequested();

        var attributes = symbol.GetAttributes();
        AttributeData? attrProcessor = null;
        AttributeData? attrProcedures = null;

        foreach(var attribute in attributes) {

            if(attribute.AttributeClass is not { } attrClass) {
                continue;
            }

            switch(attrClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) {

                case "global::LemonKit.Processors.Attributes.ProcessorAttribute":
                    attrProcessor = attribute;
                    break;
                case "global::LemonKit.Processors.Attributes.ProceduresAttribute":
                    attrProcedures = attribute;
                    break;

            }

        }

        if(attrProcessor is not { } processor) {
            return null;
        }

        token.ThrowIfCancellationRequested();
        bool useAssemblyProcedures = processor.ConstructorArguments[0].Value is true;
        ProcedureInfo[]? procedures = null;

        if(attrProcedures is not null) {
            procedures = GetProcedures(attrProcedures, token);
        }

        return new ProcessorInfo(
            classInfo,
            inputType,
            outputType,
            useAssemblyProcedures,
            procedures,
            parameters,
            GetApiInfo(attributes, symbol));

    }

    private static ProcessorApiInfo? GetApiInfo(
        ImmutableArray<AttributeData> attributes,
        INamedTypeSymbol symbol) {

        if(GetEndpointAttribute(attributes) is not { } attribute) {
            return null;
        }

        if(attribute.ConstructorArguments.FirstOrDefault().Value is not string path) {
            return null;
        }

        var allowAnonymous = attributes.Any(attr => IsAspNetAllowAnonymous(attr.AttributeClass));
        var authorize = attributes.FirstOrDefault(attr => IsAspNetAuthorize(attr.AttributeClass));

        bool isAuthorize = authorize != null;
        string policy = string.Empty;

        if(authorize is not null) {

            if(authorize.ConstructorArguments.Length > 0) {

                policy = (string)authorize.ConstructorArguments[0].Value!;

            } else if(authorize.NamedArguments.Length > 0) {

                foreach(var arg in authorize.NamedArguments) {

                    if(arg.Key != "Policy") {
                        return null;
                    }

                    if(arg.Value.Value is not string pol) {
                        return null;
                    }

                    policy = pol;

                }

            }

        }

        if(GetHttpMethod(attribute) is not { } httpMethod) {
            return null;
        }

        bool hasConfigure = HasConfigureMethod(symbol);

        return new ProcessorApiInfo(
            httpMethod,
            path,
            allowAnonymous,
            isAuthorize,
            policy,
            hasConfigure);

    }

    private static bool HasConfigureMethod(INamedTypeSymbol symbol) {

        return symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Any(method => method is {
                Name: "Configure",
                IsStatic: false,
                Parameters: [{ Type: { } paramType }]
            }
            && paramType is {
                Name: "IEndpointConventionBuilder",
                ContainingNamespace: {
                    Name: "Builder",
                    ContainingNamespace: {
                        Name: "AspNetCore",
                        ContainingNamespace: {
                            Name: "Microsoft",
                            ContainingNamespace.IsGlobalNamespace: true,
                        },
                    },
                },
            });

    }

    private static string? GetHttpMethod(AttributeData attribute) {

        if(attribute.ConstructorArguments is not { Length: >= 2 }) {
            return null;
        }

        return attribute
            .ConstructorArguments[1]
            .Value!.ToString();

    }

    private static AttributeData? GetEndpointAttribute(ImmutableArray<AttributeData> attributes) {

        return attributes.FirstOrDefault(attr => IsEndpointAttribute(attr.AttributeClass));

    }

    private static bool IsEndpointAttribute(ITypeSymbol? symbol) {

        return HttpMethods.Any(method => IsEndpointAttribute(symbol, method));

    }

    private static bool IsEndpointAttribute(ITypeSymbol? symbol, string method) {

        return symbol is { }
            && symbol.Name == $"{method}EndpointAttribute"
            && symbol.ContainingNamespace is {
                Name: "Apis",
                ContainingNamespace: {
                    Name: "Processors",
                    ContainingNamespace: {
                        Name: "LemonKit",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            };

    }

    private static bool IsAspNetAuthorize(ITypeSymbol? symbol) {

        return symbol is { Name: "AuthorizeAttribute" }
            && IsAspNetAuthorizationAttribute(symbol);

    }

    private static bool IsAspNetAllowAnonymous(ITypeSymbol? symbol) {

        return symbol is { Name: "AllowAnonymousAttribute" }
            && IsAspNetAuthorizationAttribute(symbol);

    }

    private static bool IsAspNetAuthorizationAttribute(ITypeSymbol? symbol) {

        return symbol is { }
            && symbol.ContainingNamespace is {
                Name: "Authorization",
                ContainingNamespace: {
                    Name: "AspNetCore",
                    ContainingNamespace: {
                        Name: "Microsoft",
                        ContainingNamespace.IsGlobalNamespace: true,
                    },
                },
            };

    }

    private static readonly string[] HttpMethods = ["Get", "Post", "Put", "Patch", "Delete"];

}
