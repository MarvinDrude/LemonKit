
namespace LemonKit.Results;

public sealed class RGuard
{

    public static RGuard Is { get; } = new RGuard();

    private RGuard() { }

}
