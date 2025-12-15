using System;
using System.Collections.Generic;

namespace GameCoreLib
{
    [Flags]
    public enum OutputEventDestination
    {
        None = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        Both = Client | Server,
    }

    public sealed class OutputEventBuffer
    {
        public readonly List<OutputEvent> Events = new();

        public IEnumerable<OutputEvent> Client => FilterBy(OutputEventDestination.Client);
        public IEnumerable<OutputEvent> Server => FilterBy(OutputEventDestination.Server);

        public void Clear()
        {
            Events.Clear();
        }

        private IEnumerable<OutputEvent> FilterBy(OutputEventDestination destination)
        {
            foreach (OutputEvent outputEvent in Events)
            {
                if (outputEvent.Destination.HasFlag(destination))
                    yield return outputEvent;
            }
        }
    }

    public abstract class OutputEvent
    {
        protected OutputEvent(OutputEventDestination destination)
        {
            Destination = destination;
        }

        public OutputEventDestination Destination { get; }

        public class NewKing : OutputEvent
        {
            public NewKing()
                : base(OutputEventDestination.Server) { }

            public ulong AccountId;
        }

        public class StartSpinAnimation : OutputEvent
        {
            public StartSpinAnimation()
                : base(OutputEventDestination.Client) { }

            public byte WorldId;
        }

        public class DeterminismHash : OutputEvent
        {
            public DeterminismHash()
                : base(OutputEventDestination.Both) { }

            public ushort Seq;
            public string HashString;
        }

        public class AddPointsToAccount : OutputEvent
        {
            public AddPointsToAccount()
                : base(OutputEventDestination.Server) { }

            public ulong AccountId;
            public uint Points;
        }

        public class StateUpdatedTo : OutputEvent
        {
            public StateUpdatedTo()
                : base(OutputEventDestination.Server) { }

            public GameTileState State;
            public byte WorldId;
        }
    }
}
