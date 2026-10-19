using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplykoApi.Migrations
{
    /// <inheritdoc />
    internal partial class RemoveBidirectionalUserToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_profiles_users_user_id",
                table: "user_profiles");

            migrationBuilder.DropIndex(
                name: "ix_user_profiles_user_id",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "user_profiles");

            migrationBuilder.AddForeignKey(
                name: "fk_user_profiles_users_id",
                table: "user_profiles",
                column: "id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_profiles_users_id",
                table: "user_profiles");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "user_profiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_user_profiles_users_user_id",
                table: "user_profiles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
