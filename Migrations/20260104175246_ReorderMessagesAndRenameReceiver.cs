using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureMailApp.Migrations
{
    /// <inheritdoc />
    public partial class ReorderMessagesAndRenameReceiver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages_temp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<byte[]>(type: "BLOB", nullable: false),
                    SubjectIv = table.Column<byte[]>(type: "BLOB", nullable: false),
                    EncryptedSymmetricKey = table.Column<byte[]>(type: "BLOB", nullable: false),
                    SymmetricIv = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CipherText = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Hash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
                INSERT INTO Messages_temp (Id, SenderId, ReceiverId, Subject, SubjectIv, EncryptedSymmetricKey, SymmetricIv, CipherText, Hash, Signature, CreatedAt)
                SELECT Id, SenderId, RecipientId, Subject, SubjectIv, EncryptedSymmetricKey, SymmetricIv, CipherText, Hash, Signature, CreatedAt
                FROM Messages;
            ");

            migrationBuilder.DropTable(name: "Messages");

            migrationBuilder.RenameTable(
                name: "Messages_temp",
                newName: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages_temp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CipherText = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EncryptedSymmetricKey = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Hash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    RecipientId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Subject = table.Column<byte[]>(type: "BLOB", nullable: false),
                    SubjectIv = table.Column<byte[]>(type: "BLOB", nullable: false),
                    SymmetricIv = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
                INSERT INTO Messages_temp (Id, CipherText, CreatedAt, EncryptedSymmetricKey, Hash, RecipientId, SenderId, Signature, Subject, SubjectIv, SymmetricIv)
                SELECT Id, CipherText, CreatedAt, EncryptedSymmetricKey, Hash, ReceiverId, SenderId, Signature, Subject, SubjectIv, SymmetricIv
                FROM Messages;
            ");

            migrationBuilder.DropTable(name: "Messages");

            migrationBuilder.RenameTable(
                name: "Messages_temp",
                newName: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");
        }
    }
}
