using GerenciamentoProducao.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducaoo.ViewModel
{
    public class ObraViewModel
    {
      
        public int IdObra { get; set; }
        public string Nome { get; set; }    
        public string Construtora { get; set; }
        public string Nro { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Uf { get; set; }
        public string Cnpj { get; set; }
        public int IdUsuario { get; set; }
        public IEnumerable<SelectListItem>? Usuario { get; set; }

    }
}
