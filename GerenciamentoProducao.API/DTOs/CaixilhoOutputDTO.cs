namespace GerenciamentoProducao.API.DTOs
{
    public class CaixilhoOutputDTO
    {
        public int IdCaixilho { get; set; }
        public string NomeCaixilho { get; set; }
        public string Largura { get; set; }
        public string Altura { get; set; }
        public string Quantidade { get; set; }
        public string PesoUnitario { get; set; }
        public int ObraId { get; set; }
        public string nomeObra { get; set; }
        public int IdTipoCaixilho{ get; set; }
        public string DescricaoCaixilho { get; set; }
    }
}
