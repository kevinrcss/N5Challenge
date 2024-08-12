using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace N5Challenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionTypeId = table.Column<int>(type: "int", nullable: false),
                    PermissionDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_PermissionTypes_PermissionTypeId",
                        column: x => x.PermissionTypeId,
                        principalTable: "PermissionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PermissionTypes",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Vacaciones" },
                    { 2, "Permiso médico" },
                    { 3, "Día personal" },
                    { 4, "Licencia por estudios" },
                    { 5, "Licencia por paternidad/maternidad" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "EmployeeLastName", "EmployeeName", "PermissionDate", "PermissionTypeId" },
                values: new object[,]
                {
                    { 1, "Doe", "John", new DateTime(2024, 8, 7, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3829), 1 },
                    { 2, "Smith", "Jane", new DateTime(2024, 8, 10, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3841), 2 },
                    { 3, "Johnson", "Alice", new DateTime(2024, 8, 11, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3843), 3 },
                    { 4, "Brown", "Bob", new DateTime(2024, 8, 5, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3844), 4 },
                    { 5, "Davis", "Charlie", new DateTime(2024, 7, 29, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3845), 5 },
                    { 6, "Wilson", "Diana", new DateTime(2024, 8, 9, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3846), 5 },
                    { 7, "Taylor", "Edward", new DateTime(2024, 8, 2, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3848), 2 },
                    { 8, "Thomas", "Fiona", new DateTime(2024, 8, 17, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3849), 1 },
                    { 9, "Anderson", "George", new DateTime(2024, 8, 14, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3850), 2 },
                    { 10, "Martinez", "Hannah", new DateTime(2024, 8, 13, 16, 2, 55, 230, DateTimeKind.Local).AddTicks(3851), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionTypeId",
                table: "Permissions",
                column: "PermissionTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "PermissionTypes");
        }
    }
}
