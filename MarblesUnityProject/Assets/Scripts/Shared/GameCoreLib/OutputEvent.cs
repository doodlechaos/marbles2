using System.Collections.Generic;

namespace GameCoreLib
{
    public sealed class OutputEventBuffer
    {
        public readonly List<OutputToClientEvent> Client = new();
        public readonly List<OutputToServerEvent> Server = new();

        public void Clear()
        {
            Client.Clear();
            Server.Clear();
        }
    }

    public abstract class OutputToClientEvent
    {
        public class NewKing : OutputToClientEvent
        {
            public ulong AccountId;
        }

        public class StartSpinAnimation : OutputToClientEvent
        {
            public byte WorldId;
        }
    }

    public abstract class OutputToServerEvent
    {
        public class DeterminismHash : OutputToServerEvent
        {
            public ushort Seq;
            public string HashString;
        }

        public class AddPointsToAccount : OutputToServerEvent
        {
            public ulong AccountId;
            public uint Points;
        }

        public class NewKing : OutputToServerEvent
        {
            public ulong AccountId;
        }

        public class StateUpdatedTo : OutputToServerEvent
        {
            public GameTileState State;
            public byte WorldId;

        }
    }
}
