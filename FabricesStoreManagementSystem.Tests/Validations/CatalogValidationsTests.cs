namespace FabricesStoreManagementSystem.Tests.Validations;

public class CatalogValidationsTests
{
    private readonly CatalogValidations _validator = new();

    private static CatalogRequest CreateValidCatalogRequest() => new(
        Description: "كتالوج المنتجات الأساسية 2024",
        Items: new List<CatalogProductRequest>
        {
            new(Guid.NewGuid(), 10f),
            new(Guid.NewGuid(), 15f),
            new(Guid.NewGuid(), 20f),
            new(Guid.NewGuid(), 5f)
        }
    );

    #region Description Validation Tests

    [Fact]
    public void Description_WhenNull_ShouldNotHaveValidationError()
    {
        var request = CreateValidCatalogRequest() with { Description = null };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_WhenEmptyString_ShouldHaveValidationError()
    {
        var request = CreateValidCatalogRequest() with { Description = "" };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.");
    }

    [Fact]
    public void Description_WhenWhitespace_ShouldHaveValidationError()
    {
        var request = CreateValidCatalogRequest() with { Description = "   " };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.");
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveValidationError()
    {
        var longDescription = new string('أ', 1001);
        var request = CreateValidCatalogRequest() with { Description = longDescription };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage($"الوصف لا يمكن أن يتجاوز {CatalogConfigurations.DescriptionMaxLenght} حرفًا.");
    }

    [Theory]
    [InlineData("وصف @ غير صالح")]
    [InlineData("Description#Invalid")]
    [InlineData("اختبار & فاشل")]
    public void Description_WhenContainsInvalidCharacters_ShouldHaveValidationError(string description)
    {
        var request = CreateValidCatalogRequest() with { Description = description };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف يحتوي على أحرف غير مسموح بها.");
    }

    [Theory]
    [InlineData("كتالوج المنتجات")]
    [InlineData("Product Catalog 2024")]
    [InlineData("منتجات الشركة الأساسية!")]
    [InlineData("Winter collection, new items?")]
    public void Description_WhenValid_ShouldNotHaveValidationError(string description)
    {
        var request = CreateValidCatalogRequest() with { Description = description };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Items List Validation Tests

    [Fact]
    public void Items_WhenNull_ShouldHaveValidationError()
    {
        var request = CreateValidCatalogRequest() with { Items = null! };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("قائمة المنتجات مطلوبة.");
    }

    [Fact]
    public void Items_WhenEmptyList_ShouldHaveValidationError()
    {
        var request = CreateValidCatalogRequest() with { Items = new List<CatalogProductRequest>() };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("يجب أن يحتوي الكتالوج على منتج واحد على الأقل.");
    }

    [Fact]
    public void Items_WhenTooFewItems_ShouldHaveValidationError()
    {
        var items = new List<CatalogProductRequest>
        {
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        var result = _validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("يجب أن يحتوي الكتالوج على منتج واحد على الأقل.");
    }

    [Fact]
    public void Items_WhenTooManyItems_ShouldHaveValidationError()
    {
        var items = Enumerable.Range(0, 101)
            .Select(_ => new CatalogProductRequest(Guid.NewGuid(), 1f))
            .ToList();
        var request = CreateValidCatalogRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("لا يمكن أن يحتوي الكتالوج على أكثر من 100 منتج.");
    }

    [Fact]
    public void Items_WhenDuplicateProductIds_ShouldHaveValidationError()
    {
        var duplicateId = Guid.NewGuid();
        var items = new List<CatalogProductRequest>
        {
            new(duplicateId, 10f),
            new(Guid.NewGuid(), 15f),
            new(duplicateId, 20f)
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("يوجد منتجات مكررة في الكتالوج.");
    }

    [Fact]
    public void Items_WhenTotalQuantityTooLarge_ShouldHaveValidationError()
    {
        var items = new List<CatalogProductRequest>
        {
            new(Guid.NewGuid(), 400f),
            new(Guid.NewGuid(), 400f),
            new(Guid.NewGuid(), 400f) // Total: 1200 > 1000
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("إجمالي كمية المنتجات في الكتالوج كبير جدًا.");
    }

    [Fact]
    public void Items_WhenValid_ShouldNotHaveValidationError()
    {
        var request = CreateValidCatalogRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    #endregion

    #region Business Rule Tests (IsCatalogMeaningful)
    [Fact]
    public void Catalog_WhenThreeItemsWithGoodQuantities_ShouldNotHaveMeaningfulCatalogError()
    {
        var items = new List<CatalogProductRequest>
        {
            new(Guid.NewGuid(), 5f),  // >= 1
            new(Guid.NewGuid(), 10f), // >= 1
            new(Guid.NewGuid(), 15f)  // >= 1
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x);
    }

    #endregion

    #region Complete Request Validation Tests

    [Fact]
    public void Request_WhenAllFieldsValid_ShouldPassAllValidations()
    {
        var request = CreateValidCatalogRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetInvalidCatalogTestData))]
    public void Request_WhenInvalid_ShouldHaveValidationErrors(CatalogRequest request)
    {
        var result = _validator.TestValidate(request);

        Assert.NotEmpty(result.Errors);
    }

    public static IEnumerable<object[]> GetInvalidCatalogTestData()
    {
        yield return new object[]
        {
            new CatalogRequest("", new List<CatalogProductRequest> { new(Guid.NewGuid(), 10f) })
        };

        yield return new object[]
        {
            new CatalogRequest("وصف صالح", null!)
        };

        yield return new object[]
        {
            new CatalogRequest("وصف صالح", new List<CatalogProductRequest>())
        };

        var duplicateId = Guid.NewGuid();
        yield return new object[]
        {
            new CatalogRequest("وصف صالح", new List<CatalogProductRequest>
            {
                new(duplicateId, 10f),
                new(duplicateId, 15f)
            })
        };

        yield return new object[]
        {
            new CatalogRequest("وصف صالح", new List<CatalogProductRequest>
            {
                new(Guid.NewGuid(), 500f),
                new(Guid.NewGuid(), 600f) // Total: 1100 > 1000
            })
        };
    }

    [Theory]
    [MemberData(nameof(GetValidCatalogTestData))]
    public void Request_WhenValid_ShouldNotHaveValidationErrors(CatalogRequest request)
    {
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidCatalogTestData()
    {
        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>
                {
                    new(Guid.NewGuid(), 10f),
                    new(Guid.NewGuid(), 20f),
                    new(Guid.NewGuid(), 30f)
                }
            )
        };

        yield return new object[]
        {
            new CatalogRequest(
                "كتالوج المنتجات الشتوية",
                new List<CatalogProductRequest>
                {
                    new(Guid.NewGuid(), 5f),
                    new(Guid.NewGuid(), 10f),
                    new(Guid.NewGuid(), 15f),
                    new(Guid.NewGuid(), 20f)
                }
            )
        };

        yield return new object[]
        {
            new CatalogRequest(
                "Product Catalog 2024",
                Enumerable.Range(0, 50)
                    .Select(_ => new CatalogProductRequest(Guid.NewGuid(), 10f))
                    .ToList()
            )
        };
    }

    #endregion

    #region Cascade Mode Tests

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForItems()
    {
        var request = new CatalogRequest(
            "وصف صالح",
            null! // Null items should fail first
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("قائمة المنتجات مطلوبة.");
    }

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForDescription()
    {
        var request = new CatalogRequest(
            "", // Empty description
            new List<CatalogProductRequest> { new(Guid.NewGuid(), 10f), new(Guid.NewGuid(), 10f), new(Guid.NewGuid(), 10f) }
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Catalog_WhenExactlyMaxItems_ShouldNotHaveValidationError()
    {
        var items = Enumerable.Range(0, 100)
            .Select(_ => new CatalogProductRequest(Guid.NewGuid(), 5f))
            .ToList();
        var request = CreateValidCatalogRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Items_WhenValidIndividualProductButInvalidTotal_ShouldHaveTotalQuantityError()
    {
        var items = new List<CatalogProductRequest>
        {
            new(Guid.NewGuid(), 300f), // Valid individually
            new(Guid.NewGuid(), 350f), // Valid individually
            new(Guid.NewGuid(), 400f)  // Valid individually, but total 1050 > 1000
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("إجمالي كمية المنتجات في الكتالوج كبير جدًا.");
    }

    #endregion

    #region Nested CatalogProductValidation Tests

    [Fact]
    public void Items_WhenContainsInvalidProduct_ShouldHaveNestedValidationError()
    {
        var items = new List<CatalogProductRequest>
        {
            new(Guid.NewGuid(), 10f),    // Valid
            new(Guid.Empty, 15f),        // Invalid: Empty ProductID
            new(Guid.NewGuid(), 20f)     // Valid
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        var result = _validator.TestValidate(request);

        // Should have error from nested validator
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "معرف المنتج مطلوب." ||
            e.ErrorMessage == "معرف المنتج لا يمكن أن يكون فارغًا.");
    }

    [Fact]
    public void Items_WhenProductHasInvalidQuantity_ShouldHaveNestedValidationError()
    {
        var items = new List<CatalogProductRequest>
        {
            new(Guid.NewGuid(), 10f),    // Valid
            new(Guid.NewGuid(), -5f),    // Invalid: Negative quantity
            new(Guid.NewGuid(), 20f)     // Valid
        };
        var request = CreateValidCatalogRequest() with { Items = items };

        var result = _validator.TestValidate(request);

        // Should have error from nested validator
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == "الكمية يجب أن تكون أكبر من الصفر.");
    }

    #endregion
}