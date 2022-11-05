namespace TomsToolbox.Essentials;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// A class to parse and compare semantic versions. (<see href="https://semver.org/"/>)
/// </summary>
public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>
{
    private static readonly Regex _versionRegex = new(@"(\d+)(.\d+)(.\d+)?(.\d+)?([-+]\S+)?");

    /// <summary>
    /// Initializes a new default instance of the <see cref="SemanticVersion"/> class.
    /// </summary>
    public SemanticVersion() : this(new Version(), string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="suffix">The suffix, e.g. -beta23.</param>
    public SemanticVersion(Version version, string suffix)
    {
        Version = version;
        Suffix = suffix;
    }

    /// <summary>
    /// Gets the version.
    /// </summary>
    public Version Version { get; }
    /// <summary>
    /// Gets the suffix.
    /// </summary>
    public string Suffix { get; }

    /// <summary>
    /// Parses the specified semantic version.
    /// Any prefix or suffix strings are ignored, e.g. "Version 1.2.3-beta1 with extras" is parsed as "1.2.3-beta1"
    /// </summary>
    /// <param name="input">The version string to parse</param>
    /// <returns>The first version found in the string, or the default if no version could be detected.</returns>
    public static SemanticVersion Parse(string? input)
    {
        if (input is null)
            return new SemanticVersion();

        var match = _versionRegex.Match(input);
        if (match.Success)
        {
            var captures = match.Groups;
            var versionPart = string.Join("", captures.Cast<Group>().Select(group => group.Value).Skip(1).Take(4));
            var version = new Version(versionPart);

            return new SemanticVersion(version, captures[5].Value);
        }

        return new SemanticVersion();
    }

    /// <inheritdoc />
    public bool Equals(SemanticVersion? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Equals(Version, other.Version) && string.Equals(Suffix, other.Suffix, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((SemanticVersion)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Version.GetHashCode() * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(Suffix);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Version + Suffix;
    }

    /// <inheritdoc />
    public int CompareTo(SemanticVersion? other)
    {
        if (ReferenceEquals(this, other))
            return 0;
        if (other is null)
            return 1;

        return Compare(this, other);
    }

    private static int Compare(SemanticVersion left, SemanticVersion right)
    {
        var versionComparison = Comparer<Version>.Default.Compare(left.Version, right.Version);
        if (versionComparison != 0)
            return versionComparison;

        return string.Compare(Suffix4Compare(left), Suffix4Compare(right), StringComparison.OrdinalIgnoreCase);
    }

    private static string Suffix4Compare(SemanticVersion version)
    {
        var suffix = version.Suffix;

        if (string.IsNullOrEmpty(suffix))
            return "z"; // version without suffix is newer/better than with suffix.

        return suffix;
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(SemanticVersion left, SemanticVersion right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(SemanticVersion left, SemanticVersion right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Implements the operator &gt;.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator >(SemanticVersion left, SemanticVersion right)
    {
        return Compare(left, right) > 0;
    }

    /// <summary>
    /// Implements the operator &lt;.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator <(SemanticVersion left, SemanticVersion right)
    {
        return Compare(left, right) < 0;
    }

    /// <summary>
    /// Implements the operator &gt;=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator >=(SemanticVersion left, SemanticVersion right)
    {
        return Compare(left, right) >= 0;
    }

    /// <summary>
    /// Implements the operator &lt;=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator <=(SemanticVersion left, SemanticVersion right)
    {
        return Compare(left, right) <= 0;
    }
}
