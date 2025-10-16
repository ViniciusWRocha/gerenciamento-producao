using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.Repositories;
using GerenciamentoProducaoo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducaoo.Controllers
{
    public class ObraController : Controller
    {
        private readonly IObraRepository _obraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        public ObraController(IObraRepository obraRepository, IUsuarioRepository usuarioRepository)
        {
            _obraRepository = obraRepository;
            _usuarioRepository = usuarioRepository;
        }
        private async Task<ObraViewModel> CriarObraViewModel(ObraViewModel? model = null)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            return new ObraViewModel
            {
                IdObra = model?.IdObra ?? 0,
                Nome = model?.Nome,
                Construtora = model?.Construtora,
                Nro = model?.Nro,
                Logradouro = model?.Logradouro,
                Bairro = model?.Bairro,
                Cep = model?.Cep,
                Uf = model?.Uf,
                Cnpj = model?.Cnpj,
                IdUsuario = model?.IdUsuario ?? 0,
                Usuario = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario,
                    Selected = model != null && t.IdUsuario == model.IdUsuario
                })
            };
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? IdUsuario,string? search)
        {
            var obras = await _obraRepository.GetAllAsync();
            if (IdUsuario.HasValue && IdUsuario.Value > 0)
            {
                obras = obras.Where(o => o.IdUsuario == IdUsuario.Value).ToList(); 
            }
            if (!string.IsNullOrEmpty(search))
            {
                obras = obras.Where(o => o.Nome.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            obras = obras.OrderByDescending(o => o.IdObra).ToList();

            ViewBag.Usuarios = new SelectList(await _usuarioRepository.GetAllAsync(), "IdUsuario", "NomeUsuario");
            ViewBag.TermoBusca = search;

            return View(obras);
        }


        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Create()
        {
            var model = await CriarObraViewModel();
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ObraViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
              var vm = await CriarObraViewModel(viewModel);
                return View(vm);
            }
            var obra = new Obra
            {
                Nome = viewModel.Nome,
                Construtora = viewModel.Construtora,
                Nro = viewModel.Nro,
                Logradouro = viewModel.Logradouro,
                Bairro = viewModel.Bairro,
                Cep = viewModel.Cep,
                Uf = viewModel.Uf,
                Cnpj = viewModel.Cnpj,
                IdUsuario = viewModel.IdUsuario
            };
            await _obraRepository.AddAsync(obra);
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Administrador,Gerente")]

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();
            var item = await _obraRepository.GetById(id);
            if (item == null) return NotFound();
            var vm = await CriarObraViewModel(new ObraViewModel
            {
                IdObra = item.IdObra,
                Nome = item.Nome,
                Construtora = item.Construtora,
                Nro = item.Nro,
                Logradouro = item.Logradouro,
                Bairro = item.Bairro,
                Cep = item.Cep,
                Uf = item.Uf,
                Cnpj = item.Cnpj,
                IdUsuario = item.IdUsuario,
                Usuario = (await _usuarioRepository.GetAllAsync()).Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario
                })
            });
            return View(vm);
        }


        
        [HttpPut]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ObraViewModel viewModel)
        {
            if (id != viewModel.IdObra)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                viewModel = await CriarObraViewModel(viewModel);
                return RedirectToAction(nameof(Index));
            }
            
            var obra = await _obraRepository.GetById(id);
            if (obra == null) return NotFound();
            
            obra.Nome = viewModel.Nome;
            obra.Construtora = viewModel.Construtora;
            obra.Nro = viewModel.Nro;
            obra.Logradouro = viewModel.Logradouro;
            obra.Bairro = viewModel.Bairro;
            obra.Cep = viewModel.Cep;
            obra.Uf = viewModel.Uf;
            obra.Cnpj = viewModel.Cnpj;
            obra.IdUsuario = viewModel.IdUsuario;

            await _obraRepository.UpdateAsync(obra);
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _obraRepository.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }
        

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _obraRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }

}
