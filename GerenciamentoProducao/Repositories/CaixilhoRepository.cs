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
            return await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .Include(c => c.TipoCaixilho)
                .ToListAsync();
        }

        public async Task<Caixilho> GetById(int id)
        {
            return await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .Include(c => c.TipoCaixilho)
                .FirstOrDefaultAsync(c => c.IdCaixilho == id);
        }

        public async Task UpdateAsync(Caixilho caixilho)
        {
            // Buscar o caixilho original do contexto para verificar se mudou de família
            var caixilhoOriginal = await _context.Caixilhos.AsNoTracking().FirstOrDefaultAsync(c => c.IdCaixilho == caixilho.IdCaixilho);
            var familiaAnterior = caixilhoOriginal?.IdFamiliaCaixilho;
            var familiaAtual = caixilho.IdFamiliaCaixilho;

            // Buscar a entidade rastreada do contexto
            var caixilhoTracked = await _context.Caixilhos.FindAsync(caixilho.IdCaixilho);
            if (caixilhoTracked == null)
            {
                throw new InvalidOperationException($"Caixilho com ID {caixilho.IdCaixilho} não encontrado.");
            }

            // Atualizar as propriedades da entidade rastreada
            caixilhoTracked.NomeCaixilho = caixilho.NomeCaixilho;
            caixilhoTracked.Largura = caixilho.Largura;
            caixilhoTracked.Altura = caixilho.Altura;
            caixilhoTracked.Quantidade = caixilho.Quantidade;
            caixilhoTracked.PesoUnitario = caixilho.PesoUnitario;
            caixilhoTracked.ObraId = caixilho.ObraId;
            caixilhoTracked.IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho;
            caixilhoTracked.IdTipoCaixilho = caixilho.IdTipoCaixilho;
            caixilhoTracked.Liberado = caixilho.Liberado;
            caixilhoTracked.DataLiberacao = caixilho.DataLiberacao;
            caixilhoTracked.StatusProducao = caixilho.StatusProducao;
            caixilhoTracked.Observacoes = caixilho.Observacoes;

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
