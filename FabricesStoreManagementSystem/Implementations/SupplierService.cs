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

    public async Task<Result<PaginatedList<PurchaseResponse>>> GetPurchaseBySupplier
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<PurchaseResponse>>(SupplierErrors.NotFound);

        var query = _appDbContext.Purchases.AsNoTracking()
            .Where(x => x.SupplierID == id);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(PurchaseSorts.PurchaseResponseSort(sortRequest));
        else
            query = query.OrderBy(PurchaseSorts.PurchaseResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToPurchaseResponse());

        var response = await PaginatedList<PurchaseResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
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

    public async Task<Result<PaginatedList<SupplierResponse>>> GetSuppliers
        (PaginationRequest paginationRequest, SortRequest sortRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Suppliers.AsNoTracking();
        if (includeOnlyActive)
            query = query.Where(x => x.IsActive);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(SupplierSorts.SupplierResponseSort(sortRequest));
        else
            query = query.OrderBy(SupplierSorts.SupplierResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToSupplierResponse());

        var response = await PaginatedList<SupplierResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
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
        if (!(await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id && x.IsActive, cancellationToken)))
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
