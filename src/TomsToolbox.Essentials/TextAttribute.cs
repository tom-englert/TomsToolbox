namespace TomsToolbox.Essentials;

using System;
using System.ComponentModel;

/// <summary>
/// Specifies a general usable attribute to associate text with an object, 
/// similar to  <see cref="DisplayNameAttribute"/> or <see cref="DescriptionAttribute"/>, but without a predefined usage scope.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class TextAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextAttribute"/> class.
    /// </summary>
    /// <param name="key">A user defined key to classify the usage of this text.</param>
    public TextAttribute(object key)
    {
        Key = key;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextAttribute"/> class.
    /// </summary>
    /// <param name="key">A user defined key to classify the usage of this text.</param>
    /// <param name="text">The text.</param>
    public TextAttribute(object key, string text)
        : this(key)
    {
        TextValue = text;
    }

    /// <summary>
    /// Gets the key that classifies the usage of this text.
    /// </summary>
    public object Key { get; }

    /// <summary>
    /// Gets the text associated with this attribute.
    /// </summary>
    public virtual string? Text => TextValue;

    /// <summary>
    /// Gets or sets the text to be returned by the Text property.
    /// </summary>
    protected string? TextValue
    {
        get;
        set;
    }
}