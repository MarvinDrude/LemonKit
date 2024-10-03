
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator {

    private static void RenderValidate(
        SourceProductionContext context,
        ValidationInfo? validationInfo) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

        if(validationInfo is not { } validation) {
            return;
        }

        using var cw = new CodeWriter();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using LemonKit.Validation;");

        if(validation.ClassInfo.NameSpace is { } nameSpace) {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace}.{validation.ClassInfo.Name}Validation;");

        } else {

            cw.WriteLine();
            cw.WriteLine($"namespace {validation.ClassInfo.Name}Validation;");

        }
        cw.WriteLine();

        cw.WriteLine($"public sealed class {validation.ValidatorName}Validator : IValidate<{validation.ClassInfo.FullTypeName}> {{");
        cw.UpIndent();
        cw.WriteLine();



        cw.WriteLine($"public ValidationResult Validate({validation.ClassInfo.FullTypeName}? input) {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"var result = new ValidationResult();");
        cw.WriteLine();

        foreach(var property in validation.Properties) {

            if(property.Validations.Count is 0) {
                continue;
            }

            foreach(var validate in property.Validations) {

                RenderValidation(cw, property, validate);

            }

        }

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
        ValidatePropertyInfo validate) {

        string parametersString = property.ValidationTypeFullName switch {

            "global::LemonKit.Validation.Attributes.MinLengthAttribute" => RenderMinLength(property, validate),
            _ => string.Empty

        };

        cw.WriteLine($"if(!{property.ValidationTypeFullName}.Validate) {{");



        cw.WriteLine($"}}");
        cw.WriteLine();

    }

    private static void RenderExtensions(
        SourceProductionContext context,
        ImmutableArray<ValidationInfo?> validationInfos) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

    }

}