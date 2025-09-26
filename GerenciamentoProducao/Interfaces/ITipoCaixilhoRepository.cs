using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces
{
    public interface ITipoCaixilhoRepository
    {
        Task<List<TipoCaixilho>> GetAllAsync();
         Task<TipoCaixilho> GetById(int id);
         Task AddAsync(TipoCaixilho tipoCaixilho);
         Task UpdateAsync(TipoCaixilho tipoCaixilho);
         Task DeleteAsync(int id);
    }
}
