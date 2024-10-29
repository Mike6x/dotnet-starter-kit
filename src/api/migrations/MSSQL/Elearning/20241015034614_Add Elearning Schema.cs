using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.MSSQL.Elearning
{
    /// <inheritdoc />
    public partial class AddElearningSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "elearning");

            migrationBuilder.CreateTable(
                name: "Dimension",
                schema: "elearning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullNativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dimension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dimension_Dimension_FatherId",
                        column: x => x.FatherId,
                        principalSchema: "elearning",
                        principalTable: "Dimension",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quizs",
                schema: "elearning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    QuizUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuizTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizTopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizModeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Sale = table.Column<int>(type: "int", nullable: true),
                    RatingCount = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizs_Dimension_QuizModeId",
                        column: x => x.QuizModeId,
                        principalSchema: "elearning",
                        principalTable: "Dimension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quizs_Dimension_QuizTopicId",
                        column: x => x.QuizTopicId,
                        principalSchema: "elearning",
                        principalTable: "Dimension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Quizs_Dimension_QuizTypeId",
                        column: x => x.QuizTypeId,
                        principalSchema: "elearning",
                        principalTable: "Dimension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "QuizResults",
                schema: "elearning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ut = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Ps = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Psp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tl = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    V = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    T = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPass = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizResults_Quizs_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "elearning",
                        principalTable: "Quizs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dimension_FatherId",
                schema: "elearning",
                table: "Dimension",
                column: "FatherId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_QuizId",
                schema: "elearning",
                table: "QuizResults",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizs_QuizModeId",
                schema: "elearning",
                table: "Quizs",
                column: "QuizModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizs_QuizTopicId",
                schema: "elearning",
                table: "Quizs",
                column: "QuizTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizs_QuizTypeId",
                schema: "elearning",
                table: "Quizs",
                column: "QuizTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizResults",
                schema: "elearning");

            migrationBuilder.DropTable(
                name: "Quizs",
                schema: "elearning");

            migrationBuilder.DropTable(
                name: "Dimension",
                schema: "elearning");
        }
    }
}
