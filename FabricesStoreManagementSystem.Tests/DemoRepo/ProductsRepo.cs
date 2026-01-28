namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class ProductsRepo
{
    public static List<Product> Products() => new List<Product>()
        {
            //P-001
            new Product { Id = Guid.Parse("cd8f4113-fc90-477f-8f3d-023cc98cb671"),  Code = "P-001", Color = "Red", Unit = "Meter" },
            new Product { Id = Guid.Parse("b984d319-9f44-41f1-a1dc-ea25fd5447f1"), Code = "P-001", Color = "Green", Unit = "Meter" },
            new Product { Id = Guid.Parse("21e72f95-c8bc-4b13-9bca-2949ac7f68f7"), Code = "P-001", Color = "Blue", Unit = "Meter" },
            new Product { Id = Guid.Parse("1706e837-80b4-4632-98d6-51bf38fb5f42"), Code = "P-001", Color = "Gray", Unit = "Meter" },
            new Product { Id = Guid.Parse("4eb68122-b22f-488e-ac77-3845e84996fc"), Code = "P-001", Color = "Yellow", Unit = "Meter" },
            new Product { Id = Guid.Parse("4b832d7e-cbd9-4d31-ab5b-5295b5970726"), Code = "P-001", Color = "White", Unit = "Meter" },
            new Product { Id = Guid.Parse("c9e44173-c927-484a-8964-945b7671851e"), Code = "P-001", Color = "Purple", Unit = "Meter" },
            new Product { Id = Guid.Parse("780cd1a2-ccb8-4799-a268-c92ebcb1723b"), Code = "P-001", Color = "Pink", Unit = "Meter" },
            new Product { Id = Guid.Parse("57bfa8c5-4cd2-44a7-a995-87bb63504663"), Code = "P-001", Color = "Brown", Unit = "Meter" },
            new Product { Id = Guid.Parse("0c3d688c-3ed7-401e-81a1-cef60322de9f"), Code = "P-001", Color = "Ornage", Unit = "Meter" },

            //P-002
            new Product { Id = Guid.Parse("2416114f-64f6-4018-94a5-06c28efc8f68"),  Code = "P-002", Color = "Red", Unit = "Meter" },
            new Product { Id = Guid.Parse("eb71f734-26d5-4ce0-9bf4-7d899397d4a2"), Code = "P-002", Color = "Green", Unit = "Meter" },
            new Product { Id = Guid.Parse("06f4484d-20ba-4a3d-89ea-250fa7a96355"), Code = "P-002", Color = "Blue", Unit = "Meter" },
            new Product { Id = Guid.Parse("0349240d-1909-46df-b5fe-68ab5d4068f6"), Code = "P-002", Color = "Gray", Unit = "Meter" },
            new Product { Id = Guid.Parse("333ab20a-afb3-4c56-b675-9648e5169bcb"), Code = "P-002", Color = "Yellow", Unit = "Meter" },
            new Product { Id = Guid.Parse("f3220956-ad1a-4a40-96d1-c93758d1814a"), Code = "P-002", Color = "White", Unit = "Meter" },
            new Product { Id = Guid.Parse("b3c9d91c-8761-495e-bec8-4499b39cdebc"), Code = "P-002", Color = "Purple", Unit = "Meter" },
            new Product { Id = Guid.Parse("29399aa9-9afb-4f34-b96e-cecad578319c"), Code = "P-002", Color = "Pink", Unit = "Meter" },
            new Product { Id = Guid.Parse("c1b913f3-1037-4201-bc18-ff0ede570af5"), Code = "P-002", Color = "Brown", Unit = "Meter" },
            new Product { Id = Guid.Parse("a5c3e384-1ad4-4e99-8539-3f1f6290e8cf"), Code = "P-002", Color = "Ornage", Unit = "Meter" }
        };
}
