using Dapper;

namespace LemonKit.Database.Mappers;

public sealed class DateTimeOffsetMapper : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override DateTimeOffset Parse(object value)
    {
        return value switch
        {
            DateTime dateTime => (DateTimeOffset)DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            DateTimeOffset offset => offset,
            _ => throw new InvalidOperationException("Must be datetime or datetimeoffset.")
        };
    }

    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = parameter.DbType switch
        {
            DbType.DateTime or DbType.DateTime2 or DbType.AnsiString => value.UtcDateTime,
            DbType.DateTimeOffset => value,
            _ => throw new InvalidOperationException("DateTimeOffset must be assigned to datetime column."),
        };
    }
}

public sealed class DateTimeOffsetNullableMapper : SqlMapper.TypeHandler<DateTimeOffset?>
{
    public override DateTimeOffset? Parse(object value)
    {
        return value switch
        {
            null => null,

            DateTime dateTime => (DateTimeOffset)DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            DateTimeOffset offset => offset,
            _ => throw new InvalidOperationException("Must be datetime or datetimeoffset.")
        };
    }

    public override void SetValue(IDbDataParameter parameter, DateTimeOffset? value)
    {
        parameter.Value = parameter.DbType switch
        {
            DbType.DateTime or DbType.DateTime2 or DbType.AnsiString => value?.UtcDateTime,
            DbType.DateTimeOffset => value,
            _ => throw new InvalidOperationException("DateTimeOffset must be assigned to datetime column."),
        };
    }
}
