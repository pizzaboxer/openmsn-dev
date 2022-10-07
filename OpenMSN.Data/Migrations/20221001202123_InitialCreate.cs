using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenMSN.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    activated = table.Column<bool>(type: "boolean", nullable: false),
                    activation_token = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    password_hash_md5 = table.Column<string>(type: "text", nullable: false),
                    md5salt = table.Column<string>(type: "text", nullable: false),
                    contact_list_version = table.Column<int>(type: "integer", nullable: false),
                    contact_add_alert = table.Column<bool>(type: "boolean", nullable: false),
                    contact_privacy = table.Column<string>(type: "text", nullable: false),
                    time_added = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    contact_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    list = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    target_user_id = table.Column<int>(type: "integer", nullable: false),
                    time_added = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contacts", x => x.contact_id);
                    table.ForeignKey(
                        name: "fk_contacts_users_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contacts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "activated", "activation_token", "contact_add_alert", "contact_list_version", "contact_privacy", "email_address", "md5salt", "password_hash", "password_hash_md5", "time_added", "username" },
                values: new object[,]
                {
                    { 1, true, "78196fd1-8181-45d0-97c8-16a59977acb4", true, 0, "AL", "pizzaboxer@hotmail.com", "7f30143936ba04972090", "$argon2id$v=19$m=65536,t=3,p=1$/+34CRzCS6MJv30NTB1fDQ$IoisuDjna+EYXU617erIYmXMAU6/NJdQeXx7XA+w8L0", "$argon2id$v=19$m=65536,t=3,p=1$GHWJXxxgaAz1gNFOZTeHkg$BSni28HhYC9Dx1LcVTnmyJBl4GEPR0YcfPxWTmO0hyc", new DateTimeOffset(new DateTime(2022, 10, 1, 20, 21, 23, 6, DateTimeKind.Unspecified).AddTicks(6544), new TimeSpan(0, 0, 0, 0, 0)), "pizzaboxer@hotmail.com" },
                    { 2, true, "63e9f88f-8f89-4470-a329-6f29d8b15a49", true, 0, "AL", "pizzaboxer2@hotmail.com", "6502408dbcbea1e56eeb", "$argon2id$v=19$m=65536,t=3,p=1$rfVcCqUoJNQEXNFcoD/KtA$tC0Sik/4zzCN3WSMnG6L6fHFAc2I+QeRGmjREZMsOkM", "$argon2id$v=19$m=65536,t=3,p=1$F/NwmzVUbf2BkmONdub81g$NW9o0CvFt/H3forXfcHKSWxNRdgirel9px5S6l1IpzQ", new DateTimeOffset(new DateTime(2022, 10, 1, 20, 21, 23, 6, DateTimeKind.Unspecified).AddTicks(6552), new TimeSpan(0, 0, 0, 0, 0)), "pizzaboxer2@hotmail.com" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_contacts_target_user_id",
                table: "contacts",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_contacts_user_id",
                table: "contacts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email_address",
                table: "users",
                column: "email_address",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
