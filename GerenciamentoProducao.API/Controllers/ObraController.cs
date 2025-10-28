using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.API.DTOs;


namespace GerenciamentoProducao.API.Controllers
{

    [Route("api/[controller]")]
    //[EnableCores("MyPolicy")]
    [ApiController]
    public class ObraController : Controller
    {
        private readonly IObraRepository _obraRepository;
        public ObraController(IObraRepository obraRepository)
        {
            _obraRepository = obraRepository;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public async  Task<IActionResult> GetAllObras()
        {
            var obras = await _obraRepository.GetAllAsync();
            var resultado = new List<ObraOutputDTO>();
            foreach (var obra in obras)
            {
                resultado.Add(new ObraOutputDTO
                {
                    IdObra = obra.IdObra,
                    Nome = obra.Nome,
                    Construtora = obra.Construtora,
                    Nro = obra.Nro,
                    Logradouro = obra.Logradouro,
                    Bairro = obra.Bairro,
                    Cep= obra.Cep,
                    Uf= obra.Uf,
                    Cnpj= obra.Cnpj,
                    IdUsuario= obra.IdUsuario,
                    //NomeUsuario= obra.Usuario.NomeUsuario,    
                });
            }
            return Ok(resultado);
        }
    }
}
