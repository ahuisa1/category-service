using CategoryService.Data;
using CategoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(string id)
    {
        // En Cosmos, WithPartitionKey optimiza la búsqueda
        return await _context.Categories
                             .WithPartitionKey(id)
                             .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(string id, Category category)
    {
        var existingCategory = await GetByIdAsync(id);
        if (existingCategory == null) return null;

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;
        existingCategory.IsActive = category.IsActive;

        _context.Categories.Update(existingCategory);
        await _context.SaveChangesAsync();

        return existingCategory;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var category = await GetByIdAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}