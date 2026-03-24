namespace IzTek.Carbon.Footprint.Application.Common.Models;

public abstract class ErrorCode(int code, string name, string service) : IEquatable<ErrorCode>
{
    public int Code { get; } = code;
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public string Service { get; } = service ?? throw new ArgumentNullException(nameof(service));

    public static implicit operator int(ErrorCode errorCode) => errorCode.Code;

    public static implicit operator string(ErrorCode errorCode) => errorCode.Name;

    public bool Equals(ErrorCode? other) => other is not null && (ReferenceEquals(this, other) || (Code == other.Code && Name == other.Name && Service == other.Service));

    public override bool Equals(object? obj) => Equals(obj as ErrorCode);

    public override int GetHashCode() => HashCode.Combine(Code, Name, Service);

    public static bool operator ==(ErrorCode left, ErrorCode right)
        => ReferenceEquals(left, right) || (left?.Equals(right) ?? false);

    public static bool operator !=(ErrorCode left, ErrorCode right) => !(left == right);

    public override string ToString() => $"{Service}.{Name} ({Code})";
}

public abstract class ServiceErrorCode(int code, string name, string service) : ErrorCode(code, name, service)
{
}
