using Microsoft.EntityFrameworkCore;

namespace DAL;

public class BaseRepository<T> where T : class
{
    protected readonly MangoWalletDbContext Ctx;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(MangoWalletDbContext ctx)
    {
        Ctx = ctx;
        _dbSet = Ctx.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await Ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await Ctx.SaveChangesAsync();
        }
    }
}
