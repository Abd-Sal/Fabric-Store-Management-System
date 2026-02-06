namespace FabricesStoreManagementSystem.Mapping;

public static class PaymentMapper
{
    public static PaymentResponse ToPaymentResponse(this Payment payment)
        => new PaymentResponse(payment.Id, payment.ReferenceID, payment.ReferenceType, payment.Amount, payment.PayMethod, payment.PaidAt);
}
