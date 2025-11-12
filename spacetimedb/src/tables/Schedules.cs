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

}

