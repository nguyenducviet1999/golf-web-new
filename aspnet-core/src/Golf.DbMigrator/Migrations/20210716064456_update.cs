using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Golf.DbMigrator.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityUser<Guid>");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("95a42401-e5f5-4eca-911c-a72f34b3cff9"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a3b835a2-be77-4949-9f65-f56f05c5f0ea"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e7fc0f42-49d5-443b-a25a-6b49cf57b9df"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fcbc3142-f7dc-440c-8bba-ee1b062289dc"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("75148476-66e0-479b-a328-1e0c4da67e5a"), "", "Golfer", "GOLFER" },
                    { new Guid("abc0fe8d-fb10-49a7-8540-1d67d52c283e"), "", "Course Admin", "CA" },
                    { new Guid("b8fda276-af61-4d2d-811a-9492bcd0cd93"), "", "System Admin", "SA" },
                    { new Guid("af85c918-6a22-4bbd-b93b-b89ad793bd09"), "", "GSA", "GSA" }
                });

            migrationBuilder.InsertData(
                table: "CourseExtensions",
                columns: new[] { "ID", "Icon", "Name" },
                values: new object[,]
                {
                    { 14, "Common/CourseExtension14", "Palace" },
                    { 13, "Common/CourseExtension13", "Restaurant" },
                    { 12, "Common/CourseExtension12", "Club house" },
                    { 11, "Common/CourseExtension11", "Golf cart surcharge" },
                    { 10, "Common/CourseExtension10", "Golf cart" },
                    { 8, "Common/CourseExtension8", "Locker" },
                    { 7, "Common/CourseExtension7", "Rent umbrella" },
                    { 6, "Common/CourseExtension6", "Rent shoes" },
                    { 5, "Common/CourseExtension5", "Rent clubs" },
                    { 4, "Common/CourseExtension4", "Pro shop" },
                    { 3, "Common/CourseExtension3", "Driving range" },
                    { 2, "Common/CourseExtension2", "Rent golf cart" },
                    { 9, "Common/CourseExtension9", "Caddy" },
                    { 1, "Common/CourseExtension1", "Hire caddy" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                column: "ID",
                value: new Guid("875cefd8-6e7d-4f8b-b4aa-f41a0ffe6592"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("75148476-66e0-479b-a328-1e0c4da67e5a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("abc0fe8d-fb10-49a7-8540-1d67d52c283e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("af85c918-6a22-4bbd-b93b-b89ad793bd09"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b8fda276-af61-4d2d-811a-9492bcd0cd93"));

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CourseExtensions",
                keyColumn: "ID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "ID",
                keyValue: new Guid("875cefd8-6e7d-4f8b-b4aa-f41a0ffe6592"));

            migrationBuilder.CreateTable(
                name: "IdentityUser<Guid>",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser<Guid>", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("e7fc0f42-49d5-443b-a25a-6b49cf57b9df"), "", "System Admin", "SA" },
                    { new Guid("95a42401-e5f5-4eca-911c-a72f34b3cff9"), "", "Course Admin", "CA" },
                    { new Guid("fcbc3142-f7dc-440c-8bba-ee1b062289dc"), "", "GSA", "GSA" },
                    { new Guid("a3b835a2-be77-4949-9f65-f56f05c5f0ea"), "", "Golfer", "GOLFER" }
                });
        }
    }
}
