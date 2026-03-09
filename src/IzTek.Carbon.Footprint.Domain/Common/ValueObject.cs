namespace IzTek.Carbon.Footprint.Domain.Common;

/// <summary>
/// Base class for Value Objects in Domain-Driven Design
/// Value Objects are immutable objects that are defined by their attributes rather than a unique identifier
/// </summary>
/// <remarks>
/// Key characteristics of Value Objects:
/// - No identity (no ID)
/// - Immutable (properties should be init-only or readonly)
/// - Equality based on values, not reference
/// - Can be freely replaced with another instance having the same values
///
/// Example usage:
/// <code>
/// public class Address : ValueObject
/// {
///     public string Street { get; init; }
///     public string City { get; init; }
///
///     protected override IEnumerable&lt;object&gt; GetEqualityComponents()
///     {
///         yield return Street;
///         yield return City;
///     }
/// }
/// </code>
/// </remarks>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the components that define equality for this value object
    /// Components will be compared in the order they are returned
    /// </summary>
    /// <returns>Enumerable of components to compare for equality</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether two value objects are equal
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return Equals(other);
    }

    /// <summary>
    /// Determines whether two value objects are equal
    /// Implements IEquatable for better performance (avoids boxing)
    /// </summary>
    public bool Equals(ValueObject? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (other.GetType() != GetType())
            return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns a hash code for this value object
    /// Uses a proper hash combining algorithm to avoid collisions
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            // Use a prime number based hash code combination
            // This is more robust than XOR and handles order correctly
            int hash = 17;

            foreach (var component in GetEqualityComponents())
            {
                hash = hash * 31 + (component?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Creates a shallow copy of the value object
    /// Useful for creating modified versions with record-like syntax
    /// </summary>
    protected T ShallowCopy<T>() where T : ValueObject
    {
        return (T)MemberwiseClone();
    }
}