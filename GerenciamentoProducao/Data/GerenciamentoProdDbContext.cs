using GerenciamentoProducao.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoProducao.Data
{
    public class GerenciamentoProdDbContext : DbContext
    {
        public GerenciamentoProdDbContext(DbContextOptions<GerenciamentoProdDbContext> options) : base(options) { }

        //models
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }

        // models Usuario
        public DbSet<Usuario> Usuarios { get; set; }

        //models Producao
        public DbSet<Producao> Producoes { get; set; } 
        
        //models Caixilho
        public DbSet<Caixilho> Caixilhos { get; set; }  
        
        //models TipoCaixilho
        public DbSet<TipoCaixilho> TipoCaixilhos { get; set; }

         //models Obra
        public DbSet<Obra> Obras { get; set; }
        
        //models Liberacao
        public DbSet<FamiliaCaixilho> FamCaixilhos { get; set; }

        //models MetaMensal
        public DbSet<MetaMensal> MetasMensais { get; set; }

        //models RelatorioProducao
        public DbSet<RelatorioProducao> RelatoriosProducao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}