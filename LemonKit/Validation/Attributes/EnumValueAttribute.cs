
namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a property that its value should be a valid value of the enum
/// </summary>
public sealed class EnumValueAttribute : ValidationAttribute {

    public EnumValueAttribute(
        string errorCode = ValidationDefaultCodes.ErrorEnum) {

        _ErrorCode = errorCode;

    }

    public static bool Validate<T>(T value)
        where T : struct, Enum {

        if(Enum.IsDefined(value)) {
            return true;
        }

        if(typeof(T).GetCustomAttribute<FlagsAttribute>() is not { }) {
            return false;
        }

        return value.IsValidFlags();

    }

    public static string TemplateError<T>(string errorCodeTemplate) {

        return errorCodeTemplate;

    }

}

file static class Enums {

    public static bool IsValidFlags<TEnum>(this TEnum value)
        where TEnum : struct, Enum =>
        Cache<TEnum>.Instance.IsValidFlags(value);

}

/// <summary>
/// Used to efficiently cache all valid enum values and their flags
/// </summary>
/// <typeparam name="TEnum"></typeparam>
file abstract class Cache<TEnum>
    where TEnum : struct, Enum {

    public static readonly Cache<TEnum> Instance = GetInstance();

    private static Cache<TEnum> GetInstance() =>
        Type.GetTypeCode(typeof(TEnum)) switch {

            TypeCode.Char => new Cache<TEnum, char>(),
            TypeCode.SByte => new Cache<TEnum, sbyte>(),
            TypeCode.Byte => new Cache<TEnum, byte>(),
            TypeCode.Int16 => new Cache<TEnum, short>(),
            TypeCode.UInt16 => new Cache<TEnum, ushort>(),
            TypeCode.Int32 => new Cache<TEnum, int>(),
            TypeCode.UInt32 => new Cache<TEnum, uint>(),
            TypeCode.Int64 => new Cache<TEnum, long>(),
            TypeCode.UInt64 => new Cache<TEnum, ulong>(),

            _ => throw new NotSupportedException($"Enum underlying type of {Enum.GetUnderlyingType(typeof(TEnum))} is not supported"),
        };

    public abstract bool IsValidFlags(TEnum value);
}

file sealed class Cache<TEnum, TUnderlying> : Cache<TEnum>
    where TEnum : struct, Enum
    where TUnderlying : struct,
        IComparable<TUnderlying>,
        IEquatable<TUnderlying>,
        IBinaryInteger<TUnderlying>,
        IConvertible {

    private readonly TUnderlying _AllFlags = GetAllFlags();

    private static TUnderlying GetAllFlags() {

        TUnderlying allFlags = default;
        var fields = typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach(var field in fields) {

            var value = (TUnderlying)field.GetValue(null)!;
            allFlags |= value;

        }

        return allFlags;

    }

    public override bool IsValidFlags(TEnum value) {

        var underlying = (TUnderlying)(object)value;

        return (underlying & _AllFlags) == underlying;

    }

}