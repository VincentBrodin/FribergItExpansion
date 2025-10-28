using System.Linq.Expressions;
using FribergApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergApi.Data;

public class CarRepository(ApiContext context) : GenericRepository<Car>(context)
{
    private readonly ApiContext _context = context;

    public override Task<Car?> GetAsync(Expression<Func<Car, bool>> predicate)
    {
        return _context.Cars.Include(c => c.Updates).Include(c => c.Rentals).FirstOrDefaultAsync(predicate);
    }

    public override async Task<IEnumerable<Car>> FindAsync(Expression<Func<Car, bool>> predicate)
    {
        return await _context.Cars.Include(c => c.Updates).Include(c => c.Rentals).Where(predicate).ToListAsync();
    }

    public override async Task<IEnumerable<Car>> AllAsync()
    {
        return await _context.Cars.Include(c => c.Updates).Include(c => c.Rentals).ToListAsync();
    }
}
