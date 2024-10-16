
namespace LemonKit.Generators.Settings;

internal sealed partial class SettingsGenerator
{

    private readonly record struct SettingsInfo
    {

        public readonly ClassInfo ClassInfo;

        public readonly EquatableArray<EnvironmentVariableInfo> EnvironmentVariables;

        public readonly EquatableArray<JsonFileInfo> JsonFiles;

        public SettingsInfo(
            ClassInfo classInfo,
            EnvironmentVariableInfo[] envVars,
            JsonFileInfo[] jsonVars)
        {

            ClassInfo = classInfo;

            EnvironmentVariables = new EquatableArray<EnvironmentVariableInfo>(envVars);
            JsonFiles = new EquatableArray<JsonFileInfo>(jsonVars);

        }

    }

    private readonly record struct EnvironmentVariableInfo
    {

        public readonly string PropertyName;
        public readonly string Name;
        public readonly string Type;

        public EnvironmentVariableInfo(
            string propertyName,
            string name,
            string type)
        {

            PropertyName = propertyName;
            Name = name;
            Type = type;

        }

    }

    private readonly record struct JsonFileInfo
    {

        public readonly string PropertyName;
        public readonly string FileName;
        public readonly string Type;

        public JsonFileInfo(
            string propertyName,
            string fileName,
            string type)
        {

            PropertyName = propertyName;
            FileName = fileName;
            Type = type;

        }

    }

}
