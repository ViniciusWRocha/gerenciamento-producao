using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table("TipoCaixilho")]
    public class TipoCaixilho
    {
        [Key]
        public int IdTipoCaixilho { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O Nome do Tipo de Caixilho deve ter entre 3 e 100 caracteres")]
        public string DescricaoCaixilho { get; set; }
    }
}
