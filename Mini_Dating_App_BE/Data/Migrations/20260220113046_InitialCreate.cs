using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Dating_App_BE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    Bio = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    AvailabilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availability", x => x.AvailabilityId);
                    table.ForeignKey(
                        name: "FK_Availability_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserAId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserBId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Match_User_UserAId",
                        column: x => x.UserAId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Match_User_UserBId",
                        column: x => x.UserBId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLike",
                columns: table => new
                {
                    LikerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LikedId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLike", x => new { x.LikerId, x.LikedId });
                    table.ForeignKey(
                        name: "FK_UserLike_User_LikedId",
                        column: x => x.LikedId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLike_User_LikerId",
                        column: x => x.LikerId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Availability_UserId",
                table: "Availability",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_UserAId",
                table: "Match",
                column: "UserAId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_UserBId",
                table: "Match",
                column: "UserBId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLike_LikedId",
                table: "UserLike",
                column: "LikedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "UserLike");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
