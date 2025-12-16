using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FPMathLib;
using MemoryPack;
using SpacetimeDB;

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
    }

    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    [MemoryPackUnion(0, typeof(SimpleBattleRoyale))]
    public abstract partial class GameTileBase : TileBase
    {
        [MemoryPackOrder(7)]
        public GameBidCfg GameBidCfg;

        [MemoryPackOrder(8)]
        public Rarity TileRarity;

        [MemoryPackOrder(9)]
        public byte TileRarityMultiplier;

        [MemoryPackOrder(10)]
        public GameTileState State = GameTileState.Spinning;

        [MemoryPackOrder(11), MemoryPackInclude]
        protected int stateSteps = 0;

        //Alive players
        [MemoryPackOrder(12), MemoryPackInclude]
        public List<ulong> ActiveContestants = new List<ulong>();

        //Eliminated players (The reverse order here is the order they are ranked in the podium)
        [MemoryPackOrder(13), MemoryPackInclude]
        public List<ulong> EliminatedContestants = new List<ulong>();

        [MemoryPackOrder(14)]
        public uint TotalMarblesBid;

        /// <summary>
        /// Number of simulation steps spent in the current state.
        /// </summary>
        [MemoryPackIgnore]
        public int StateSteps => stateSteps;

        public GameTileBase()
            : base() { }

        public override void InitTile(byte tileWorldId)
        {
            // Set the state before calling base to ensure the event has the correct WorldId
            SetState(GameTileState.Spinning);
            base.InitTile(tileWorldId);
        }

        public void InitRarity(Rarity tileRarity, byte tileRarityMultiplier)
        {
            TileRarity = tileRarity;
            TileRarityMultiplier = tileRarityMultiplier;
        }

        public virtual void StartGameplay(InputEvent.GameplayStartInput gameplayStartInput)
        {
            TotalMarblesBid = gameplayStartInput.TotalMarblesBid;

            Logger.Log($"Starting gameplay with {gameplayStartInput.Entrants.Length} entrants");

            //Enter all contestants into the game, regardless of if they're spawned or not yet
            foreach (var entrant in gameplayStartInput.Entrants)
                EnterContestant(entrant.AccountId);

            SetState(GameTileState.Gameplay);
        }

        public void EnterContestant(ulong accountId)
        {
            ActiveContestants.Add(accountId);
        }

        public void EliminateContestant(ulong accountId)
        {
            ActiveContestants.Remove(accountId);
            EliminatedContestants.Add(accountId);

            if (ActiveContestants.Count <= 0)
                FinishGameplay();
        }

        public virtual void FinishGameplay()
        {
            //If there are straggling active contestants, force them to be eliminated.
            foreach (var accountId in ActiveContestants)
                EliminateContestant(accountId);

            SetState(GameTileState.ScoreScreen);
            currentOutputEvents?.Events.Add(
                new OutputEvent.GameplayFinished
                {
                    Prize = CalculateTotalPrize(),
                    AccountIdsInRankOrder = EliminatedContestants
                        .AsEnumerable()
                        .Reverse()
                        .ToArray(),
                    WorldId = TileWorldId,
                }
            );
        }

        public uint CalculateTotalPrize()
        {
            //Prize is total marbles bid * tile rarity multiplier
            return TotalMarblesBid * TileRarityMultiplier;
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
            Logger.Log($"SetState: {State} --> {state}");

            currentOutputEvents?.Events.Add(
                new OutputEvent.StateUpdatedTo { State = state, WorldId = TileWorldId }
            );
            stateSteps = 0;
            State = state;
        }

        public override void FlagMarbleToExplode(MarbleComponent marble)
        {
            if (marble == null)
            {
                Logger.Error($"MarbleComponent is null in DestroyMarble");
                return;
            }
            EliminateContestant(marble.AccountId);

            base.FlagMarbleToExplode(marble);
        }
    }
}
