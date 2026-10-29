using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionProfesora.Api.Migrations
{
    /// <inheritdoc />
    public partial class MateriasYAsociaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProfesora = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materias_Profesoras_IdProfesora",
                        column: x => x.IdProfesora,
                        principalTable: "Profesoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlumnosMaterias",
                columns: table => new
                {
                    IdAlumno = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdMateria = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnosMaterias", x => new { x.IdAlumno, x.IdMateria });
                    table.ForeignKey(
                        name: "FK_AlumnosMaterias_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlumnosMaterias_Materias_IdMateria",
                        column: x => x.IdMateria,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnosMaterias_IdMateria",
                table: "AlumnosMaterias",
                column: "IdMateria");

            migrationBuilder.CreateIndex(
                name: "IX_Materias_IdProfesora_Nombre",
                table: "Materias",
                columns: new[] { "IdProfesora", "Nombre" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnosMaterias");

            migrationBuilder.DropTable(
                name: "Materias");
        }
    }
}
