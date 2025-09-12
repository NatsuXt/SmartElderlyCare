using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "billing_cycle_end",
                table: "FeeSettlement",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "billing_cycle_start",
                table: "FeeSettlement",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "FeeSettlement",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "FeeDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    fee_settlement_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    fee_type = table.Column<string>(type: "NVARCHAR2(50)", nullable: false),
                    description = table.Column<string>(type: "NVARCHAR2(200)", nullable: false),
                    amount = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    start_date = table.Column<DateTime>(type: "DATE", nullable: true),
                    end_date = table.Column<DateTime>(type: "DATE", nullable: true),
                    quantity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    unit_price = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeDetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeeDetails_FeeSettlement_fee_settlement_id",
                        column: x => x.fee_settlement_id,
                        principalTable: "FeeSettlement",
                        principalColumn: "settlement_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeeDetails_fee_settlement_id",
                table: "FeeDetails",
                column: "fee_settlement_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeeDetails");

            migrationBuilder.DropColumn(
                name: "billing_cycle_end",
                table: "FeeSettlement");

            migrationBuilder.DropColumn(
                name: "billing_cycle_start",
                table: "FeeSettlement");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "FeeSettlement");
        }
    }
}
