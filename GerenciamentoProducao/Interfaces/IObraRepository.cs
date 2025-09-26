using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces
{
    public interface IObraRepository
    {
        Task<List<Obra>> GetAllAsync();
         Task<Obra> GetById(int id);
         Task AddAsync(Obra obra);
         Task UpdateAsync(Obra obra);
         Task DeleteAsync(int id);
    }
}
