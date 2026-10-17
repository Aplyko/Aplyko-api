namespace AplykoApi.Services;

internal abstract class GlobalService
{
    protected readonly AppDbContext _context;

    protected GlobalService(AppDbContext context)
    {
        _context = context;
    }
}
