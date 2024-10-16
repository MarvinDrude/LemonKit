
namespace LemonKit.Generators.Observe;

internal sealed partial class ObserveGenerator
{

    internal readonly record struct ObserveInfo
    {

        public readonly ClassInfo ClassInfo;

        public readonly string ActivitySourceName;
        public readonly string? MeterName;
        public readonly string Version;

        public ObserveInfo(
            ClassInfo classInfo,
            string activitySourceName,
            string? meterName,
            string version)
        {

            ClassInfo = classInfo;

            ActivitySourceName = activitySourceName;
            MeterName = meterName;
            Version = version;

        }

    }

}