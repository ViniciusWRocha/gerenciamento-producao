using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoProducao.Repositories
{
    public class CaixilhoRepository : ICaixilhoRepository
    {
        private readonly GerenciamentoProdDbContext _context;
        
        public CaixilhoRepository(GerenciamentoProdDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Caixilho caixilho)
        {
            await _context.Caixilhos.AddAsync(caixilho);
            await _context.SaveChangesAsync();
            
            // Recalcular o peso total da família
            await AtualizarPesoFamiliaAsync(caixilho.IdFamiliaCaixilho);
        }

        public async Task DeleteAsync(int id)
        {
            var caixilho = await _context.Caixilhos.FindAsync(id);
            if (caixilho != null)
            {
                var familiaId = caixilho.IdFamiliaCaixilho; // Salvar ID da família antes de remover
                _context.Caixilhos.Remove(caixilho);
                await _context.SaveChangesAsync();
                
                // Recalcular o peso total da família
                await AtualizarPesoFamiliaAsync(familiaId);
            }
        }

        public async Task<List<Caixilho>> GetAllAsync()
        {
            return await _context.Caixilhos.ToListAsync();
        }

        public async Task<Caixilho> GetById(int id)
        {
            return await _context.Caixilhos.FindAsync(id);
        }

        public async Task UpdateAsync(Caixilho caixilho)
        {
            // Buscar o caixilho original para verificar se mudou de família
            var caixilhoOriginal = await _context.Caixilhos.AsNoTracking().FirstOrDefaultAsync(c => c.IdCaixilho == caixilho.IdCaixilho);
            var familiaAnterior = caixilhoOriginal?.IdFamiliaCaixilho;
            var familiaAtual = caixilho.IdFamiliaCaixilho;

            _context.Caixilhos.Update(caixilho);
            await _context.SaveChangesAsync();
            
            // Recalcular o peso da família atual
            await AtualizarPesoFamiliaAsync(familiaAtual);
            
            // Se mudou de família, recalcular também a família anterior
            if (familiaAnterior.HasValue && familiaAnterior.Value != familiaAtual)
            {
                await AtualizarPesoFamiliaAsync(familiaAnterior.Value);
            }
        }

        // Método privado para atualizar o peso da família
        private async Task AtualizarPesoFamiliaAsync(int familiaId)
        {
            // Calcular o peso total dos caixilhos desta família
            var pesoTotal = await _context.Caixilhos
                .Where(c => c.IdFamiliaCaixilho == familiaId)
                .SumAsync(c => c.PesoUnitario * c.Quantidade);

            // Atualizar o peso na família
            var familia = await _context.FamCaixilhos.FindAsync(familiaId);
            if (familia != null)
            {
                familia.PesoTotal = (int)pesoTotal;
                await _context.SaveChangesAsync();
            }
        }
    }
}
