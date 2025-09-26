using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table ("FamiliaCaixilho")]
    public class FamiliaCaixilho
    {
        [Key]
        public int IdFamiliaCaixilho { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Familia de Caixilhos deve ter entre 3 e 100 caracteres")]
        public string DescricaoFamilia{ get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int PesoTotal { get; set; }


    }
}
