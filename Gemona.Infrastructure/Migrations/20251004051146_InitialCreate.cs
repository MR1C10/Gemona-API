using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gemona.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    categoria_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    imagem_categoria_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.categoria_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "endereco",
                columns: table => new
                {
                    endereco_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    rua = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bairro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    complemento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cep = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: false),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_endereco", x => x.endereco_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "profissional",
                columns: table => new
                {
                    profissional_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    imagem_perfil_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_nascimento = table.Column<DateTime>(type: "DATE", nullable: false),
                    senha_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profissional", x => x.profissional_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sub_categorias",
                columns: table => new
                {
                    sub_categoria_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    categoria_id = table.Column<int>(type: "int", nullable: false),
                    imagem_subcategoria_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sub_categorias", x => x.sub_categoria_id);
                    table.ForeignKey(
                        name: "FK_sub_categorias_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "categoria_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    cliente_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    imagem_perfil_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    endereco_id = table.Column<int>(type: "int", nullable: true),
                    data_nascimento = table.Column<DateTime>(type: "DATE", nullable: false),
                    senha_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente", x => x.cliente_id);
                    table.ForeignKey(
                        name: "FK_cliente_endereco_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "endereco",
                        principalColumn: "endereco_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "estabelecimento",
                columns: table => new
                {
                    estabelecimento_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cnpj = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    imagem_estabelecimento_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    profissional_id = table.Column<int>(type: "int", nullable: false),
                    endereco_id = table.Column<int>(type: "int", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estabelecimento", x => x.estabelecimento_id);
                    table.ForeignKey(
                        name: "FK_estabelecimento_endereco_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "endereco",
                        principalColumn: "endereco_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_estabelecimento_profissional_profissional_id",
                        column: x => x.profissional_id,
                        principalTable: "profissional",
                        principalColumn: "profissional_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "horario_funcionamento",
                columns: table => new
                {
                    horario_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    estabelecimento_id = table.Column<int>(type: "int", nullable: false),
                    dia_semana = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    hora_abertura = table.Column<TimeOnly>(type: "TIME", nullable: true),
                    hora_fechamento = table.Column<TimeOnly>(type: "TIME", nullable: true),
                    fechado = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horario_funcionamento", x => x.horario_id);
                    table.ForeignKey(
                        name: "FK_horario_funcionamento_estabelecimento_estabelecimento_id",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimento",
                        principalColumn: "estabelecimento_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "servicos",
                columns: table => new
                {
                    servico_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sub_categoria_id = table.Column<int>(type: "int", nullable: false),
                    preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    imagem_servico_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estabelecimento_id = table.Column<int>(type: "int", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos", x => x.servico_id);
                    table.ForeignKey(
                        name: "FK_servicos_estabelecimento_estabelecimento_id",
                        column: x => x.estabelecimento_id,
                        principalTable: "estabelecimento",
                        principalColumn: "estabelecimento_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_servicos_sub_categorias_sub_categoria_id",
                        column: x => x.sub_categoria_id,
                        principalTable: "sub_categorias",
                        principalColumn: "sub_categoria_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedido",
                columns: table => new
                {
                    pedido_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    servico_id = table.Column<int>(type: "int", nullable: false),
                    data_solicitacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_conclusao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    data_agendamento = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    valor_final = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: false, defaultValue: "Solicitado")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido", x => x.pedido_id);
                    table.ForeignKey(
                        name: "FK_pedido_cliente_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "cliente",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedido_servicos_servico_id",
                        column: x => x.servico_id,
                        principalTable: "servicos",
                        principalColumn: "servico_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "avaliacoes",
                columns: table => new
                {
                    avaliacoes_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    pedido_id = table.Column<int>(type: "int", nullable: false),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    nota = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    comentario = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    imagem_comentario_url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avaliacoes", x => x.avaliacoes_id);
                    table.ForeignKey(
                        name: "FK_avaliacoes_cliente_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "cliente",
                        principalColumn: "cliente_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_avaliacoes_pedido_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedido",
                        principalColumn: "pedido_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedido_historico",
                columns: table => new
                {
                    pedido_historico_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    pedido_id = table.Column<int>(type: "int", nullable: false),
                    status_anterior = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status_novo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_alteracao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "DATETIME(6)", nullable: true),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido_historico", x => x.pedido_historico_id);
                    table.ForeignKey(
                        name: "FK_pedido_historico_pedido_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedido",
                        principalColumn: "pedido_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_cliente_id",
                table: "avaliacoes",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacoes_pedido_id",
                table: "avaliacoes",
                column: "pedido_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cliente_cpf",
                table: "cliente",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cliente_email",
                table: "cliente",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cliente_endereco_id",
                table: "cliente",
                column: "endereco_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estabelecimento_cnpj",
                table: "estabelecimento",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estabelecimento_endereco_id",
                table: "estabelecimento",
                column: "endereco_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estabelecimento_profissional_id",
                table: "estabelecimento",
                column: "profissional_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_horario_funcionamento_estabelecimento_id",
                table: "horario_funcionamento",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_cliente_id",
                table: "pedido",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_servico_id",
                table: "pedido",
                column: "servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_historico_pedido_id",
                table: "pedido_historico",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_profissional_cpf",
                table: "profissional",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profissional_email",
                table: "profissional",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_servicos_estabelecimento_id",
                table: "servicos",
                column: "estabelecimento_id");

            migrationBuilder.CreateIndex(
                name: "IX_servicos_sub_categoria_id",
                table: "servicos",
                column: "sub_categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_sub_categorias_categoria_id",
                table: "sub_categorias",
                column: "categoria_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avaliacoes");

            migrationBuilder.DropTable(
                name: "horario_funcionamento");

            migrationBuilder.DropTable(
                name: "pedido_historico");

            migrationBuilder.DropTable(
                name: "pedido");

            migrationBuilder.DropTable(
                name: "cliente");

            migrationBuilder.DropTable(
                name: "servicos");

            migrationBuilder.DropTable(
                name: "estabelecimento");

            migrationBuilder.DropTable(
                name: "sub_categorias");

            migrationBuilder.DropTable(
                name: "endereco");

            migrationBuilder.DropTable(
                name: "profissional");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
