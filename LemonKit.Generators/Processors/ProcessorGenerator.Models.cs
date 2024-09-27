
namespace LemonKit.Generators.Processors;

public partial class ProcessorGenerator {

    private readonly record struct ProcedureInfo {

        public readonly ClassInfo ClassInfo;

        public readonly string Type;
        public readonly string TypeNoGenerics;

        public readonly string InputType;
        public readonly string OutputType;

        public ProcedureInfo(
            ClassInfo classInfo,
            string type,
            string typeNoGenerics,
            string inputType,
            string outputType) {

            ClassInfo = classInfo;

            Type = type;
            TypeNoGenerics = typeNoGenerics;

            InputType = inputType;
            OutputType = outputType;

        }

    }

    private readonly record struct ProcessorInfo {

        public readonly ClassInfo ClassInfo;

        public readonly RTypeInfo InputType;
        public readonly RTypeInfo OutputType;

        public readonly bool UseAssemblyProcedures;

        public readonly EquatableArray<ProcedureInfo> Procedures;
        public readonly EquatableArray<ParameterInfo> Parameters;

        public ProcessorInfo(
            ClassInfo classInfo,
            RTypeInfo inputType,
            RTypeInfo outputType,
            bool useAssemblyProcedures,
            ProcedureInfo[] procedures,
            ParameterInfo[] parameters) {

            ClassInfo = classInfo;

            InputType = inputType;
            OutputType = outputType;

            UseAssemblyProcedures = useAssemblyProcedures;

            Procedures = new EquatableArray<ProcedureInfo>(procedures);
            Parameters = new EquatableArray<ParameterInfo>(parameters);

        }

    }

    private readonly record struct ProcedureClassInfo {

        public readonly ClassInfo ClassInfo;

        public readonly RTypeInfo InputType;
        public readonly RTypeInfo OutputType;

        public readonly EquatableArray<ParameterInfo> Parameters;

        public ProcedureClassInfo(
            ClassInfo classInfo,
            RTypeInfo inputType,
            RTypeInfo outputType,
            ParameterInfo[] parameters) {

            ClassInfo = classInfo;

            InputType = inputType;
            OutputType = outputType;

            Parameters = new EquatableArray<ParameterInfo>(parameters);

        }

    }

}
