namespace FabricesStoreManagementSystem.Tests.Validations;

public class ProductValidationsTests
{
    private readonly ProductValidations _validator = new();

    private static ProductRequest CreateValidRequest(
        string? name = "قماش قطني",
        string code = "ABC-123",
        string color = "أحمر",
        string unit = "قطعة",
        string? material = "قطن"
    )
    {
        return new ProductRequest(
            Name: name,
            Code: code,
            Color: color,
            Unit: unit,
            Material: material
        );
    }

    #region Name

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_WhenProvidedButEmpty_ShouldHaveValidationError(string name)
    {
        var model = CreateValidRequest(name: name);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("Product")]
    [InlineData("قماش@")]
    public void Name_WithInvalidCharacters_ShouldHaveValidationError(string name)
    {
        var model = CreateValidRequest(name: name);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Name_WhenNull_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest(name: null);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Code

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Code_WhenEmpty_ShouldHaveValidationError(string code)
    {
        var model = CreateValidRequest(code: code);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Code_WithValidValue_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest(code: "ABC-123_X");

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    #endregion

    #region Color

    [Theory]
    [InlineData("")]
    public void Color_WithInvalidValue_ShouldHaveValidationError(string color)
    {
        var model = CreateValidRequest(color: color);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void Color_WithArabicValue_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest(color: "أزرق");

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }

    #endregion

    #region Unit

    [Theory]
    [InlineData("")]
    [InlineData("box")]
    [InlineData("قطعة1")]
    public void Unit_WithInvalidValue_ShouldHaveValidationError(string unit)
    {
        var model = CreateValidRequest(unit: unit);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Unit);
    }

    [Theory]
    [InlineData("قطعة")]
    [InlineData("كيلوغرام")]
    [InlineData("متر")]
    public void Unit_WithValidPredefinedValue_ShouldNotHaveValidationError(string unit)
    {
        var model = CreateValidRequest(unit: unit);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Unit);
    }

    #endregion

    #region Material

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Material_WhenProvidedButEmpty_ShouldHaveValidationError(string material)
    {
        var model = CreateValidRequest(material: material);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Material);
    }

    [Theory]
    [InlineData("Cotton")]
    [InlineData("قطن1")]
    public void Material_WithInvalidCharacters_ShouldHaveValidationError(string material)
    {
        var model = CreateValidRequest(material: material);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Material_WhenNull_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest(material: null);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Material);
    }

    #endregion

    #region Happy Path

    [Fact]
    public void ProductRequest_WithAllValidValues_ShouldPassValidation()
    {
        var model = CreateValidRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
