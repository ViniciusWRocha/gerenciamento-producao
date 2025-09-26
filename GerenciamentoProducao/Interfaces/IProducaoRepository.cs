using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces
{
    public interface ÍProducaoRepository
    {
        Task<List<Producao>> GetAllAsync();
         Task<Producao> GetById(int id);
         Task AddAsync(Producao producao);
         Task UpdateAsync(Producao producao);
         Task DeleteAsync(int id);
    }
}
