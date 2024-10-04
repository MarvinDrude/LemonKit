
namespace LemonKit.Generators.Validation;

internal sealed partial class ValidationGenerator {

    private static (string Parameters, string ErrorParameters) RenderMinLength(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderMaxLength(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderEqual(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderNotEqual(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderEmpty(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderNoneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderNotEmpty(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderNoneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderContains(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderNotContains(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderGreaterThan(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderGreaterThanOrEqual(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderLessThan(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderLessThanOrEqual(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderOneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderEnumValue(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        return RenderNoneNormalParameter(property, validate, services);

    }

    private static (string Parameters, string ErrorParameters) RenderOneNormalParameter(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        if(validate.IsService && validate.ServicePath is not null
            && validate.ServiceTypeFullName is not null) {

            string accessPath = string.Join(".", validate.ServicePath);
            var registration = services[validate.ServiceTypeFullName];

            string shared = $"{registration.InstanceName}.{accessPath}";
            string parameters = $"input.{property.PropertyName}, {shared}";

            return (parameters, shared);

        } else {

            string shared = $"{validate.ConstructorArgs[0].Value}";
            string parameters = $"input.{property.PropertyName}, {shared}";

            return (parameters, shared);

        }

    }

    private static (string Parameters, string ErrorParameters) RenderNoneNormalParameter(
        ValidationPropertyInfo property,
        ValidatePropertyInfo validate,
        Dictionary<string, ServiceRegistration> services) {

        string parameters = $"input.{property.PropertyName}";

        return (parameters, string.Empty);

    }

}
