namespace FabricesStoreManagementSystem.Mapping;

public static class CatalogMapper
{
    public static CatalogResponse ToCatalogResponse(this Catalog catalog)
        => new CatalogResponse(
                catalog.Id, catalog.CatalogCode, catalog.Description,
                catalog.Status, catalog.IsPurchased, catalog.Price,
                catalog.PaidAmount, catalog.IsPaid,
                catalog.CreatedAt,catalog.LastUpdateAt, null
            );

    public static CatalogResponse ToCatalogResponseWithItems(this Catalog catalog)
        => new CatalogResponse(
                catalog.Id, catalog.CatalogCode, catalog.Description,
                catalog.Status, catalog.IsPurchased, catalog.Price,
                catalog.PaidAmount, catalog.IsPaid,
                catalog.CreatedAt, catalog.LastUpdateAt,
                catalog.CatalogsProducts.Select(x => x.ToCatalogProductResponse()).ToList()
            );

    public static CatalogProductResponse ToCatalogProductResponse(this CatalogProduct catalogProduct)
        => new CatalogProductResponse(
                catalogProduct.Id, catalogProduct.ProductID,
                catalogProduct.CatalogID, catalogProduct.Quantity,
                catalogProduct.IsDeducted
            );

    public static AssignCatalogResponse ToAssignCatalogResponse(this CatalogAssign catalogAssign)
        => new AssignCatalogResponse(
                catalogAssign.Id, catalogAssign.CustomerID, catalogAssign.CatalogID,
                catalogAssign.AssignedAt, catalogAssign.ReturnedAt
            );
}
