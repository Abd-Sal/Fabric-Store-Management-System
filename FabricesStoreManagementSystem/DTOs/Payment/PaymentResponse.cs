namespace FabricesStoreManagementSystem.DTOs.Payment;

public record PaymentResponse(
    Guid Id,
    Guid ReferenceID,
    ReferenceTypes ReferenceTypes,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTime PaidAt
);