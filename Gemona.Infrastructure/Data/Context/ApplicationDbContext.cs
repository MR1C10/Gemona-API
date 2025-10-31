using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Profissional> Profissionais { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<SubCategoria> SubCategorias { get; set; }
        public DbSet<Estabelecimento> Estabelecimentos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoHistorico> PedidoHistoricos { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<HorarioFuncionamento> HorariosFuncionamento { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}