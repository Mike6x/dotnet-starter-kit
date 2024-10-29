using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.MSSQL.Elearning
{
    /// <inheritdoc />
    public partial class FixQuizRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizs_Dimension_QuizModeId",
                schema: "elearning",
                table: "Quizs");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizs_Dimension_QuizTopicId",
                schema: "elearning",
                table: "Quizs");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizs_Dimension_QuizTypeId",
                schema: "elearning",
                table: "Quizs");

            migrationBuilder.DropTable(
                name: "Dimension",
                schema: "elearning");

            migrationBuilder.DropIndex(
                name: "IX_Quizs_QuizModeId",
                schema: "elearning",
                table: "Quizs");

            migrationBuilder.DropIndex(
                name: "IX_Quizs_QuizTopicId",
                schema: "elearning",
                table: "Quizs");

            migrationBuilder.DropIndex(
                name: "IX_Quizs_QuizTypeId",
                schema: "elearning",
                table: "Quizs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dimension",
                schema: "elearning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FatherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullNativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Dimension_FatherId",
                schema: "elearning",
                table: "Dimension",
                column: "FatherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizs_Dimension_QuizModeId",
                schema: "elearning",
                table: "Quizs",
                column: "QuizModeId",
                principalSchema: "elearning",
                principalTable: "Dimension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizs_Dimension_QuizTopicId",
                schema: "elearning",
                table: "Quizs",
                column: "QuizTopicId",
                principalSchema: "elearning",
                principalTable: "Dimension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizs_Dimension_QuizTypeId",
                schema: "elearning",
                table: "Quizs",
                column: "QuizTypeId",
                principalSchema: "elearning",
                principalTable: "Dimension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
