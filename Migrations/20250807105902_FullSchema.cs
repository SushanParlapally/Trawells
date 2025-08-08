using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelDesk.Migrations
{
    /// <inheritdoc />
    public partial class FullSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 496, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 496, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 496, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 496, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 7, 16, 29, 1, 497, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9591));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9593));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9594));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9596));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9668));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9670));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9671));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9414));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9416));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9417));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9419));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9700));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9703));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9705));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9708));
        }
    }
}
