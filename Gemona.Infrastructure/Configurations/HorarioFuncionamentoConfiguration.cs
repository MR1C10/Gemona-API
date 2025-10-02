using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Configurations
{
    public class HorarioFuncionamentoConfiguration : IEntityTypeConfiguration<HorarioFuncionamento>
    {
        public void Configure(EntityTypeBuilder<HorarioFuncionamento> builder)
        {
            builder.ToTable("horario_funcionamento");

            builder.HasKey(h => h.HorarioId);
            builder.Property(h => h.HorarioId).HasColumnName("horario_id");

            builder.Property(h => h.EstabelecimentoId)
                .HasColumnName("estabelecimento_id")
                .IsRequired();

            builder.Property(h => h.Diasemana)
                .HasColumnName("dia_semana")
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(h => h.HoraAbertura)
                .HasColumnName("hora_abertura")
                .HasColumnType("TIME");

            builder.Property(h => h.HoraFechamento)
                .HasColumnName("hora_fechamento")
                .HasColumnType("TIME");

            builder.Property(h => h.Fechado)
                .HasColumnName("fechado")
                .HasDefaultValue(false);

            builder.Property(h => h.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(h => h.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(h => h.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(h => h.Estabelecimento)
                .WithMany(e => e.HorarioFuncionamento)
                .HasForeignKey(h => h.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}