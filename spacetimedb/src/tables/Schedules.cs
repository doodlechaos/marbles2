using SpacetimeDB;

public static partial class Module
{
    [Table(Scheduled = nameof(ClockUpdate), ScheduledAt = nameof(ScheduledAt))]
    public partial struct ClockSchedule
    {
        [PrimaryKey]
        public ulong Id;
        public ScheduleAt ScheduledAt;
    }

    [Table(Scheduled = nameof(TakeDeterminismSnap), ScheduledAt = nameof(ScheduledAt))]
    public partial struct DeterminismSnapSchedule
    {
        [PrimaryKey]
        public ulong Id;
        public ScheduleAt ScheduledAt;
    }

/*     [Table(Scheduled = nameof(ActiveAccountCache.EvictStale), ScheduledAt = nameof(ScheduledAt))]
    public partial struct AccountCacheEvictionSchedule
    {
        [PrimaryKey]
        public ulong Id;
        public ScheduleAt ScheduledAt;
    } */
}
