namespace FabricesStoreManagementSystem.Implementations;

public class SupplierService(AppDbContext appDbContext, ILogger<SupplierService> logger) : ISupplierService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<SupplierService> _logger = logger;

    public async Task<Result<SupplierResponse>> CreateSupplier
        (SupplierRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for supplier email");
        if (request.Email is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Email != null && x.Email != request.Email, cancellationToken)
            )
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictEmail);

        _logger.LogInformation("check for supplier phone");
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

        _logger.LogInformation("supplier was added with id({id})", supplier.Id);

        return Result.Success(supplier.ToSupplierResponse());
    }

    public async Task<Result<PaginatedList<PurchaseResponse>>> GetPurchasesBySupplier
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest? searchRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<PurchaseResponse>>(SupplierErrors.NotFound);

        var query = _appDbContext.Purchases.AsNoTracking()
            .Where(x => x.SupplierID == id);

        if (searchRequest is not null)
            query = query
                .Where(x => PurchaseSearchs.PurchaseResponseSearch(searchRequest).ToString().ToLower().Contains(searchRequest.Search));

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(PurchaseSorts.PurchaseResponseSort(sortRequest));
        else
            query = query.OrderByDescending(PurchaseSorts.PurchaseResponseSort(sortRequest));

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
        (PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest? searchRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Suppliers.AsNoTracking();
        if (includeOnlyActive)
            query = query.Where(x => x.IsActive);

        if (searchRequest is not null)
            query = query
                .Where(x => SupplierSearchs.SupplierResponseSearch(searchRequest).ToString().ToLower().Contains(searchRequest.Search));

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(SupplierSorts.SupplierResponseSort(sortRequest));
        else
            query = query.OrderByDescending(SupplierSorts.SupplierResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToSupplierResponse());

        var response = await PaginatedList<SupplierResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> ToggleSupplierStatus
        (Guid id, bool? state, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for supplier existance");
        if (await _appDbContext.Suppliers.FindAsync(id, cancellationToken) is not { } supplier)
        {
            _logger.LogError("not found supplier({id})", id);
            return Result.Failure(SupplierErrors.NotFound);
        }

        _logger.LogInformation("supplier with id({id}), check if state of request is same supplier state", id);
        if (state.HasValue && supplier.IsActive == state)
            return Result.Success();

        _logger.LogInformation("starrt updating supplier with id({id})", supplier.Id);
        await _appDbContext.Suppliers.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsActive, state.HasValue ? state : !supplier.IsActive),
                cancellationToken
            );
        _logger.LogInformation("supplier with id({id}) state updated to {state}", id, !supplier.IsActive);
        return Result.Success();
    }

    public async Task<Result> UpdateSupplier
        (Guid id, SupplierRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for supplier({id}) existance", id);
        if (!(await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x => x.Id == id && x.IsActive, cancellationToken)))
            return Result.Failure(SupplierErrors.NotFound);

        _logger.LogInformation("check for supplier email");
        if (request.Email is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Email != null && x.Email != request.Email && x.Id != id, cancellationToken)
            )
        {
            _logger.LogError("email conflict");
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictEmail);
        }

        _logger.LogInformation("check for supplier phone");
        if (request.Phone is not null &&
            await _appDbContext.Suppliers.AsNoTracking().AnyAsync(x =>
            x.Phone != null && x.Phone != request.Phone && x.Id != id, cancellationToken)
            )
        {
            _logger.LogError("phone conflict");
            return Result.Failure<SupplierResponse>(SupplierErrors.ConflictPhone);
        }

        _logger.LogInformation("starrt updating supplier with id({id})", id);
        await _appDbContext.Suppliers.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.Name, request.Name)
                    .SetProperty(x => x.Email, request.Email)
                    .SetProperty(x => x.Phone, request.Phone)
                    .SetProperty(x => x.Address, request.Address),
                    cancellationToken
            );
        _logger.LogInformation("supplier was updated with id({id})", id);
        return Result.Success();
    }
}
