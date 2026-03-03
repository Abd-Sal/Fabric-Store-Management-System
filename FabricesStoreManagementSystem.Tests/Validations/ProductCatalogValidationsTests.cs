using System.Globalization;

namespace FabricesStoreManagementSystem.Tests.Validations;

public class ProductCatalogValidationsTests
{
    private readonly ProductCatalogValidations _validator = new();

    private static Guid ValidId => Guid.NewGuid();
    private static decimal D(string value) => decimal.Parse(value, CultureInfo.InvariantCulture);

    #region Id validations

    [Fact]
    public void Id_Should_Fail_When_Empty()
    {
        var request = new ProductCatalogRequest(
            Id: Guid.Empty,
            Quantity: 1.0m
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("معرف المنتج لا يمكن أن يكون فارغًا.");
    }

    #endregion

    #region Quantity basic validations

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-0.5")]
    public void Quantity_Should_Fail_When_Less_Than_Or_Equal_Zero(string quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            D(quantity)
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية يجب أن تكون أكبر من الصفر.");
    }

    [Theory]
    [InlineData("0.01")]
    [InlineData("0.05")]
    public void Quantity_Should_Fail_When_Less_Than_Minimum(string quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            D(quantity)
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية يجب أن تكون 0.1 على الأقل.");
    }

    [Theory]
    [InlineData("20.1")]
    [InlineData("50")]
    public void Quantity_Should_Fail_When_Greater_Than_Maximum(string quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            D(quantity)
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية لا يمكن أن تتجاوز 20.");
    }

    #endregion

    #region Decimal precision validations

    [Theory]
    [InlineData("0.1")]
    [InlineData("0.5")]
    [InlineData("1.0")]
    [InlineData("2.5")]
    [InlineData("10")]
    public void Quantity_Should_Pass_When_Has_At_Most_One_Decimal(string quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            D(quantity)
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Tenth increment validations
    [Theory]
    [InlineData("1.111")]
    [InlineData("2.551")]
    [InlineData("10.999")]
    [InlineData("0.2221")]
    [InlineData("0.123")]
    [InlineData("2.351")]
    public void Quantity_Should_Fail_When_More_Than_One_Decimal(string quantity)
    {
        var request = new ProductCatalogRequest(ValidId, D(quantity));

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("الكمية يمكن أن تحتوي على منزلتين عشريتين كحد أقصى.");
    }


    [Theory]
    [InlineData("0.1")]
    [InlineData("0.2")]
    [InlineData("0.5")]
    [InlineData("1.0")]
    [InlineData("2.5")]
    [InlineData("10.0")]
    public void Quantity_Should_Pass_When_Multiple_Of_Point_One(string quantity)
    {
        var request = new ProductCatalogRequest(
            ValidId,
            D(quantity)
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
            Quantity: 1.5m
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
