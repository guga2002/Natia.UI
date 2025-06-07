using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Natia.Core.Migrations
{
    /// <inheritdoc />
    public partial class dfggh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Satellite",
                table: "neurals",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "neurals",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Alarm_For_Display",
                table: "Infos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Greetings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Greetings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name_Of_Chanell",
                table: "Chanells",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NameForSPeak",
                table: "Chanells",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ChanellFormat",
                table: "Chanells",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Transcoders_Card_In_Transcoder",
                table: "Transcoders",
                column: "Card_In_Transcoder");

            migrationBuilder.CreateIndex(
                name: "IX_Transcoders_Emr_Number",
                table: "Transcoders",
                column: "Emr_Number");

            migrationBuilder.CreateIndex(
                name: "IX_Transcoders_Port_In_Transcoder",
                table: "Transcoders",
                column: "Port_In_Transcoder");

            migrationBuilder.CreateIndex(
                name: "IX_recievers_Card_In_Reciever",
                table: "recievers",
                column: "Card_In_Reciever");

            migrationBuilder.CreateIndex(
                name: "IX_recievers_Emr_Number",
                table: "recievers",
                column: "Emr_Number");

            migrationBuilder.CreateIndex(
                name: "IX_recievers_Port_In_Reciever",
                table: "recievers",
                column: "Port_In_Reciever");

            migrationBuilder.CreateIndex(
                name: "IX_neurals_ChannelName",
                table: "neurals",
                column: "ChannelName");

            migrationBuilder.CreateIndex(
                name: "IX_neurals_IsCritical",
                table: "neurals",
                column: "IsCritical");

            migrationBuilder.CreateIndex(
                name: "IX_neurals_IsError",
                table: "neurals",
                column: "IsError");

            migrationBuilder.CreateIndex(
                name: "IX_neurals_Satellite",
                table: "neurals",
                column: "Satellite");

            migrationBuilder.CreateIndex(
                name: "IX_Infos_Alarm_For_Display",
                table: "Infos",
                column: "Alarm_For_Display");

            migrationBuilder.CreateIndex(
                name: "IX_Greetings_Category",
                table: "Greetings",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Greetings_Text",
                table: "Greetings",
                column: "Text");

            migrationBuilder.CreateIndex(
                name: "IX_Desclamblers_Card_In_Desclambler",
                table: "Desclamblers",
                column: "Card_In_Desclambler");

            migrationBuilder.CreateIndex(
                name: "IX_Desclamblers_Emr_Number",
                table: "Desclamblers",
                column: "Emr_Number");

            migrationBuilder.CreateIndex(
                name: "IX_Desclamblers_Port_In_Desclambler",
                table: "Desclamblers",
                column: "Port_In_Desclambler");

            migrationBuilder.CreateIndex(
                name: "IX_Chanells_ChanellFormat",
                table: "Chanells",
                column: "ChanellFormat");

            migrationBuilder.CreateIndex(
                name: "IX_Chanells_Is_From_Optic",
                table: "Chanells",
                column: "Is_From_Optic");

            migrationBuilder.CreateIndex(
                name: "IX_Chanells_Name_Of_Chanell",
                table: "Chanells",
                column: "Name_Of_Chanell");

            migrationBuilder.CreateIndex(
                name: "IX_Chanells_Port_In_250",
                table: "Chanells",
                column: "Port_In_250");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transcoders_Card_In_Transcoder",
                table: "Transcoders");

            migrationBuilder.DropIndex(
                name: "IX_Transcoders_Emr_Number",
                table: "Transcoders");

            migrationBuilder.DropIndex(
                name: "IX_Transcoders_Port_In_Transcoder",
                table: "Transcoders");

            migrationBuilder.DropIndex(
                name: "IX_recievers_Card_In_Reciever",
                table: "recievers");

            migrationBuilder.DropIndex(
                name: "IX_recievers_Emr_Number",
                table: "recievers");

            migrationBuilder.DropIndex(
                name: "IX_recievers_Port_In_Reciever",
                table: "recievers");

            migrationBuilder.DropIndex(
                name: "IX_neurals_ChannelName",
                table: "neurals");

            migrationBuilder.DropIndex(
                name: "IX_neurals_IsCritical",
                table: "neurals");

            migrationBuilder.DropIndex(
                name: "IX_neurals_IsError",
                table: "neurals");

            migrationBuilder.DropIndex(
                name: "IX_neurals_Satellite",
                table: "neurals");

            migrationBuilder.DropIndex(
                name: "IX_Infos_Alarm_For_Display",
                table: "Infos");

            migrationBuilder.DropIndex(
                name: "IX_Greetings_Category",
                table: "Greetings");

            migrationBuilder.DropIndex(
                name: "IX_Greetings_Text",
                table: "Greetings");

            migrationBuilder.DropIndex(
                name: "IX_Desclamblers_Card_In_Desclambler",
                table: "Desclamblers");

            migrationBuilder.DropIndex(
                name: "IX_Desclamblers_Emr_Number",
                table: "Desclamblers");

            migrationBuilder.DropIndex(
                name: "IX_Desclamblers_Port_In_Desclambler",
                table: "Desclamblers");

            migrationBuilder.DropIndex(
                name: "IX_Chanells_ChanellFormat",
                table: "Chanells");

            migrationBuilder.DropIndex(
                name: "IX_Chanells_Is_From_Optic",
                table: "Chanells");

            migrationBuilder.DropIndex(
                name: "IX_Chanells_Name_Of_Chanell",
                table: "Chanells");

            migrationBuilder.DropIndex(
                name: "IX_Chanells_Port_In_250",
                table: "Chanells");

            migrationBuilder.AlterColumn<string>(
                name: "Satellite",
                table: "neurals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChannelName",
                table: "neurals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Alarm_For_Display",
                table: "Infos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Greetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Greetings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name_Of_Chanell",
                table: "Chanells",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameForSPeak",
                table: "Chanells",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChanellFormat",
                table: "Chanells",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
