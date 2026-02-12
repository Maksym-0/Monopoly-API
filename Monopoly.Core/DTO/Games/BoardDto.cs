using Monopoly.Core.Models.Game;

namespace Monopoly.Core.DTO.Games
{
    public class BoardDto
    {
        public Guid Id { get; set; }
        public List<CellDto> Cells { get; set; } = new();
        public List<MonopolyDto> Monopolies { get; set; } = new();

        public BoardDto(GameBoard board)
        {
            Id = board.Id;
            foreach (var cell in board.Cells)
            {
                Cells.Add(new CellDto(cell));
            }
            Cells = Cells.OrderBy(c => c.Number).ToList();
            foreach(var monopoly in board.Monopolies)
            {
                Monopolies.Add(new MonopolyDto(monopoly));
            }
            Monopolies = Monopolies.OrderBy(m => m.Index).ToList();
        }
        public BoardDto() { }
    }
}
