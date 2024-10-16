
namespace LemonKit.Processors;

/// <summary>
/// Source-generated classes for processors use this interface
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IProcessor<TInput, TOutput>
{

    /// <summary>
    /// Will process inputs and will return a task of type output. Does include procedures that might run.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Task<TOutput> Process(TInput request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);

    public Func<TInput, IServiceProvider, CancellationToken, Task<TOutput>> BuildProcess(IServiceProvider serviceProvider);

}
