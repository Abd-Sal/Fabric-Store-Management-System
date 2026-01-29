namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class PurchaseItemsRepo
{
    public static List<PurchaseItem> PurchaseItems() => new List<PurchaseItem>
    {
        new PurchaseItem{ Id = Guid.Parse("88ccaa7d-73b1-47aa-998b-6d84a5aa633c"), ProductID = ProductsRepo.Products()[0].Id, PurchaseID = PurchasesRepo.Purchases()[0].Id, Quantity = 2, UnitCost = 50 },

        new PurchaseItem{ Id = Guid.Parse("7d1ea178-b2af-4ae9-ba6f-5413d9c7cee3"), ProductID = ProductsRepo.Products()[0].Id, PurchaseID = PurchasesRepo.Purchases()[1].Id, Quantity = 4, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("6a993a7a-91cb-4d79-be2b-a1065dde28be"), ProductID = ProductsRepo.Products()[1].Id, PurchaseID = PurchasesRepo.Purchases()[1].Id, Quantity = 4, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("4182ed32-b597-41b0-8de2-e61e19dac3aa"), ProductID = ProductsRepo.Products()[2].Id, PurchaseID = PurchasesRepo.Purchases()[1].Id, Quantity = 4, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("7d1cd4ae-7da0-4576-b23b-8347d6bedda2"), ProductID = ProductsRepo.Products()[3].Id, PurchaseID = PurchasesRepo.Purchases()[1].Id, Quantity = 4, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("d26cf476-6082-4a34-a076-799999be04e6"), ProductID = ProductsRepo.Products()[4].Id, PurchaseID = PurchasesRepo.Purchases()[1].Id, Quantity = 4, UnitCost = 50 },

        new PurchaseItem{ Id = Guid.Parse("75d26f0e-d528-42be-b461-853c23f86c0f"), ProductID = ProductsRepo.Products()[0].Id, PurchaseID = PurchasesRepo.Purchases()[2].Id, Quantity = 2, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("cc2e4e38-7d45-4162-83a7-ccf32ee63111"), ProductID = ProductsRepo.Products()[1].Id, PurchaseID = PurchasesRepo.Purchases()[2].Id, Quantity = 2, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("0f2e6a6b-07db-4c93-8948-0061661aff44"), ProductID = ProductsRepo.Products()[2].Id, PurchaseID = PurchasesRepo.Purchases()[2].Id, Quantity = 2, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("8a98c263-3503-4d8a-959e-ac934e77a129"), ProductID = ProductsRepo.Products()[3].Id, PurchaseID = PurchasesRepo.Purchases()[2].Id, Quantity = 2, UnitCost = 50 },
        new PurchaseItem{ Id = Guid.Parse("55b9539d-35e6-4b46-9cfd-cc87e04e5b98"), ProductID = ProductsRepo.Products()[5].Id, PurchaseID = PurchasesRepo.Purchases()[2].Id, Quantity = 2, UnitCost = 50 },
    };
}
