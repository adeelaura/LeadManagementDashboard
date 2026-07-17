using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LeadManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false, defaultValue: "#0D6EFD"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    FromStatusId = table.Column<int>(type: "int", nullable: false),
                    ToStatusId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadActivities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadActivities_Statuses_FromStatusId",
                        column: x => x.FromStatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeadActivities_Statuses_ToStatusId",
                        column: x => x.ToStatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "ColorCode", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "#0D6EFD", 1, true, "New" },
                    { 2, "#FFC107", 2, true, "Contacted" },
                    { 3, "#198754", 3, true, "Qualified" },
                    { 4, "#6C757D", 4, true, "Closed" }
                });

            migrationBuilder.InsertData(
                table: "Leads",
                columns: new[] { "Id", "Company", "CreatedAt", "Email", "FirstName", "LastName", "Phone", "StatusId" },
                values: new object[,]
                {
                    { 1, "Northstar Digital", new DateTimeOffset(new DateTime(2026, 6, 25, 8, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "aisha.khan@example.com", "Aisha", "Khan", "+971 50 555 0101", 1 },
                    { 2, "BluePeak Logistics", new DateTimeOffset(new DateTime(2026, 6, 24, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "david.brown@example.com", "David", "Brown", "+971 50 555 0102", 2 },
                    { 3, "Vertex Consulting", new DateTimeOffset(new DateTime(2026, 6, 22, 13, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "maria.garcia@example.com", "Maria", "Garcia", "+971 50 555 0103", 3 },
                    { 4, "Crescent Holdings", new DateTimeOffset(new DateTime(2026, 6, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "omar.ali@example.com", "Omar", "Ali", "+971 50 555 0104", 4 },
                    { 5, "Atlas Retail Group", new DateTimeOffset(new DateTime(2026, 6, 27, 16, 20, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sophie.turner@example.com", "Sophie", "Turner", "+971 50 555 0105", 1 }
                });

            migrationBuilder.InsertData(
                table: "LeadActivities",
                columns: new[] { "Id", "ChangedAt", "FromStatusId", "LeadId", "Note", "ToStatusId" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2026, 6, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 2, "Status changed by user", 2 },
                    { 2, new DateTimeOffset(new DateTime(2026, 6, 23, 9, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 3, "Status changed by user", 2 },
                    { 3, new DateTimeOffset(new DateTime(2026, 6, 24, 14, 10, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 3, "Status changed by user", 3 },
                    { 4, new DateTimeOffset(new DateTime(2026, 6, 21, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 4, "Status changed by user", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_FromStatusId",
                table: "LeadActivities",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_LeadId_ChangedAt",
                table: "LeadActivities",
                columns: new[] { "LeadId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_ToStatusId",
                table: "LeadActivities",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CreatedAt",
                table: "Leads",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Email",
                table: "Leads",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_StatusId",
                table: "Leads",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_IsActive_DisplayOrder",
                table: "Statuses",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_Name",
                table: "Statuses",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadActivities");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Statuses");
        }
    }
}
