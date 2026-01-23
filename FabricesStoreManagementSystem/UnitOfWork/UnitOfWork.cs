namespace FabricesStoreManagementSystem.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public AppDbContext _appDbContext { get; set; }
    public UnitOfWork(
        AppDbContext appDbContext,
        IOptionsMonitor<AuthOptions> authOptions
    )
    {
        _appDbContext = appDbContext;
        CustomerService = new CustomerService(appDbContext);
        SupplierService = new SupplierService(appDbContext);
        ProductService = new ProductService(appDbContext);
        PurchaseService = new PurchaseService(appDbContext, ProductService);
        SaleService = new SaleService(appDbContext);
        CatalogService = new CatalogService(appDbContext);
    }

    public ICatalogService CatalogService{ get; }

    public ICustomerService CustomerService { get; }

    public ISupplierService SupplierService { get; }

    public IProductService ProductService { get; }

    public IPurchaseService PurchaseService { get; }

    public ISaleService SaleService { get; }


    public int SaveChanges()
        => _appDbContext.SaveChanges();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _appDbContext.SaveChangesAsync(cancellationToken);
}