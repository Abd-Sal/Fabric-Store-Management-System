namespace FabricesStoreManagementSystem.Tests.Implementations;

public class ExpenseServiceTests
{
    [Theory]
    [MemberData(nameof(ExpenseServiceTestsHelpers.GetExpenseCreateTestData), MemberType = typeof(ExpenseServiceTestsHelpers))]
    public async Task CreateExpense_ShouldSccess
        (ExpenseRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ExpenseService>.Instance;
        var service = new ExpenseService(db, logger);
        
        //Act
        var result = await service.CreateExpense(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var expense = await db.Expenses.SingleOrDefaultAsync(x => x.Id == result.Value.Id);
        expense.Should().NotBe(null);
        expense.Message.Should().Be(request.Message);
    }

    [Theory]
    [MemberData(nameof(ExpenseServiceTestsHelpers.GetExpenseSuccessTestData), MemberType = typeof(ExpenseServiceTestsHelpers))]
    public async Task GetExpense_ShouldSuccess
        (Guid id)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ExpenseService>.Instance;
        var service = new ExpenseService(db, logger);

        await db.Expenses.AddRangeAsync(ExpensesRepo.Expenses());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetExpense(id);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
    }

    [Theory]
    [MemberData(nameof(ExpenseServiceTestsHelpers.GetExpenseFailTestData), MemberType = typeof(ExpenseServiceTestsHelpers))]
    public async Task GetExpense_ShouldFail
        (Guid id, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ExpenseService>.Instance;
        var service = new ExpenseService(db, logger);

        await db.Expenses.AddRangeAsync(ExpensesRepo.Expenses());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetExpense(id);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(ExpenseServiceTestsHelpers.GetExpenseTestData), MemberType = typeof(ExpenseServiceTestsHelpers))]
    public async Task GetExpenses_ShouldSuccess
        (PaginationRequest paginationRequest, DateRangeRequest dateRangeRequest, SortRequest sortRequest, SearchRequest searchRequest)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ExpenseService>.Instance;
        var service = new ExpenseService(db, logger);

        await db.Expenses.AddRangeAsync(ExpensesRepo.Expenses());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetExpenses(paginationRequest, dateRangeRequest, sortRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ExpenseServiceTestsHelpers.GetExpenseRemoveSuccessTestData), MemberType = typeof(ExpenseServiceTestsHelpers))]
    public async Task RemoveExpense_ShouldSuccess
        (Guid id)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ExpenseService>.Instance;
        var service = new ExpenseService(db, logger);

        await db.Expenses.AddRangeAsync(ExpensesRepo.Expenses());
        await db.Payments.AddRangeAsync(PaymentsRepo.Payments());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemoveExpense(id);
        if(result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ExpenseServiceTestsHelpers.GetExpenseRemoveFailTestData), MemberType = typeof(ExpenseServiceTestsHelpers))]
    public async Task RemoveExpense_ShouldFail
        (Guid id, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ExpenseService>.Instance;
        var service = new ExpenseService(db, logger);

        await db.Expenses.AddRangeAsync(ExpensesRepo.Expenses());
        await db.Payments.AddRangeAsync(PaymentsRepo.Payments());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemoveExpense(id);
        if(result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
