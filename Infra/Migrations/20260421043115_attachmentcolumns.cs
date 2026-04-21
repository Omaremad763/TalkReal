using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class attachmentcolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Attachment_IsProcessed",
                table: "Messages",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attachment_PublicId",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachment_IsProcessed",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Attachment_PublicId",
                table: "Messages");
        }
    }
}
