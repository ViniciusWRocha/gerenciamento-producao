using GerenciamentoProducao.Data;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoProducao.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly GerenciamentoProdDbContext _context;
        private readonly IObraRepository _obraRepository;
        private readonly IProducaoRepository _producaoRepository;
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public DashboardController(
            GerenciamentoProdDbContext context,
            IObraRepository obraRepository,
            IProducaoRepository producaoRepository,
            ICaixilhoRepository caixilhoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _context = context;
            _obraRepository = obraRepository;
            _producaoRepository = producaoRepository;
            _caixilhoRepository = caixilhoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Atualizar peso produzido das obras baseado nos caixilhos liberados
            await AtualizarPesoProduzidoObras();
            
            var metricas = await ObterMetricas();
            var producoesRecentes = await ObterProducoesRecentes();
            var obrasRecentes = await ObterObrasRecentes();
            var alertas = await ObterAlertas();

            var dashboardData = new DashboardViewModel
            {
                Metricas = metricas,
                ProducoesRecentes = producoesRecentes,
                ObrasRecentes = obrasRecentes,
                Alertas = alertas
            };
            
            return View(dashboardData);
        }

        [HttpGet]
        public async Task<IActionResult> GetMetricas()
        {
            var metricas = await ObterMetricas();
            return Json(metricas);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducaoPorMes()
        {
            var producaoPorMes = await ObterProducaoPorMes();
            return Json(producaoPorMes);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusObras()
        {
            var statusObras = await ObterStatusObras();
            return Json(statusObras);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopUsuarios()
        {
            var topUsuarios = await ObterTopUsuarios();
            return Json(topUsuarios);
        }

        [HttpGet]
        public async Task<IActionResult> GetObrasPorBandeira()
        {
            var obrasPorBandeira = await ObterObrasPorBandeira();
            return Json(obrasPorBandeira);
        }

        [HttpGet]
        public async Task<IActionResult> GetCaixilhosParaLiberacao()
        {
            var caixilhos = await _context.Caixilhos
                .Where(c => !c.Liberado)
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .Take(10)
                .Select(c => new
                {
                    Id = c.IdCaixilho,
                    Nome = c.NomeCaixilho,
                    Obra = c.Obra != null ? c.Obra.Nome : "N/A",
                    Familia = c.FamiliaCaixilho != null ? c.FamiliaCaixilho.DescricaoFamilia : "N/A",
                    Peso = c.PesoUnitario * c.Quantidade,
                    DataTerminoObra = c.Obra != null ? c.Obra.DataTermino : DateTime.MinValue,
                    Prioridade = c.Obra != null ? c.Obra.Prioridade : "Normal"
                })
                .ToListAsync();

            return Json(caixilhos);
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarProgressoObras()
        {
            await AtualizarPesoProduzidoObras();
            return Json(new { success = true, message = "Progresso das obras atualizado com sucesso!" });
        }


        private async Task<MetricasViewModel> ObterMetricas()
        {
            var totalObras = await _context.Obras.CountAsync();
            var totalProducoes = await _context.Producoes.CountAsync();
            var totalCaixilhos = await _context.Caixilhos.CountAsync();
            var totalUsuarios = await _context.Usuarios.CountAsync(u => u.Ativo);

            var producoesMesAtual = await _context.Producoes
                .Where(p => p.DataProducao.Month == DateTime.Now.Month && 
                           p.DataProducao.Year == DateTime.Now.Year)
                .CountAsync();

            var producoesConcluidas = await _context.Producoes
                .Where(p => p.Produzido)
                .CountAsync();

            var producoesPendentes = await _context.Producoes
                .Where(p => !p.Produzido)
                .CountAsync();

            var pesoTotalCaixilhos = await _context.Caixilhos
                .SumAsync(c => c.PesoUnitario * c.Quantidade);

            // Peso produzido no mês atual - baseado em caixilhos liberados (em toneladas)
            var pesoProduzidoMes = await _context.Caixilhos
                .Where(c => c.Liberado && 
                           c.DataLiberacao.HasValue &&
                           c.DataLiberacao.Value.Month == DateTime.Now.Month &&
                           c.DataLiberacao.Value.Year == DateTime.Now.Year)
                .SumAsync(c => c.PesoUnitario * c.Quantidade) / 1000.0f;
            var caixilhosLiberados = await _context.Caixilhos
                .Where(c => c.Liberado)
                .CountAsync();

            var caixilhosPendentesLiberacao = await _context.Caixilhos
                .Where(c => !c.Liberado)
                .CountAsync();

            var obrasEmAndamento = await _context.Obras
                .Where(o => o.StatusObra == "Em Andamento")
                .CountAsync();

            var obrasAtrasadas = await _context.Obras
                .Where(o => o.DataTermino < DateTime.Now && o.StatusObra != "Concluída")
                .CountAsync();

            return new MetricasViewModel
            {
                TotalObras = totalObras,
                TotalProducoes = totalProducoes,
                TotalCaixilhos = totalCaixilhos,
                TotalUsuarios = totalUsuarios,
                ProducoesMesAtual = producoesMesAtual,
                ProducoesConcluidas = producoesConcluidas,
                ProducoesPendentes = producoesPendentes,
                PesoTotalCaixilhos = pesoTotalCaixilhos,
                PesoProduzidoMes = pesoProduzidoMes,
                CaixilhosLiberados = caixilhosLiberados,
                CaixilhosPendentesLiberacao = caixilhosPendentesLiberacao,
                ObrasEmAndamento = obrasEmAndamento,
                ObrasAtrasadas = obrasAtrasadas
            };
        }

        private async Task<List<ProducaoRecenteViewModel>> ObterProducoesRecentes()
        {
            var producoes = await _context.Producoes
                .Include(p => p.Usuario)
                .Include(p => p.FamiliaCaixilho)
                .OrderByDescending(p => p.DataProducao)
                .Take(5)
                .ToListAsync();

            return producoes.Select(p => new ProducaoRecenteViewModel
            {
                Id = p.IdProducao,
                Nome = p.NomeProducao,
                Data = p.DataProducao,
                Status = p.Produzido ? "Concluída" : "Pendente",
                Usuario = p.Usuario?.NomeUsuario ?? "N/A",
                Familia = p.FamiliaCaixilho?.DescricaoFamilia ?? "N/A"
            }).ToList();
        }

        private async Task<List<ObraRecenteViewModel>> ObterObrasRecentes()
        {
            var obras = await _context.Obras
                .Include(o => o.Usuario)
                .OrderByDescending(o => o.IdObra)
                .Take(5)
                .ToListAsync();

            return obras.Select(o => new ObraRecenteViewModel
            {
                Id = o.IdObra,
                Nome = o.Nome,
                Construtora = o.Construtora,
                Usuario = o.Usuario?.NomeUsuario ?? "N/A",
                DataCriacao = o.DataInicio,
                Status = o.StatusObra,
                Bandeira = o.Bandeira,
                PercentualConclusao = o.PesoFinal > 0 ? Math.Min(100, (o.PesoProduzido / o.PesoFinal) * 100) : 0,
                PesoFinal = o.PesoFinal,
                PesoProduzido = o.PesoProduzido
            }).ToList();
        }

        private async Task<List<AlertaViewModel>> ObterAlertas()
        {
            var alertas = new List<AlertaViewModel>();

            // Produções atrasadas (mais de 7 dias)
            var producoesAtrasadas = await _context.Producoes
                .Where(p => !p.Produzido && p.DataProducao < DateTime.Now.AddDays(-7))
                .CountAsync();

            if (producoesAtrasadas > 0)
            {
                alertas.Add(new AlertaViewModel
                {
                    Tipo = "warning",
                    Titulo = "Produções Atrasadas",
                    Mensagem = $"{producoesAtrasadas} produção(ões) com mais de 7 dias de atraso",
                    Icone = "⚠️"
                });
            }

            // Obras atrasadas
            var obrasAtrasadas = await _context.Obras
                .Where(o => o.DataTermino < DateTime.Now && o.StatusObra != "Concluída")
                .CountAsync();

            if (obrasAtrasadas > 0)
            {
                alertas.Add(new AlertaViewModel
                {
                    Tipo = "danger",
                    Titulo = "Obras Atrasadas",
                    Mensagem = $"{obrasAtrasadas} obra(s) com prazo vencido",
                    Icone = "🚨"
                });
            }

            // Caixilhos não liberados
            var caixilhosNaoLiberados = await _context.Caixilhos
                .Where(c => !c.Liberado)
                .CountAsync();

            if (caixilhosNaoLiberados > 0)
            {
                alertas.Add(new AlertaViewModel
                {
                    Tipo = "info",
                    Titulo = "Caixilhos Aguardando Liberação",
                    Mensagem = $"{caixilhosNaoLiberados} caixilho(s) aguardando liberação para produção",
                    Icone = "⏳"
                });
            }

            return alertas;
        }

        private async Task<List<ProducaoPorMesViewModel>> ObterProducaoPorMes()
        {
            var ultimos6Meses = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .ToList();

            var producoesPorMes = new List<ProducaoPorMesViewModel>();

            foreach (var mes in ultimos6Meses)
            {
                var total = await _context.Producoes
                    .Where(p => p.DataProducao.Month == mes.Month && 
                               p.DataProducao.Year == mes.Year)
                    .CountAsync();

                var concluidas = await _context.Producoes
                    .Where(p => p.DataProducao.Month == mes.Month && 
                               p.DataProducao.Year == mes.Year && 
                               p.Produzido)
                    .CountAsync();

                // Peso produzido no mês - baseado em caixilhos liberados (em toneladas)
                var pesoProduzido = await _context.Caixilhos
                    .Where(c => c.Liberado && 
                               c.DataLiberacao.HasValue &&
                               c.DataLiberacao.Value.Month == mes.Month &&
                               c.DataLiberacao.Value.Year == mes.Year)
                    .SumAsync(c => c.PesoUnitario * c.Quantidade) / 1000.0f;

           
                producoesPorMes.Add(new ProducaoPorMesViewModel
                {
                    Mes = mes.ToString("MMM/yyyy"),
                    Total = total,
                    Concluidas = concluidas,
                    Pendentes = total - concluidas,
                    PesoProduzido = pesoProduzido
                });
            }

            return producoesPorMes;
        }

        private async Task<List<StatusObraViewModel>> ObterStatusObras()
        {
            var obrasVerde = await _context.Obras
                .Where(o => o.Bandeira == "Verde")
                .CountAsync();

            var obrasAmarela = await _context.Obras
                .Where(o => o.Bandeira == "Amarela")
                .CountAsync();

            var obrasVermelha = await _context.Obras
                .Where(o => o.Bandeira == "Vermelha")
                .CountAsync();

            var obrasCritica = await _context.Obras
                .Where(o => o.Bandeira == "Crítica")
                .CountAsync();

            return new List<StatusObraViewModel>
            {
                new StatusObraViewModel { Status = "Verde", Quantidade = obrasVerde },
                new StatusObraViewModel { Status = "Amarela", Quantidade = obrasAmarela },
                new StatusObraViewModel { Status = "Vermelha", Quantidade = obrasVermelha },
                new StatusObraViewModel { Status = "Crítica", Quantidade = obrasCritica }
            };
        }

        private async Task<List<TopUsuarioViewModel>> ObterTopUsuarios()
        {
            return await _context.Producoes
                .Include(p => p.Usuario)
                .Where(p => p.Usuario != null)
                .GroupBy(p => new { p.Usuario!.IdUsuario, p.Usuario.NomeUsuario })
                .Select(g => new TopUsuarioViewModel
                {
                    Nome = g.Key.NomeUsuario,
                    TotalProducoes = g.Count(),
                    ProducoesConcluidas = g.Count(p => p.Produzido)
                })
                .OrderByDescending(u => u.TotalProducoes)
                .Take(5)
                .ToListAsync();
        }

        private async Task<List<ObraPorBandeiraViewModel>> ObterObrasPorBandeira()
        {
            return await _context.Obras
                .GroupBy(o => o.Bandeira)
                .Select(g => new ObraPorBandeiraViewModel
                {
                    Bandeira = g.Key,
                    Quantidade = g.Count(),
                    PesoTotal = g.Sum(o => o.PesoFinal),
                    PesoProduzido = g.Sum(o => o.PesoProduzido)
                })
                .ToListAsync();
        }

        private async Task<List<CaixilhoParaLiberacaoViewModel>> ObterCaixilhosParaLiberacao()
        {
            return await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .Where(c => !c.Liberado)
                .OrderBy(c => c.Obra!.DataTermino)
                .Take(10)
                .Select(c => new CaixilhoParaLiberacaoViewModel
                {
                    Id = c.IdCaixilho,
                    Nome = c.NomeCaixilho,
                    Obra = c.Obra!.Nome,
                    Familia = c.FamiliaCaixilho!.DescricaoFamilia,
                    Peso = c.PesoUnitario * c.Quantidade,
                    DataTerminoObra = c.Obra.DataTermino,
                    Prioridade = c.Obra.Prioridade
                })
                .ToListAsync();
        }

        // Método para atualizar o peso produzido das obras baseado nos caixilhos liberados
        private async Task AtualizarPesoProduzidoObras()
        {
            var obras = await _context.Obras.ToListAsync();
            
            foreach (var obra in obras)
            {
                // Calcular peso total dos caixilhos liberados desta obra
                var pesoProduzido = await _context.Caixilhos
                    .Where(c => c.ObraId == obra.IdObra && c.Liberado)
                    .SumAsync(c => c.PesoUnitario * c.Quantidade);

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
            }

            await _context.SaveChangesAsync();
        }
    }
}