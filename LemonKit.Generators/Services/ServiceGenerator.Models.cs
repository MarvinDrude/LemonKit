
namespace LemonKit.Generators.Services;

internal sealed partial class ServiceGenerator
{

    private readonly record struct ServiceInfo
    {

        public readonly ClassInfo ClassInfo;
        public readonly ClassInfo InterfaceInfo;

        public readonly EquatableArray<FieldInfo> Fields;

        public readonly EquatableArray<ModulePropertyInfo> Modules;

        public ServiceInfo(
            ClassInfo classInfo,
            ClassInfo interfaceInfo,
            FieldInfo[] fields,
            ModulePropertyInfo[] modules)
        {

            ClassInfo = classInfo;
            InterfaceInfo = interfaceInfo;

            Fields = new EquatableArray<FieldInfo>(fields);
            Modules = new EquatableArray<ModulePropertyInfo>(modules);

        }

    }

    private readonly record struct ModulePropertyInfo
    {

        public readonly string TypeFullName;
        public readonly string ImplementationTypeFullName;

        public readonly string Name;

        public ModulePropertyInfo(
            string typeFullName,
            string implTypeFullName,
            string name)
        {

            TypeFullName = typeFullName;
            ImplementationTypeFullName = implTypeFullName;

            Name = name;

        }

    }

    private readonly record struct FieldInfo
    {

        public readonly string TypeFullName;
        public readonly string Name;

        public FieldInfo(
            string typeFullName,
            string name)
        {

            TypeFullName = typeFullName;
            Name = name;

        }

    }

    private readonly record struct ModuleInfo
    {

        public readonly ClassInfo ClassInfo;

        public readonly EquatableArray<ParameterInfo> ConstructorArgs;

        public readonly string InterfaceTypeFullName;

        public ModuleInfo(
            ClassInfo classInfo,
            ParameterInfo[] args,
            string interfaceTypeFullName)
        {

            ClassInfo = classInfo;

            ConstructorArgs = new EquatableArray<ParameterInfo>(args);
            InterfaceTypeFullName = interfaceTypeFullName;

        }

    }

}
