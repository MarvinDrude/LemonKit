﻿
namespace LemonKit.Startup;

/// <summary>
/// Used to mark a class that should be ran before and after startup
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class StartupModule : Attribute
{
    private readonly int PrePriority;
    private readonly int PostPriority;

    private readonly string MethodName;

    public StartupModule(
        int prePriority = 1,
        int postPriority = 1,
        string extensionName = "OnStartup")
    {
        PrePriority = prePriority;
        PostPriority = postPriority;

        MethodName = extensionName;
    }
}
