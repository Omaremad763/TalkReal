using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations;

/// <inheritdoc />
public partial class RenameColumnsTableMessage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Timestamp",
            table: "Messages",
            newName: "SentAt");

        migrationBuilder.AddColumn<string>(
            name: "ReceiverId",
            table: "Messages",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ReceiverId",
            table: "Messages");

        migrationBuilder.RenameColumn(
            name: "SentAt",
            table: "Messages",
            newName: "Timestamp");
    }
}
