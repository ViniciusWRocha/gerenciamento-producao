using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.Repositories;
using GerenciamentoProducao.Services;
using GerenciamentoProducaoo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducaoo.Controllers
{
    public class ObraController : Controller
    {
        private readonly string _calendarId;


        private readonly IObraRepository _obraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly GoogleCalendarService _calendarService;

        
        public ObraController(IObraRepository obraRepository, IUsuarioRepository usuarioRepository, GoogleCalendarService calendarService, IConfiguration configuration)
        {
            _obraRepository = obraRepository;
            _usuarioRepository = usuarioRepository;
            _calendarService = calendarService;
            _calendarId = configuration["Google:key"];
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
                DataInicio = model?.DataInicio ?? DateTime.Now,
                DataTermino = model?.DataTermino ?? DateTime.Now.AddMonths(6),
                PesoFinal = model?.PesoFinal ?? 0,
                PesoProduzido = model?.PesoProduzido ?? 0,
                StatusObra = model?.StatusObra ?? "Planejada",
                Prioridade = model?.Prioridade ?? "Normal",
                Bandeira = model?.Bandeira ?? "Verde",
                PercentualConclusao = model?.PercentualConclusao ?? 0,
                DataConclusao = model?.DataConclusao,
                Observacoes = model?.Observacoes,
                Finalizado = model?.Finalizado ?? false,
                IdUsuario = model?.IdUsuario ?? 0,
                
                GoogleCalendarEventId = model?.GoogleCalendarEventId,
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
            var obras = await _obraRepository.GetAllNaoFinalizadosAsync();
            if (IdUsuario.HasValue && IdUsuario.Value > 0)
            {
                obras = obras.Where(o => o.IdUsuario == IdUsuario.Value).Where(o => o.Finalizado == false).ToList(); 
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
        [Authorize(Roles = "Administrador,Gerente")]
        [HttpGet]

        public async Task<IActionResult> Finalizados()
        {
            var obras = await _obraRepository.GetAllFinalizadosAsync();
            obras = obras.OrderByDescending(o => o.IdObra).ToList();
            return View(obras);
        }


        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Create()
        {
            var model = await CriarObraViewModel();
            return View(model);
        }

        [Authorize(Roles = "Administrador,Gerente")]
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
                DataInicio = viewModel.DataInicio,
                DataTermino = viewModel.DataTermino,
                PesoFinal = viewModel.PesoFinal,
                PesoProduzido = viewModel.PesoProduzido,
                StatusObra = viewModel.StatusObra,
                Prioridade = viewModel.Prioridade,
                Bandeira = viewModel.Bandeira,
                PercentualConclusao = viewModel.PercentualConclusao,
                DataConclusao = viewModel.DataConclusao,
                Observacoes = viewModel.Observacoes,
                Finalizado = viewModel.Finalizado,
                IdUsuario = viewModel.IdUsuario
            };
            
            // Criar evento no Google Calendar primeiro
            try
            {

                var calendarId = _calendarId;
                var eventTitle = $"🏗️ Obra: {obra.Nome}";
                var eventDescription = $"Obra: {obra.Nome}\n" +
                                    $"Construtora: {obra.Construtora}\n" +
                                    $"Endereço: {obra.Logradouro}, {obra.Nro} - {obra.Bairro}\n" +
                                    $"Status: {obra.StatusObra}\n" +
                                    $"Prioridade: {obra.Prioridade}\n" +
                                    $"Peso Final: {obra.PesoFinal} kg\n" +
                                    (!string.IsNullOrEmpty(obra.Observacoes) ? $"Observações: {obra.Observacoes}" : "");
                
                var calendarEvent = _calendarService.CreateEvent(
                    calendarId,
                    eventTitle,
                    obra.DataInicio,
                    obra.DataTermino,
                    eventDescription
                );
                
                // Armazenar o ID do evento na obra
                obra.GoogleCalendarEventId = calendarEvent.Id;
                
                TempData["SuccessMessage"] = $"Obra criada com sucesso! Evento adicionado ao calendário: {calendarEvent.HtmlLink}";
            }
            catch (Exception ex)
            {
                // Log do erro, mas não impede a criação da obra
                TempData["WarningMessage"] = $"Obra criada com sucesso, mas houve erro ao adicionar ao calendário: {ex.Message}";
            }
            
            await _obraRepository.AddAsync(obra);
            
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Administrador,Gerente")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();
     

            var item = await _obraRepository.GetById(id);
            if (item == null)
                return NotFound();

            // Cria o ViewModel baseado na entidade Obra
            var vm = new ObraViewModel
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
                DataInicio = item.DataInicio,
                DataTermino = item.DataTermino,
                PesoFinal = item.PesoFinal,
                PesoProduzido = item.PesoProduzido,
                StatusObra = item.StatusObra,
                Prioridade = item.Prioridade,
                Bandeira = item.Bandeira,
                PercentualConclusao = item.PercentualConclusao,
                DataConclusao = item.DataConclusao,
                Observacoes = item.Observacoes,
                Finalizado = item.Finalizado,
                GoogleCalendarEventId = item.GoogleCalendarEventId,
                IdUsuario = item.IdUsuario,
                Usuario = (await _usuarioRepository.GetAllAsync())
                    .Select(t => new SelectListItem
                    {
                        Value = t.IdUsuario.ToString(),
                        Text = t.NomeUsuario
                    })
            };

            return View(vm); 
        }

        

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ObraViewModel viewModel)
        {
            if (id != viewModel.IdObra) return NotFound();

            if (!ModelState.IsValid)
            {
                // recria as opções do dropdown antes de voltar pra view
                viewModel.Usuario = (await _usuarioRepository.GetAllAsync())
                    .Select(t => new SelectListItem
                    {
                        Value = t.IdUsuario.ToString(),
                        Text = t.NomeUsuario
                    });
                return View(viewModel);
            }

            var obra = await _obraRepository.GetById(id);
            if (obra == null)
                return NotFound();

            obra.Nome = viewModel.Nome;
            obra.Construtora = viewModel.Construtora;
            obra.Nro = viewModel.Nro;
            obra.Logradouro = viewModel.Logradouro;
            obra.Bairro = viewModel.Bairro;
            obra.Cep = viewModel.Cep;
            obra.Uf = viewModel.Uf;
            obra.Cnpj = viewModel.Cnpj;
            obra.DataInicio = viewModel.DataInicio;
            obra.DataTermino = viewModel.DataTermino;
            obra.PesoFinal = viewModel.PesoFinal;
            obra.PesoProduzido = viewModel.PesoProduzido;
            obra.StatusObra = viewModel.StatusObra;
            obra.Prioridade = viewModel.Prioridade;
            obra.Bandeira = viewModel.Bandeira;
            obra.PercentualConclusao = viewModel.PercentualConclusao;
            obra.DataConclusao = viewModel.DataConclusao;
            obra.Observacoes = viewModel.Observacoes;
            obra.IdUsuario = viewModel.IdUsuario;

            // Atualizar evento no Google Calendar (se existir)
            try
            {
                var calendarId = _calendarId;
                var eventTitle = $"🏗️ Obra: {obra.Nome}";
                var eventDescription = $"Obra: {obra.Nome}\n" +
                                    $"Construtora: {obra.Construtora}\n" +
                                    $"Endereço: {obra.Logradouro}, {obra.Nro} - {obra.Bairro}\n" +
                                    $"Status: {obra.StatusObra}\n" +
                                    $"Prioridade: {obra.Prioridade}\n" +
                                    $"Peso Final: {obra.PesoFinal} kg\n" +
                                    (!string.IsNullOrEmpty(obra.Observacoes) ? $"Observações: {obra.Observacoes}" : "");
                
                if (!string.IsNullOrEmpty(obra.GoogleCalendarEventId))
                {
                    // Atualizar evento existente
                    var calendarEvent = _calendarService.UpdateEvent(
                        calendarId,
                        obra.GoogleCalendarEventId,
                        eventTitle,
                        obra.DataInicio,
                        obra.DataTermino,
                        eventDescription
                    );
                    
                    TempData["SuccessMessage"] = $"Obra atualizada com sucesso! Evento atualizado no calendário.";
                }
                else
                {
                    // Criar novo evento se não existir ID
                    var calendarEvent = _calendarService.CreateEvent(
                        calendarId,
                        eventTitle,
                        obra.DataInicio,
                        obra.DataTermino,
                        eventDescription
                    );
                    
                    obra.GoogleCalendarEventId = calendarEvent.Id;
                    TempData["SuccessMessage"] = $"Obra atualizada com sucesso! Novo evento criado no calendário.";
                }
            }
            catch (Exception ex)
            {
                TempData["WarningMessage"] = $"Obra atualizada com sucesso, mas houve erro ao atualizar o calendário: {ex.Message}";
            }
            
            await _obraRepository.UpdateAsync(obra);

            return RedirectToAction(nameof(Index));
        }



        // GET: Obra/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obra = await _obraRepository.GetById(id.Value);
            if (obra == null)
            {
                return NotFound();
            }

            return View(obra);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idObra)
        {
            // Buscar a obra antes de deletar para obter o ID do evento
            var obra = await _obraRepository.GetById(idObra);
            if (obra == null)
            {
                return NotFound();
            }

            // Deletar evento do Google Calendar (se existir)
            if (!string.IsNullOrEmpty(obra.GoogleCalendarEventId))
            {
                try
                {
                    var calendarId = _calendarId;
                    _calendarService.DeleteEvent(calendarId, obra.GoogleCalendarEventId);
                    TempData["SuccessMessage"] = "Obra e evento do calendário deletados com sucesso!";
                }
                catch (Exception ex)
                {
                    TempData["WarningMessage"] = $"Obra deletada com sucesso, mas houve erro ao deletar o evento do calendário: {ex.Message}";
                }
            }
            else
            {
                TempData["SuccessMessage"] = "Obra deletada com sucesso!";
            }

            await _obraRepository.DeleteAsync(idObra);
            return RedirectToAction(nameof(Index));
        }

    }

}
