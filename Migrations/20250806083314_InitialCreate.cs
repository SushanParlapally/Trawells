using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelDesk.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MobileNum = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    ManagerId = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TravelRequests",
                columns: table => new
                {
                    TravelRequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    ReasonForTravel = table.Column<string>(type: "text", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ToDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FromLocation = table.Column<string>(type: "text", nullable: false),
                    ToLocation = table.Column<string>(type: "text", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    TicketUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelRequests", x => x.TravelRequestId);
                    table.ForeignKey(
                        name: "FK_TravelRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TravelRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "CreatedBy", "CreatedOn", "DepartmentName", "IsActive", "ModifiedBy", "ModifiedOn" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9591), "IT", true, null, null },
                    { 2, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9593), "HR", true, null, null },
                    { 3, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9594), "Admin", true, null, null },
                    { 4, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9596), "Travel", true, null, null }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "ProjectId", "CreatedBy", "CreatedOn", "IsActive", "ModifiedBy", "ModifiedOn", "ProjectName" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9668), true, null, null, "Project Alpha" },
                    { 2, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9670), true, null, null, "Project Beta" },
                    { 3, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9671), true, null, null, "Project Gamma" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "CreatedBy", "CreatedOn", "IsActive", "ModifiedBy", "ModifiedOn", "RoleName" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9414), true, null, null, "Admin" },
                    { 2, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9416), true, null, null, "TravelAdmin" },
                    { 3, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9417), true, null, null, "Manager" },
                    { 4, 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9419), true, null, null, "Employee" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "CreatedBy", "CreatedOn", "DepartmentId", "Email", "FirstName", "IsActive", "LastName", "ManagerId", "MobileNum", "ModifiedBy", "ModifiedOn", "Password", "RoleId" },
                values: new object[,]
                {
                    { 1, "123 Admin Street", 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9700), 3, "admin@traveldesk.com", "Admin", true, "User", null, "1234567890", null, null, "admin123", 1 },
                    { 2, "456 Travel Street", 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9703), 4, "traveladmin@traveldesk.com", "Travel", true, "Admin", 1, "2345678901", null, null, "travel123", 2 },
                    { 3, "789 Manager Street", 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9705), 1, "manager@traveldesk.com", "Manager", true, "User", 1, "3456789012", null, null, "manager123", 3 },
                    { 4, "321 Employee Street", 1, new DateTime(2025, 8, 6, 14, 3, 13, 715, DateTimeKind.Utc).AddTicks(9708), 1, "employee@traveldesk.com", "Employee", true, "User", 3, "4567890123", null, null, "employee123", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelRequests_DepartmentId",
                table: "TravelRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRequests_ProjectId",
                table: "TravelRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelRequests_UserId",
                table: "TravelRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagerId",
                table: "Users",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelRequests");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
