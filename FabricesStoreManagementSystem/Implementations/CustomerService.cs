namespace FabricesStoreManagementSystem.Implementations;

public class CustomerService(AppDbContext appDbContext) : ICustomerService
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<CustomerResponse>> CreateCustomer
        (CustomerRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Email is not null &&
            await _appDbContext.Customers.AnyAsync(x => x.Email != null &&
                x.Email == request.Email, cancellationToken))
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictEmail);

        if (request.Phone is not null &&
            await _appDbContext.Customers.AnyAsync(x => x.Phone != null &&
                x.Phone == request.Phone, cancellationToken))
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictPhone);

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
        };
        await _appDbContext.Customers.AddAsync(customer, cancellationToken);
        return Result.Success(customer.ToCustomerResponse());
    }

    public async Task<Result<CustomerResponse>> GetCustomer
        (Guid id, bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Customers.FindAsync(id, cancellationToken) is not { } customer)
            return Result.Failure<CustomerResponse>(CustomerErrors.NotFound);
        if(includeOnlyActive && !customer.IsActive)
            return Result.Failure<CustomerResponse>(CustomerErrors.NotFound);
        return Result.Success(customer.ToCustomerResponse());
    }

    public async Task<Result<PaginatedList<CustomerResponse>>> GetCustomers
        (PaginationRequest paginationRequest, SortRequest sortRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Customers.AsNoTracking();
        if (includeOnlyActive)
            query = query.Where(x => x.IsActive);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(CustomerSorts.CustomerResponseSort(sortRequest));
        else
            query = query.OrderBy(CustomerSorts.CustomerResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToCustomerResponse());

        var response = await PaginatedList<CustomerResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> UpdateCustomer
        (Guid id, CustomerRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Id == id && x.IsActive, cancellationToken)))
            return Result.Failure<List<SaleResponse>>(CustomerErrors.NotFound);

        if (request.Email is not null &&
            await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Email != null &&
                x.Email == request.Email && x.Id != id, cancellationToken))
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictEmail);

        if (request.Phone is not null &&
            await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Phone != null &&
                x.Phone == request.Phone && x.Id != id, cancellationToken))
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictPhone);

        await _appDbContext.Customers.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.FirstName, request.FirstName)
                    .SetProperty(x => x.LastName, request.LastName)
                    .SetProperty(x => x.Email, request.Email)
                    .SetProperty(x => x.Phone, request.Phone)
                    .SetProperty(x => x.Address, request.Address)
                    ,
                    cancellationToken
            );

        return Result.Success();
    }

    public async Task<Result<PaginatedList<SaleResponse>>> GetSalesByCustomer
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<SaleResponse>>(CustomerErrors.NotFound);

        var query = _appDbContext.Sales.AsNoTracking()
            .Where(x => x.CustomerID == id);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(SaleSorts.SaleResponseSort(sortRequest));
        else
            query = query.OrderBy(SaleSorts.SaleResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToSaleResponseWithNoItems());

        var response = await PaginatedList<SaleResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> ToggleCustomerStatus
        (Guid id, bool? state, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Customers.FindAsync(id, cancellationToken) is not { } customer)
            return Result.Failure<List<SaleResponse>>(CustomerErrors.NotFound);

        if (state.HasValue && customer.IsActive == state)
            return Result.Success();

        await _appDbContext.Customers.Where(x => x.Id == customer.Id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsActive, state.HasValue ? state : !customer.IsActive),
                    cancellationToken
            );

        return Result.Success();
    }
}

