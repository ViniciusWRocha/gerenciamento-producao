using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table("RelatorioProducao")]
    public class RelatorioProducao
    {
        [Key]
        public int IdRelatorio { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataRelatorio { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Mes { get; set; }

        public float PesoTotalProduzido { get; set; }

        public int TotalCaixilhosProduzidos { get; set; }

        public int TotalFamiliasProduzidas { get; set; }

        public float EficienciaProducao { get; set; } // Percentual de eficiência

        public float TempoMedioProducao { get; set; } // Em horas

        public string StatusMeta { get; set; } = "Em Andamento"; // Em Andamento, Atingida, Não Atingida

        [StringLength(1000)]
        public string? Observacoes { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}
