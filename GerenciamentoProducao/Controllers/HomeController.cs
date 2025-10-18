using System.Diagnostics;
using System.Xml.Linq;
using GerenciamentoProducao.Data;
using GerenciamentoProducao.Models;
using Microsoft.AspNetCore.Mvc;

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
            if(!User?.Identity?.IsAuthenticated == true)
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

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ImportarObra(IFormFile arquivoXml)
        //{
        //    if (arquivoXml == null || arquivoXml.Length == 0)
        //    {
        //        ViewBag.Mensagem = "Arquivo inválido.";
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
        //        IdUsuario = 3 // Defina conforme sua lógica de usuário
        //    };

        //    _context.Obras.Add(obra);
        //    await _context.SaveChangesAsync();

        //    ViewBag.Mensagem = "Importação realizada com sucesso.";
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
                return View("Index");
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
                        IdUsuario = 3 // Defina conforme sua lógica de usuário
                    };

                    _context.Obras.Add(obra);
                    // Não chame SaveChangesAsync aqui
                    resultados.Add($"Arquivo '{arquivoXml.FileName}': Importação realizada com sucesso.");
                }
                catch (Exception ex)
                {
                    resultados.Add($"Arquivo '{arquivoXml.FileName}': Erro - {ex.Message}");
                }
            }

            // Salve todas as alterações de uma vez
            await _context.SaveChangesAsync();

            ViewBag.Resultados = resultados;
            return View("Index");
        }

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return View();
        //}

    }
}
