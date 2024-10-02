
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator {

    private static void RenderValidate(
        SourceProductionContext context,
        ValidationInfo? validationInfo) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

    }

    private static void RenderExtensions(
        SourceProductionContext context,
        ImmutableArray<ValidationInfo?> validationInfos) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

    }

}