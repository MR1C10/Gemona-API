using Microsoft.EntityFrameworkCore;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets das entidades
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Profissional> Profissionais { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<SubCategoria> SubCategorias { get; set; }
        public DbSet<Estabelecimento> Estabelecimentos { get; set; }
        public DbSet<HorarioFuncionamento> HorarioFuncionamento { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoHistorico> PedidoHistorico { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            ConfigurarSoftDelete(modelBuilder);

        }

        private static void ConfigurarSoftDelete(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Endereco>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Cliente>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Profissional>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Categoria>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<SubCategoria>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Estabelecimento>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<HorarioFuncionamento>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Servico>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Pedido>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<PedidoHistorico>().HasQueryFilter(e => e.Ativo);
            modelBuilder.Entity<Avaliacao>().HasQueryFilter(e => e.Ativo);
        }
        
        public override int SaveChanges()
        {
            ConfigurarAuditoria();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConfigurarAuditoria();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ConfigurarAuditoria()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var agora = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.DataCriacao = agora;
                        break;
                    case EntityState.Modified:
                        entry.Entity.DataAtualizacao = agora;
                        break;
                }
            }
        }
    }
}