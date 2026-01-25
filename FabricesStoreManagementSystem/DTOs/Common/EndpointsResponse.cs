namespace FabricesStoreManagementSystem.DTOs.Common;

public record EndpointsResponse(
    (string url, string description)[] EndpointsDetails
);