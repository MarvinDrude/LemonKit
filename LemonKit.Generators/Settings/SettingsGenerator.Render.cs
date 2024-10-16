
namespace LemonKit.Generators.Settings;

internal sealed partial class SettingsGenerator
{

    private static void RenderSetting(
        in SourceProductionContext context,
        in SettingsInfo? settingCheck,
        in string assemblyName)
    {

        if(settingCheck is not { } setting)
        {
            return;
        }

        var token = context.CancellationToken;
        token.ThrowIfCancellationRequested();

        using var cw = new CodeWriter();

        cw.WriteLine($"#nullable enable");
        cw.WriteLine();

        cw.WriteLine($"using System;");

        if(setting.ClassInfo.NameSpace is { } nameSpace)
        {

            cw.WriteLine();
            cw.WriteLine($"namespace {nameSpace};");

        }
        cw.WriteLine();

        cw.WriteLine($"{ClassInfoBuilder.GetClassString(setting.ClassInfo, true)} {{");
        cw.UpIndent();
        cw.WriteLine();

        cw.WriteLine($"public static ISettings Create(ISettingsContainer container) {{");
        cw.UpIndent();
        cw.WriteLine();

        token.ThrowIfCancellationRequested();
        Dictionary<string, string> instantiateMapping = [];

        if(setting.EnvironmentVariables is { Count: > 0 })
        {

            cw.WriteLine($"ArgumentNullException.ThrowIfNull(container.EnvironmentProvider, \"Environment provider not found for '{setting.ClassInfo.Name}'.\");");

            foreach(var envProperty in setting.EnvironmentVariables)
            {

                cw.WriteLine();
                cw.WriteLine($"if(!container.EnvironmentProvider.TryGetValue(\"{envProperty.Name}\", out var env{envProperty.PropertyName})) {{");

                cw.UpIndent();
                cw.WriteLine($"throw new Exception(\"{envProperty.Name} not found in environment variables.\");");
                cw.DownIndent();

                cw.WriteLine($"}}");
                instantiateMapping[envProperty.PropertyName] = $"env{envProperty.PropertyName}";

            }

            token.ThrowIfCancellationRequested();

        }

        foreach(var jsonProperty in setting.JsonFiles)
        {

            cw.WriteLine();
            cw.WriteLine($"if(!container.FileProviders.TryGetValue(\"{jsonProperty.FileName}\", out var json{jsonProperty.PropertyName}Check)) {{");

            cw.UpIndent();
            cw.WriteLine($"throw new Exception(\"{jsonProperty.FileName} not found in providers.\");");
            cw.DownIndent();

            cw.WriteLine($"}}");
            cw.WriteLine($"if(json{jsonProperty.PropertyName}Check is not JsonFileProvider<{jsonProperty.Type}> json{jsonProperty.PropertyName}) {{");

            cw.UpIndent();
            cw.WriteLine($"throw new Exception(\"{jsonProperty.FileName} has not generic type of {jsonProperty.Type}.\");");
            cw.DownIndent();

            cw.WriteLine($"}}");
            cw.WriteLine($"if(json{jsonProperty.PropertyName}.CurrentValue is not {{ }} json{jsonProperty.PropertyName}Value) {{");

            cw.UpIndent();
            cw.WriteLine($"throw new Exception(\"{jsonProperty.FileName} value is null, meaning deserialization failed.\");");
            cw.DownIndent();

            cw.WriteLine($"}}");
            instantiateMapping[jsonProperty.PropertyName] = $"json{jsonProperty.PropertyName}Value";

            token.ThrowIfCancellationRequested();

        }

        cw.WriteLine();
        cw.WriteLine($"return new {setting.ClassInfo.Name}() {{");
        cw.UpIndent();

        foreach(var keypair in instantiateMapping)
        {

            cw.WriteLine($"{keypair.Key} = {keypair.Value},");

        }

        cw.DownIndent();
        cw.WriteLine($"}};");

        cw.DownIndent();
        cw.WriteLine();
        cw.WriteLine($"}}");

        cw.WriteLine();
        cw.DownIndent();

        cw.WriteLine($"}}");

        token.ThrowIfCancellationRequested();
        context.AddSource($"Settings.{setting.ClassInfo.NameSpace ?? ""}.{setting.ClassInfo.Name}.g.cs", cw.ToString());
    }

}
