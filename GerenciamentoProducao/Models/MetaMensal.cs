using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table("MetaMensal")]
    public class MetaMensal
    {
        [Key]
        public int IdMetaMensal { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Mes { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float MetaPesoKg { get; set; } // Meta em kg (ex: 30000 kg = 30 toneladas)

        public float PesoProduzido { get; set; } = 0;

        public float PercentualAtingido { get; set; } = 0;

        public bool MetaAtingida { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}
