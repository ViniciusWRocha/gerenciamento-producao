using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table ("Caixilho")]
    public class Caixilho
    {
        [Key]
        public int IdCaixilho { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome do Caixilho deve ter entre 3 e 100 caracteres")]
        public string NomeCaixilho { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Largura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Altura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Quantidade{ get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float PesoUnitario { get; set; }


        public int ObraId { get; set; }
        public virtual Obra? Obra{ get; set; }

        public int IdFamiliaCaixilho { get; set; }
        public virtual FamiliaCaixilho? FamiliaCaixilho { get; set; }

        public int IdTipoCaixilho { get; set; }
        public virtual TipoCaixilho? TipoCaixilho{ get; set; }



    }
}
