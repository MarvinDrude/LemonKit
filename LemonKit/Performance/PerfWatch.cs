
namespace LemonKit.Performance;

public sealed class PerfWatch : Stopwatch
{
    public static PerfWatch StartTimer()
    {
        PerfWatch watch = new();
        watch.Start();

        return watch;
    }

    public long StopNs()
    {
        Stop();
        return (long)Elapsed.TotalNanoseconds;
    }

    public double StopMs()
    {
        Stop();
        return Elapsed.TotalMilliseconds;
    }

    public double StopSeconds()
    {
        Stop();
        return Math.Floor(Elapsed.TotalSeconds * 100) / 100;
    }

    public double StopMinutes()
    {
        Stop();
        return Math.Floor(Elapsed.TotalMinutes * 100) / 100;
    }

    public TimeSpan StopTimeSpan()
    {
        Stop();
        return Elapsed;
    }
}
