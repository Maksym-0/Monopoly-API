using Monopoly.Core.Models.Game;
using Monopoly.Core.Models.Room;

namespace Monopoly.Core.Factories
{
    internal static class PlayerFactory
    {
        internal static List<Player> CreatePlayers(List<PlayerInRoom> playersInRoom, Guid gameId)
        {
            List<Player> players = new();

            for (int i = 0; i < playersInRoom.Count; i++)
            {
                Player player = new Player(Guid.NewGuid(), playersInRoom[i].AccountId, gameId, playersInRoom[i].Account, 
                    playersInRoom[i].Name, playersInRoom[i].Index);
                players.Add(player);
            }

            return players;
        }
    }
}
