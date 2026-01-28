namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class ProductInventoriesRepo
{
    public static List<Inventory> Inventories() => new List<Inventory>()
        {
            new Inventory { Id = Guid.Parse("181a0f16-3f31-4221-8b0f-c7c43a7a0d99"), ProductID = ProductsRepo.Products()[0].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("d2ec5235-20c0-4cfa-ba77-8193658e38ce"), ProductID = ProductsRepo.Products()[1].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("91b0b4c2-9d4a-4a42-bd19-2603e8fffdbb"), ProductID = ProductsRepo.Products()[2].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("12a91d32-20c5-4e04-a60d-3fab2128c9c2"), ProductID = ProductsRepo.Products()[3].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("a834ad91-0674-48b8-8902-3b92d3392769"), ProductID = ProductsRepo.Products()[4].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("923bdd17-73e3-4b77-a7c9-5312f4cf03c1"), ProductID = ProductsRepo.Products()[5].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("f2e5e0f2-7c02-46e8-8c78-c8e041b91ce0"), ProductID = ProductsRepo.Products()[6].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("c493ef9b-4c6c-4ce6-92ef-793a6118109a"), ProductID = ProductsRepo.Products()[7].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("a471ad22-f84b-45b5-a796-8b92e6a6c0ff"), ProductID = ProductsRepo.Products()[8].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("0fa3943c-0fef-477e-8de0-7521bfda42f9"), ProductID = ProductsRepo.Products()[9].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("9035d59d-9ef1-4392-bc91-58869938d78f"), ProductID = ProductsRepo.Products()[10].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("9dae54a0-bb33-49f3-ad8b-f4eee9895f24"), ProductID = ProductsRepo.Products()[11].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("1f940527-eb55-4658-b766-d0138307946f"), ProductID = ProductsRepo.Products()[12].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("e450ba1f-9b7a-484c-a8b1-21e006479b9a"), ProductID = ProductsRepo.Products()[13].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("efba8c46-9880-4602-abe5-c85f075294fc"), ProductID = ProductsRepo.Products()[14].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("300ef669-cb87-4d1a-a7ba-c704af1fde8c"), ProductID = ProductsRepo.Products()[15].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("ee3ea951-04a5-4efa-956b-de5ebcc3ba7c"), ProductID = ProductsRepo.Products()[16].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("58d24970-25c8-4403-b2a6-8ba268b0d1b1"), ProductID = ProductsRepo.Products()[17].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("bca2496f-0c2c-45ba-b1b2-deef1a28fab8"), ProductID = ProductsRepo.Products()[18].Id, CurrentQuantity = 10 },
            new Inventory { Id = Guid.Parse("6039d0a1-bea9-498d-a748-899f231529bf"), ProductID = ProductsRepo.Products()[19].Id, CurrentQuantity = 10 }
        };
}
