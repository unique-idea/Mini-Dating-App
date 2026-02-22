using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Dating_App_BE.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLike",
                table: "UserLike");

            migrationBuilder.RenameColumn(
                name: "MatchedAt",
                table: "Match",
                newName: "Status");

            migrationBuilder.AddColumn<Guid>(
                name: "UserLikeId",
                table: "UserLike",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "UserAConfirmed",
                table: "Match",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserBConfirmed",
                table: "Match",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLike",
                table: "UserLike",
                column: "UserLikeId");

            migrationBuilder.CreateTable(
                name: "ScheduledDate",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledDate", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_ScheduledDate_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLike_LikerId",
                table: "UserLike",
                column: "LikerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledDate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLike",
                table: "UserLike");

            migrationBuilder.DropIndex(
                name: "IX_UserLike_LikerId",
                table: "UserLike");

            migrationBuilder.DropColumn(
                name: "UserLikeId",
                table: "UserLike");

            migrationBuilder.DropColumn(
                name: "UserAConfirmed",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "UserBConfirmed",
                table: "Match");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Match",
                newName: "MatchedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLike",
                table: "UserLike",
                columns: new[] { "LikerId", "LikedId" });
        }
    }
}
