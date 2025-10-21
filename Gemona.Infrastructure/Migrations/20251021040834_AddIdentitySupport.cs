using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gemona.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentitySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_avaliacoes_cliente_cliente_id",
                table: "avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_cliente_endereco_endereco_id",
                table: "cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_estabelecimento_profissional_profissional_id",
                table: "estabelecimento");

            migrationBuilder.DropForeignKey(
                name: "FK_pedido_cliente_cliente_id",
                table: "pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_profissional",
                table: "profissional");

            migrationBuilder.DropIndex(
                name: "IX_profissional_email",
                table: "profissional");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cliente",
                table: "cliente");

            migrationBuilder.DropIndex(
                name: "IX_cliente_email",
                table: "cliente");

            migrationBuilder.DropColumn(
                name: "email",
                table: "profissional");

            migrationBuilder.DropColumn(
                name: "senha_hash",
                table: "profissional");

            migrationBuilder.DropColumn(
                name: "telefone",
                table: "profissional");

            migrationBuilder.DropColumn(
                name: "email",
                table: "cliente");

            migrationBuilder.DropColumn(
                name: "senha_hash",
                table: "cliente");

            migrationBuilder.DropColumn(
                name: "telefone",
                table: "cliente");

            migrationBuilder.RenameTable(
                name: "profissional",
                newName: "Profissionais");

            migrationBuilder.RenameTable(
                name: "cliente",
                newName: "Clientes");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Profissionais",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "cpf",
                table: "Profissionais",
                newName: "Cpf");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Profissionais",
                newName: "Ativo");

            migrationBuilder.RenameColumn(
                name: "imagem_perfil_url",
                table: "Profissionais",
                newName: "ImagemPerfilUrl");

            migrationBuilder.RenameColumn(
                name: "data_nascimento",
                table: "Profissionais",
                newName: "DataNascimento");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "Profissionais",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "data_atualizacao",
                table: "Profissionais",
                newName: "DataAtualizacao");

            migrationBuilder.RenameColumn(
                name: "profissional_id",
                table: "Profissionais",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_profissional_cpf",
                table: "Profissionais",
                newName: "IX_Profissionais_Cpf");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Clientes",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "cpf",
                table: "Clientes",
                newName: "Cpf");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Clientes",
                newName: "Ativo");

            migrationBuilder.RenameColumn(
                name: "imagem_perfil_url",
                table: "Clientes",
                newName: "ImagemPerfilUrl");

            migrationBuilder.RenameColumn(
                name: "endereco_id",
                table: "Clientes",
                newName: "EnderecoId");

            migrationBuilder.RenameColumn(
                name: "data_nascimento",
                table: "Clientes",
                newName: "DataNascimento");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "Clientes",
                newName: "DataCriacao");

            migrationBuilder.RenameColumn(
                name: "data_atualizacao",
                table: "Clientes",
                newName: "DataAtualizacao");

            migrationBuilder.RenameColumn(
                name: "cliente_id",
                table: "Clientes",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_cliente_endereco_id",
                table: "Clientes",
                newName: "IX_Clientes_EnderecoId");

            migrationBuilder.RenameIndex(
                name: "IX_cliente_cpf",
                table: "Clientes",
                newName: "IX_Clientes_Cpf");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Profissionais",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ImagemPerfilUrl",
                table: "Profissionais",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataNascimento",
                table: "Profissionais",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataCriacao",
                table: "Profissionais",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Profissionais",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Profissionais",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ImagemPerfilUrl",
                table: "Clientes",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataNascimento",
                table: "Clientes",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataCriacao",
                table: "Clientes",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Clientes",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Clientes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profissionais",
                table: "Profissionais",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_avaliacoes_Clientes_cliente_id",
                table: "avaliacoes",
                column: "cliente_id",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_AspNetUsers_Id",
                table: "Clientes",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_endereco_EnderecoId",
                table: "Clientes",
                column: "EnderecoId",
                principalTable: "endereco",
                principalColumn: "endereco_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_estabelecimento_Profissionais_profissional_id",
                table: "estabelecimento",
                column: "profissional_id",
                principalTable: "Profissionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_Clientes_cliente_id",
                table: "pedido",
                column: "cliente_id",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profissionais_AspNetUsers_Id",
                table: "Profissionais",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_avaliacoes_Clientes_cliente_id",
                table: "avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_AspNetUsers_Id",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_endereco_EnderecoId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_estabelecimento_Profissionais_profissional_id",
                table: "estabelecimento");

            migrationBuilder.DropForeignKey(
                name: "FK_pedido_Clientes_cliente_id",
                table: "pedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Profissionais_AspNetUsers_Id",
                table: "Profissionais");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profissionais",
                table: "Profissionais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "Profissionais",
                newName: "profissional");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "cliente");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "profissional",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "profissional",
                newName: "cpf");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "profissional",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "ImagemPerfilUrl",
                table: "profissional",
                newName: "imagem_perfil_url");

            migrationBuilder.RenameColumn(
                name: "DataNascimento",
                table: "profissional",
                newName: "data_nascimento");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "profissional",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "DataAtualizacao",
                table: "profissional",
                newName: "data_atualizacao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "profissional",
                newName: "profissional_id");

            migrationBuilder.RenameIndex(
                name: "IX_Profissionais_Cpf",
                table: "profissional",
                newName: "IX_profissional_cpf");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "cliente",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "cliente",
                newName: "cpf");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "cliente",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "ImagemPerfilUrl",
                table: "cliente",
                newName: "imagem_perfil_url");

            migrationBuilder.RenameColumn(
                name: "EnderecoId",
                table: "cliente",
                newName: "endereco_id");

            migrationBuilder.RenameColumn(
                name: "DataNascimento",
                table: "cliente",
                newName: "data_nascimento");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "cliente",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "DataAtualizacao",
                table: "cliente",
                newName: "data_atualizacao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cliente",
                newName: "cliente_id");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_EnderecoId",
                table: "cliente",
                newName: "IX_cliente_endereco_id");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_Cpf",
                table: "cliente",
                newName: "IX_cliente_cpf");

            migrationBuilder.AlterColumn<string>(
                name: "nome",
                table: "profissional",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "imagem_perfil_url",
                table: "profissional",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_nascimento",
                table: "profissional",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "profissional",
                type: "DATETIME(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_atualizacao",
                table: "profissional",
                type: "DATETIME(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "profissional_id",
                table: "profissional",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "profissional",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "senha_hash",
                table: "profissional",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "telefone",
                table: "profissional",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "nome",
                table: "cliente",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "imagem_perfil_url",
                table: "cliente",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_nascimento",
                table: "cliente",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_criacao",
                table: "cliente",
                type: "DATETIME(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_atualizacao",
                table: "cliente",
                type: "DATETIME(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "cliente_id",
                table: "cliente",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "cliente",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "senha_hash",
                table: "cliente",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "telefone",
                table: "cliente",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_profissional",
                table: "profissional",
                column: "profissional_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cliente",
                table: "cliente",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_profissional_email",
                table: "profissional",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cliente_email",
                table: "cliente",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_avaliacoes_cliente_cliente_id",
                table: "avaliacoes",
                column: "cliente_id",
                principalTable: "cliente",
                principalColumn: "cliente_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cliente_endereco_endereco_id",
                table: "cliente",
                column: "endereco_id",
                principalTable: "endereco",
                principalColumn: "endereco_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_estabelecimento_profissional_profissional_id",
                table: "estabelecimento",
                column: "profissional_id",
                principalTable: "profissional",
                principalColumn: "profissional_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_cliente_cliente_id",
                table: "pedido",
                column: "cliente_id",
                principalTable: "cliente",
                principalColumn: "cliente_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
