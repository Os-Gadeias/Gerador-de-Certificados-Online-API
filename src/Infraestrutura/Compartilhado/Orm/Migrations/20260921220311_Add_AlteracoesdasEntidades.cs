using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificados.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_AlteracoesdasEntidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBCertificado_TbCurso_Id",
                table: "TBCertificado");

            migrationBuilder.AddColumn<string>(
                name: "CaminhoZip",
                table: "TbCurso",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TbCurso",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NomeAluno",
                table: "TBCertificado",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CaminhoPdf",
                table: "TBCertificado",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CursoId",
                table: "TBCertificado",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TBCertificado",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TBCertificado_CursoId",
                table: "TBCertificado",
                column: "CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TBCertificado_TbCurso_CursoId",
                table: "TBCertificado",
                column: "CursoId",
                principalTable: "TbCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBCertificado_TbCurso_CursoId",
                table: "TBCertificado");

            migrationBuilder.DropIndex(
                name: "IX_TBCertificado_CursoId",
                table: "TBCertificado");

            migrationBuilder.DropColumn(
                name: "CaminhoZip",
                table: "TbCurso");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TbCurso");

            migrationBuilder.DropColumn(
                name: "CaminhoPdf",
                table: "TBCertificado");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "TBCertificado");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TBCertificado");

            migrationBuilder.AlterColumn<string>(
                name: "NomeAluno",
                table: "TBCertificado",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddForeignKey(
                name: "FK_TBCertificado_TbCurso_Id",
                table: "TBCertificado",
                column: "Id",
                principalTable: "TbCurso",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
