namespace LemonKit.MainDemo.Database.Models;

public sealed class Pet
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Color { get; set; }

    public required float Height { get; set; }

    public sealed class Sql
    {
        public const string TableName = "pet";
    }
}