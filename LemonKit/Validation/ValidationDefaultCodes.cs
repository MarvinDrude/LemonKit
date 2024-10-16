
namespace LemonKit.Validation;

public static class ValidationDefaultCodes
{

    public const string ErrorMinLength = "E_MUST_MIN_LENGTH:{MinLength}";
    public const string ErrorMaxLength = "E_MUST_MAX_LENGTH:{MaxLength}";

    public const string ErrorEmpty = "E_MUST_EMPTY";
    public const string ErrorNotEmpty = "E_MUST_NOT_EMPTY";

    public const string ErrorEqual = "E_MUST_EQUAL:{ToString}";
    public const string ErrorNotEqual = "E_MUST_NOT_EQUAL:{ToString}";

    public const string ErrorEnum = "E_MUST_VALID_ENUM";

    public const string ErrorGreaterThan = "E_MUST_GREATER_THAN:{Number}";
    public const string ErrorGreaterThanOrEqual = "E_MUST_GREATER_THAN_OR_EQUAL:{Number}";

    public const string ErrorLessThan = "E_MUST_LESS_THAN:{Number}";
    public const string ErrorLessThanOrEqual = "E_MUST_LESS_THAN_OR_EQUAL:{Number}";

    public const string ErrorContains = "E_MUST_CONTAIN:{ToString}";
    public const string ErrorNotContains = "E_MUST_NOT_CONTAIN:{ToString}";

}

