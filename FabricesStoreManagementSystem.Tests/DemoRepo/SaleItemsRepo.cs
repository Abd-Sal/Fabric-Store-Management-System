namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class SaleItemsRepo
{
    public static List<SaleItem> SaleItems() => new List<SaleItem>
    {
        new SaleItem{ Id = Guid.Parse("4e3fe106-4062-4e62-99b3-f8d95a4fd699"), ProductID = ProductsRepo.Products()[0].Id, SaleID = SalesRepo.Sales()[0].Id, Quantity = 2, UnitPrice = 50 },

        new SaleItem{ Id = Guid.Parse("f7c18f2d-9924-4530-a043-899206c7af1d"), ProductID = ProductsRepo.Products()[0].Id, SaleID = SalesRepo.Sales()[1].Id, Quantity = 4, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("cf7484d3-ec87-40c7-a48a-e7a0667dc1e8"), ProductID = ProductsRepo.Products()[1].Id, SaleID = SalesRepo.Sales()[1].Id, Quantity = 4, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("d0262e5a-5dc4-466d-bd3b-50543e8d7376"), ProductID = ProductsRepo.Products()[2].Id, SaleID = SalesRepo.Sales()[1].Id, Quantity = 4, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("9c1ce9b7-2011-40d3-9fc8-649c3376c4d4"), ProductID = ProductsRepo.Products()[3].Id, SaleID = SalesRepo.Sales()[1].Id, Quantity = 4, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("f6c43521-d41c-4e4c-98bc-e8743e4cea06"), ProductID = ProductsRepo.Products()[4].Id, SaleID = SalesRepo.Sales()[1].Id, Quantity = 4, UnitPrice = 50 },

        new SaleItem{ Id = Guid.Parse("4f746b51-7847-49ce-9034-ec513c765b79"), ProductID = ProductsRepo.Products()[0].Id, SaleID = SalesRepo.Sales()[2].Id, Quantity = 2, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("e95df002-6809-47c6-a6cb-edecd0988ade"), ProductID = ProductsRepo.Products()[1].Id, SaleID = SalesRepo.Sales()[2].Id, Quantity = 2, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("3ab0e482-1852-4b10-a746-d656efd48dff"), ProductID = ProductsRepo.Products()[2].Id, SaleID = SalesRepo.Sales()[2].Id, Quantity = 2, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("77a661b5-ef74-4070-92c8-6a02aca34322"), ProductID = ProductsRepo.Products()[3].Id, SaleID = SalesRepo.Sales()[2].Id, Quantity = 2, UnitPrice = 50 },
        new SaleItem{ Id = Guid.Parse("96693ab4-311d-4ae5-80e2-f6646c50a41a"), ProductID = ProductsRepo.Products()[5].Id, SaleID = SalesRepo.Sales()[2].Id, Quantity = 2, UnitPrice = 50 },
    };

}
