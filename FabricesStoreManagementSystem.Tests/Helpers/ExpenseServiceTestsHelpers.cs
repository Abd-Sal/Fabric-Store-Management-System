namespace FabricesStoreManagementSystem.Tests.Helpers;

public class ExpenseServiceTestsHelpers
{
    public static IEnumerable<object[]> GetExpenseCreateTestData()
    {
        yield return new object[] { new ExpenseRequest("test 4", 112.2m, 100m) };
        yield return new object[] { new ExpenseRequest("test 5", 112.812m, 100m) };
    }
    
    public static IEnumerable<object[]> GetExpenseSuccessTestData()
    {
        yield return new object[] { ExpensesRepo.Expenses()[0].Id };
        yield return new object[] { ExpensesRepo.Expenses()[2].Id };
    }

    public static IEnumerable<object[]> GetExpenseFailTestData()
    {
        yield return new object[] { Guid.Parse("a192fa42-e9b4-4b96-90ef-676fcacd38e6"), ExpenseErrors.NotFound };
        yield return new object[] { Guid.Parse("825f06eb-f8fd-4073-acfe-4813f0f058cf"), ExpenseErrors.NotFound };
    }
        
    public static IEnumerable<object[]> GetExpenseTestData()
    {
        yield return new object[] {
            new PaginationRequest(1, 10),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            new SortRequest("code", "desc"),
            new SearchRequest("code", "P-00")
        };
        yield return new object[] {
            new PaginationRequest(1, 10),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            new SortRequest("message", "desc"),
            new SearchRequest("code", "P-00")
        };
        yield return new object[] {
            new PaginationRequest(1, 10),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            new SortRequest("id", "asc"),
            new SearchRequest("id", "P-00")
        };
    }

    public static IEnumerable<object[]> GetExpenseRemoveSuccessTestData()
    {
        yield return new object[] { ExpensesRepo.Expenses()[0].Id };
    }

    public static IEnumerable<object[]> GetExpenseRemoveFailTestData()
    {
        yield return new object[] { Guid.Parse("ef4a2919-f595-4996-99d2-6c31680811b2"), ExpenseErrors.NotFound };
        yield return new object[] { ExpensesRepo.Expenses()[1].Id, ExpenseErrors.ExpenseIsVeryOld };
    }
}
