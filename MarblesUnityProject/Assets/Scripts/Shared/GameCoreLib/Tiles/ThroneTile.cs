using System;
using MemoryPack;

namespace GameCoreLib
{
    /// <summary>
    /// The ThroneTile displays the current king and throne room environment.
    /// Handles throne capture when marbles collide with the king body.
    /// </summary>
    [Serializable]
    [MemoryPackable(SerializeLayout.Explicit)]
    public partial class ThroneTile : TileBase
    {
        public ThroneTile()
            : base() { }

        /// <summary>
        /// Crown a new king when a marble captures the throne.
        /// Explodes the capturing marble and sends the NewKing event to the server.
        /// </summary>
        public void CrownNewKing(MarbleComponent marble)
        {
            if (marble == null || !marble.IsAlive)
                return;

            ulong accountId = marble.AccountId;

            Logger.Log($"New king crowned: {accountId}");

            // Explode the capturing marble
            ExplodeMarble(marble);

            SetKingServerId(accountId);
        }

        public void SetKingServerId(ulong accountId)
        {
            // Send the NewKing event to the server
            if (currentOutputEvents != null)
            {
                currentOutputEvents.Server.Add(
                    new OutputToServerEvent.NewKing { AccountId = accountId }
                );
            }
        }
    }
}
