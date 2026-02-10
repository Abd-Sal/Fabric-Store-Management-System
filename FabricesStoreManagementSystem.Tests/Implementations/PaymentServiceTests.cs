namespace FabricesStoreManagementSystem.Tests.Implementations;

public class PaymentServiceTests
{
    [Theory]
    [MemberData(nameof(PaymentServiceTestsHelpers.GetPaymentsTestsData), MemberType = typeof(PaymentServiceTestsHelpers))]
    public async Task GetPayments_shouldSuccess
        (PaginationRequest paginationRequest, DateRangeRequest dateRangeRequest, Guid searchReferanceID)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PaymentService>.Instance;
        var sevice = new PaymentService(db, logger);

        //Act
        var result = await sevice.GetPayments(paginationRequest, dateRangeRequest, searchReferanceID);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}
