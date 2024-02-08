using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using iRecipe.Data;
using iRecipe.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace iRecipe.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;

        public RecipesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;

            _client = _httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Recipe.OrderByDescending(e => e.Id).ToListAsync());
        }

        // GET: Recipes/Search
        public async Task<IActionResult> Search()
        {
            return View(await _context.Recipe.ToListAsync());
        }

        // POST: Recipes/SearchResult
        public async Task<IActionResult> SearchResult(string SearchQuery)
        {

            // Send a GET request to the MealDB API with the search query
            HttpResponseMessage response = await _client.GetAsync($"search.php?s={SearchQuery}");

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into an object
                JObject result = JObject.Parse(responseBody);
                ViewBag.SearchQuery = SearchQuery;

                // Pass the result to the view

                return View("SearchResult", result);
            }
            else
            {
                // If the request was not successful, return an error view or handle the error accordingly
                return View("Error");
            }
        }


        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            HttpResponseMessage response = await _client.GetAsync($"lookup.php?i={id}");

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into an object
                JObject result = JObject.Parse(responseBody);

                // Pass the result to the view

                return View("Details", result);
            }
            else
            {
                // If the request was not successful, return an error view or handle the error accordingly
                return View("Error");
            }
        }


        // GET: Recipes/Add/5
        public async Task<IActionResult> Add(int? id)
        {
           
            HttpResponseMessage response = await _client.GetAsync($"lookup.php?i={id}");

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into an object
                JObject result = JObject.Parse(responseBody);

                // Create an instance of your modal and assign the mapped data
                var mealModal = new Recipe
                {
                    RecipeId = int.Parse(result["meals"][0]["idMeal"].ToString()),
                    RecipeName = result["meals"][0]["strMeal"].ToString(),
                    RecipeArea = result["meals"][0]["strArea"].ToString(),
                    RecipeCategory = result["meals"][0]["strCategory"].ToString(),
                };

                bool isValueExists = _context.Recipe.Any(e => e.RecipeId == mealModal.RecipeId);

                if (!isValueExists)
                {
                    _context.Recipe.Add(mealModal);

                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipe.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipe.Any(e => e.Id == id);
        }
    }
}
