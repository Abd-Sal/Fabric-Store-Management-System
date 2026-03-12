namespace FabricesStoreManagementSystem.DTOs.Expense;

public record ExpenseRequest(
    string Message,
    decimal DollarPriceInSyr,
    decimal SyrianAmount
);