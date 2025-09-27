using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces
{
    public interface IFamiliaCaixilhoRepository
    {
         Task<List<FamiliaCaixilho>> GetAllAsync();
         Task<FamiliaCaixilho> GetById(int id);
         Task AddAsync(FamiliaCaixilho familiaCaixilho);
         Task UpdateAsync(FamiliaCaixilho familiaCaixilho);
         Task DeleteAsync(int id);
        Task<string?> GetByIdAsync(int id);
    }
}
