using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelDesk.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6864));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6870));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6875));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6880));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6970));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6975));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(6979));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(3463));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(3478));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(3482));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(3486));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedBy", "CreatedOn", "DepartmentId", "Email", "FirstName", "IsActive", "LastName", "ManagerId", "MobileNum", "ModifiedBy", "ModifiedOn", "Password", "RoleId" },
                values: new object[,]
                {
                    { 1, "123 Admin Street", 1, new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(7107), 3, "admin@traveldesk.com", "Admin", true, "User", null, "1234567890", null, null, "admin123", 1 },
                    { 2, "456 Travel Street", 1, new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(7488), 4, "traveladmin@traveldesk.com", "Travel", true, "Admin", 1, "2345678901", null, null, "travel123", 2 },
                    { 3, "789 Manager Street", 1, new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(7495), 1, "manager@traveldesk.com", "Manager", true, "User", 1, "3456789012", null, null, "manager123", 3 },
                    { 4, "321 Employee Street", 1, new DateTime(2025, 7, 23, 7, 40, 44, 508, DateTimeKind.Local).AddTicks(7502), 1, "employee@traveldesk.com", "Employee", true, "User", 3, "4567890123", null, null, "employee123", 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3047));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3049));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3050));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3052));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3081));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3082));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "ProjectId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(3084));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(2880));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(2882));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(2883));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 9, 4, 18, 59, 33, 839, DateTimeKind.Local).AddTicks(2885));
        }
    }
}
