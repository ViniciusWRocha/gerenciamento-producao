using GerenciamentoProducao.API.DTOs;
using GerenciamentoProducao.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducao.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaixilhoController : Controller
    {
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly IObraRepository _obraRepository;
        public CaixilhoController(ICaixilhoRepository caixilhoRepository, IObraRepository obraRepository)
        {
            _caixilhoRepository = caixilhoRepository;
            _obraRepository = obraRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCaixilhos()
        {
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            var resultado = new List<CaixilhoOutputDTO>();
            foreach (var caixilho in caixilhos)
            {
                var obra = await _obraRepository.GetById(caixilho.ObraId);
                resultado.Add(new CaixilhoOutputDTO
                {
                    IdCaixilho = caixilho.IdCaixilho,
                    NomeCaixilho = caixilho.NomeCaixilho,
                    Largura = caixilho.Largura.ToString(),
                    Altura = caixilho.Altura.ToString(),
                    Quantidade = caixilho.Quantidade.ToString(),
                    PesoUnitario = caixilho.PesoUnitario.ToString(),
                    ObraId = caixilho.ObraId,
                    nomeObra = obra != null ? obra.Nome : string.Empty,
                    IdTipoCaixilho = caixilho.IdTipoCaixilho,
                    DescricaoCaixilho = caixilho.TipoCaixilho != null ? caixilho.TipoCaixilho.DescricaoCaixilho : string.Empty
                });
            }
            return Ok(resultado);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCaixilhoById(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null)
            {
                return NotFound();
            }
            var obra = await _obraRepository.GetById(caixilho.ObraId);
            var resultado = new CaixilhoOutputDTO
            {
                IdCaixilho = caixilho.IdCaixilho,
                NomeCaixilho = caixilho.NomeCaixilho,
                Largura = caixilho.Largura.ToString(),
                Altura = caixilho.Altura.ToString(),
                Quantidade = caixilho.Quantidade.ToString(),
                PesoUnitario = caixilho.PesoUnitario.ToString(),
                ObraId = caixilho.ObraId,
                nomeObra = obra != null ? obra.Nome : string.Empty,
                IdTipoCaixilho = caixilho.IdTipoCaixilho,
                DescricaoCaixilho = caixilho.TipoCaixilho != null ? caixilho.TipoCaixilho.DescricaoCaixilho : string.Empty
            };
            return Ok(resultado);
        }

    }
}
