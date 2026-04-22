using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations;

/// <inheritdoc />
public partial class ConversationTableColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ParticipantAId",
            table: "Conversations",
            type: "character varying(450)",
            maxLength: 450,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ParticipantBId",
            table: "Conversations",
            type: "character varying(450)",
            maxLength: 450,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_Unique_Conversation_Participants",
            table: "Conversations",
            columns: new[] { "ParticipantAId", "ParticipantBId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Unique_Conversation_Participants",
            table: "Conversations");

        migrationBuilder.DropColumn(
            name: "ParticipantAId",
            table: "Conversations");

        migrationBuilder.DropColumn(
            name: "ParticipantBId",
            table: "Conversations");
    }
}
