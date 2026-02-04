namespace FabricesStoreManagementSystem.Tests.Validations;

public class AssignCatalogValidationsTests
{
    private readonly AssignCatalogValidations _validator = new();

    private static AssignCatalogRequest CreateValidRequest() => new(
        CustomerID: Guid.NewGuid(),
        CatalogID: Guid.NewGuid()
    );

    #region CustomerID Validation Tests

    [Fact]
    public void CustomerID_WhenEmptyGuid_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { CustomerID = Guid.Empty };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.CustomerID)
            .WithErrorMessage("معرف العميل غير صالح.");
    }

    [Fact]
    public void CustomerID_WhenValid_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.CustomerID);
    }

    #endregion

    #region CatalogID Validation Tests

    [Fact]
    public void CatalogID_WhenEmptyGuid_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { CatalogID = Guid.Empty };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.CatalogID)
            .WithErrorMessage("معرف الكتالوج غير صالح.");
    }

    [Fact]
    public void CatalogID_WhenValid_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.CatalogID);
    }

    #endregion

    #region Business Rule Tests

    [Fact]
    public void Request_WhenCustomerIDEqualsCatalogID_ShouldHaveValidationError()
    {
        var sameId = Guid.NewGuid();
        var request = new AssignCatalogRequest(sameId, sameId);

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("لا يمكن تعيين كتالوج لنفسه.");
    }

    [Fact]
    public void Request_WhenCustomerIDEmptyAndCatalogIDEmpty_ShouldNotHaveSelfAssignmentError()
    {
        var request = new AssignCatalogRequest(Guid.Empty, Guid.Empty);

        var result = _validator.TestValidate(request);

        var hasSelfAssignmentError = result.Errors
            .Any(e => e.ErrorMessage == "لا يمكن تعيين كتالوج لنفسه.");

        Assert.False(hasSelfAssignmentError);
    }

    [Fact]
    public void Request_WhenCustomerIDEmpty_ShouldNotHaveSelfAssignmentError()
    {
        var request = new AssignCatalogRequest(Guid.Empty, Guid.NewGuid());

        var result = _validator.TestValidate(request);

        var hasSelfAssignmentError = result.Errors
            .Any(e => e.ErrorMessage == "لا يمكن تعيين كتالوج لنفسه.");

        Assert.False(hasSelfAssignmentError);
    }

    [Fact]
    public void Request_WhenCatalogIDEmpty_ShouldNotHaveSelfAssignmentError()
    {
        var request = new AssignCatalogRequest(Guid.NewGuid(), Guid.Empty);

        var result = _validator.TestValidate(request);

        var hasSelfAssignmentError = result.Errors
            .Any(e => e.ErrorMessage == "لا يمكن تعيين كتالوج لنفسه.");

        Assert.False(hasSelfAssignmentError);
    }

    [Fact]
    public void Request_WhenDifferentIDs_ShouldNotHaveSelfAssignmentError()
    {
        var request = CreateValidRequest();

        var result = _validator.TestValidate(request);

        var hasSelfAssignmentError = result.Errors
            .Any(e => e.ErrorMessage == "لا يمكن تعيين كتالوج لنفسه.");

        Assert.False(hasSelfAssignmentError);
    }

    #endregion

    #region Complete Valid Request Tests

    [Fact]
    public void Request_WhenAllFieldsValid_ShouldPassAllValidations()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetInvalidRequestTestData))]
    public void Request_WhenInvalid_ShouldHaveValidationErrors(AssignCatalogRequest request)
    {
        var result = _validator.TestValidate(request);

        Assert.NotEmpty(result.Errors);
    }

    public static IEnumerable<object[]> GetInvalidRequestTestData()
    {
        yield return new object[]
        {
            new AssignCatalogRequest(Guid.Empty, Guid.NewGuid())
        };

        yield return new object[]
        {
            new AssignCatalogRequest(Guid.NewGuid(), Guid.Empty)
        };

        yield return new object[]
        {
            new AssignCatalogRequest(Guid.Empty, Guid.Empty)
        };

        var sameId = Guid.NewGuid();
        yield return new object[]
        {
            new AssignCatalogRequest(sameId, sameId)
        };
    }

    [Theory]
    [MemberData(nameof(GetValidRequestTestData))]
    public void Request_WhenValid_ShouldNotHaveValidationErrors(AssignCatalogRequest request)
    {
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidRequestTestData()
    {
        yield return new object[]
        {
            new AssignCatalogRequest(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("22222222-2222-2222-2222-222222222222")
            )
        };

        yield return new object[]
        {
            new AssignCatalogRequest(
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            )
        };

        yield return new object[]
        {
            new AssignCatalogRequest(
                Guid.NewGuid(),
                Guid.NewGuid()
            )
        };
    }

    #endregion

    #region Edge Case Tests
    [Fact]
    public void Request_WithSameNonEmptyGuids_ShouldHaveSelfAssignmentError()
    {
        var sameId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        var request = new AssignCatalogRequest(sameId, sameId);

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("لا يمكن تعيين كتالوج لنفسه.");
    }

    [Fact]
    public void Request_WithEmptyGuids_ShouldHaveMultipleErrors()
    {
        var request = new AssignCatalogRequest(Guid.Empty, Guid.Empty);
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CustomerID);
        result.ShouldHaveValidationErrorFor(x => x.CatalogID);
        Assert.Equal(4, result.Errors.Count);

        // Optional: Verify specific error messages
        var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.Contains("معرف العميل مطلوب.", errorMessages);
        Assert.Contains("معرف العميل غير صالح.", errorMessages);
        Assert.Contains("معرف الكتالوج مطلوب.", errorMessages);
        Assert.Contains("معرف الكتالوج غير صالح.", errorMessages);
    }

    [Fact]
    public void Request_WithNewGuidFormat_ShouldBeValid()
    {
        var request = new AssignCatalogRequest(
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }
    #endregion
}