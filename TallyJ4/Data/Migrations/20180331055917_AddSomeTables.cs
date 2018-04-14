using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TallyJ4.Data.Migrations
{
    public partial class AddSomeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Election",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BallotProcessRaw = table.Column<string>(nullable: true),
                    C_RowVersion = table.Column<byte[]>(nullable: true),
                    CanReceive = table.Column<string>(nullable: true),
                    CanVote = table.Column<string>(nullable: true),
                    Convenor = table.Column<string>(nullable: true),
                    DateOfElection = table.Column<DateTime>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    ElectionMode = table.Column<string>(nullable: true),
                    ElectionPasscode = table.Column<string>(nullable: true),
                    ElectionType = table.Column<string>(nullable: true),
                    EnvNumModeRaw = table.Column<string>(nullable: true),
                    HidePreBallotPages = table.Column<bool>(nullable: true),
                    LastEnvNum = table.Column<int>(nullable: true),
                    LinkedElectionGuid = table.Column<Guid>(nullable: true),
                    LinkedElectionKind = table.Column<string>(nullable: true),
                    ListForPublic = table.Column<bool>(nullable: true),
                    ListedForPublicAsOf = table.Column<DateTime>(nullable: true),
                    MaskVotingMethod = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NumberExtra = table.Column<int>(nullable: true),
                    NumberToElect = table.Column<int>(nullable: true),
                    OwnerLoginId = table.Column<string>(nullable: true),
                    ShowAsTest = table.Column<bool>(nullable: true),
                    ShowFullReport = table.Column<bool>(nullable: true),
                    T24 = table.Column<bool>(nullable: false),
                    TallyStatus = table.Column<string>(nullable: true),
                    UseCallInButton = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Election", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BallotsCollected = table.Column<int>(nullable: true),
                    ContactInfo = table.Column<string>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    Lat = table.Column<string>(nullable: true),
                    LocationGuid = table.Column<Guid>(nullable: false),
                    Long = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: true),
                    TallyStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnlineTempBallots",
                columns: table => new
                {
                    OnlineTempBallotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    PersonGuidList = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineTempBallots", x => x.OnlineTempBallotId);
                    table.ForeignKey(
                        name: "FK_OnlineTempBallots_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineTempBallots_ApplicationUserId",
                table: "OnlineTempBallots",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Election");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "OnlineTempBallots");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }
    }
}
