using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorCertificados.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Corrijido_Curso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CursoId1",
                table: "TBCertificado",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBCertificado_CursoId1",
                table: "TBCertificado",
                column: "CursoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TBCertificado_TbCurso_CursoId1",
                table: "TBCertificado",
                column: "CursoId1",
                principalTable: "TbCurso",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBCertificado_TbCurso_CursoId1",
                table: "TBCertificado");

            migrationBuilder.DropIndex(
                name: "IX_TBCertificado_CursoId1",
                table: "TBCertificado");

            migrationBuilder.DropColumn(
                name: "CursoId1",
                table: "TBCertificado");
        }
    }
}
