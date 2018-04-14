using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TallyJ4.Data.Migrations
{
    public partial class MoreTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnlineTempBallotId",
                table: "OnlineTempBallots",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EnvNumModeRaw",
                table: "Election",
                newName: "ExtraFuture3");

            migrationBuilder.RenameColumn(
                name: "BallotProcessRaw",
                table: "Election",
                newName: "ExtraFuture2");

            migrationBuilder.AlterColumn<bool>(
                name: "T24",
                table: "Election",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "BallotProcess",
                table: "Election",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvNumMode",
                table: "Election",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraFuture1",
                table: "Election",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ballot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BallotGuid = table.Column<Guid>(nullable: false),
                    BallotNumAtComputer = table.Column<int>(nullable: false),
                    C_BallotCode = table.Column<string>(nullable: true),
                    C_RowVersion = table.Column<byte[]>(nullable: true),
                    ComputerCode = table.Column<string>(nullable: true),
                    LocationGuid = table.Column<Guid>(nullable: false),
                    StatusCode = table.Column<string>(nullable: true),
                    Teller1 = table.Column<string>(nullable: true),
                    Teller2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ballot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JoinElectionUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    Role = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    UsersId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinElectionUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinElectionUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AsOf = table.Column<DateTime>(nullable: false),
                    ComputerCode = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    LocationGuid = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgeGroup = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    BahaiId = table.Column<string>(nullable: true),
                    C_FullName = table.Column<string>(nullable: true),
                    C_FullNameFL = table.Column<string>(nullable: true),
                    C_RowVersion = table.Column<byte[]>(nullable: true),
                    C_RowVersionInt = table.Column<long>(nullable: true),
                    CanReceiveVotes = table.Column<bool>(nullable: true),
                    CanVote = table.Column<bool>(nullable: true),
                    CombinedInfo = table.Column<string>(nullable: true),
                    CombinedInfoAtStart = table.Column<string>(nullable: true),
                    CombinedSoundCodes = table.Column<string>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    EnvNum = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    IneligibleReasonGuid = table.Column<Guid>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    OtherInfo = table.Column<string>(nullable: true),
                    OtherLastNames = table.Column<string>(nullable: true),
                    OtherNames = table.Column<string>(nullable: true),
                    PersonGuid = table.Column<Guid>(nullable: false),
                    RegistrationLog = table.Column<string>(nullable: true),
                    RegistrationTime = table.Column<DateTime>(nullable: true),
                    Teller1 = table.Column<string>(nullable: true),
                    Teller2 = table.Column<string>(nullable: true),
                    VotingLocationGuid = table.Column<Guid>(nullable: true),
                    VotingMethod = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CloseToNext = table.Column<bool>(nullable: true),
                    CloseToPrev = table.Column<bool>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    ForceShowInOther = table.Column<bool>(nullable: true),
                    IsTieResolved = table.Column<bool>(nullable: true),
                    IsTied = table.Column<bool>(nullable: true),
                    PersonGuid = table.Column<Guid>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    RankInExtra = table.Column<int>(nullable: true),
                    Section = table.Column<string>(nullable: true),
                    TieBreakCount = table.Column<int>(nullable: true),
                    TieBreakGroup = table.Column<int>(nullable: true),
                    TieBreakRequired = table.Column<bool>(nullable: true),
                    VoteCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BallotsNeedingReview = table.Column<int>(nullable: true),
                    BallotsReceived = table.Column<int>(nullable: true),
                    CalledInBallots = table.Column<int>(nullable: true),
                    DroppedOffBallots = table.Column<int>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    InPersonBallots = table.Column<int>(nullable: true),
                    MailedInBallots = table.Column<int>(nullable: true),
                    NumEligibleToVote = table.Column<int>(nullable: true),
                    NumVoters = table.Column<int>(nullable: true),
                    ResultType = table.Column<string>(nullable: true),
                    SpoiledBallots = table.Column<int>(nullable: true),
                    SpoiledManualBallots = table.Column<int>(nullable: true),
                    SpoiledVotes = table.Column<int>(nullable: true),
                    TotalVotes = table.Column<int>(nullable: true),
                    UseOnReports = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultTie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    IsResolved = table.Column<bool>(nullable: true),
                    NumInTie = table.Column<int>(nullable: false),
                    NumToElect = table.Column<int>(nullable: false),
                    TieBreakGroup = table.Column<int>(nullable: false),
                    TieBreakRequired = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultTie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teller",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    C_RowVersion = table.Column<byte[]>(nullable: true),
                    ElectionGuid = table.Column<Guid>(nullable: false),
                    IsHeadTeller = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UsingComputerCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BallotGuid = table.Column<Guid>(nullable: false),
                    C_RowVersion = table.Column<byte[]>(nullable: true),
                    InvalidReasonGuid = table.Column<Guid>(nullable: true),
                    PersonCombinedInfo = table.Column<string>(nullable: true),
                    PersonGuid = table.Column<Guid>(nullable: true),
                    PositionOnBallot = table.Column<int>(nullable: false),
                    SingleNameElectionCount = table.Column<int>(nullable: true),
                    StatusCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Election_ElectionGuid",
                table: "Election",
                column: "ElectionGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JoinElectionUser_UsersId",
                table: "JoinElectionUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ballot");

            migrationBuilder.DropTable(
                name: "JoinElectionUser");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "ResultSummary");

            migrationBuilder.DropTable(
                name: "ResultTie");

            migrationBuilder.DropTable(
                name: "Teller");

            migrationBuilder.DropTable(
                name: "Vote");

            migrationBuilder.DropIndex(
                name: "IX_Election_ElectionGuid",
                table: "Election");

            migrationBuilder.DropColumn(
                name: "BallotProcess",
                table: "Election");

            migrationBuilder.DropColumn(
                name: "EnvNumMode",
                table: "Election");

            migrationBuilder.DropColumn(
                name: "ExtraFuture1",
                table: "Election");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OnlineTempBallots",
                newName: "OnlineTempBallotId");

            migrationBuilder.RenameColumn(
                name: "ExtraFuture3",
                table: "Election",
                newName: "EnvNumModeRaw");

            migrationBuilder.RenameColumn(
                name: "ExtraFuture2",
                table: "Election",
                newName: "BallotProcessRaw");

            migrationBuilder.AlterColumn<bool>(
                name: "T24",
                table: "Election",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
