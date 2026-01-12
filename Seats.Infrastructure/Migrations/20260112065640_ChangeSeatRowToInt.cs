using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seats.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSeatRowToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM seats WHERE \"Row\" !~ '^[0-9]+$'");
            migrationBuilder.Sql("ALTER TABLE seats ALTER COLUMN \"Row\" TYPE integer USING \"Row\"::integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Row",
                table: "seats",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
