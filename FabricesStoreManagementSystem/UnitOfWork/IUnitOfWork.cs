namespace FabricesStoreManagementSystem.UnitOfWork;

public interface IUnitOfWork
{
    public ICatalogService CatalogService { get; }
    public ICustomerService CustomerService { get; }
    public ISupplierService SupplierService{ get; }
    public IProductService ProductService { get; }
    public IPurchaseService PurchaseService { get; }
    public ISaleService SaleService { get; }
    public IPaymentService PaymentService { get; }
    public IExpenseService ExpenseService { get; }
    public IAuthService AuthService { get; }

    public int SaveChanges();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
