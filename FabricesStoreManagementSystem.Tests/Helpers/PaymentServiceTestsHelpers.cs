namespace FabricesStoreManagementSystem.Tests.Helpers;

public class PaymentServiceTestsHelpers
{
    public static IEnumerable<object[]> GetPaymentsTestsData()
    {
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            null,
            Guid.Parse("f8deb235-bf79-410c-b0d0-9470c0105e59")
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
            Guid.Parse("f8deb235-bf79-410c-b0d0-9470c0105e59")
        };
    }
}
