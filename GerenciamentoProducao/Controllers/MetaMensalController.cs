using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GerenciamentoProducao.Data;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Controllers
{
    public class MetaMensalController : Controller
    {
        private readonly GerenciamentoProdDbContext _context;

        public MetaMensalController(GerenciamentoProdDbContext context)
        {
            _context = context;
        }

        // GET: MetaMensal
        public async Task<IActionResult> Index()
        {
            var metas = await _context.MetasMensais
                .Include(m => m.Usuario)
                .ToListAsync();
            return View(metas);
        }

        // GET: MetaMensal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metaMensal = await _context.MetasMensais
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.IdMetaMensal == id);
            if (metaMensal == null)
            {
                return NotFound();
            }

            return View(metaMensal);
        }

        // GET: MetaMensal/Create
        public IActionResult Create()
        {
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario");
            return View();
        }

        // POST: MetaMensal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMetaMensal,Ano,Mes,MetaPesoKg,IdUsuario,Observacoes")] MetaMensal metaMensal)
        {
            if (ModelState.IsValid)
            {
                metaMensal.DataCriacao = DateTime.Now;
                _context.Add(metaMensal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario", metaMensal.IdUsuario);
            return View(metaMensal);
        }

        // GET: MetaMensal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metaMensal = await _context.MetasMensais.FindAsync(id);
            if (metaMensal == null)
            {
                return NotFound();
            }
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario", metaMensal.IdUsuario);
            return View(metaMensal);
        }

        // POST: MetaMensal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMetaMensal,Ano,Mes,MetaPesoKg,PesoProduzido,PercentualAtingido,MetaAtingida,IdUsuario,Observacoes")] MetaMensal metaMensal)
        {
            if (id != metaMensal.IdMetaMensal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    metaMensal.DataAtualizacao = DateTime.Now;
                    _context.Update(metaMensal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MetaMensalExists(metaMensal.IdMetaMensal))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario", metaMensal.IdUsuario);
            return View(metaMensal);
        }

        // GET: MetaMensal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metaMensal = await _context.MetasMensais
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.IdMetaMensal == id);
            if (metaMensal == null)
            {
                return NotFound();
            }

            return View(metaMensal);
        }

        // POST: MetaMensal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var metaMensal = await _context.MetasMensais.FindAsync(id);
            if (metaMensal != null)
            {
                _context.MetasMensais.Remove(metaMensal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MetaMensalExists(int id)
        {
            return _context.MetasMensais.Any(e => e.IdMetaMensal == id);
        }
    }
}
