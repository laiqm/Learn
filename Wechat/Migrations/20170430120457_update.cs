using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Wechat.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CheckLoginRetryCount = table.Column<int>(nullable: false),
                    MessagePrefix = table.Column<string>(nullable: true),
                    MessageSuffix = table.Column<string>(nullable: true),
                    SendMsgFailRetryCount = table.Column<int>(nullable: false),
                    SendMsgFailRetryInteval = table.Column<int>(nullable: false),
                    SendMsgInteval = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");
        }
    }
}
