namespace iRecipe.Models
{
    public class Recipe
    {

        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string RecipeArea { get; set; }
        public string RecipeCategory { get; set;}

        public Recipe()
        {
            
        }
    }
}
