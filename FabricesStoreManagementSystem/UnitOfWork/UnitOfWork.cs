namespace FabricesStoreManagementSystem.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public AppDbContext _appDbContext { get; set; }
    public UnitOfWork(
        AppDbContext appDbContext,
        IOptionsMonitor<AuthOptions> authOptions,
        ILogger<ProductService> productServiceLogger,
        ILogger<PurchaseService> purchaseServiceLogger,
        ILogger<SaleService> saleServiceLogger,
        ILogger<CustomerService> customerServiceLogger,
        ILogger<SupplierService> supplierServiceLogger,
        ILogger<CatalogService> catalogServiceLogger
    )
    {
        _appDbContext = appDbContext;
        CustomerService = new CustomerService(appDbContext, customerServiceLogger);
        SupplierService = new SupplierService(appDbContext, supplierServiceLogger);
        ProductService = new ProductService(appDbContext, productServiceLogger);
        PurchaseService = new PurchaseService(appDbContext, ProductService, purchaseServiceLogger);
        SaleService = new SaleService(appDbContext, saleServiceLogger);
        CatalogService = new CatalogService(appDbContext, catalogServiceLogger);
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