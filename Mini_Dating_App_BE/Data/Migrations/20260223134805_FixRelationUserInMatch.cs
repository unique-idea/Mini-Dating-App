using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Dating_App_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationUserInMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_User_UserAUserId",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_User_UserBUserId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_UserAUserId",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_UserBUserId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "UserAUserId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "UserBUserId",
                table: "Match");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserAUserId",
                table: "Match",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserBUserId",
                table: "Match",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Match_UserAUserId",
                table: "Match",
                column: "UserAUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_UserBUserId",
                table: "Match",
                column: "UserBUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Match_User_UserAUserId",
                table: "Match",
                column: "UserAUserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Match_User_UserBUserId",
                table: "Match",
                column: "UserBUserId",
                principalTable: "User",
                principalColumn: "UserId");
        }
    }
}
