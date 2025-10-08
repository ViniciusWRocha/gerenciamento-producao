using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table("Producao")]
    public class Producao
    {
        [Key]
        public int IdProducao { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string NomeProducao { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataProducao { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public bool Produzido { get; set; }

        //[Required]
        [StringLength(200)]
        public string? Descricao { get; set; }

        public bool EhLiberado { get; set; }


        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }


        [ForeignKey("FamiliaCaixilho")]
        public int FamiliaCaixilhoId { get; set; }
        public virtual FamiliaCaixilho? FamiliaCaixilho { get; set; }
    }
}
