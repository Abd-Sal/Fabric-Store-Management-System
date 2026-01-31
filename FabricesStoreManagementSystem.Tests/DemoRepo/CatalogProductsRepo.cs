namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class CatalogProductsRepo
{
    public static List<CatalogProduct> CatalogProducts() => new List<CatalogProduct>()
    {
        new CatalogProduct{ Id = Guid.Parse("1dfb957b-4e0b-4efa-b996-2f02ebe33085"), CatalogID = CatalogsRepo.Catalogs()[0].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[0].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("5241f631-aa4f-4f59-b40b-a3ddd3fa99f6"), CatalogID = CatalogsRepo.Catalogs()[0].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[1].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("284a49ff-e62e-42ec-b8f7-d1cecb3133ca"), CatalogID = CatalogsRepo.Catalogs()[0].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[2].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("79ebc0f8-61e3-4eaa-ad5e-3dd62049427b"), CatalogID = CatalogsRepo.Catalogs()[0].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[3].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("6d05d601-9a98-4264-9c08-10aca0143d7e"), CatalogID = CatalogsRepo.Catalogs()[0].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[4].Id, Quantity = 1 },

        new CatalogProduct{ Id = Guid.Parse("1b02c874-cbb4-4cab-9db3-3bcaca99ee10"), CatalogID = CatalogsRepo.Catalogs()[1].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[0].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("b65553d1-fc75-4fc0-bd6c-c876112bbecf"), CatalogID = CatalogsRepo.Catalogs()[1].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[1].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("1d8562dd-3cdc-47d1-b729-4341cec5b646"), CatalogID = CatalogsRepo.Catalogs()[1].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[2].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("6db3c0c2-3b12-4a22-9b34-7dcf7abc51b6"), CatalogID = CatalogsRepo.Catalogs()[1].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[3].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("127100c8-ed97-4e67-b3da-f55a754b3ddd"), CatalogID = CatalogsRepo.Catalogs()[1].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[4].Id, Quantity = 1 },

        new CatalogProduct{ Id = Guid.Parse("3972ca94-fcd0-4087-90c8-3242fac5eb03"), CatalogID = CatalogsRepo.Catalogs()[2].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[5].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("e4e19cb9-24f5-4847-9019-1fc8c79d74c2"), CatalogID = CatalogsRepo.Catalogs()[2].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[6].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("c80787a6-54d5-4be5-9b1c-fe2008904f86"), CatalogID = CatalogsRepo.Catalogs()[2].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[7].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("8dcec837-c446-4c50-9e6a-9e2fdcfc1ca4"), CatalogID = CatalogsRepo.Catalogs()[2].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[8].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("4bba508a-51c9-47de-85d2-2e2d4fccfc69"), CatalogID = CatalogsRepo.Catalogs()[2].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[9].Id, Quantity = 1 },

        new CatalogProduct{ Id = Guid.Parse("57524c72-0f8f-46c2-9a20-f3128a05dc85"), CatalogID = CatalogsRepo.Catalogs()[3].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[5].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("23f49922-3cd8-4e13-a8f5-390d863cdd45"), CatalogID = CatalogsRepo.Catalogs()[3].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[6].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("cf39b9f3-b9b4-439c-b96d-bfdc6988c932"), CatalogID = CatalogsRepo.Catalogs()[3].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[7].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("fe10684e-c212-4450-be65-50177bd9235d"), CatalogID = CatalogsRepo.Catalogs()[3].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[8].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("fe21b689-d278-4f15-9951-b53610a10fd9"), CatalogID = CatalogsRepo.Catalogs()[3].Id, IsDeducted = false, PorductID = ProductsRepo.Products()[9].Id, Quantity = 1 },


        new CatalogProduct{ Id = Guid.Parse("a2c2235d-0332-45b0-a964-0be968325dec"), CatalogID = CatalogsRepo.Catalogs()[4].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[0].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("8ee4d8de-c6e1-4181-a069-4d95cde6b231"), CatalogID = CatalogsRepo.Catalogs()[4].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[1].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("daa27957-641f-4be9-a6ae-5fbd5802e083"), CatalogID = CatalogsRepo.Catalogs()[4].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[2].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("a828dbf6-83db-48f7-b6a9-1cb020ac843c"), CatalogID = CatalogsRepo.Catalogs()[4].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[3].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("6e2389c2-e724-45e6-b233-b787e93493c4"), CatalogID = CatalogsRepo.Catalogs()[4].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[4].Id, Quantity = 1 },

        new CatalogProduct{ Id = Guid.Parse("c7cd2e4b-7918-42b7-b23b-4d5c3543890d"), CatalogID = CatalogsRepo.Catalogs()[5].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[5].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("36e7fefc-7701-4d3e-b7ce-9a0322874b88"), CatalogID = CatalogsRepo.Catalogs()[5].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[6].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("9a673405-4f4b-473e-ab8d-9ad68c1bd423"), CatalogID = CatalogsRepo.Catalogs()[5].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[7].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("393bf73e-73b9-4500-a666-e93285441ab3"), CatalogID = CatalogsRepo.Catalogs()[5].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[8].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("bf6a220d-1fd1-4d83-9270-eac4c964fd3e"), CatalogID = CatalogsRepo.Catalogs()[5].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[9].Id, Quantity = 1 },

        new CatalogProduct{ Id = Guid.Parse("4dcadcef-5d52-4097-995c-8fa4cd529d29"), CatalogID = CatalogsRepo.Catalogs()[6].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[0].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("9bb46697-b76a-47b0-8e56-20c85764af5b"), CatalogID = CatalogsRepo.Catalogs()[6].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[1].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("8b6d31d7-e8e1-47d7-b5ff-ccd181a8f447"), CatalogID = CatalogsRepo.Catalogs()[6].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[2].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("85bb1004-fab8-4478-af3a-81066b79a538"), CatalogID = CatalogsRepo.Catalogs()[6].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[3].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("ec94786f-d53b-4ad2-bee4-d601a9d56772"), CatalogID = CatalogsRepo.Catalogs()[6].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[4].Id, Quantity = 1 },

        new CatalogProduct{ Id = Guid.Parse("6581bc2b-41a7-4549-9181-00024858a0d8"), CatalogID = CatalogsRepo.Catalogs()[7].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[5].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("73e8acf8-b22e-4b01-9fa1-c5d47899b7d3"), CatalogID = CatalogsRepo.Catalogs()[7].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[6].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("f26e4db2-a8af-47ba-b426-fdebb446d06e"), CatalogID = CatalogsRepo.Catalogs()[7].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[7].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("b97e47ae-9f5c-4d0d-9391-0cc1b15a50b4"), CatalogID = CatalogsRepo.Catalogs()[7].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[8].Id, Quantity = 1 },
        new CatalogProduct{ Id = Guid.Parse("9180b8c2-ea74-48ef-93fe-c83bb4eed939"), CatalogID = CatalogsRepo.Catalogs()[7].Id, IsDeducted = true, PorductID = ProductsRepo.Products()[9].Id, Quantity = 1 },
    };
}
