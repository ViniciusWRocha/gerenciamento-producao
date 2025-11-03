using System.Diagnostics;
using System.Xml.Linq;
using GerenciamentoProducao.Data;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoProducao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GerenciamentoProdDbContext _context;

        public HomeController(ILogger<HomeController> logger, GerenciamentoProdDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if(!User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Usuario");
            }

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ImportarObra(IFormFile arquivoXml)
        //{
        //    if (arquivoXml == null || arquivoXml.Length == 0)
        //    {
        //        ViewBag.Mensagem = "Arquivo inv�lido.";
        //        return View("Index");
        //    }

        //    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //    using var stream = arquivoXml.OpenReadStream();
        //    using var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("windows-1252"));
        //    var xdoc = XDocument.Load(reader);

        //    var dadosObra = xdoc.Root.Element("DADOS_OBRA");
        //    var enderecoObra = dadosObra?.Element("ENDERECO_OBRA");
        //    var dadosCliente = xdoc.Root.Element("DADOS_CLIENTE");

        //    var obra = new Obra
        //    {
        //        Nome = dadosObra?.Element("NOME")?.Value,
        //        Construtora = dadosCliente?.Element("NOME")?.Value,
        //        Cnpj = dadosCliente?.Element("CNPJ_CPF")?.Value,
        //        Logradouro = enderecoObra?.Element("END_LOGR")?.Value,
        //        Nro = enderecoObra?.Element("END_NUMERO")?.Value,
        //        Bairro = enderecoObra?.Element("END_BAIRRO")?.Value,
        //        Cep = enderecoObra?.Element("END_CEP")?.Value,
        //        Uf = enderecoObra?.Element("END_UF")?.Value,
        //        IdUsuario = 3 // Defina conforme sua l�gica de usu�rio
        //    };

        //    _context.Obras.Add(obra);
        //    await _context.SaveChangesAsync();

        //    ViewBag.Mensagem = "Importa��o realizada com sucesso.";
        //    return View("Index");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarObras(List<IFormFile> arquivosXml)
        {
            var resultados = new List<string>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            if (arquivosXml == null || arquivosXml.Count == 0)
            {
                resultados.Add("Nenhum arquivo selecionado.");
                ViewBag.Resultados = resultados;
                return await Index();
            }

            foreach (var arquivoXml in arquivosXml)
            {
                try
                {
                    // Reabra o stream para cada arquivo
                    using var stream = arquivoXml.OpenReadStream();
                    using var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("windows-1252"));
                    var xdoc = XDocument.Load(reader);

                    var dadosObra = xdoc.Root.Element("DADOS_OBRA");
                    var enderecoObra = dadosObra?.Element("ENDERECO_OBRA");
                    var dadosCliente = xdoc.Root.Element("DADOS_CLIENTE");

                    var obra = new Obra
                    {
                        Nome = dadosObra?.Element("NOME")?.Value,
                        Construtora = dadosCliente?.Element("NOME")?.Value,
                        Cnpj = dadosCliente?.Element("CNPJ_CPF")?.Value,
                        Logradouro = enderecoObra?.Element("END_LOGR")?.Value,
                        Nro = enderecoObra?.Element("END_NUMERO")?.Value,
                        Bairro = enderecoObra?.Element("END_BAIRRO")?.Value,
                        Cep = enderecoObra?.Element("END_CEP")?.Value,
                        Uf = enderecoObra?.Element("END_UF")?.Value,
                        IdUsuario = 3 // Defina conforme sua l�gica de usu�rio
                    };

                    _context.Obras.Add(obra);
                    // N�o chame SaveChangesAsync aqui
                    resultados.Add($"Arquivo '{arquivoXml.FileName}': Importa��o realizada com sucesso.");
                }
                catch (Exception ex)
                {
                    resultados.Add($"Arquivo '{arquivoXml.FileName}': Erro - {ex.Message}");
                }
            }

            // Salve todas as altera��es de uma vez
            await _context.SaveChangesAsync();

            ViewBag.Resultados = resultados;
            return await Index();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducaoPorMes()
        {
            var data = await ObterProducaoPorMes();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusObras()
        {
            var data = await ObterStatusObras();
            return Json(data);
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

            var pesoProduzidoMes = 0.0f;
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

            var metaAtual = await _context.MetasMensais
                .Where(m => m.Ano == DateTime.Now.Year && m.Mes == DateTime.Now.Month)
                .FirstOrDefaultAsync();

            if (metaAtual != null)
            {
                var diasRestantes = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                var pesoRestante = metaAtual.MetaPesoKg - metaAtual.PesoProduzido;
                var pesoNecessarioPorDia = diasRestantes > 0 ? pesoRestante / diasRestantes : pesoRestante;

                if (pesoNecessarioPorDia > 2.0f)
                {
                    alertas.Add(new AlertaViewModel
                    {
                        Tipo = "warning",
                        Titulo = "Meta Mensal em Risco",
                        Mensagem = $"Necessário produzir {pesoNecessarioPorDia:F1} toneladas/dia para atingir a meta",
                        Icone = "📊"
                    });
                }
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

                var pesoProduzido = await _context.Caixilhos
                    .Where(c => c.StatusProducao == "Concluído" &&
                               c.DataLiberacao.HasValue &&
                               c.DataLiberacao.Value.Month == mes.Month &&
                               c.DataLiberacao.Value.Year == mes.Year)
                    .SumAsync(c => c.PesoUnitario * c.Quantidade);

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

        private async Task AtualizarPesoProduzidoObras()
        {
            var obras = await _context.Obras.ToListAsync();

            foreach (var obra in obras)
            {
                var pesoProduzido = await _context.Caixilhos
                    .Where(c => c.ObraId == obra.IdObra && c.Liberado)
                    .SumAsync(c => c.PesoUnitario * c.Quantidade);

                obra.PesoProduzido = pesoProduzido;

                if (obra.PesoFinal > 0)
                {
                    obra.PercentualConclusao = Math.Min(100, (pesoProduzido / obra.PesoFinal) * 100);
                }
                else
                {
                    obra.PercentualConclusao = 0;
                }

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
