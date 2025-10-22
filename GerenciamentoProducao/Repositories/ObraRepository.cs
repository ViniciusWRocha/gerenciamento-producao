using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoProducao.Repositories
{
    public class ObraRepository : IObraRepository
    {
        private readonly GerenciamentoProdDbContext _context;
        public ObraRepository(GerenciamentoProdDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Obra obra)
        {
            await _context.Obras.AddAsync(obra);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var obra = await _context.Obras.FindAsync(id);
            if (obra != null)
            {
                _context.Obras.Remove(obra);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Obra>> GetAllAsync()
        {
            return await _context.Obras.ToListAsync();
        }

        public async Task<Obra> GetById(int id)
        {
            return await _context.Obras.FindAsync(id);

        }

        public async Task UpdateAsync(Obra obra)
        {
            _context.Obras.Update(obra);
            await _context.SaveChangesAsync();
        }
    }
}
