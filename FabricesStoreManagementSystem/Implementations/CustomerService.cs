namespace FabricesStoreManagementSystem.Implementations;

public class CustomerService(AppDbContext appDbContext, ILogger<CustomerService> logger) : ICustomerService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<CustomerService> _logger = logger;

    public async Task<Result<CustomerResponse>> CreateCustomer
        (CustomerRequest request, CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("check for customer email");
        if (request.Email is not null &&
            await _appDbContext.Customers.AnyAsync(x => x.Email != null &&
                x.Email == request.Email, cancellationToken))
        {
            _logger.LogError("email conflict");
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictEmail);
        }

        _logger.LogInformation("check for customer phone");
        if (request.Phone is not null &&
            await _appDbContext.Customers.AnyAsync(x => x.Phone != null &&
                x.Phone == request.Phone, cancellationToken))
        {
            _logger.LogError("phone conflict");
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictPhone);
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
        };
        await _appDbContext.Customers.AddAsync(customer, cancellationToken);
        _logger.LogInformation("customer was added with id({id})", customer.Id);
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
        (PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest searchRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Customers.AsNoTracking();
        if (includeOnlyActive)
            query = query.Where(x => x.IsActive);

        if (searchRequest is not null && searchRequest.Search is not null)
            query = query.CustomerResponseSearch(searchRequest);

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(CustomerSorts.CustomerResponseSort(sortRequest));
        else
            query = query.OrderByDescending(CustomerSorts.CustomerResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToCustomerResponse());

        var response = await PaginatedList<CustomerResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> UpdateCustomer
        (Guid id, CustomerRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for customer existance");
        if (await _appDbContext.Customers.SingleOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken) is not { } customer)
        {
            _logger.LogError("customer({id}) not found", id);
            return Result.Failure<List<SaleResponse>>(CustomerErrors.NotFound);
        }

        _logger.LogInformation("check for customer email");
        if (request.Email is not null &&
            await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Email != null &&
                x.Email == request.Email && x.Id != id, cancellationToken))
        {
            _logger.LogError("email conflict");
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictEmail);
        }

        _logger.LogInformation("check for customer phone");
        if (request.Phone is not null &&
            await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Phone != null &&
                x.Phone == request.Phone && x.Id != id, cancellationToken))
        {
            _logger.LogError("phone conflict");
            return Result.Failure<CustomerResponse>(CustomerErrors.ConflictPhone);
        }

        _logger.LogInformation("start updating customer({id})", id);

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
        _appDbContext.Customers.Update(customer);
        _logger.LogInformation("customer was updateded with id({id})", id);
        return Result.Success();
    }

    public async Task<Result<PaginatedList<SaleResponse>>> GetSalesByCustomer
        (Guid id, PaginationRequest paginationRequest, SearchInvoiceNumberRequest invoiceNumberRequest, DateRangeRequest dateRangeRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<SaleResponse>>(CustomerErrors.NotFound);

        var query = _appDbContext.Sales.AsNoTracking()
            .Where(x => x.CustomerID == id);

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.CreatedAt >= utcFrom && x.CreatedAt <= utcTo);
        }

        if (!string.IsNullOrWhiteSpace(invoiceNumberRequest.InvoiceNumber))
            query = query.Where(x => x.InvoiceNumber.StartsWith(invoiceNumberRequest.InvoiceNumber));

        query = query.OrderByDescending(x => x.CreatedAt);

        var result = query
            .Select(x => x.ToSaleResponseWithNoItems());

        var response = await PaginatedList<SaleResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> ToggleCustomerStatus
        (Guid id, bool? state, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for customer existance");
        if (await _appDbContext.Customers.FindAsync(id, cancellationToken) is not { } customer)
        {
            _logger.LogError("customer not found");
            return Result.Failure<List<SaleResponse>>(CustomerErrors.NotFound);
        }

        _logger.LogInformation("customer id({id}), check if state request is same of customer state", id);
        if (state.HasValue && customer.IsActive == state)
            return Result.Success();

        _logger.LogInformation("start updating customer with id({id})", id);
        customer.IsActive = state.HasValue ? (bool)state : !customer.IsActive;
        _appDbContext.Customers.Update(customer);
        _logger.LogInformation("customer with id({id}) state updated to {state}",id, !customer.IsActive);
        return Result.Success();
    }

    public async Task<Result<List<CustomerResponse>>> GetCustomerForBill
        (CustomerSearchForBillRequest request, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Customers.AsNoTracking()
                .Where(x => x.IsActive &&
                    EF.Functions.Like(x.FirstName + " " + x.LastName, $"%{request.Name}%"));

        var result = query
            .Select(x => x.ToCustomerResponse())
            .ToListAsync(cancellationToken);

        return Result.Success(await result);

    }

    public async Task<Result<PaginatedList<AssignCatalogResponse>>> GetCustomerCatalogs
        (Guid id, PaginationRequest paginationRequest, SearchCatalogByCodeRequest searchCatalogByCodeRequest, DateRangeRequest dateRangeRequest, bool includeReturned = false, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<AssignCatalogResponse>>(CustomerErrors.NotFound);

        var query = _appDbContext.CatalogsAssigns.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Catalog)
            .Where(x => x.CustomerID == id);

        if (!includeReturned)
            query = query.Where(x => !x.ReturnedAt.HasValue);

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.AssignedAt >= utcFrom && x.AssignedAt <= utcTo);
        }

        if (!string.IsNullOrWhiteSpace(searchCatalogByCodeRequest.Code))
            query = query.Where(x => x.Catalog.CatalogCode.StartsWith(searchCatalogByCodeRequest.Code));

        query = query.OrderByDescending(x => x.AssignedAt);

        var result = query
            .Select(x => x.ToAssignCatalogResponse());

        var response = await PaginatedList<AssignCatalogResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }
}

