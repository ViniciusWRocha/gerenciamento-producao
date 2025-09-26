using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table("TipoUsuario")]
    public class TipoUsuario
    {
        [Key]
        public int IdTipoUsuario { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string NomeTipoUsuario { get; set; }
    }
}
