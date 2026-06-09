using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monopoly.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddTradingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Propositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false),
                    CellNumbers = table.Column<List<int>>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    OffererId = table.Column<Guid>(type: "uuid", nullable: false),
                    OffererPropositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OffereeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OffereePropositionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeOffers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeOffers_Players_OffereeId",
                        column: x => x.OffereeId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeOffers_Players_OffererId",
                        column: x => x.OffererId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeOffers_Propositions_OffereePropositionId",
                        column: x => x.OffereePropositionId,
                        principalTable: "Propositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeOffers_Propositions_OffererPropositionId",
                        column: x => x.OffererPropositionId,
                        principalTable: "Propositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeOffers_GameId",
                table: "TradeOffers",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeOffers_OffereeId",
                table: "TradeOffers",
                column: "OffereeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOffers_OffereePropositionId",
                table: "TradeOffers",
                column: "OffereePropositionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeOffers_OffererId",
                table: "TradeOffers",
                column: "OffererId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOffers_OffererPropositionId",
                table: "TradeOffers",
                column: "OffererPropositionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeOffers");

            migrationBuilder.DropTable(
                name: "Propositions");
        }
    }
}
