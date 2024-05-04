using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Data.EF.Sqlite.Migrations
{
    public partial class RemoveMediaThumbnailUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Media");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Media",
                type: "TEXT",
                nullable: true);
        }
    }
}
