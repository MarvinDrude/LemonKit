
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

        cw.WriteLine($"public sealed class Validator : IValidate<{validation.ClassInfo.FullTypeName}> {{");
        cw.UpIndent();
        cw.WriteLine();



        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{validation.ClassInfo.NameSpace ?? "Global"}.{validation.ClassInfo.Name}.g.cs", cw.ToString());

    }

    private static void RenderExtensions(
        SourceProductionContext context,
        ImmutableArray<ValidationInfo?> validationInfos) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

    }

}