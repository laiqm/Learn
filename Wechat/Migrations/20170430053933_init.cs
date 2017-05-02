using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Wechat.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Display = table.Column<string>(nullable: true),
                    OpenId = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    ContactDetail = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<int>(nullable: true),
                    FriendCount = table.Column<int>(nullable: false),
                    GroupCount = table.Column<int>(nullable: false),
                    PublicCount = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserDetail = table.Column<string>(nullable: true),
                    UserDisplay = table.Column<string>(nullable: true),
                    ZombieCount = table.Column<int>(nullable: false),
                    ZombieList = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_Account_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Task_CreatorId",
                table: "Task",
                column: "CreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
