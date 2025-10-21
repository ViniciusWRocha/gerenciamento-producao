using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using GerenciamentoProducao.Data;
using GerenciamentoProducao.Models;
using System.Globalization;
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

        public IActionResult Index()
        {
            if (!User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Usuario");
            }
            return View();
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
                return View("Index");
            }

            var caixilhosParaSalvar = new List<Caixilho>();

            foreach (var arquivoXml in arquivosXml)
            {
                try
                {
                    using var stream = arquivoXml.OpenReadStream();
                    using var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("windows-1252"));
                    var xdoc = XDocument.Load(reader);

                    var dadosObra = xdoc.Root.Element("DADOS_OBRA");
                    var enderecoObra = dadosObra?.Element("ENDERECO_OBRA");
                    var dadosCliente = xdoc.Root.Element("DADOS_CLIENTE");
                    var tipologias = xdoc.Root.Element("TIPOLOGIAS")?.Elements("TIPOLOGIA");

                    var obra = new Obra
                    {
                        Nome = dadosObra?.Element("NOME")?.Value,
                        //USANDO O NOME DO CLIENTE COMO NOME DA CONTRUTORA MUDAR !!!!!!!!!!!!!!!!!!!!!!!
                        Construtora = dadosCliente?.Element("NOME")?.Value,
                        Cnpj = dadosCliente?.Element("CNPJ_CPF")?.Value,
                        Logradouro = enderecoObra?.Element("END_LOGR")?.Value,
                        Nro = enderecoObra?.Element("END_NUMERO")?.Value,
                        Bairro = enderecoObra?.Element("END_BAIRRO")?.Value,
                        Cep = enderecoObra?.Element("END_CEP")?.Value,
                        Uf = enderecoObra?.Element("END_UF")?.Value,
                        IdUsuario = 3
                    };

                    _context.Obras.Add(obra);
                    await _context.SaveChangesAsync();

                    int idObra = obra.IdObra;

                    if (tipologias == null)
                    {
                        resultados.Add($"Arquivo '{arquivoXml.FileName}': Não foram encontradas tipologias.");
                        continue;
                    }


                    var familiasAgrupadas = tipologias
                        .GroupBy(t => t.Element("CODESQD")?.Value)
                        .Where(g => !string.IsNullOrEmpty(g.Key));

                    var mapFamiliaId = new Dictionary<string, int>();

                    foreach (var grupo in familiasAgrupadas)
                    {
                        float pesoTotalCalculado = grupo.Sum(t =>
                            (float.TryParse(t.Element("PESO_UNIT")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float pu) ? pu : 0f) * (int.TryParse(t.Element("QTDE")?.Value, out int q) ? q : 0)
                        );

                        var codesqd = grupo.Key;

                        var familiaDb = await _context.FamCaixilhos.FirstOrDefaultAsync(f => f.DescricaoFamilia == codesqd);

                        if (familiaDb == null)
                        {
                            var novaFamilia = new FamiliaCaixilho
                            {
                                DescricaoFamilia = codesqd,
                                PesoTotal = (int)Math.Round(pesoTotalCalculado)
                            };
                            _context.FamCaixilhos.Add(novaFamilia);
                            await _context.SaveChangesAsync();
                            mapFamiliaId.Add(codesqd, novaFamilia.IdFamiliaCaixilho);
                        }
                        else
                        {
                            mapFamiliaId.Add(codesqd, familiaDb.IdFamiliaCaixilho);
                        }
                    }


                    var tiposAgrupados = tipologias
                        .Select(t => t.Element("TIPO")?.Value)
                        .Where(t => !string.IsNullOrEmpty(t))
                        .Distinct();

                    var mapTipoId = new Dictionary<string, int>();

                    foreach (var tipo in tiposAgrupados)
                    {
            
                        var tipoDb = await _context.TipoCaixilhos.FirstOrDefaultAsync(t => t.DescricaoCaixilho == tipo);

                        if (tipoDb == null)
                        {
                            var novoTipo = new TipoCaixilho
                            {
                                DescricaoCaixilho = tipo
                            };
                            _context.TipoCaixilhos.Add(novoTipo);
                            await _context.SaveChangesAsync();
                            mapTipoId.Add(tipo, novoTipo.IdTipoCaixilho);
                        }
                        else
                        {
                            mapTipoId.Add(tipo, tipoDb.IdTipoCaixilho);
                        }
                    }


                    //CAIXILHOS INDIVIDUAIS
                    foreach (var tipologia in tipologias)
                    {
                        var codesqd = tipologia.Element("CODESQD")?.Value;
                        var tipo = tipologia.Element("TIPO")?.Value;

                        // Conversão de Largura/Altura
                        int.TryParse(tipologia.Element("LARGURA")?.Value, out int largura);
                        int.TryParse(tipologia.Element("ALTURA")?.Value, out int altura);

                        // Conversão PesoUnitario 
                        float.TryParse(tipologia.Element("PESO_UNIT")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float pesoUnitario);

                        // Conversão de Quantidade
                        int.TryParse(tipologia.Element("QTDE")?.Value, out int quantidade);

                        var caixilho = new Caixilho
                        {
                            NomeCaixilho = tipologia.Element("DESCR")?.Value,
                            Largura = largura,
                            Altura = altura,
                            Quantidade = quantidade,
                            PesoUnitario = pesoUnitario,

                            
                            ObraId = idObra,
                            IdFamiliaCaixilho = mapFamiliaId.GetValueOrDefault(codesqd),
                            IdTipoCaixilho = mapTipoId.GetValueOrDefault(tipo)
                        };

                        caixilhosParaSalvar.Add(caixilho);
                    }

                    _context.Caixilhos.AddRange(caixilhosParaSalvar);

                    resultados.Add($"Arquivo '{arquivoXml.FileName}': Importação de {caixilhosParaSalvar.Count} caixilhos concluída.");
                    caixilhosParaSalvar.Clear(); 
                }
                catch (Exception ex)
                {
                    resultados.Add($"Arquivo '{arquivoXml.FileName}': Erro Crítico - {ex.Message}");
                }
            }
            await _context.SaveChangesAsync();

            ViewBag.Resultados = resultados;
            return View("Index");
        }
    }

}
