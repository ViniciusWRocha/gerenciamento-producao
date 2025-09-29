using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces
{
    public interface IProducaoRepository
    {
        Task<List<Producao>> GetAllAsync();
         Task<Producao> GetById(int id);
         Task AddAsync(Producao producao);
         Task UpdateAsync(Producao producao);
         Task DeleteAsync(int id);
        Task<string?> GetByIdAsync(int id);
    }
}
