namespace FabricesStoreManagementSystem.Mapping;

public static class SupplierMapper
{
    public static SupplierResponse ToSupplierResponse(this Supplier supplier)
        => new SupplierResponse(
                supplier.Id, supplier.Name, supplier.Email,
                supplier.Phone, supplier.Address, supplier.IsActive,
                supplier.CreatedAt
            );
}
