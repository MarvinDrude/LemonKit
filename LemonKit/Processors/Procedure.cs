
namespace LemonKit.Processors;

/// <summary>
/// Procedures can be used to run same logic in front of multiple processors that share the same requirements.
/// </summary>
public abstract class Procedure<TInput, TOutput> {

    /// <summary>
    /// Next procedure to call next in line
    /// </summary>
    private Procedure<TInput, TOutput>? _Next;

    /// <summary>
    /// Will process 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<TOutput> Process(TInput request, CancellationToken cancellationToken);

    /// <summary>
    /// Set the next procedure after this procedure
    /// </summary>
    /// <param name="next"></param>
    public void SetNextProcedure(Procedure<TInput, TOutput> next) {

        _Next = next;

    }

    /// <summary>
    /// Call the next procedure in line
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected Task<TOutput> Next(TInput request, CancellationToken cancellationToken) {

        if(_Next is null) {
            throw new InvalidOperationException("Next procedure must be set before next call.");
        }

        return _Next.Process(request, cancellationToken);

    }

}
