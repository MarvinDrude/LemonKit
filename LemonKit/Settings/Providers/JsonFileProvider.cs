
namespace LemonKit.Settings.Providers;

/// <summary>
/// Provider for json files to read config from that and automatically update
/// </summary>
public sealed class JsonFileProvider<T> : SettingsProvider
    where T : class
{

    /// <summary>
    /// Current deserialized value
    /// </summary>
    public T? CurrentValue => _Value;

    /// <summary>
    /// Make sure reload logic is ran one after each other if many changes are pushed
    /// </summary>
    private readonly SemaphoreSlim _Semaphore = new(1, 1);

    /// <summary>
    /// File name of the json file to match and check
    /// </summary>
    private readonly string _FileName;

    /// <summary>
    /// Full folder path where the json file is located in
    /// </summary>
    private readonly string _FolderPath;

    /// <summary>
    /// Full file name with directory path
    /// </summary>
    private readonly string _FullName;

    /// <summary>
    /// Json context used to deserialize
    /// </summary>
    private readonly JsonSerializerContext _Context;

    /// <summary>
    /// Current deserialized value
    /// </summary>
    private T? _Value;

    /// <summary>
    /// If watch is enabled, this is used to get change notifications from file system
    /// </summary>
    private readonly FileSystemWatcher? _Watcher;

    public JsonFileProvider(
        string fileName,
        JsonSerializerContext jsonContext,
        string? folderPath = null,
        bool watch = true)
    {

        _FileName = fileName;
        _FolderPath = folderPath ?? AppDomain.CurrentDomain.BaseDirectory;
        _Context = jsonContext;

        string fullPath = Path.Combine(_FolderPath, _FileName);
        if(!File.Exists(fullPath))
        {
            throw new InvalidOperationException($"The settings json file '{fullPath}' does not exist.");
        }
        _FullName = fullPath;

        if(watch)
        {

            _Watcher = new FileSystemWatcher(_FolderPath)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                IncludeSubdirectories = false,
            };

            _Watcher.Changed += async (s, args) => { await OnFileSystemChange(); };
            _Watcher.Filters.Add(_FileName);

            _Watcher.EnableRaisingEvents = true;

        }

    }

    /// <inheritdoc />
    public override async Task Reload()
    {

        await _Semaphore.WaitAsync();

        try
        {

            string rawText = await File.ReadAllTextAsync(_FullName);
            T? temp = JsonSerializer.Deserialize(rawText, typeof(T), _Context) as T
                ?? throw new Exception($"Json settings file '{_FullName}' deserialization ended in null. Check your format.");
            // throwing is here ok as "control flow" because this is a fundamental part that when throwing is very exceptional

            _Value = temp;
            OnUpdate();

        }
        catch(Exception error)
        {

            throw new Exception($"Json settings file '{_FullName}' deserialization ended in exception: {error}"); // just catched to use the finally
            // this means saving a json file after start in a malformed state will result in a server shutdown, maybe change in future?

        }
        finally
        {

            _Semaphore.Release();

        }

    }

    /// <summary>
    /// Callback when watcher noticed change
    /// </summary>
    /// <returns></returns>
    private async Task OnFileSystemChange()
    {

        await Reload();

    }

}
