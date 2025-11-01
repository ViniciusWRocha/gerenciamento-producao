using GerenciamentoProducao.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
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
        
        // Novos campos para esquadrias
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public float PesoFinal { get; set; }
        public float PesoProduzido { get; set; }
        public string StatusObra { get; set; } = "Planejada";
        public string Prioridade { get; set; } = "Normal";
        public string Bandeira { get; set; } = "Verde";
        public float PercentualConclusao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string? Observacoes { get; set; }
        public bool Finalizado { get; set; } 

        public string? ImagemObraPath { get; set; }

        public IFormFile? ImagemUpload { get; set; }


        // ID do evento no Google Calendar
        public string? GoogleCalendarEventId { get; set; }
        
        public int IdUsuario { get; set; }
        public IEnumerable<SelectListItem>? Usuario { get; set; }

    }
}
