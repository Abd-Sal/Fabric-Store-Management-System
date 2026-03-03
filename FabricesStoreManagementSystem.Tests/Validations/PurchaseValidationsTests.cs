namespace FabricesStoreManagementSystem.Tests.Validations;

public class PurchaseValidationsTests
{
    private readonly PurchaseValidations _validator = new();

    private static PurchaseItemRequest CreateValidItem(
        Guid? productId = null,
        decimal quantity = 1.0m,
        decimal unitCost = 10.0m)
    {
        return new PurchaseItemRequest(
            productId ?? Guid.NewGuid(),
            quantity,
            unitCost
        );
    }

    private static PurchaseRequest CreateValidRequest(
        Guid? supplierId = null,
        decimal paidAmount = 10.0m,
        List<PurchaseItemRequest>? items = null)
    {
        return new PurchaseRequest(
            supplierId ?? Guid.NewGuid(),
            paidAmount,
            items ?? new List<PurchaseItemRequest> { CreateValidItem() }
        );
    }

    // ----------------------------
    // SupplierID tests
    // ----------------------------
    [Fact]
    public void SupplierID_Empty_ShouldHaveValidationError()
    {
        var model = CreateValidRequest(supplierId: Guid.Empty);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SupplierID);
    }

    [Fact]
    public void SupplierID_Valid_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SupplierID);
    }

    // ----------------------------
    // PaidAmount tests
    // ----------------------------
    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void PaidAmount_Negative_ShouldHaveValidationError(decimal paidAmount)
    {
        var model = CreateValidRequest(paidAmount: paidAmount);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PaidAmount);
    }

    [Theory]
    [InlineData(1.001)]
    [InlineData(10.999)]
    public void PaidAmount_MoreThanTwoDecimalPlaces_ShouldHaveValidationError(decimal paidAmount)
    {
        var model = CreateValidRequest(paidAmount: paidAmount);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PaidAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100.50)]
    public void PaidAmount_Valid_ShouldNotHaveValidationError(decimal paidAmount)
    {
        var model = CreateValidRequest(paidAmount: paidAmount);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.PaidAmount);
    }

    [Fact]
    public void PaidAmount_GreaterThanTotalCost_ShouldHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(quantity: 1, unitCost: 10)
        };
        var model = CreateValidRequest(paidAmount: 20, items: items);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void PaidAmount_LessOrEqualTotalCost_ShouldNotHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(quantity: 2, unitCost: 10)
        };
        var model = CreateValidRequest(paidAmount: 20, items: items);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    // ----------------------------
    // PurchaseItems tests
    // ----------------------------
    [Fact]
    public void PurchaseItems_Empty_ShouldHaveValidationError()
    {
        var model = CreateValidRequest(items: new List<PurchaseItemRequest>());

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PurchaseItems);
    }

    [Fact]
    public void PurchaseItems_DuplicateProductIds_ShouldHaveValidationError()
    {
        var id = Guid.NewGuid();
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(productId: id),
            CreateValidItem(productId: id)
        };
        var model = CreateValidRequest(items: items);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PurchaseItems);
    }

    [Fact]
    public void PurchaseItems_TooManyItems_ShouldHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>();
        for (int i = 0; i < 101; i++)
        {
            items.Add(CreateValidItem());
        }
        var model = CreateValidRequest(items: items);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PurchaseItems);
    }

    [Fact]
    public void PurchaseItems_Valid_ShouldNotHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(),
            CreateValidItem()
        };
        var model = CreateValidRequest(items: items);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.PurchaseItems);
    }

    // ----------------------------
    // Total cost rules
    // ----------------------------
    [Fact]
    public void TotalCost_ExceedsMaximum_ShouldHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(quantity: 10000m, unitCost: 1000m)
        };
        var model = CreateValidRequest(items: items);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void TotalCost_Zero_ShouldHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(quantity: 0m, unitCost: 10m)
        };
        var model = CreateValidRequest(items: items);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrors();
    }

    [Fact]
    public void TotalCost_WithinLimits_ShouldNotHaveValidationError()
    {
        var items = new List<PurchaseItemRequest>
        {
            CreateValidItem(quantity: 1m, unitCost: 100m),
            CreateValidItem(quantity: 2m, unitCost: 50m)
        };
        var model = CreateValidRequest(items: items);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x);
    }
}
