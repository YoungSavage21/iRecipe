using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iRecipe.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipeTag",
                table: "Recipe",
                newName: "RecipeArea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipeArea",
                table: "Recipe",
                newName: "RecipeTag");
        }
    }
}
