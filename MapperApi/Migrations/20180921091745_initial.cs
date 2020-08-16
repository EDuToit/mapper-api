using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MapperApi.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiveUser",
                columns: table => new
                {
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveUser", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.UniqueConstraint("Email", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "LiveLocation",
                columns: table => new
                {
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    UserID = table.Column<Guid>(nullable: false),
                    PointRaw = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveLocation", x => new { x.CreatedAt, x.UserID });
                    table.ForeignKey(
                        name: "FK_LiveLocation_LiveUser_UserID",
                        column: x => x.UserID,
                        principalTable: "LiveUser",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ZoneID = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    Info = table.Column<string>(nullable: true),
                    ParentZoneID = table.Column<Guid>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    UserId = table.Column<Guid>(nullable: false),
                    ZoneName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.ZoneID);
                    table.ForeignKey(
                        name: "FK_Zones_Zones_ParentZoneID",
                        column: x => x.ParentZoneID,
                        principalTable: "Zones",
                        principalColumn: "ZoneID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zones_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    ElementId = table.Column<Guid>(nullable: false),
                    ClassType = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    Discriminator = table.Column<string>(nullable: false),
                    ElementType = table.Column<int>(nullable: false),
                    Info = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    ZoneID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.ElementId);
                    table.ForeignKey(
                        name: "FK_Elements_Zones_ZoneID",
                        column: x => x.ZoneID,
                        principalTable: "Zones",
                        principalColumn: "ZoneID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Elements_ZoneID",
                table: "Elements",
                column: "ZoneID");

            migrationBuilder.CreateIndex(
                name: "IX_LiveLocation_UserID",
                table: "LiveLocation",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_ParentZoneID",
                table: "Zones",
                column: "ParentZoneID");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_UserId",
                table: "Zones",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "LiveLocation");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "LiveUser");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
