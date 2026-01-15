namespace FabricesStoreManagementSystem.Implementations;

public class SupplierService(AppDbContext appDbContext) : ISupplierService
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<SupplierResponse>> CreateSupplier
        (SupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Email is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Email != null && x.Email != request.Email, cancellationToken)
            )
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictEmail);

        if (request.Phone is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Phone != null && x.Phone != request.Phone, cancellationToken)
            )
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictPhone);

        var supplier = new Supplier
        {
            Address = request.Address,
            Email = request.Email,
            Phone = request.Phone,
            Name = request.Name
        };
        await _appDbContext.Suppliers.AddAsync(supplier, cancellationToken);
        return Result.Success(supplier.ToSupplierResponse());
    }

    public async Task<Result<List<PurchaseResponse>>> GetPurchaseBySupplier
        (Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<List<PurchaseResponse>>(SupplierErrors.NotFound);

        var result =await  _appDbContext.Purchases.AsNoTracking()
            .Where(x => x.Id == id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToPurchaseResponse())
            .ToListAsync(cancellationToken);

        return Result.Success(result);
    }

    public async Task<Result<SupplierResponse>> GetSupplier
        (Guid id, bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Suppliers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) is not { } supplier)
            return Result.Failure<SupplierResponse>(SupplierErrors.NotFound);
        if(includeOnlyActive && !supplier.IsActive)
            return Result.Failure<SupplierResponse>(SupplierErrors.NotFound);
        return Result.Success(supplier.ToSupplierResponse());
    }

    public async Task<Result<List<SupplierResponse>>> GetSuppliers
        (bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Suppliers.AsNoTracking();
        if (includeOnlyActive)
            query = query.Where(x => x.IsActive);
        query = query.OrderByDescending(x => x.CreatedAt);
        var result = await query
            .Select(x => x.ToSupplierResponse())
            .ToListAsync(cancellationToken);
        return Result.Success(result);
    }

    public async Task<Result> ToggleSupplierStatus
        (Guid id, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Suppliers.FindAsync(id, cancellationToken) is not { } supplier)
            return Result.Failure(SupplierErrors.NotFound);

        await _appDbContext.Suppliers.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsActive, !supplier.IsActive),
                cancellationToken
            );
        return Result.Success();
    }

    public async Task<Result> UpdateSupplier
        (Guid id, SupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)) ||
            (await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id && !x.IsActive, cancellationToken))
        )
            return Result.Failure(SupplierErrors.NotFound);

        if (request.Email is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Email != null && x.Email != request.Email && x.Id != id, cancellationToken)
            )
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictEmail);

        if (request.Phone is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Phone != null && x.Phone != request.Phone && x.Id != id, cancellationToken)
            )
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictPhone);
        await _appDbContext.Suppliers.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.Name, request.Name)
                    .SetProperty(x => x.Email, request.Email)
                    .SetProperty(x => x.Phone, request.Phone)
                    .SetProperty(x => x.Address, request.Address),
                    cancellationToken
            );
        return Result.Success();
    }
}
