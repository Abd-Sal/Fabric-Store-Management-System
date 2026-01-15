namespace FabricesStoreManagementSystem.Mapping;

public static class CustomerMapper
{
    public static CustomerResponse ToCustomerResponse(this Customer customer)
        => new CustomerResponse(
                customer.Id, customer.FirstName,
                customer.LastName, customer.Email,
                customer.Phone, customer.Address,
                customer.IsActive, customer.CreatedAt
            );
}
