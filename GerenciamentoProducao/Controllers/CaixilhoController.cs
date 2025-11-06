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
            
            // Determinar os valores corretos para os IDs
            int obraId = model?.ObraId ?? 0;
            int familiaId = model?.IdFamiliaCaixilho > 0 ? model.IdFamiliaCaixilho : (model?.FamiliaCaixilhoId ?? 0);
            int tipoId = model?.IdTipoCaixilho > 0 ? model.IdTipoCaixilho : (model?.TipoCaixilhoId ?? 0);
            
            return new CaixilhoViewModel
            {
                IdCaixilho = model?.IdCaixilho ?? 0,
                NomeCaixilho = model?.NomeCaixilho,
                Largura = model?.Largura ?? 0,
                Altura = model?.Altura ?? 0,
                Quantidade = model?.Quantidade ?? 0,
                PesoUnitario = model?.PesoUnitario ?? 0,
                ObraId = obraId,
                // Preencher ambas as propriedades para compatibilidade
                FamiliaCaixilhoId = familiaId,
                IdFamiliaCaixilho = familiaId,
                TipoCaixilhoId = tipoId,
                IdTipoCaixilho = tipoId,
                // Campos adicionais
                Liberado = model?.Liberado ?? false,
                DataLiberacao = model?.DataLiberacao,
                StatusProducao = model?.StatusProducao ?? "Pendente",
                Observacoes = model?.Observacoes,
                //Dropdrowns 
                Obra = obras.Select(o => new SelectListItem
                {
                    Value = o.IdObra.ToString(),
                    Text = o.Nome,
                    Selected = o.IdObra == obraId
                }),
                FamiliaCaixilho = familiaCaixilhos.Select(f => new SelectListItem
                {
                    Value = f.IdFamiliaCaixilho.ToString(),
                    Text = f.DescricaoFamilia,
                    Selected = f.IdFamiliaCaixilho == familiaId
                }),
                TipoCaixilho = tipoCaixilhos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoCaixilho.ToString(),
                    Text = t.DescricaoCaixilho,
                    Selected = t.IdTipoCaixilho == tipoId
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
            // Validar se os IDs de chave estrangeira existem
            if (caixilhoViewModel.IdFamiliaCaixilho > 0)
            {
                var familiaExiste = await _familiaCaixilhoRepository.GetByIdAsync(caixilhoViewModel.IdFamiliaCaixilho);
                if (familiaExiste == null)
                {
                    ModelState.AddModelError("IdFamiliaCaixilho", "A família de caixilho selecionada não existe.");
                }
            }
            else
            {
                // Tentar usar FamiliaCaixilhoId como fallback
                if (caixilhoViewModel.FamiliaCaixilhoId > 0)
                {
                    var familiaExiste = await _familiaCaixilhoRepository.GetByIdAsync(caixilhoViewModel.FamiliaCaixilhoId);
                    if (familiaExiste == null)
                    {
                        ModelState.AddModelError("IdFamiliaCaixilho", "A família de caixilho selecionada não existe.");
                    }
                    else
                    {
                        caixilhoViewModel.IdFamiliaCaixilho = caixilhoViewModel.FamiliaCaixilhoId;
                    }
                }
                else
                {
                    ModelState.AddModelError("IdFamiliaCaixilho", "É necessário selecionar uma família de caixilho.");
                }
            }
            
            if (caixilhoViewModel.IdTipoCaixilho > 0)
            {
                var tipoExiste = await _tipoCaixilhoRepository.GetById(caixilhoViewModel.IdTipoCaixilho);
                if (tipoExiste == null)
                {
                    ModelState.AddModelError("IdTipoCaixilho", "O tipo de caixilho selecionado não existe.");
                }
            }
            else
            {
                // Tentar usar TipoCaixilhoId como fallback
                if (caixilhoViewModel.TipoCaixilhoId > 0)
                {
                    var tipoExiste = await _tipoCaixilhoRepository.GetById(caixilhoViewModel.TipoCaixilhoId);
                    if (tipoExiste == null)
                    {
                        ModelState.AddModelError("IdTipoCaixilho", "O tipo de caixilho selecionado não existe.");
                    }
                    else
                    {
                        caixilhoViewModel.IdTipoCaixilho = caixilhoViewModel.TipoCaixilhoId;
                    }
                }
                else
                {
                    ModelState.AddModelError("IdTipoCaixilho", "É necessário selecionar um tipo de caixilho.");
                }
            }
            
            if (ModelState.IsValid)
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
                    IdFamiliaCaixilho = caixilhoViewModel.IdFamiliaCaixilho,
                    IdTipoCaixilho = caixilhoViewModel.IdTipoCaixilho
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
                // Preencher ambas as propriedades para compatibilidade
                FamiliaCaixilhoId = caixilho.IdFamiliaCaixilho,
                IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho,
                TipoCaixilhoId = caixilho.IdTipoCaixilho,
                IdTipoCaixilho = caixilho.IdTipoCaixilho,
                
                // Campos adicionais
                Liberado = caixilho.Liberado,
                DataLiberacao = caixilho.DataLiberacao,
                StatusProducao = caixilho.StatusProducao,
                Observacoes = caixilho.Observacoes,
                
                Obra = obras.Select(o => new SelectListItem
                {
                    Value = o.IdObra.ToString(),
                    Text = o.Nome,
                    Selected = o.IdObra == caixilho.ObraId
                }),
                FamiliaCaixilho = familiaCaixilhos.Select(f => new SelectListItem
                {
                    Value = f.IdFamiliaCaixilho.ToString(),
                    Text = f.DescricaoFamilia,
                    Selected = f.IdFamiliaCaixilho == caixilho.IdFamiliaCaixilho
                }),
                TipoCaixilho = tipoCaixilhos.Select(t => new SelectListItem
                {
                    Value = t.IdTipoCaixilho.ToString(),
                    Text = t.DescricaoCaixilho,
                    Selected = t.IdTipoCaixilho == caixilho.IdTipoCaixilho
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
            
            // Remover erros de validação dos objetos de navegação (não são necessários)
            ModelState.Remove("Obra");
            ModelState.Remove("FamiliaCaixilho");
            ModelState.Remove("TipoCaixilho");
            
            // Validar se os IDs de chave estrangeira são válidos
            if (viewModel.ObraId <= 0)
            {
                ModelState.AddModelError("ObraId", "É necessário selecionar uma obra.");
            }
            
            if (viewModel.IdFamiliaCaixilho <= 0)
            {
                // Tentar usar FamiliaCaixilhoId como fallback
                if (viewModel.FamiliaCaixilhoId > 0)
                {
                    viewModel.IdFamiliaCaixilho = viewModel.FamiliaCaixilhoId;
                }
                else
                {
                    ModelState.AddModelError("IdFamiliaCaixilho", "É necessário selecionar uma família de caixilho.");
                }
            }
            
            if (viewModel.IdTipoCaixilho <= 0)
            {
                // Tentar usar TipoCaixilhoId como fallback
                if (viewModel.TipoCaixilhoId > 0)
                {
                    viewModel.IdTipoCaixilho = viewModel.TipoCaixilhoId;
                }
                else
                {
                    ModelState.AddModelError("IdTipoCaixilho", "É necessário selecionar um tipo de caixilho.");
                }
            }
            
            if (ModelState.IsValid) {
                var caixilho = await _caixilhoRepository.GetById(id);
                if (caixilho == null) { return NotFound(); }
                
                // Atualizar todas as propriedades
                caixilho.NomeCaixilho = viewModel.NomeCaixilho;
                caixilho.Largura = viewModel.Largura;
                caixilho.Altura = viewModel.Altura;
                caixilho.Quantidade = viewModel.Quantidade;
                caixilho.PesoUnitario = viewModel.PesoUnitario;
                caixilho.ObraId = viewModel.ObraId;
                caixilho.IdFamiliaCaixilho = viewModel.IdFamiliaCaixilho;
                caixilho.IdTipoCaixilho = viewModel.IdTipoCaixilho;
                caixilho.Liberado = viewModel.Liberado;
                caixilho.DataLiberacao = viewModel.DataLiberacao;
                caixilho.StatusProducao = viewModel.StatusProducao;
                caixilho.Observacoes = viewModel.Observacoes;
                
                await _caixilhoRepository.UpdateAsync(caixilho);
                return RedirectToAction(nameof(Index));
            }
            viewModel = await CriarCaixilhoViewModel(viewModel);
            return View(viewModel);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caixilho = await _caixilhoRepository.GetById(id.Value);
            if (caixilho == null)
            {
                return NotFound();
            }

            return View(caixilho);
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
                obra.Finalizado = true;
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
