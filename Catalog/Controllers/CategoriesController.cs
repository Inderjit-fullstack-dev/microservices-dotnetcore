using Catalog.Database;
using Catalog.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _context.Categories.ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(long id)
        {
            var result = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(long id, Category category)
        {
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (categoryInDb == null)
                return NotFound("Data not found.");

            categoryInDb.CategoryName = category.CategoryName;
            await _context.SaveChangesAsync();

            return Ok(categoryInDb);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (categoryInDb == null)
                return NotFound("Data not found.");

            _context.Categories.Remove(categoryInDb);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
    }
}
