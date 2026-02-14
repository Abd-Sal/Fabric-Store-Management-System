namespace FabricesStoreManagementSystem.Abstraction;

public class Error : IComparable<Error>, IEquatable<Error>
{
    public static Error None = new Error(string.Empty, string.Empty, null);

    public Error(string code, string description, int? statusCode)
    {
        Code = code;
        Description = description;
        StatusCode = statusCode;
    }

    public string Code { get; }
    public string Description { get; }
    public int? StatusCode { get; } = null;

    public bool Equals(Error? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Code == other.Code &&
               Description == other.Description &&
               StatusCode == other.StatusCode;
    }

    public override bool Equals(object? obj) => Equals(obj as Error);

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Description, StatusCode);
    }

    public static bool operator ==(Error? left, Error? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Error? left, Error? right) => !(left == right);

    public int CompareTo(Error? other)
    {
        if (other is null)
            return 1;

        var codeComparison = string.Compare(Code, other.Code, StringComparison.OrdinalIgnoreCase);
        if (codeComparison != 0)
            return codeComparison;

        if (StatusCode is null && other.StatusCode is null)
            return 0;
        if (StatusCode is null)
            return -1;
        if (other.StatusCode is null)
            return 1;

        var statusCodeComparison = StatusCode.Value.CompareTo(other.StatusCode.Value);
        if (statusCodeComparison != 0)
            return statusCodeComparison;

        return string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);
    }
}