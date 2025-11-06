using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoProducao.Repositories
{
    public class TipoCaixilhoRepository : ITipoCaixilhoRepository
    {
        private readonly GerenciamentoProdDbContext _context;
        public TipoCaixilhoRepository(GerenciamentoProdDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TipoCaixilho tipoCaixilho)
        {
            await _context.TipoCaixilhos.AddAsync(tipoCaixilho);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Tipocaixilho = await _context.TipoCaixilhos.FindAsync(id);
            if (Tipocaixilho  != null)
            {
                _context.TipoCaixilhos.Remove(Tipocaixilho);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TipoCaixilho>> GetAllAsync()
        {
            return await _context.TipoCaixilhos.ToListAsync();

        }

        public async Task<TipoCaixilho> GetById(int id)
        {
            return await _context.TipoCaixilhos.FindAsync(id);

        }

        public async Task UpdateAsync(TipoCaixilho tipoCaixilho)
        {
            // Buscar a entidade rastreada do contexto
            var tipoCaixilhoTracked = await _context.TipoCaixilhos.FindAsync(tipoCaixilho.IdTipoCaixilho);
            if (tipoCaixilhoTracked == null)
            {
                throw new InvalidOperationException($"TipoCaixilho com ID {tipoCaixilho.IdTipoCaixilho} não encontrado.");
            }

            // Atualizar as propriedades da entidade rastreada
            tipoCaixilhoTracked.DescricaoCaixilho = tipoCaixilho.DescricaoCaixilho;

            await _context.SaveChangesAsync();
        }
    }
}
