using Microsoft.EntityFrameworkCore.Migrations;

namespace HCGStudio.DongBot.App.Migrations
{
    public partial class Application : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ServiceRecords",
                table => new
                {
                    ServiceRecordId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<long>(nullable: false),
                    ServiceName = table.Column<string>(maxLength: 500, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ServiceRecords", x => x.ServiceRecordId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ServiceRecords");
        }
    }
}