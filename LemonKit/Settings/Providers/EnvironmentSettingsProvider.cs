
namespace LemonKit.Settings.Providers;

/// <summary>
/// Provider that has all environemnt variables (not reloadable without restart)
/// </summary>
public sealed class EnvironmentSettingsProvider : SettingsProvider {

    /// <summary>
    /// Key value pair of each Environemnt variable
    /// </summary>
    private readonly Dictionary<string, string> _Entries = [];

    /// <summary>
    /// Tries to get the value for a given key, returns true if found, else false
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) {

        return _Entries.TryGetValue(key, out value);

    }

    /// <summary>
    /// Tries to get the value for a given key, returns null if not found
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string? Get(string key) {

        if(!TryGetValue(key, out var str)) {
            return null;
        }

        return str;

    }

    /// <inheritdoc />
    public override Task Reload() {

        _Entries.Clear();
        var rawVariables = Environment.GetEnvironmentVariables();
        var keys = rawVariables.Keys;
        
        foreach(var keyOb in keys) {

            string key = (string)keyOb;
            string? value = (string?)rawVariables[key];

            if(value is null) {
                continue;
            }

            _Entries[GetKey(key)] = value;

        }

        return Task.CompletedTask;

    }

    /// <summary>
    /// Convert __ to : for compability
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string GetKey(string key) {

        return key.Replace("__", ":");

    }

}
