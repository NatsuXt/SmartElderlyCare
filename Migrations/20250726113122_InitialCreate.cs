using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NursingHome.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationLogs",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    staff_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    operation_time = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    operation_type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    operation_description = table.Column<string>(type: "CLOB", nullable: false),
                    affected_entity = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    operation_status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    ip_address = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    device_type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLogs", x => x.log_id);
                });

            migrationBuilder.CreateTable(
                name: "SystemAnnouncements",
                columns: table => new
                {
                    announcement_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    announcement_date = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    announcement_type = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    announcement_content = table.Column<string>(type: "CLOB", nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    audience = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    created_by = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    read_status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    comments = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAnnouncements", x => x.announcement_id);
                });

            migrationBuilder.CreateTable(
                name: "VisitorRegistrations",
                columns: table => new
                {
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    family_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    elderly_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    visitor_name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    visit_time = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    relationship_to_elderly = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    visit_reason = table.Column<string>(type: "CLOB", nullable: false),
                    visit_type = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    approval_status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorRegistrations", x => x.visitor_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationLogs");

            migrationBuilder.DropTable(
                name: "SystemAnnouncements");

            migrationBuilder.DropTable(
                name: "VisitorRegistrations");
        }
    }
}
