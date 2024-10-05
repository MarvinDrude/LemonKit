
namespace LemonKit.Generators.Observe;

internal sealed partial class ObserveGenerator {

    private static void Render(
        SourceProductionContext context,
        ObserveInfo? observeInfo) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

        if(observeInfo is not { } observe) {
            return;
        }

        using var cw = new CodeWriter();

        token.ThrowIfCancellationRequested();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine($"using System.Threading;");
        cw.WriteLine($"using System.Diagnostics;");
        cw.WriteLine($"using System.Diagnostics.Metrics;");

        if(observe.ClassInfo.NameSpace is { } nameSpace) {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace};");

        }
        cw.WriteLine();

        cw.WriteLine($"{ClassInfoBuilder.GetClassString(observe.ClassInfo)} {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"private static readonly ActivitySource _ActivitySource = ObserveContainer.CreateActivitySource(\"{observe.ActivitySourceName}\", \"{observe.Version}\");");
        cw.WriteLineIf(observe.MeterName is not null, $"private static readonly Meter _Meter = ObserveContainer.CreateMeter(\"{observe.MeterName}\", \"{observe.Version}\");");

        cw.WriteLine();
        cw.WriteLine($"public static void InitializeObservability() {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"_ = _ActivitySource.Name; // make sure the static fields are initialized by accessing ones at start");
        cw.WriteLineIf(observe.MeterName is not null, $"_ = _Meter.Name;");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.WriteLine($"private static Activity? StartActivity(string name) {{");
        cw.UpIndent();

        cw.WriteLine("return _ActivitySource.StartActivity(name);");

        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.WriteLine($"private static Activity? StartActivity([CallerMemberName] string methodName = \"\", string className = \"{observe.ClassInfo.Name}\") {{");
        cw.UpIndent();

        cw.WriteLine($"return _ActivitySource.StartActivity($\"{{className}}.{{methodName}}\");");

        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"{observe.ClassInfo.NameSpace ?? "Global"}.{observe.ClassInfo.Name}.g.cs", cw.ToString());

    }

    private static void RenderContainer(
        SourceProductionContext context,
        ImmutableArray<ObserveInfo?> observeInfos) {

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

        using var cw = new CodeWriter();
        token.ThrowIfCancellationRequested();

        cw.WriteLine("#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");
        cw.WriteLine();

        cw.WriteLine($"namespace LemonKit.Observe;");
        cw.WriteLine();

        cw.WriteLine($"public static class ObserveContainerExtensions {{");
        cw.UpIndent();

        cw.WriteLine();
        cw.WriteLine($"public static void Init() {{");
        cw.UpIndent();
        cw.WriteLine();

        foreach(var observeInfo in observeInfos) {

            if(observeInfo is not { } observe) {
                continue;
            }

            cw.WriteLine($"{observe.ClassInfo.FullTypeName}.InitializeObservability();");

        }

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();
        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"LemonKit.Observe.ObserveContainer.g.cs", cw.ToString());

    }

}