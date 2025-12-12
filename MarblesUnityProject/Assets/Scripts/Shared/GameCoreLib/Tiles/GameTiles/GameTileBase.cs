using System;
using FPMathLib;
using MemoryPack;

#nullable enable

namespace GameCoreLib
{
    public enum GameTileState
    {
        Spinning,
        OpeningDoor,
        Bidding,
        Gameplay,
        ScoreScreen,
        ClosingDoor,

        ReadyToSpin,
        //    Finished,
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(SimpleBattleRoyale))]
    public abstract partial class GameTileBase : TileBase
    {
        [MemoryPackOrder(7)]
        public GameTileState State = GameTileState.Spinning;

        [MemoryPackOrder(8), MemoryPackInclude]
        protected int stateSteps = 0;

        /// <summary>
        /// Number of simulation steps spent in the current state.
        /// </summary>
        [MemoryPackIgnore]
        public int StateSteps => stateSteps;

        public GameTileBase()
            : base() { }

        public override void Initialize(byte tileWorldId)
        {
            // Set the state before calling base to ensure the event has the correct WorldId
            SetState(GameTileState.Spinning);
            base.Initialize(tileWorldId);
        }

        public virtual void StartGameplay(InputEvent.GameplayStartInput gameplayStartInput)
        {
            throw new NotImplementedException();
        }

        public const float SPINNING_DURATION_SEC = 5.0f;
        public const float CLOSING_DOOR_DURATION_SEC = 2.0f;
        public const float OPENING_DOOR_DURATION_SEC = 2.0f;
        public const float SCORE_SCREEN_DURATION_SEC = 5.0f;
        public const float GAMEPLAY_MAX_DURATION_SEC = 60.0f;

        public override void Step()
        {
            float stateDurationSec = stateSteps / 60.0f;
            if (State == GameTileState.Spinning)
            {
                if (stateDurationSec >= SPINNING_DURATION_SEC)
                    SetState(GameTileState.OpeningDoor);
            }
            else if (State == GameTileState.OpeningDoor)
            {
                if (stateDurationSec >= OPENING_DOOR_DURATION_SEC)
                    SetState(GameTileState.Bidding);
            }
            else if (State == GameTileState.Bidding)
            {
                // Bidding → Gameplay transition is controlled by the server via StartGameTile input event.
                // This ensures gameplay only starts when the other tile is ready for bidding
                // and minimum auction spots are filled.
            }
            else if (State == GameTileState.Gameplay)
            {
                if (stateDurationSec >= GAMEPLAY_MAX_DURATION_SEC)
                    SetState(GameTileState.ScoreScreen);
            }
            else if (State == GameTileState.ScoreScreen)
            {
                if (stateDurationSec >= SCORE_SCREEN_DURATION_SEC)
                    SetState(GameTileState.ClosingDoor);
            }
            else if (State == GameTileState.ClosingDoor)
            {
                if (stateDurationSec >= CLOSING_DOOR_DURATION_SEC)
                    SetState(GameTileState.ReadyToSpin);
            }

            // Call base Step which handles physics, collision events, and marble detection
            base.Step();
            stateSteps++;
        }

        public void SetState(GameTileState state)
        {
            currentOutputEvents?.Server.Add(
                new OutputToServerEvent.StateUpdatedTo { State = state, WorldId = TileWorldId }
            );
            stateSteps = 0;
            State = state;
        }

        /// <summary>
        /// Override to assign elimination order when a marble is destroyed.
        /// </summary>
        protected override void DestroyMarble(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            OnMarbleEliminated(marble);
            base.DestroyMarble(marble);
        }

        /// <summary>
        /// Called when a marble is eliminated. Override in derived classes to assign elimination order.
        /// </summary>
        protected virtual void OnMarbleEliminated(MarbleComponent marble) { }
    }
}
