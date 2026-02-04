namespace FabricesStoreManagementSystem.Tests.Validations;

public class ProductCatalogValidationsTests
{
    private readonly ProductCatalogValidations _validator = new();

    private static Guid ValidId => Guid.NewGuid();

    #region Id validations

    [Fact]
    public void Id_Should_Fail_When_Empty()
    {
        var request = new ProductCatalogRequest(
            Id: Guid.Empty,
            Quantity: 1.0f
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("معرف المنتج لا يمكن أن يكون فارغًا.");
    }

    #endregion

    #region Quantity basic validations

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    [InlineData(-0.5f)]
    public void Quantity_Should_Fail_When_Less_Than_Or_Equal_Zero(float quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            quantity
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية يجب أن تكون أكبر من الصفر.");
    }

    [Theory]
    [InlineData(0.01f)]
    [InlineData(0.05f)]
    public void Quantity_Should_Fail_When_Less_Than_Minimum(float quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            quantity
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية يجب أن تكون 0.1 على الأقل.");
    }

    [Theory]
    [InlineData(20.1f)]
    [InlineData(50f)]
    public void Quantity_Should_Fail_When_Greater_Than_Maximum(float quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            quantity
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية لا يمكن أن تتجاوز 20.");
    }

    #endregion

    #region Decimal precision validations

    [Theory]
    [InlineData(0.1f)]
    [InlineData(0.5f)]
    [InlineData(1.0f)]
    [InlineData(2.5f)]
    [InlineData(10f)]
    public void Quantity_Should_Pass_When_Has_At_Most_One_Decimal(float quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            quantity
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Tenth increment validations
    [Theory]
    [InlineData(1.11f)]
    [InlineData(2.55f)]
    [InlineData(10.999f)]
    [InlineData(0.222f)]
    [InlineData(0.123f)]
    [InlineData(2.35f)]
    public void Quantity_Should_Fail_When_More_Than_One_Decimal(float quantity)
    {
        var request = new ProductCatalogRequest(ValidId, quantity);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية يمكن أن تحتوي على منزلة عشرية واحدة كحد أقصى.");
    }


    [Theory]
    [InlineData(0.1f)]
    [InlineData(0.2f)]
    [InlineData(0.5f)]
    [InlineData(1.0f)]
    [InlineData(2.5f)]
    [InlineData(10.0f)]
    public void Quantity_Should_Pass_When_Multiple_Of_Point_One(float quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            quantity
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Happy path

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var request = new ProductCatalogRequest(
            Id: ValidId,
            Quantity: 1.5f
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
