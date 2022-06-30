using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviePro.Data.Migrations
{
    public partial class _Collection2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Movie",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Movie");
        }
    }
}
