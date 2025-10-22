using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.ViewModel
{
    public class DashboardViewModel
    {
        public MetricasViewModel Metricas { get; set; } = new();
        public List<ProducaoRecenteViewModel> ProducoesRecentes { get; set; } = new();
        public List<ObraRecenteViewModel> ObrasRecentes { get; set; } = new();
        public List<AlertaViewModel> Alertas { get; set; } = new();
        public MetaMensalViewModel? MetaMensal { get; set; }
    }

    public class MetricasViewModel
    {
        public int TotalObras { get; set; }
        public int TotalProducoes { get; set; }
        public int TotalCaixilhos { get; set; }
        public int TotalUsuarios { get; set; }
        public int ProducoesMesAtual { get; set; }
        public int ProducoesConcluidas { get; set; }
        public int ProducoesPendentes { get; set; }
        public float PesoTotalCaixilhos { get; set; }
        
        // Novas métricas específicas para esquadrias
        public float PesoProduzidoMes { get; set; }
        public int CaixilhosLiberados { get; set; }
        public int CaixilhosPendentesLiberacao { get; set; }
        public int ObrasEmAndamento { get; set; }
        public int ObrasAtrasadas { get; set; }
    }

    public class ProducaoRecenteViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Familia { get; set; } = string.Empty;
    }

    public class ObraRecenteViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Construtora { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Bandeira { get; set; } = string.Empty;
        public float PercentualConclusao { get; set; }
        public float PesoFinal { get; set; }
        public float PesoProduzido { get; set; }
    }

    public class AlertaViewModel
    {
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string Icone { get; set; } = string.Empty;
    }

    public class ProducaoPorMesViewModel
    {
        public string Mes { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Concluidas { get; set; }
        public int Pendentes { get; set; }
        public float PesoProduzido { get; set; }
    }

    public class StatusObraViewModel
    {
        public string Status { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }

    public class TopUsuarioViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public int TotalProducoes { get; set; }
        public int ProducoesConcluidas { get; set; }
    }

    public class MetaMensalViewModel
    {
        public float MetaPeso { get; set; }
        public float PesoProduzido { get; set; }
        public float PercentualAtingido { get; set; }
        public int DiasRestantes { get; set; }
        public bool MetaAtingida { get; set; }
    }

    public class ObraPorBandeiraViewModel
    {
        public string Bandeira { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public float PesoTotal { get; set; }
        public float PesoProduzido { get; set; }
    }

    public class CaixilhoParaLiberacaoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Obra { get; set; } = string.Empty;
        public string Familia { get; set; } = string.Empty;
        public float Peso { get; set; }
        public DateTime DataTerminoObra { get; set; }
        public string Prioridade { get; set; } = string.Empty;
    }
}
