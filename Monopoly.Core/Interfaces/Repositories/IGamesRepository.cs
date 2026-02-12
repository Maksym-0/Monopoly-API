using Monopoly.Core.Models.Game;

namespace Monopoly.Core.Interfaces.Repositories
{
    public interface IGamesRepository
    {
        public Task<Game?> GetById(Guid id);
        public Task Add(Game game);
        public Task DeleteById(Guid id);
    }
}
