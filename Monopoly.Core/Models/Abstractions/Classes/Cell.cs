using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Models.Abstractions.Classes
{
    public abstract class Cell
    {
        public Cell(Guid id, Guid boardId, int number, string name)
        {
            Id = id;
            BoardId = boardId;
            Number = number;
            Name = name;
        }
        public Cell() { }

        public Guid Id { get; protected set; }
        
        public Guid BoardId { get; protected set; }
        public GameBoard? Board { get; protected set; }

        public int Number { get; protected set; }
        public string Name { get; protected set; }

        public bool Special { get; protected set; }

        public abstract string ApplyEffect(Player player);
    }
}
