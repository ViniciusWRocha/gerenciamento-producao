using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GerenciamentoProducao.Interfaces;


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

        public IActionResult Index()
        {
            return View();
        }
    }
}
