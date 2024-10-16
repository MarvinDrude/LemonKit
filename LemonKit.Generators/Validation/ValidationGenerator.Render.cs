
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator
{

    private static void RenderValidate(
        SourceProductionContext context,
        ValidationInfo? validationInfo)
    {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

        if(validationInfo is not { } validation)
        {
            return;
        }

        using var cw = new CodeWriter();
        var services = GetServices(validation);

        token.ThrowIfCancellationRequested();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using LemonKit.Validation;");

        if(validation.ClassInfo.NameSpace is { } nameSpace)
        {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace}.{validation.ClassInfo.Name}Validation;");

        }
        else
        {

            cw.WriteLine();
            cw.WriteLine($"namespace {validation.ClassInfo.Name}Validation;");

        }
        cw.WriteLine();

        cw.WriteLine($"public sealed class {validation.ValidatorName}Validator : IValidate<{validation.ClassInfo.FullTypeName}> {{");
        cw.UpIndent();
        cw.WriteLine();

        foreach(var keypair in services)
        {

            cw.WriteLine($"private readonly {keypair.Key} {keypair.Value.InstanceName};");

        }

        cw.WriteLineIf(services.Count > 0, "");

        cw.WriteLine($"public {validation.ValidatorName}Validator(");
        cw.UpIndent();

        var parameters = services.Select(keypair => $"{keypair.Key} {keypair.Value.ParameterName}");
        cw.Write(string.Join(",\n", parameters), multiLine: true);

        cw.WriteLine($") {{");
        cw.WriteLine();

        foreach(var keypair in services)
        {

            cw.WriteLine($"{keypair.Value.InstanceName} = {keypair.Value.ParameterName};");

        }

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");
        cw.WriteLine();

        cw.WriteLine($"public ValidationResult Validate({validation.ClassInfo.FullTypeName}? input) {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"var result = new ValidationResult();");
        cw.WriteLine();

        cw.WriteLine($"if(input is null) {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"result.AddErrorCode(\"__Instance\", \"MUST_NOT_NULL\");");
        cw.WriteLine($"return result;");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");
        cw.WriteLine();

        foreach(var property in validation.Properties)
        {

            if(property.Validations.Count is 0)
            {
                continue;
            }

            foreach(var validate in property.Validations)
            {

                RenderValidation(cw, property, validate, services);

            }

        }

        if(validation.HasExtraValidateMethod
            && validation.ExtraValidateParameters is not null)
        {

            cw.WriteLine($"{validation.ClassInfo.FullTypeName}.ExtraValidate(");
            cw.UpIndent();

            for(int i = 0; i < validation.ExtraValidateParameters.Value.Count; i++)
            {

                var parameter = validation.ExtraValidateParameters.Value[i];
                var isLast = validation.ExtraValidateParameters.Value.Count - 1 == i;
                var addition = isLast ? string.Empty : ",";

                if(parameter.Type is "global::LemonKit.Validation.ValidationResult")
                {
                    cw.WriteLine($"result{addition}");
                    continue;
                }
                if(parameter.Type == validation.ClassInfo.FullTypeName)
                {
                    cw.WriteLine($"input{addition}");
                    continue;
                }
                if(!services.TryGetValue(parameter.Type, out var registration))
                {
                    return; // something went wrong
                }
                cw.WriteLine($"{registration.InstanceName}{addition}");

            }

            cw.DownIndent();
            cw.WriteLine(");");

        }

        cw.WriteLine();
        cw.WriteLine("return result;");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{validation.ClassInfo.NameSpace ?? "Global"}.{validation.ClassInfo.Name}.g.cs", cw.ToString());

    }

    private static void RenderValidation(
        CodeWriter cw,
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services)
    {

        var (parametersString, errorParametersString) = validate.ClassInfo.FullTypeName switch
        {

            "global::LemonKit.Validation.Attributes.MinLengthAttribute" => RenderMinLength(property, validate, services),
            "global::LemonKit.Validation.Attributes.MaxLengthAttribute" => RenderMaxLength(property, validate, services),
            "global::LemonKit.Validation.Attributes.EqualAttribute" => RenderEqual(property, validate, services),
            "global::LemonKit.Validation.Attributes.NotEqualAttribute" => RenderNotEqual(property, validate, services),
            "global::LemonKit.Validation.Attributes.EmptyAttribute" => RenderEmpty(property, validate, services),
            "global::LemonKit.Validation.Attributes.NotEmptyAttribute" => RenderNotEmpty(property, validate, services),
            "global::LemonKit.Validation.Attributes.ContainsAttribute" => RenderContains(property, validate, services),
            "global::LemonKit.Validation.Attributes.NotContainsAttribute" => RenderNotContains(property, validate, services),
            "global::LemonKit.Validation.Attributes.GreaterThanAttribute" => RenderGreaterThan(property, validate, services),
            "global::LemonKit.Validation.Attributes.GreaterThanOrEqualAttribute" => RenderGreaterThanOrEqual(property, validate, services),
            "global::LemonKit.Validation.Attributes.LessThanAttribute" => RenderLessThan(property, validate, services),
            "global::LemonKit.Validation.Attributes.LessThanOrEqualAttribute" => RenderLessThanOrEqual(property, validate, services),
            "global::LemonKit.Validation.Attributes.EnumValueAttribute" => RenderEnumValue(property, validate, services),
            _ => (string.Empty, string.Empty)

        };

        string genericParameter = validate.IsGenericMethod ? $"<{property.TypeFullName}>" : string.Empty;

        cw.WriteLine($"if(!{validate.ClassInfo.FullTypeName}.Validate{genericParameter}({parametersString})) {{");

        cw.UpIndent();
        cw.WriteLine($"result.AddErrorCode(\"{property.PropertyName}\", {validate.ClassInfo.FullTypeName}.TemplateError(\"{validate.ErrorCode}\"{(errorParametersString == string.Empty ? "" : ", ")}{errorParametersString}));");
        cw.DownIndent();

        cw.WriteLine($"}}");
        cw.WriteLine();

    }

    private static Dictionary<string, ServiceRegistration> GetServices(ValidationInfo validation)
    {

        Dictionary<string, ServiceRegistration> result = [];
        int count = 0;

        if(validation.HasExtraValidateMethod
            && validation.ExtraValidateParameters is not null)
        {

            foreach(var parameter in validation.ExtraValidateParameters)
            {

                if(parameter.Type is "global::LemonKit.Validation.ValidationResult")
                {
                    continue;
                }
                if(parameter.Type == validation.ClassInfo.FullTypeName)
                {
                    continue;
                }
                if(result.ContainsKey(parameter.Type))
                {
                    continue;
                }

                result[parameter.Type] = new ServiceRegistration()
                {
                    InstanceName = $"_Service{count}",
                    ParameterName = $"service{count}"
                };
                count++;

            }

        }

        foreach(var property in validation.Properties)
        {

            if(property.Validations.Count is 0)
            {
                continue;
            }

            foreach(var validate in property.Validations)
            {

                if(!validate.IsService || validate.ServiceTypeFullName is null)
                {
                    continue;
                }

                if(result.ContainsKey(validate.ServiceTypeFullName))
                {
                    continue;
                }

                result[validate.ServiceTypeFullName] = new ServiceRegistration()
                {
                    InstanceName = $"_Service{count}",
                    ParameterName = $"service{count}"
                };
                count++;

            }

        }

        return result;

    }

    private static void RenderExtensions(
        SourceProductionContext context,
        ImmutableArray<ValidationInfo?> validationInfos)
    {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using System.Text;");
        cw.WriteLine($"using Microsoft.Extensions.DependencyInjection;");
        cw.WriteLine($"using LemonKit.Validation;");

        cw.WriteLine();
        cw.WriteLine($"namespace LemonKit.Extensions;");
        cw.WriteLine();

        cw.WriteLine($"internal static class IServiceCollectionValidationExtensions {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"public static IServiceCollection AddKitValidators(this IServiceCollection collection) {{");
        cw.UpIndent();
        cw.WriteLine();

        foreach(var validationInfo in validationInfos)
        {

            if(validationInfo is not { } validation)
            {
                continue;
            }

            string nameSpaceString;

            if(validation.ClassInfo.NameSpace is { } nameSpace)
            {
                nameSpaceString = $"{nameSpace}.{validation.ClassInfo.Name}Validation";
            }
            else
            {
                nameSpaceString = $"{validation.ClassInfo.Name}Validation";
            }

            cw.WriteLine($"_ = collection.AddSingleton<IValidate<{validation.ClassInfo.FullTypeName}>, {nameSpaceString}.{validation.ValidatorName}Validator>();");

        }

        cw.WriteLine();
        cw.WriteLine($"return collection;");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{"LemonKit.Extensions"}.{"IServiceCollectionValidationExtensions"}.g.cs", cw.ToString());

    }

}