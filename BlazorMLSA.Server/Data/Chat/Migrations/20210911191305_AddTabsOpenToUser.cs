using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorMLSA.Server.Data.Chat.Migrations
{
    public partial class AddTabsOpenToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TabsOpen",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TabsOpen",
                table: "Users");
        }
    }
}
