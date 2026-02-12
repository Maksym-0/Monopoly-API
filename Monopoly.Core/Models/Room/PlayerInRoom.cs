using Monopoly.Core.Models.Account;

namespace Monopoly.Core.Models.Room
{
    public class PlayerInRoom
    {
        public Guid Id { get; private set; }

        public Guid AccountId { get; private set; }
        public Account.Account? Account { get; private set; }

        public string Name { get; private set; }
        public int Index { get; private set; }

        public Guid RoomId { get; private set; }
        public Room? Room { get; private set; }

        public PlayerInRoom(Guid id, Guid accountId, Guid roomId, Account.Account? account, string name, int index)
        {
            Id = id;
            AccountId = accountId;
            Account = account;
            RoomId = roomId;
            Name = name;
            Index = index;
        }
        public PlayerInRoom() { }

        internal void ChangeIndex(int newIndex)
        {
            Index = newIndex;
        }
    }
}
