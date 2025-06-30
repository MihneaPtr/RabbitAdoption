using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RabbitAdoption.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYearsRabbitExperienceToAdoptionRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Preferences_Size",
                table: "AdoptionRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsRabbitExperience",
                table: "AdoptionRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preferences_Size",
                table: "AdoptionRequests");

            migrationBuilder.DropColumn(
                name: "YearsRabbitExperience",
                table: "AdoptionRequests");
        }
    }
}
