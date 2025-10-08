using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("pedido");

            builder.HasKey(p => p.PedidoId);
            builder.Property(p => p.PedidoId).HasColumnName("pedido_id");

            builder.Property(p => p.ClienteId)
                .HasColumnName("cliente_id")
                .IsRequired();

            builder.Property(p => p.ServicoId)
                .HasColumnName("servico_id")
                .IsRequired();

            builder.Property(p => p.DataSolicitacao)
                .HasColumnName("data_solicitacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(p => p.DataConclusao)
                .HasColumnName("data_conclusao")
                .HasColumnType("DATETIME(6)");

            builder.Property(p => p.DataAgendamento)
                .HasColumnName("data_agendamento")
                .HasColumnType("DATETIME(6)");

            builder.Property(p => p.ValorFinal)
                .HasColumnName("valor_final")
                .HasPrecision(10, 2);

            builder.Property(p => p.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(p => p.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(p => p.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Servico)
                .WithMany(s => s.Pedidos)
                .HasForeignKey(p => p.ServicoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Historicos)
                .WithOne(h => h.Pedido)
                .HasForeignKey(h => h.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Avaliacao)
                .WithOne(a => a.Pedido)
                .HasForeignKey<Avaliacao>(a => a.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}