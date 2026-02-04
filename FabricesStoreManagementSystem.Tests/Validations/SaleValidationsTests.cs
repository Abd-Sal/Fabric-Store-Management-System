namespace FabricesStoreManagementSystem.Tests.Validations;

public class SaleValidationsTests
{
    private readonly SaleValidations _validator = new();

    // ----------------------------
    // CustomerID tests
    // ----------------------------
    [Fact]
    public void CustomerID_Empty_ShouldHaveValidationError()
    {
        var model = new SaleRequest(Guid.Empty, 0m, 0m, new List<SaleItemRequest> { CreateValidSaleItem() });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CustomerID)
              .WithErrorMessage("معرف العميل لا يمكن أن يكون فارغًا.");
    }

    // ----------------------------
    // Discount tests
    // ----------------------------
    [Theory]
    [InlineData(-1)]
    [InlineData(10001)]
    public void Discount_Invalid_ShouldHaveValidationError(decimal discount)
    {
        var model = new SaleRequest(Guid.NewGuid(), discount, 0m, new List<SaleItemRequest> { CreateValidSaleItem() });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Discount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(500)]
    [InlineData(10000)]
    public void Discount_Valid_ShouldNotHaveValidationError(decimal discount)
    {
        var model = new SaleRequest(Guid.NewGuid(), discount, 0m, new List<SaleItemRequest> { CreateValidSaleItem() });

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Discount);
    }

    // ----------------------------
    // PaidAmount tests
    // ----------------------------
    [Theory]
    [InlineData(-1)]
    public void PaidAmount_Invalid_ShouldHaveValidationError(decimal paidAmount)
    {
        var model = new SaleRequest(Guid.NewGuid(), 0m, paidAmount, new List<SaleItemRequest> { CreateValidSaleItem() });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PaidAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1.45)]
    public void PaidAmount_Valid_ShouldNotHaveValidationError(decimal paidAmount)
    {
        var model = new SaleRequest(Guid.NewGuid(), 0m, paidAmount, new List<SaleItemRequest> { CreateValidSaleItem() });

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.PaidAmount);
    }

    // ----------------------------
    // SaleItems tests
    // ----------------------------
    [Fact]
    public void SaleItems_Null_ShouldHaveValidationError()
    {
        var model = new SaleRequest(Guid.NewGuid(), 0m, 0m, null);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SaleItems)
              .WithErrorMessage("عناصر البيع مطلوبة.");
    }

    [Fact]
    public void SaleItems_Empty_ShouldHaveValidationError()
    {
        var model = new SaleRequest(Guid.NewGuid(), 0m, 0m, new List<SaleItemRequest>());

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SaleItems)
              .WithErrorMessage("يجب أن تحتوي على عنصر بيع واحد على الأقل.");
    }

    [Fact]
    public void SaleItems_DuplicateProductIDs_ShouldHaveValidationError()
    {
        var item1 = CreateValidSaleItem();
        var item2 = new SaleItemRequest(item1.ProductID, 2f, 10m);

        var model = new SaleRequest(Guid.NewGuid(), 0m, 0m, new List<SaleItemRequest> { item1, item2 });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SaleItems)
              .WithErrorMessage("تم العثور على معرفات منتجات مكررة في عناصر البيع.");
    }

    // ----------------------------
    // Discount cross-validation tests
    // ----------------------------
    [Fact]
    public void Discount_GreaterThanSubtotal_ShouldHaveValidationError()
    {
        var item = new SaleItemRequest(Guid.NewGuid(), 2f, 50m); // subtotal = 100
        var model = new SaleRequest(Guid.NewGuid(), 150m, 0, new List<SaleItemRequest> { item });

        var result = _validator.TestValidate(model);

        // Now check for discount error
        result.Errors.Should().ContainSingle(e =>
            e.ErrorMessage == "مبلغ الخصم لا يمكن أن يتجاوز المجموع الفرعي."
        );
    }

    [Fact]
    public void PaidAmount_GreaterThanNetTotal_ShouldHaveValidationError()
    {
        var item = new SaleItemRequest(Guid.NewGuid(), 2f, 50m); // subtotal = 100
        var model = new SaleRequest(Guid.NewGuid(), 20m, 90m, new List<SaleItemRequest> { item }); // net = 80

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrors();
    }

    // ----------------------------
    // Helpers
    // ----------------------------
    private SaleItemRequest CreateValidSaleItem()
    {
        return new SaleItemRequest(Guid.NewGuid(), 1f, 10m);
    }
}
