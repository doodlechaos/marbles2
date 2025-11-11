using SpacetimeDB;

public static partial class Module
{

        [Table(Name = "clock_schedule", Scheduled = nameof(ClockUpdate),  ScheduledAt = nameof(ScheduledAt))]

        public partial struct ClockSchedule
        {
            [PrimaryKey]
            public ulong Id;
            public ScheduleAt ScheduledAt;
        }

        [Reducer]
        public static void ClockUpdate(ReducerContext ctx, ClockSchedule schedule)
        {
            Log.Info("Clock update scheduled");
        }

        [Table(Name = "determinism_check_schedule", Scheduled = nameof(DeterminismCheck),  ScheduledAt = nameof(ScheduledAt))]
        public partial struct DeterminismCheckSchedule
        {
            [PrimaryKey]
            [AutoInc]
            public ulong Id;
            public ScheduleAt ScheduledAt;
        }

        [Reducer]
        public static void DeterminismCheck(ReducerContext ctx, DeterminismCheckSchedule schedule)
        {
            Log.Info("Determinism check scheduled");
        }
}

