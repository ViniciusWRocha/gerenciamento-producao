using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GerenciamentoProducao.Data;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Controllers
{
    public class RelatorioProducaoController : Controller
    {
        private readonly GerenciamentoProdDbContext _context;

        public RelatorioProducaoController(GerenciamentoProdDbContext context)
        {
            _context = context;
        }

        // GET: RelatorioProducao
        public async Task<IActionResult> Index()
        {
            var relatorios = await _context.RelatoriosProducao
                .Include(r => r.Usuario)
                .ToListAsync();
            return View(relatorios);
        }

        // GET: RelatorioProducao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatorioProducao = await _context.RelatoriosProducao
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.IdRelatorio == id);
            if (relatorioProducao == null)
            {
                return NotFound();
            }

            return View(relatorioProducao);
        }

        // GET: RelatorioProducao/Create
        public IActionResult Create()
        {
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario");
            return View();
        }

        // POST: RelatorioProducao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRelatorio,DataRelatorio,Ano,Mes,PesoTotalProduzido,TotalCaixilhosProduzidos,TotalFamiliasProduzidas,EficienciaProducao,TempoMedioProducao,StatusMeta,Observacoes,IdUsuario")] RelatorioProducao relatorioProducao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relatorioProducao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario", relatorioProducao.IdUsuario);
            return View(relatorioProducao);
        }

        // GET: RelatorioProducao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatorioProducao = await _context.RelatoriosProducao.FindAsync(id);
            if (relatorioProducao == null)
            {
                return NotFound();
            }
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario", relatorioProducao.IdUsuario);
            return View(relatorioProducao);
        }

        // POST: RelatorioProducao/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRelatorio,DataRelatorio,Ano,Mes,PesoTotalProduzido,TotalCaixilhosProduzidos,TotalFamiliasProduzidas,EficienciaProducao,TempoMedioProducao,StatusMeta,Observacoes,IdUsuario")] RelatorioProducao relatorioProducao)
        {
            if (id != relatorioProducao.IdRelatorio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relatorioProducao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelatorioProducaoExists(relatorioProducao.IdRelatorio))
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
            ViewBag.IdUsuario = new SelectList(_context.Usuarios, "IdUsuario", "NomeUsuario", relatorioProducao.IdUsuario);
            return View(relatorioProducao);
        }

        // GET: RelatorioProducao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatorioProducao = await _context.RelatoriosProducao
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.IdRelatorio == id);
            if (relatorioProducao == null)
            {
                return NotFound();
            }

            return View(relatorioProducao);
        }

        // POST: RelatorioProducao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relatorioProducao = await _context.RelatoriosProducao.FindAsync(id);
            if (relatorioProducao != null)
            {
                _context.RelatoriosProducao.Remove(relatorioProducao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelatorioProducaoExists(int id)
        {
            return _context.RelatoriosProducao.Any(e => e.IdRelatorio == id);
        }
    }
}