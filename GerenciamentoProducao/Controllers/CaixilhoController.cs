using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducaoo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//foi o ryan que fez
namespace GerenciamentoProducaoo.Controllers
{
    public class CaixilhoController : Controller
    {
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly ITipoCaixilhoRepository _tipoCaixilhoRepository;
        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;
        private readonly IObraRepository _obraRepository;
        public CaixilhoController(ICaixilhoRepository caixilhoRepository, ITipoCaixilhoRepository tipoCaixilhoRepository, IFamiliaCaixilhoRepository familiaCaixilhoRepository, IObraRepository obraRepository)
        {
            _caixilhoRepository = caixilhoRepository;
            _tipoCaixilhoRepository = tipoCaixilhoRepository;
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
            _obraRepository = obraRepository;
        }

        private async Task<CaixilhoViewModel> CriarCaixilhoViewModel(CaixilhoViewModel? model = null)
        {
            var tipoCaixilhos = await _tipoCaixilhoRepository.GetAllAsync();
            var familiaCaixilhos = await _familiaCaixilhoRepository.GetAllAsync();
            var obras = await _obraRepository.GetAllAsync();
            return new CaixilhoViewModel
            {
                IdCaixilho = model?.IdCaixilho ?? 0,
                NomeCaixilho = model?.NomeCaixilho,
                Largura = model?.Largura ?? 0,
                Altura = model?.Altura ?? 0,
                Quantidade = model?.Quantidade ?? 0,
                PesoUnitario = model?.PesoUnitario ?? 0,
                ObraId = model?.ObraId ?? 0,
                IdFamiliaCaixilho = model?.FamiliaCaixilhoId ?? 0,
                IdTipoCaixilho = model?.TipoCaixilhoId ?? 0,
                //Dropdrowns 
                Obra = obras.Select(o => new SelectListItem
                {
                    Value = o.IdObra.ToString(),
                    Text = o.Nome
                }),
                FamiliaCaixilho = familiaCaixilhos.Select(f => new SelectListItem
                {
                    Value = f.IdFamiliaCaixilho.ToString(),
                    Text = f.DescricaoFamilia
                }),
                TipoCaixilho = tipoCaixilhos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoCaixilho.ToString(),
                    Text = t.DescricaoCaixilho
                })
            };
        }
        public async Task<IActionResult> Index()
        {
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            return View(caixilhos);
        }


        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Create()
        {
            var model = await CriarCaixilhoViewModel();
            return View(model);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CaixilhoViewModel caixilhoViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Mapear ViewModel para Model
                var caixilho = new Caixilho
                {
                    NomeCaixilho = caixilhoViewModel.NomeCaixilho,
                    Largura = caixilhoViewModel.Largura,
                    Altura = caixilhoViewModel.Altura,
                    Quantidade = caixilhoViewModel.Quantidade,
                    PesoUnitario = caixilhoViewModel.PesoUnitario,
                    ObraId = caixilhoViewModel.ObraId,
                    IdFamiliaCaixilho = caixilhoViewModel.FamiliaCaixilhoId,
                    IdTipoCaixilho = caixilhoViewModel.TipoCaixilhoId
                };
                await _caixilhoRepository.AddAsync(caixilho);
                return RedirectToAction(nameof(Index));
            }
            var model = await CriarCaixilhoViewModel(caixilhoViewModel);
            return View(model);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            var tipoCaixilhos = await _tipoCaixilhoRepository.GetAllAsync();
            var familiaCaixilhos = await _familiaCaixilhoRepository.GetAllAsync();
            var obras = await _obraRepository.GetAllAsync();

            if (caixilho == null) return NotFound();
            var model = new CaixilhoViewModel
            {
                IdCaixilho = caixilho.IdCaixilho,
                NomeCaixilho = caixilho.NomeCaixilho,
                Largura = caixilho.Largura,
                Altura = caixilho.Altura,
                Quantidade = caixilho.Quantidade,
                PesoUnitario = caixilho.PesoUnitario,

                ObraId = caixilho.ObraId,
                FamiliaCaixilhoId = caixilho.IdFamiliaCaixilho,
                TipoCaixilhoId = caixilho.IdTipoCaixilho,
                //Dropdrowns 
                Obra = obras.Select(o => new SelectListItem
                {
                    Value = o.IdObra.ToString(),
                    Text = o.Nome
                }),
                FamiliaCaixilho = familiaCaixilhos.Select(f => new SelectListItem
                {
                    Value = f.IdFamiliaCaixilho.ToString(),
                    Text = f.DescricaoFamilia
                }),
                TipoCaixilho = tipoCaixilhos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoCaixilho.ToString(),
                    Text = t.DescricaoCaixilho
                })
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CaixilhoViewModel viewModel)
        {
            if (id != viewModel.IdCaixilho)
            {
                return NotFound();
            }
            if (!ModelState.IsValid) {
                var caixilho = await _caixilhoRepository.GetById(id);
                if (caixilho == null) { return NotFound(); }
                caixilho.NomeCaixilho = viewModel.NomeCaixilho;
                caixilho.Largura = viewModel.Largura;
                caixilho.Altura = viewModel.Altura;
                caixilho.Quantidade = viewModel.Quantidade;
                caixilho.PesoUnitario = viewModel.PesoUnitario;
                caixilho.ObraId = viewModel.ObraId;
                caixilho.IdFamiliaCaixilho = viewModel.FamiliaCaixilhoId;
                caixilho.IdTipoCaixilho = viewModel.TipoCaixilhoId;
                await _caixilhoRepository.UpdateAsync(caixilho);
                return RedirectToAction(nameof(Index));
            }
            //Se o modelo não estiver válido, recarrega o formulário
            // podemos trocar para outra coisa invés disso
            viewModel = await CriarCaixilhoViewModel(viewModel);
            return View(viewModel);
        }


        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _caixilhoRepository.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _caixilhoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Método para liberar um caixilho individual
        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Liberar(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null)
                return NotFound();

            caixilho.Liberado = true;
            caixilho.DataLiberacao = DateTime.Now;
            caixilho.StatusProducao = "Liberado";

            await _caixilhoRepository.UpdateAsync(caixilho);
            
            // Atualizar peso produzido da obra
            await AtualizarPesoObra(caixilho.ObraId);
            
            return Json(new { success = true, message = "Caixilho liberado com sucesso!" });
        }

        // Método para liberar todos os caixilhos de uma família
        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> LiberarFamilia(int familiaId)
        {
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            var caixilhosFamilia = caixilhos.Where(c => c.IdFamiliaCaixilho == familiaId && !c.Liberado).ToList();

            var obrasAfetadas = new HashSet<int>();

            foreach (var caixilho in caixilhosFamilia)
            {
                caixilho.Liberado = true;
                caixilho.DataLiberacao = DateTime.Now;
                caixilho.StatusProducao = "Liberado";
                await _caixilhoRepository.UpdateAsync(caixilho);
                
                obrasAfetadas.Add(caixilho.ObraId);
            }

            // Atualizar peso das obras afetadas
            foreach (var obraId in obrasAfetadas)
            {
                await AtualizarPesoObra(obraId);
            }

            return Json(new { success = true, message = $"Liberados {caixilhosFamilia.Count} caixilhos da família!" });
        }

        // View para gerenciar liberações
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Liberacoes()
        {
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            var caixilhosPendentes = caixilhos.Where(c => !c.Liberado).ToList();
            
            return View(caixilhosPendentes);
        }

        // Método para atualizar o peso produzido de uma obra específica
        private async Task AtualizarPesoObra(int obraId)
        {
            var obra = await _obraRepository.GetById(obraId);
            if (obra == null) return;

            // Calcular peso total dos caixilhos liberados desta obra
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            var pesoProduzido = caixilhos
                .Where(c => c.ObraId == obraId && c.Liberado)
                .Sum(c => c.PesoUnitario * c.Quantidade);

            // Atualizar o peso produzido da obra
            obra.PesoProduzido = pesoProduzido;
            
            // Calcular percentual de conclusão baseado no peso
            if (obra.PesoFinal > 0)
            {
                obra.PercentualConclusao = Math.Min(100, (pesoProduzido / obra.PesoFinal) * 100);
            }
            else
            {
                obra.PercentualConclusao = 0;
            }

            // Atualizar status da obra baseado no progresso
            if (obra.PercentualConclusao >= 100)
            {
                obra.StatusObra = "Concluída";
                obra.DataConclusao = DateTime.Now;
            }
            else if (obra.PercentualConclusao > 0)
            {
                obra.StatusObra = "Em Andamento";
            }

            await _obraRepository.UpdateAsync(obra);
        }

    }
}
