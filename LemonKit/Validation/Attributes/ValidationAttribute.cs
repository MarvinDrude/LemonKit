﻿
namespace LemonKit.Validation.Attributes;

/// <summary>
/// Marks a property to be validated
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValidationAttribute : Attribute {

    protected string? _ErrorCode;

    protected Type? _Type;

    protected string[]? _AccessPath;

}