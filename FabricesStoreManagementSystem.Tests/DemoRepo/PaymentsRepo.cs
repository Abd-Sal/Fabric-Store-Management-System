namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class PaymentsRepo
{
    public static List<Payment> Payments() => new List<Payment>()
    {
        new Payment { Id = Guid.Parse("25dd77e7-fa55-4795-8e81-557cfdd2fb08"), Amount = ExpensesRepo.Expenses()[0].SyrianAmount, PayMethod = PaymentMethod.Cash, ReferenceID = ExpensesRepo.Expenses()[0].Id, ReferenceType = ReferenceTypes.Expense, PaidAt = DateTime.Parse("2026-01-12")},
        new Payment { Id = Guid.Parse("67342bb8-1b52-4f70-8c9b-184f932a8eec"), Amount = ExpensesRepo.Expenses()[1].SyrianAmount, PayMethod = PaymentMethod.Cash, ReferenceID = ExpensesRepo.Expenses()[1].Id, ReferenceType = ReferenceTypes.Expense, PaidAt = DateTime.Parse("2026-02-12")},
        new Payment { Id = Guid.Parse("4c1e91d7-d431-4f8a-bb86-91ae425d4ee4"), Amount = ExpensesRepo.Expenses()[2].SyrianAmount, PayMethod = PaymentMethod.Cash, ReferenceID = ExpensesRepo.Expenses()[2].Id, ReferenceType = ReferenceTypes.Expense, PaidAt = DateTime.Parse("2026-03-12")},
    };
}
