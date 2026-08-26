using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateconversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Conversations");
        }
    }
}
