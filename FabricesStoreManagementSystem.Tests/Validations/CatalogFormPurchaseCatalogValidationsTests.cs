namespace FabricesStoreManagementSystem.Tests.Validations;

public class CatalogFormPurchaseCatalogValidationsTests
{
    private readonly CatalogFormPurchaseCatalogValidations _validator = new();

    private static CatalogFormPurchaseCatalogRequest CreateValidRequest(
        Guid? supplierId = null,
        string? description = "وصف صحيح",
        List<Guid>? items = null,
        decimal amount = 100,
        decimal paidAmount = 50)
    {
        return new CatalogFormPurchaseCatalogRequest(
            supplierId ?? Guid.NewGuid(),
            description,
            items ?? new List<Guid> { Guid.NewGuid() },
            amount,
            paidAmount
        );
    }

    #region SupplierID

    [Fact]
    public void Should_have_error_when_SupplierID_is_empty()
    {
        var model = CreateValidRequest(Guid.Empty);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SupplierID);
    }

    [Fact]
    public void Should_not_have_error_when_SupplierID_is_valid()
    {
        var model = CreateValidRequest(Guid.NewGuid());

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SupplierID);
    }

    #endregion

    #region Description

    [Fact]
    public void Should_not_have_error_when_Description_is_empty_string()
    {
        var model = CreateValidRequest(description: "");

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_have_error_when_Description_exceeds_max_length()
    {
        var longDescription = new string('a', 501);
        var model = CreateValidRequest(description: longDescription);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_not_have_error_when_Description_is_null()
    {
        var model = CreateValidRequest(description: null);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Items

    [Fact]
    public void Should_have_error_when_Items_is_empty()
    {
        var model = CreateValidRequest(items: new List<Guid>());

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Should_have_error_when_Items_contains_empty_Guid()
    {
        var model = CreateValidRequest(items: new List<Guid> { Guid.Empty });

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Should_not_have_error_when_Items_are_valid()
    {
        var model = CreateValidRequest(items: new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Items);
    }

    #endregion

    #region Amount

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_have_error_when_Amount_is_not_greater_than_zero(decimal amount)
    {
        var model = CreateValidRequest(amount: amount);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    #endregion

    #region PaidAmount

    [Theory]
    [InlineData(-1)]
    public void Should_have_error_when_PaidAmount_is_negative(decimal paidAmount)
    {
        var model = CreateValidRequest(paidAmount: paidAmount);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PaidAmount);
    }

    [Theory]
    [InlineData(100, 150)]
    [InlineData(50, 51)]
    public void Should_have_error_when_PaidAmount_exceeds_Amount(decimal amount, decimal paidAmount)
    {
        var model = CreateValidRequest(amount: amount, paidAmount: paidAmount);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PaidAmount);
    }

    [Fact]
    public void Should_not_have_error_when_PaidAmount_equals_Amount()
    {
        var model = CreateValidRequest(amount: 100, paidAmount: 100);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.PaidAmount);
    }

    #endregion

    #region Object-level validation

    [Fact]
    public void Should_have_error_when_PaidAmount_greater_than_Amount_at_object_level()
    {
        var model = CreateValidRequest(amount: 100, paidAmount: 150);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    #endregion
}
