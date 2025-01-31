using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Natia.Core.Migrations
{
    /// <inheritdoc />
    public partial class mgrt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chanells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChanellFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port_In_250 = table.Column<int>(type: "int", nullable: false),
                    Is_From_Optic = table.Column<bool>(type: "bit", nullable: false),
                    Name_Of_Chanell = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameForSPeak = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chanells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emr100info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceEmr = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emr100info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emr110info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceEmr = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emr110info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emr120info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceEmr = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emr120info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emr130info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceEmr = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emr130info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emr200info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceEmr = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emr200info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "emr60info",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceEmr = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emr60info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Greetings",
                columns: table => new
                {
                    GreetingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Greetings", x => x.GreetingId);
                });

            migrationBuilder.CreateTable(
                name: "neurals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhatNatiaSaid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsError = table.Column<bool>(type: "bit", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    WhatWasTopic = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Satellite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuggestedSolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_neurals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Desclamblers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emr_Number = table.Column<int>(type: "int", nullable: false),
                    Card_In_Desclambler = table.Column<int>(type: "int", nullable: false),
                    Port_In_Desclambler = table.Column<int>(type: "int", nullable: false),
                    Chanell_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desclamblers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desclamblers_Chanells_Chanell_Id",
                        column: x => x.Chanell_Id,
                        principalTable: "Chanells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Infos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alarm_For_Display = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHanell_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Infos_Chanells_CHanell_Id",
                        column: x => x.CHanell_Id,
                        principalTable: "Chanells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recievers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emr_Number = table.Column<int>(type: "int", nullable: false),
                    Card_In_Reciever = table.Column<int>(type: "int", nullable: false),
                    Port_In_Reciever = table.Column<int>(type: "int", nullable: false),
                    Chanell_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recievers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recievers_Chanells_Chanell_Id",
                        column: x => x.Chanell_Id,
                        principalTable: "Chanells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transcoders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emr_Number = table.Column<int>(type: "int", nullable: false),
                    Card_In_Transcoder = table.Column<int>(type: "int", nullable: false),
                    Port_In_Transcoder = table.Column<int>(type: "int", nullable: false),
                    Chanell_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transcoders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transcoders_Chanells_Chanell_Id",
                        column: x => x.Chanell_Id,
                        principalTable: "Chanells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Desclamblers_Chanell_Id",
                table: "Desclamblers",
                column: "Chanell_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Infos_CHanell_Id",
                table: "Infos",
                column: "CHanell_Id");

            migrationBuilder.CreateIndex(
                name: "IX_recievers_Chanell_Id",
                table: "recievers",
                column: "Chanell_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transcoders_Chanell_Id",
                table: "Transcoders",
                column: "Chanell_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Desclamblers");

            migrationBuilder.DropTable(
                name: "emr100info");

            migrationBuilder.DropTable(
                name: "emr110info");

            migrationBuilder.DropTable(
                name: "emr120info");

            migrationBuilder.DropTable(
                name: "emr130info");

            migrationBuilder.DropTable(
                name: "emr200info");

            migrationBuilder.DropTable(
                name: "emr60info");

            migrationBuilder.DropTable(
                name: "Greetings");

            migrationBuilder.DropTable(
                name: "Infos");

            migrationBuilder.DropTable(
                name: "neurals");

            migrationBuilder.DropTable(
                name: "recievers");

            migrationBuilder.DropTable(
                name: "Transcoders");

            migrationBuilder.DropTable(
                name: "Chanells");
        }
    }
}
