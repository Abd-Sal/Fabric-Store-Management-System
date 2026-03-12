namespace FabricesStoreManagementSystem.Tests.Validations;

public class ExpenseValidationsTests
{
    private readonly ExpenseValidations _validator = new();

    [Fact]
    public void Should_Fail_When_Message_Is_Empty()
    {
        var request = new ExpenseRequest("", 1000.123m, 5000.123m);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "وصف المصروف مطلوب.");
    }

    [Fact]
    public void Should_Fail_When_Message_Exceeds_100_Characters()
    {
        var message = new string('a', 101);
        var request = new ExpenseRequest(message, 1000.123m, 5000.123m);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "وصف المصروف يجب ألا يتجاوز 100 حرف.");
    }

    [Fact]
    public void Should_Fail_When_DollarPrice_Is_Zero_Or_Less()
    {
        var request = new ExpenseRequest("شراء قماش", 0m, 5000.123m);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "سعر الدولار بالليرة السورية يجب أن يكون أكبر من صفر.");
    }

    [Fact]
    public void Should_Fail_When_DollarPrice_Has_More_Than_3_Decimals()
    {
        var request = new ExpenseRequest("شراء قماش", 1000.1234m, 5000.123m);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "سعر الدولار يجب ألا يحتوي على أكثر من 3 أرقام بعد الفاصلة.");
    }

    [Fact]
    public void Should_Fail_When_SyrianAmount_Is_Zero_Or_Less()
    {
        var request = new ExpenseRequest("شراء قماش", 1000.123m, 0m);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "المبلغ بالليرة السورية يجب أن يكون أكبر من صفر.");
    }

    [Fact]
    public void Should_Fail_When_SyrianAmount_Has_More_Than_3_Decimals()
    {
        var request = new ExpenseRequest("شراء قماش", 1000.123m, 5000.1234m);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "المبلغ يجب ألا يحتوي على أكثر من 3 أرقام بعد الفاصلة.");
    }

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var request = new ExpenseRequest("شراء قماش", 1000.123m, 5000.123m);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}