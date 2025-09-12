using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivitySchedule",
                columns: table => new
                {
                    activity_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    activity_name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    activity_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    is_chargeable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    fee = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySchedule", x => x.activity_id);
                });

            migrationBuilder.CreateTable(
                name: "ElderlyInfo",
                columns: table => new
                {
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    gender = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    birth_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    id_card_number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    contact_phone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    address = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    emergency_contact = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElderlyInfo", x => x.elderly_id);
                });

            migrationBuilder.CreateTable(
                name: "StaffInfo",
                columns: table => new
                {
                    staff_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    gender = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    position = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    contact_phone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    hire_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    salary = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffInfo", x => x.staff_id);
                });

            migrationBuilder.CreateTable(
                name: "ElderlyActivity",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    activity_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElderlyActivity", x => x.id);
                    table.ForeignKey(
                        name: "FK_ElderlyActivity_ActivitySchedule_activity_id",
                        column: x => x.activity_id,
                        principalTable: "ActivitySchedule",
                        principalColumn: "activity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElderlyActivity_ElderlyInfo_elderly_id",
                        column: x => x.elderly_id,
                        principalTable: "ElderlyInfo",
                        principalColumn: "elderly_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomManagement",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    room_number = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    room_type = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    daily_rate = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    check_in_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    check_out_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomManagement", x => x.room_id);
                    table.ForeignKey(
                        name: "FK_RoomManagement_ElderlyInfo_elderly_id",
                        column: x => x.elderly_id,
                        principalTable: "ElderlyInfo",
                        principalColumn: "elderly_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeSettlement",
                columns: table => new
                {
                    settlement_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    total_amount = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    insurance_amount = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    personal_payment = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    settlement_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    payment_status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    payment_method = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    staff_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeSettlement", x => x.settlement_id);
                    table.ForeignKey(
                        name: "FK_FeeSettlement_ElderlyInfo_elderly_id",
                        column: x => x.elderly_id,
                        principalTable: "ElderlyInfo",
                        principalColumn: "elderly_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeeSettlement_StaffInfo_staff_id",
                        column: x => x.staff_id,
                        principalTable: "StaffInfo",
                        principalColumn: "staff_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalOrder",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    staff_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    order_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    medicine_name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    quantity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    unit_price = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalOrder", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_MedicalOrder_ElderlyInfo_elderly_id",
                        column: x => x.elderly_id,
                        principalTable: "ElderlyInfo",
                        principalColumn: "elderly_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalOrder_StaffInfo_staff_id",
                        column: x => x.staff_id,
                        principalTable: "StaffInfo",
                        principalColumn: "staff_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NursingPlan",
                columns: table => new
                {
                    plan_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    staff_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    plan_start_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    plan_end_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    care_type = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    priority = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    evaluation_status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NursingPlan", x => x.plan_id);
                    table.ForeignKey(
                        name: "FK_NursingPlan_ElderlyInfo_elderly_id",
                        column: x => x.elderly_id,
                        principalTable: "ElderlyInfo",
                        principalColumn: "elderly_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NursingPlan_StaffInfo_staff_id",
                        column: x => x.staff_id,
                        principalTable: "StaffInfo",
                        principalColumn: "staff_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NursingPlanFeeSettlement",
                columns: table => new
                {
                    FeeSettlementssettlement_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NursingPlansplan_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NursingPlanFeeSettlement", x => new { x.FeeSettlementssettlement_id, x.NursingPlansplan_id });
                    table.ForeignKey(
                        name: "FK_NursingPlanFeeSettlement_FeeSettlement_FeeSettlementssettlement_id",
                        column: x => x.FeeSettlementssettlement_id,
                        principalTable: "FeeSettlement",
                        principalColumn: "settlement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NursingPlanFeeSettlement_NursingPlan_NursingPlansplan_id",
                        column: x => x.NursingPlansplan_id,
                        principalTable: "NursingPlan",
                        principalColumn: "plan_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElderlyActivity_activity_id",
                table: "ElderlyActivity",
                column: "activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_ElderlyActivity_elderly_id",
                table: "ElderlyActivity",
                column: "elderly_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeeSettlement_elderly_id",
                table: "FeeSettlement",
                column: "elderly_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeeSettlement_staff_id",
                table: "FeeSettlement",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalOrder_elderly_id",
                table: "MedicalOrder",
                column: "elderly_id");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalOrder_staff_id",
                table: "MedicalOrder",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_NursingPlan_elderly_id",
                table: "NursingPlan",
                column: "elderly_id");

            migrationBuilder.CreateIndex(
                name: "IX_NursingPlan_staff_id",
                table: "NursingPlan",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_NursingPlanFeeSettlement_NursingPlansplan_id",
                table: "NursingPlanFeeSettlement",
                column: "NursingPlansplan_id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomManagement_elderly_id",
                table: "RoomManagement",
                column: "elderly_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElderlyActivity");

            migrationBuilder.DropTable(
                name: "MedicalOrder");

            migrationBuilder.DropTable(
                name: "NursingPlanFeeSettlement");

            migrationBuilder.DropTable(
                name: "RoomManagement");

            migrationBuilder.DropTable(
                name: "ActivitySchedule");

            migrationBuilder.DropTable(
                name: "FeeSettlement");

            migrationBuilder.DropTable(
                name: "NursingPlan");

            migrationBuilder.DropTable(
                name: "ElderlyInfo");

            migrationBuilder.DropTable(
                name: "StaffInfo");
        }
    }
}
