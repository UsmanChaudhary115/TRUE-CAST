using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVS.Migrations
{
    /// <inheritdoc />
    public partial class ElectionResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElectionResults",
                columns: table => new
                {
                    ElectionResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectionTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElectionStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ElectionEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CandidatePartyAffiliation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VotesReceived = table.Column<int>(type: "int", nullable: false),
                    TotalVotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionResults", x => x.ElectionResultId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectionResults");
        }
    }
}
