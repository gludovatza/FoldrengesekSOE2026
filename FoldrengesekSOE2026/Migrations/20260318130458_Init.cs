using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoldrengesekSOE2026.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Telepulesek",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Varmegye = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telepulesek", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Naplok",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ido = table.Column<TimeSpan>(type: "time", nullable: false),
                    Magnitudo = table.Column<double>(type: "float", nullable: false),
                    Intenzitas = table.Column<double>(type: "float", nullable: false),
                    TelepulesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Naplok", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Naplok_Telepulesek_TelepulesID",
                        column: x => x.TelepulesID,
                        principalTable: "Telepulesek",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Naplok_TelepulesID",
                table: "Naplok",
                column: "TelepulesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Naplok");

            migrationBuilder.DropTable(
                name: "Telepulesek");
        }
    }
}
