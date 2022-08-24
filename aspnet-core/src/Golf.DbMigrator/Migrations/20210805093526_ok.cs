using System;
using Golf.Domain.Post;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Golf.DbMigrator.Migrations
{
    public partial class ok : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReferenceObject_ReferenceObjectID",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReferenceObject");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReferenceObjectID",
                table: "Reports");

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
                table: "SystemSettings",
                keyColumn: "ID",
                keyValue: new Guid("875cefd8-6e7d-4f8b-b4aa-f41a0ffe6592"));

            migrationBuilder.DropColumn(
                name: "ReferenceObjectID",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PhotoIDs",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "PhotoID",
                table: "Comments");

            migrationBuilder.AddColumn<ReferenceObject>(
                name: "ReferenceObject",
                table: "Reports",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerID",
                table: "Groups",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PhotoNames",
                table: "Courses",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerID",
                table: "CourseReviews",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PhotoNames",
                table: "CourseReviews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoNames",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedByIp = table.Column<string>(nullable: true),
                    Revoked = table.Column<DateTime>(nullable: true),
                    RevokedByIp = table.Column<string>(nullable: true),
                    ReplacedByToken = table.Column<string>(nullable: true),
                    ReasonRevoked = table.Column<string>(nullable: true),
                    GolferId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_GolferId",
                        column: x => x.GolferId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("458d5b6e-f057-482f-b40d-9d595dabc40e"), "", "System Admin", "SA" },
                    { new Guid("75ab7e5b-eed5-4e9f-b4f0-f96c05d17452"), "", "Course Admin", "CA" },
                    { new Guid("75664d73-9b81-4d15-b11d-c8bd9b5d5586"), "", "GSA", "GSA" },
                    { new Guid("22f786ca-fd8f-4aee-8e71-20bb3b86a7b4"), "", "Golfer", "GOLFER" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                column: "ID",
                value: new Guid("107f8cbb-31c0-4bc1-b6b1-eb6041e76395"));

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerID",
                table: "Groups",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GolferID",
                table: "GroupMembers",
                column: "GolferID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupID",
                table: "GroupMembers",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_OwnerID",
                table: "CourseReviews",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_GolferId",
                table: "RefreshToken",
                column: "GolferId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseReviews_AspNetUsers_OwnerID",
                table: "CourseReviews",
                column: "OwnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_AspNetUsers_GolferID",
                table: "GroupMembers",
                column: "GolferID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Groups_GroupID",
                table: "GroupMembers",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_OwnerID",
                table: "Groups",
                column: "OwnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseReviews_AspNetUsers_OwnerID",
                table: "CourseReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_AspNetUsers_GolferID",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Groups_GroupID",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_OwnerID",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_Groups_OwnerID",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_GroupMembers_GolferID",
                table: "GroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_GroupMembers_GroupID",
                table: "GroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_CourseReviews_OwnerID",
                table: "CourseReviews");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("22f786ca-fd8f-4aee-8e71-20bb3b86a7b4"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("458d5b6e-f057-482f-b40d-9d595dabc40e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("75664d73-9b81-4d15-b11d-c8bd9b5d5586"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("75ab7e5b-eed5-4e9f-b4f0-f96c05d17452"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "ID",
                keyValue: new Guid("107f8cbb-31c0-4bc1-b6b1-eb6041e76395"));

            migrationBuilder.DropColumn(
                name: "ReferenceObject",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PhotoNames",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "CourseReviews");

            migrationBuilder.DropColumn(
                name: "PhotoNames",
                table: "CourseReviews");

            migrationBuilder.DropColumn(
                name: "PhotoNames",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceObjectID",
                table: "Reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoIDs",
                table: "Courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoID",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReferenceObject",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    WebLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceObject", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("b8fda276-af61-4d2d-811a-9492bcd0cd93"), "", "System Admin", "SA" },
                    { new Guid("abc0fe8d-fb10-49a7-8540-1d67d52c283e"), "", "Course Admin", "CA" },
                    { new Guid("af85c918-6a22-4bbd-b93b-b89ad793bd09"), "", "GSA", "GSA" },
                    { new Guid("75148476-66e0-479b-a328-1e0c4da67e5a"), "", "Golfer", "GOLFER" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                column: "ID",
                value: new Guid("875cefd8-6e7d-4f8b-b4aa-f41a0ffe6592"));

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReferenceObjectID",
                table: "Reports",
                column: "ReferenceObjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReferenceObject_ReferenceObjectID",
                table: "Reports",
                column: "ReferenceObjectID",
                principalTable: "ReferenceObject",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
