
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator
{

    private readonly record struct ValidationInfo
    {

        public readonly ClassInfo ClassInfo;

        public readonly bool HasExtraValidateMethod;
        public readonly bool IsReferenceType;

        public readonly string ValidatorName;

        public readonly EquatableArray<ParameterInfo>? ExtraValidateParameters;
        public readonly EquatableArray<ValidationPropertyInfo> Properties;

        public ValidationInfo(
            ClassInfo classInfo,
            bool hasExtraValidateMethod,
            bool isReferenceType,
            string validatorName,
            ParameterInfo[]? extraParameters,
            ValidationPropertyInfo[] properties)
        {

            ClassInfo = classInfo;

            HasExtraValidateMethod = hasExtraValidateMethod;
            IsReferenceType = isReferenceType;

            ValidatorName = validatorName;

            ExtraValidateParameters = extraParameters is not null ? new EquatableArray<ParameterInfo>(extraParameters) : null;
            Properties = new EquatableArray<ValidationPropertyInfo>(properties);

        }

    }

    private readonly record struct ValidationPropertyInfo
    {

        public readonly string PropertyName;
        public readonly string TypeFullName;

        public readonly bool IsNullable;
        public readonly bool IsValidation;
        public readonly bool IsReferenceType;

        public readonly string ValidationTypeFullName;
        public readonly EquatableArray<ValidatePropertyInfo> Validations;

        public ValidationPropertyInfo(
            string propertyName,
            string typeFullName,
            bool isNullable,
            bool isValidation,
            bool isReferenceType,
            string validationTypeFullName,
            ValidatePropertyInfo[] validations)
        {

            PropertyName = propertyName;
            TypeFullName = typeFullName;

            IsNullable = isNullable;
            IsValidation = isValidation;
            IsReferenceType = isReferenceType;

            ValidationTypeFullName = validationTypeFullName;
            Validations = new EquatableArray<ValidatePropertyInfo>(validations);

        }

    }

    private readonly record struct ValidatePropertyInfo
    {

        public readonly bool IsGenericMethod;
        public readonly bool IsNullable;

        public readonly bool IsService;
        public readonly string? ServiceTypeFullName;
        public readonly EquatableArray<string>? ServicePath;

        public readonly EquatableArray<ValidateParameterInfo> Parameters;
        public readonly EquatableArray<ConstructorArgInfo> ConstructorArgs;
        public readonly string ErrorCode;

        public readonly ClassInfo ClassInfo;

        public ValidatePropertyInfo(
            bool isGenericMethod,
            bool isNullable,
            bool isService,
            string? serviceTypeFullName,
            string[]? servicePath,
            ValidateParameterInfo[] parameters,
            ConstructorArgInfo[] constructorArgs,
            string errorCode,
            ClassInfo classInfo)
        {

            ClassInfo = classInfo;

            IsGenericMethod = isGenericMethod;
            IsNullable = isNullable;

            IsService = isService;
            ServiceTypeFullName = serviceTypeFullName;
            ServicePath = servicePath != null ? new EquatableArray<string>(servicePath) : null;

            Parameters = new EquatableArray<ValidateParameterInfo>(parameters);
            ConstructorArgs = new EquatableArray<ConstructorArgInfo>(constructorArgs);

            ErrorCode = errorCode;

        }

    }

    private readonly record struct ValidateParameterInfo
    {

        public readonly string Name;
        public readonly string FullTypeName;

        public ValidateParameterInfo(
            string name,
            string fullTypeName)
        {

            Name = name;
            FullTypeName = fullTypeName;

        }

    }

    private readonly record struct ConstructorArgInfo
    {

        public readonly string Value;

        public ConstructorArgInfo(
            string value)
        {

            Value = value;

        }

    }

    private sealed class ServiceRegistration
    {

        public required string InstanceName { get; set; }

        public required string ParameterName { get; set; }

    }

}
