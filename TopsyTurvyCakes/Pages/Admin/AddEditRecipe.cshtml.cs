using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopsyTurvyCakes.Models;

namespace TopsyTurvyCakes.Pages.Admin
{
    public class AddEditRecipeModel : PageModel
    {
        private readonly IRecipesService recipeService;

        [FromRoute]
        public long? Id { get; set; }

        public bool IsNewRecipe
        {
            get { return Id == null; }
        }

        [BindProperty]
        public Recipe Recipe { get; set; }

        [BindProperty]
        public IFormFile Image { get; set; }

        public AddEditRecipeModel(IRecipesService recipesService)
        {
            this.recipeService = recipesService;
        }

        public async Task OnGetAsync()
        {
            Recipe = await recipeService.FindAsync(Id.GetValueOrDefault())
                ?? new Recipe();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var recipe = Recipe = await recipeService.FindAsync(Id.GetValueOrDefault())
                ?? new Recipe();

            recipe.Name = Recipe.Name;
            recipe.Description = Recipe.Description;
            recipe.Ingredients = Recipe.Description;
            recipe.Directions = Recipe.Ingredients;

            if (Image != null)
            {
                using (var stream = new System.IO.MemoryStream())
                {
                    await Image.CopyToAsync(stream);

                    recipe.Image = stream.ToArray();
                    recipe.ImageContentType = Image.ContentType;
                }
            }

            await recipeService.SaveAsync(recipe);
            return RedirectToPage("/Recipe", new { recipe.Id });
        }

        public async Task<IActionResult> OnPostDelete()
        {
            await recipeService.DeleteAsync(Id.Value);
            return RedirectToPage("/Index");
        }
    }
}
