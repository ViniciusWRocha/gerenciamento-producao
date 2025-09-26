using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciamentoProducao.Models
{
    [Table ("Obra")]
    public class Obra
    {
        [Key]
        public int IdObra { get; set; }
        [Required (ErrorMessage ="Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; set; } 

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A Construtora deve ter entre 3 e 100 caracteres")]
        public string Construtora { get; set; }

        [Required (ErrorMessage ="Campo Obrigatório")]
        [StringLength(100, ErrorMessage = "O Número não permite Nulo")]
        public string Nro { get; set; }


        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Logradouro deve ter entre 3 e 200 caracteres")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Bairro deve ter entre 3 e 100 caracteres")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(9,ErrorMessage = "CEP deve ter 9 caracteres")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(2, ErrorMessage = "CEP deve ter 2 caracteres")]
        public string Uf { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14, ErrorMessage = "CEP deve ter 14  caracteres")]
        public string Cnpj { get; set; }


        //lista de caixilhos
        //public List<Caixilho> Caixilhos { get; set; }

        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario{ get; set; }

        //public int IdCaixilho { get; set; }
        //public virtual Caixilho? Caixilho { get; set; }
    }
}
