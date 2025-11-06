using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducao.Controllers
{
    public class FamiliaCaixilhoController : Controller
    {

        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;
        public FamiliaCaixilhoController(IFamiliaCaixilhoRepository familiaCaixilhoRepository)
        {
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _familiaCaixilhoRepository.GetAllAsync();
            return View(lista);
        }


        [Authorize(Roles = "Administrador,Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FamiliaCaixilho familia)
        {
            if (ModelState.IsValid)
            {
                // Inicializar o peso como 0 - será calculado automaticamente
                familia.PesoTotal = 0;
                await _familiaCaixilhoRepository.AddAsync(familia);
                return RedirectToAction(nameof(Index));
            }
            return View(familia);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<ActionResult> Edit(int id)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null)
            {
                return NotFound();
            }
            return View(familia);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FamiliaCaixilho familia)
        {
            if (id != familia.IdFamiliaCaixilho)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _familiaCaixilhoRepository.UpdateAsync(familia);
                return RedirectToAction(nameof(Index));
            }
            return View(familia);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            
            var item = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _familiaCaixilhoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        // GET: FamiliaCaixilho/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id.Value);
            if (familia == null)
            {
                return NotFound();
            }
            return View(familia);
        }

        // POST: FamiliaCaixilho/RecalcularPesos
        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> RecalcularPesos()
        {
            try
            {
                var familias = await _familiaCaixilhoRepository.GetAllAsync();
                int familiasAtualizadas = 0;

                foreach (var familia in familias)
                {
                    await _familiaCaixilhoRepository.AtualizarPesoTotalAsync(familia.IdFamiliaCaixilho);
                    familiasAtualizadas++;
                }

                TempData["SuccessMessage"] = $"Pesos recalculados com sucesso para {familiasAtualizadas} famílias.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao recalcular pesos: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
