namespace Monopoly.Core.Factories
{
    internal static class MonopolyFactory
    {
        internal static List<Models.Game.Monopoly> CreateMonopolies(Guid boardId)
        {
            List<Models.Game.Monopoly> monopolies = new();

            for(int i = 0; i < Constants.CellsMonopolyTypes.Count; i++)
            {
                int currentMonopolyIndex = i + 1;
                Models.Game.Monopoly monopoly = new Models.Game.Monopoly(Guid.NewGuid(), boardId, currentMonopolyIndex);
                monopolies.Add(monopoly);
            }

            return monopolies;
        }
    }
}
