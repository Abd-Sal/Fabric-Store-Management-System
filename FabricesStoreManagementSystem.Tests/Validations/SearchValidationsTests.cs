namespace FabricesStoreManagementSystem.Tests.Validations;

public class SearchValidationsTests
{
    private readonly SearchValidations _validator = new();

    private static SearchRequest CreateRequest(
        string search = null,
        string searchColumn = null)
    {
        return new SearchRequest(search, searchColumn);
    }

    #region Search

    [Theory]
    [InlineData("a")]
    public void Search_WithLessThanTwoCharacters_ShouldHaveValidationError(string search)
    {
        var model = CreateRequest(search: search);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Search);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Search_WithWhitespaceOnly_ShouldNotHaveValidationError(string search)
    {
        var model = CreateRequest(search: search);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Search);
    }

    [Fact]
    public void Search_ExceedingMaxLength_ShouldHaveValidationError()
    {
        var search = new string('a', 101);
        var model = CreateRequest(search: search, searchColumn: "name");

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Search);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("قماش")]
    [InlineData("  test  ")]
    public void Search_WithValidValue_ShouldNotHaveValidationError(string search)
    {
        var model = CreateRequest(search: search, searchColumn: "name");

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Search);
    }

    [Fact]
    public void Search_WhenNullOrEmpty_ShouldNotHaveValidationError()
    {
        var model = CreateRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Search);
    }

    #endregion

    #region SearchColumn

    [Theory]
    [InlineData("1column")]
    [InlineData("column-name")]
    [InlineData("column name")]
    [InlineData("column@")]
    public void SearchColumn_WithInvalidFormat_ShouldHaveValidationError(string column)
    {
        var model = CreateRequest(search: "test", searchColumn: column);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SearchColumn);
    }

    [Fact]
    public void SearchColumn_ExceedingMaxLength_ShouldHaveValidationError()
    {
        var column = new string('a', 51);
        var model = CreateRequest(search: "test", searchColumn: column);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SearchColumn);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("createdAt")]
    [InlineData("_id")]
    [InlineData("column_1")]
    public void SearchColumn_WithValidValue_ShouldNotHaveValidationError(string column)
    {
        var model = CreateRequest(search: "test", searchColumn: column);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SearchColumn);
    }

    [Fact]
    public void SearchColumn_WhenNull_ShouldNotHaveValidationError()
    {
        var model = CreateRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.SearchColumn);
    }

    #endregion

    #region Cross Validation

    [Fact]
    public void Search_ProvidedWithoutSearchColumn_ShouldHaveConsistencyError()
    {
        var model = CreateRequest(search: "test", searchColumn: null);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("SearchConsistency");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SearchColumn_ProvidedWithInvalidSearch_ShouldHaveConsistencyError(string search)
    {
        var model = CreateRequest(search: search, searchColumn: "name");

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Theory]
    [InlineData("a")]
    public void Search_WithLessThanTwoCharacters_ShouldHaveSearchError(string search)
    {
        var model = CreateRequest(search: search, searchColumn: "name");

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Search);
    }

    [Fact]
    public void Search_And_SearchColumn_Valid_ShouldNotHaveConsistencyErrors()
    {
        var model = CreateRequest(search: "test", searchColumn: "name");

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor("SearchConsistency");
    }

    #endregion

    #region Happy Path

    [Fact]
    public void EmptySearchRequest_ShouldPassValidation()
    {
        var model = CreateRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidSearchRequest_ShouldPassValidation()
    {
        var model = CreateRequest(
            search: "قماش",
            searchColumn: "name"
        );

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
