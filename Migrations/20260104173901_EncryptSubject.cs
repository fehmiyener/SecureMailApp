using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureMailApp.Migrations
{
    /// <inheritdoc />
    public partial class EncryptSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Subject",
                table: "Messages",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<byte[]>(
                name: "SubjectIv",
                table: "Messages",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectIv",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");
        }
    }
}
