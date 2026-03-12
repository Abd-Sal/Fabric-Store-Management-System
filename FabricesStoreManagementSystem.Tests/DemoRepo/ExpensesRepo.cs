namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class ExpensesRepo
{
    public static List<Expense> Expenses() => new List<Expense>()
    {
        new Expense { Id = Guid.Parse("a192dbc3-51ae-4725-a752-b08a9b38005b"), Message = "test 1", SyrianAmount = 1000, DollarPriceInSyr = 118.2m, CreatedAt = DateTime.Parse("2026-03-12")},
        new Expense { Id = Guid.Parse("5ec37521-fa15-48c3-8c09-4422e73bf348"), Message = "test 2", SyrianAmount = 200, DollarPriceInSyr = 110m, CreatedAt = DateTime.Parse("2026-01-12")},
        new Expense { Id = Guid.Parse("df86801f-30e6-4302-a7af-fe50c844f06d"), Message = "test 3", SyrianAmount = 100, DollarPriceInSyr = 115.3m, CreatedAt = DateTime.Parse("2026-02-12")},
    };
}
