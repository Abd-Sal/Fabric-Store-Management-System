namespace FabricesStoreManagementSystem.Tests.Validations;

public class PaginationValidationsTests
{
    private readonly PaginationValidations _validator = new();

    #region Page validations

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Page_Should_Fail_When_Less_Than_Min(int page)
    {
        var request = new PaginationRequest(
            Page: page,
            PageSize: 10
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Page)
              .WithErrorMessage("رقم الصفحة يجب أن يكون 1 على الأقل.");
    }

    [Theory]
    [InlineData(1001)]
    [InlineData(2000)]
    public void Page_Should_Fail_When_Greater_Than_Max(int page)
    {
        var request = new PaginationRequest(
            Page: page,
            PageSize: 10
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Page)
              .WithErrorMessage("رقم الصفحة لا يمكن أن يتجاوز 1000.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(1000)]
    public void Page_Should_Pass_When_Within_Valid_Range(int page)
    {
        var request = new PaginationRequest(
            Page: page,
            PageSize: 10
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Page);
    }

    #endregion

    #region PageSize validations

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-20)]
    public void PageSize_Should_Fail_When_Less_Than_Min(int pageSize)
    {
        var request = new PaginationRequest(
            Page: 1,
            PageSize: pageSize
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
              .WithErrorMessage("حجم الصفحة يجب أن يكون 1 على الأقل.");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(500)]
    public void PageSize_Should_Fail_When_Greater_Than_Max(int pageSize)
    {
        var request = new PaginationRequest(
            Page: 1,
            PageSize: pageSize
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
              .WithErrorMessage("حجم الصفحة لا يمكن أن يتجاوز 100.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void PageSize_Should_Pass_When_Within_Valid_Range(int pageSize)
    {
        var request = new PaginationRequest(
            Page: 1,
            PageSize: pageSize
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    #endregion

    #region Combined / happy path

    [Fact]
    public void Should_Pass_When_Page_And_PageSize_Are_Valid()
    {
        var request = new PaginationRequest(
            Page: 1,
            PageSize: 10
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_With_Default_Values()
    {
        var request = new PaginationRequest();

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
