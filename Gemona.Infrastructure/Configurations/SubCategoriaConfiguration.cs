using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Configurations
{
    public class SubCategoriaConfiguration : IEntityTypeConfiguration<SubCategoria>
    {
        public void Configure(EntityTypeBuilder<SubCategoria> builder)
        {
            builder.ToTable("sub_categoria");

            builder.HasKey(s => s.SubCategoriaId);
            builder.Property(s => s.SubCategoriaId).HasColumnName("sub_categoria_id");

            builder.Property(s => s.Nome)
                .HasColumnName("nome")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.ImagemSubCategoriaUrl)
                .HasColumnName("imagem_subcategoria_url")
                .HasMaxLength(255);

            builder.Property(s => s.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("DATETIME(6)")
                .IsRequired();

            builder.Property(s => s.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("DATETIME(6)");

            builder.Property(s => s.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(s => s.Categoria)
                .WithMany(c => c.SubCategorias)
                .HasForeignKey(s => s.SubCategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Servicos)
                .WithOne(serv => serv.SubCategoria)
                .HasForeignKey(serv => serv.SubCategoriaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}