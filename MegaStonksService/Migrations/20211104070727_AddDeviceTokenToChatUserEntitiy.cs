using Microsoft.EntityFrameworkCore.Migrations;

namespace MegaStonksService.Migrations
{
    public partial class AddDeviceTokenToChatUserEntitiy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceToken",
                table: "ChatUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceToken",
                table: "ChatUsers");
        }
    }
}
