namespace FabricesStoreManagementSystem.DTOs.Expense;

public record ExpenseResponse(
    Guid Id,
    string Message,
    decimal DollarPriceInSyr,
    decimal SyrianAmount,
    decimal DollarAmount,
    DateTime CreatedAt
);