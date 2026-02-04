namespace FabricesStoreManagementSystem.Tests.Validations;

public class DateRangeValidationsTests
{
    private readonly DateRangeValidations _validator = new();

    private static DateOnly Today =>
        DateOnly.FromDateTime(DateTime.UtcNow);

    #region From validations

    [Theory]
    [InlineData(1)] // future date
    [InlineData(10)]
    public void From_Should_Fail_When_In_The_Future(int daysInFuture)
    {
        var request = new DateRangeRequest(
            From: Today.AddDays(daysInFuture),
            To: Today
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.From)
              .WithErrorMessage("تاريخ البداية لا يمكن أن يكون في المستقبل.");
    }

    [Fact]
    public void From_Should_Fail_When_Default()
    {
        var request = new DateRangeRequest(
            From: default,
            To: Today
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.From);
    }

    #endregion

    #region To validations

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    public void To_Should_Fail_When_In_The_Future(int daysInFuture)
    {
        var request = new DateRangeRequest(
            From: Today,
            To: Today.AddDays(daysInFuture)
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.To)
              .WithErrorMessage("تاريخ النهاية لا يمكن أن يكون في المستقبل.");
    }

    [Fact]
    public void To_Should_Fail_When_Default()
    {
        var request = new DateRangeRequest(
            From: Today,
            To: default
        );

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.To);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void To_Should_Fail_When_Before_From(int daysDifference)
    {
        var from = Today;
        var to = from.AddDays(daysDifference);

        var request = new DateRangeRequest(from, to);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.To)
              .WithErrorMessage("تاريخ النهاية يجب أن يكون في نفس يوم أو بعد تاريخ البداية.");
    }

    #endregion

    #region Range length validations

    [Theory]
    [InlineData(366)]
    [InlineData(400)]
    public void DateRange_Should_Fail_When_Exceeds_One_Year(int rangeDays)
    {
        var from = Today.AddDays(-rangeDays);
        var to = Today;

        var request = new DateRangeRequest(from, to);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.To)
              .WithErrorMessage("نطاق التاريخ لا يمكن أن يتجاوز سنة واحدة.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(30)]
    [InlineData(365)]
    public void DateRange_Should_Be_Valid_When_Within_One_Year(int rangeDays)
    {
        var from = Today.AddDays(-rangeDays);
        var to = Today;

        var request = new DateRangeRequest(from, to);

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Happy path

    [Fact]
    public void Should_Pass_When_Dates_Are_Valid()
    {
        var request = new DateRangeRequest(
            From: Today.AddDays(-7),
            To: Today
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
