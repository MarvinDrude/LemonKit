namespace LemonKit.Results;

/// <summary>
/// Result class that is either in success or error state.
/// </summary>
/// <typeparam name="TSuccess">Type of the success state</typeparam>
/// <typeparam name="TError">Type of the error state</typeparam>
public sealed class Result<TSuccess, TError> : IEquatable<Result<TSuccess, TError>> {

    [MemberNotNullWhen(false, nameof(_Value), nameof(Value))]
    [MemberNotNullWhen(true, nameof(_Error), nameof(Error))]
    public bool IsError => _Type == ResultType.Error;

    [MemberNotNullWhen(true, nameof(_Value), nameof(Value))]
    [MemberNotNullWhen(false, nameof(_Error), nameof(Error))]
    public bool IsSuccess => _Type == ResultType.Success;

    public TError? Error => _Error;
    public TSuccess? Value => _Value;

    private readonly TError? _Error;
    private readonly TSuccess? _Value;

    private readonly ResultType _Type;

    public Result(TSuccess value) {

        _Type = ResultType.Success;
        _Value = value;

    }

    public Result(TError error) {

        _Type = ResultType.Error;
        _Error = error;

    }

    public static implicit operator Result<TSuccess, TError>(TSuccess value) => new(value);
    public static implicit operator Result<TSuccess, TError>(TError error) => new(error);

    public static bool operator ==(Result<TSuccess, TError> left, Result<TSuccess, TError> right) => left.Equals(right);
    public static bool operator !=(Result<TSuccess, TError> left, Result<TSuccess, TError> right) => !(left == right);

    public override int GetHashCode() => IsError ? _Error.GetHashCode() : _Value.GetHashCode();
    public override bool Equals(object? obj) => obj is Result<TSuccess, TError> res && Equals(res);
    public override string ToString() => (IsError ? _Error.ToString() : _Value.ToString()) ?? "(null)";

    public bool Equals(Result<TSuccess, TError>? other) {

        return (
            other is { } check && (
                (check.IsError && IsError && check.Error.Equals(Error))
                || (check.IsSuccess && IsSuccess && check.Value.Equals(Value))
            )
        );

    }

    public void Deconstruct(out bool success, out TSuccess? value) {

        success = IsSuccess;
        value = _Value;

    }

    public void Deconstruct(out bool success, out TSuccess? value, out TError? error) {

        success = IsSuccess;
        value = _Value;
        error = _Error;

    }

    private enum ResultType : byte {

        Success = 1,
        Error = 2

    }

}