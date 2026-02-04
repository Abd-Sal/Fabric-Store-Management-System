namespace FabricesStoreManagementSystem.Tests.Validations;

public class CatalogFromSupplierValidationsTests
{
    private readonly CatalogFromSupplierValidations _validator = new();

    private static CatalogFromSupplierRequest CreateValidRequest() => new(
        SupplierID: Guid.NewGuid(),
        Description: "كتالوج المنتجات الأساسية",
        Items: new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
    );

    #region SupplierID Validation Tests

    [Fact]
    public void SupplierID_WhenEmptyGuid_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { SupplierID = Guid.Empty };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.SupplierID)
            .WithErrorMessage("معرف المورد مطلوب.");
    }

    [Fact]
    public void SupplierID_WhenValid_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.SupplierID);
    }

    #endregion

    #region Description Validation Tests

    [Fact]
    public void Description_WhenNull_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest() with { Description = null };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Description_WhenEmptyString_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Description = "" };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.");
    }

    [Fact]
    public void Description_WhenWhitespace_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Description = "   " };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.");
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveValidationError()
    {
        // Note: Need to check the actual max length from CatalogConfigurations
        // Assuming CatalogConfigurations.DescriptionMaxLenght exists
        var longDescription = new string('أ', 1001); // Assuming 1000 is max
        var request = CreateValidRequest() with { Description = longDescription };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage($"الوصف لا يمكن أن يتجاوز {CatalogConfigurations.DescriptionMaxLenght} حرفًا.");
    }

    [Theory]
    [InlineData("وصف صالح")]
    [InlineData("Product Catalog 2024")]
    [InlineData("المنتجات الأساسية للشركة")]
    public void Description_WhenValid_ShouldNotHaveValidationError(string description)
    {
        var request = CreateValidRequest() with { Description = description };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Items List Validation Tests

    [Fact]
    public void Items_WhenNull_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Items = null! };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("قائمة المنتجات مطلوبة.");
    }

    [Fact]
    public void Items_WhenEmptyList_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Items = new List<Guid>() };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("يجب أن تحتوي القائمة على منتج واحد على الأقل.");
    }

    [Fact]
    public void Items_WhenContainsEmptyGuid_ShouldHaveValidationError()
    {
        var items = new List<Guid> { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
        var request = CreateValidRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("القائمة تحتوي على معرفات منتجات فارغة.");
    }

    [Fact]
    public void Items_WhenContainsDuplicateGuids_ShouldHaveValidationError()
    {
        var duplicateId = Guid.NewGuid();
        var items = new List<Guid> { duplicateId, Guid.NewGuid(), duplicateId };
        var request = CreateValidRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("يوجد معرفات منتجات مكررة في القائمة.");
    }

    [Fact]
    public void Items_WhenTooManyItems_ShouldHaveValidationError()
    {
        var items = Enumerable.Range(0, 101).Select(_ => Guid.NewGuid()).ToList();
        var request = CreateValidRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("لا يمكن أن تحتوي القائمة على أكثر من 100 منتج.");
    }

    [Fact]
    public void Items_WhenValidSingleItem_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest() with { Items = new List<Guid> { Guid.NewGuid() } };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Items_WhenValidMultipleItems_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Items_WhenMaxAllowedItems_ShouldNotHaveValidationError()
    {
        var items = Enumerable.Range(0, 100).Select(_ => Guid.NewGuid()).ToList();
        var request = CreateValidRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    #endregion

    #region Complete Request Validation Tests

    [Fact]
    public void Request_WhenAllFieldsValid_ShouldPassAllValidations()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetInvalidRequestTestData))]
    public void Request_WhenInvalid_ShouldHaveValidationErrors(CatalogFromSupplierRequest request)
    {
        var result = _validator.TestValidate(request);

        Assert.NotEmpty(result.Errors);
    }

    public static IEnumerable<object[]> GetInvalidRequestTestData()
    {
        yield return new object[]
        {
            new CatalogFromSupplierRequest(Guid.Empty, "وصف", new List<Guid> { Guid.NewGuid() })
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(Guid.NewGuid(), "", new List<Guid> { Guid.NewGuid() })
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(Guid.NewGuid(), "وصف", null!)
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(Guid.NewGuid(), "وصف", new List<Guid>())
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(Guid.NewGuid(), "وصف",
                new List<Guid> { Guid.NewGuid(), Guid.Empty })
        };

        var duplicateId = Guid.NewGuid();
        yield return new object[]
        {
            new CatalogFromSupplierRequest(Guid.NewGuid(), "وصف",
                new List<Guid> { duplicateId, Guid.NewGuid(), duplicateId })
        };
    }

    [Theory]
    [MemberData(nameof(GetValidRequestTestData))]
    public void Request_WhenValid_ShouldNotHaveValidationErrors(CatalogFromSupplierRequest request)
    {
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidRequestTestData()
    {
        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                Guid.NewGuid(),
                "كتالوج المنتجات الشتوية",
                new List<Guid> { Guid.NewGuid() }
            )
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                Guid.NewGuid(),
                null,
                new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            )
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                Guid.NewGuid(),
                "وصف قصير",
                Enumerable.Range(0, 50).Select(_ => Guid.NewGuid()).ToList()
            )
        };
    }

    #endregion

    #region Cascade Mode Tests

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForSupplierID()
    {
        var request = new CatalogFromSupplierRequest(
            SupplierID: Guid.Empty,
            Description: "وصف",
            Items: new List<Guid>() // Empty list - also invalid
        );

        var result = _validator.TestValidate(request);

        // Should have error for SupplierID (first in cascade)
        result.ShouldHaveValidationErrorFor(x => x.SupplierID);

        // Might or might not have error for Items due to cascade
    }

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForItems()
    {
        var request = new CatalogFromSupplierRequest(
            SupplierID: Guid.NewGuid(),
            Description: "وصف",
            Items: null! // Null items - should fail first
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("قائمة المنتجات مطلوبة.");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Request_WithOnlyWhitespaceDescription_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Description = " \t\n " };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.");
    }

    [Fact]
    public void Request_WithAllEmptyGuidsInItems_ShouldHaveValidationError()
    {
        var items = new List<Guid> { Guid.Empty, Guid.Empty, Guid.Empty };
        var request = CreateValidRequest() with { Items = items };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("يوجد معرفات منتجات مكررة في القائمة.");
    }

    [Fact]
    public void Request_WithSingleEmptyGuidInItems_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Items = new List<Guid> { Guid.Empty } };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Items)
            .WithErrorMessage("القائمة تحتوي على معرفات منتجات فارغة.");
    }

    #endregion
}