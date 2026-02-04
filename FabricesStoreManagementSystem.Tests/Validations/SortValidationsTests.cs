namespace FabricesStoreManagementSystem.Tests.Validations;

public class SortValidationsTests
{
    private readonly SortValidations _validator = new();

    private static SortRequest CreateRequest(
        string? sortColumn = null,
        string? sortDir = null)
    {
        return new SortRequest(sortColumn, sortDir);
    }

    #region SortColumn

    [Theory]
    [InlineData("1column")]
    [InlineData("column-name")]
    [InlineData("column name")]
    [InlineData("column@")]
    public void SortColumn_WithInvalidFormat_ShouldHaveValidationError(string column)
    {
        var model = CreateRequest(sortColumn: column);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SortColumn);
    }

    [Fact]
    public void SortColumn_ExceedingMaxLength_ShouldHaveValidationError()
    {
        var column = new string('a', 51);
        var model = CreateRequest(sortColumn: column);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SortColumn);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("createdAt")]
    [InlineData("_id")]
    [InlineData("column_1")]
    public void SortColumn_WithValidValue_ShouldNotHaveValidationError(string column)
    {
        var model = CreateRequest(sortColumn: column);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SortColumn);
    }

    [Fact]
    public void SortColumn_WhenNull_ShouldNotHaveValidationError()
    {
        var model = CreateRequest(sortColumn: null);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SortColumn);
    }

    #endregion

    #region SortDir

    [Theory]
    [InlineData("up")]
    [InlineData("down")]
    [InlineData("ascendingg")]
    [InlineData("descend")]
    public void SortDir_WithInvalidValue_ShouldHaveValidationError(string dir)
    {
        var model = CreateRequest(sortDir: dir);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SortDir);
    }

    [Theory]
    [InlineData("asc")]
    [InlineData("desc")]
    [InlineData("ASC")]
    [InlineData("descending")]
    [InlineData("Ascending")]
    public void SortDir_WithValidValue_ShouldNotHaveValidationError(string dir)
    {
        var model = CreateRequest(sortDir: dir);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SortDir);
    }

    [Fact]
    public void SortDir_WhenNull_ShouldNotHaveValidationError()
    {
        var model = CreateRequest(sortDir: null);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SortDir);
    }

    #endregion

    #region Cross Validation

    [Fact]
    public void SortDir_ProvidedWithoutSortColumn_ShouldHaveCrossValidationError()
    {
        var model = CreateRequest(
            sortColumn: null,
            sortDir: "asc"
        );

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("SortConsistency");
    }

    [Fact]
    public void SortDir_WithSortColumn_ShouldNotHaveCrossValidationError()
    {
        var model = CreateRequest(
            sortColumn: "name",
            sortDir: "desc"
        );

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor("SortConsistency");
    }

    #endregion

    #region Happy Path

    [Fact]
    public void EmptySortRequest_ShouldPassValidation()
    {
        var model = CreateRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidSortColumnAndDirection_ShouldPassValidation()
    {
        var model = CreateRequest(
            sortColumn: "createdAt",
            sortDir: "asc"
        );

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
