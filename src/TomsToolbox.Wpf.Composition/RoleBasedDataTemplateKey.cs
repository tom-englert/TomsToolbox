namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Windows;

    using HashCode = Essentials.HashCode;

    /// <summary>
    /// A resource key for data templates, like the <see cref="DataTemplateKey"/>, but adding a <see cref="Role"/> property to distinguish
    /// several data templates for different roles.
    /// </summary>
    /// <remarks>
    /// Mainly used in conjunction with the <see cref="RoleBasedDataTemplateSelector"/>.
    /// </remarks>
    public class RoleBasedDataTemplateKey : TemplateKey, IEquatable<RoleBasedDataTemplateKey>
    {
        private object? _role;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBasedDataTemplateKey"/> class.
        /// </summary>
        public RoleBasedDataTemplateKey()
            : base(TemplateType.DataTemplate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleBasedDataTemplateKey"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="role">The role.</param>
        public RoleBasedDataTemplateKey(object? dataType, object? role)
            : base(TemplateType.DataTemplate, dataType)
        {
            _role = role;
        }

        /// <summary>
        /// Gets or sets the role. The role is immutable and can be set only once.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Object is immutable.</exception>
        public object? Role
        {
            get => _role;
            set
            {
                if ((_role != null) && (_role != value))
                    throw new InvalidOperationException("Object is immutable.");

                _role = value;
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(RoleBasedDataTemplateKey? other)
        {
            return (other != null) && base.Equals(other) && Equals(_role, other._role);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? o)
        {
            return Equals(o as RoleBasedDataTemplateKey);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode : Must be settable by property to use it in XAML; during lifetime it's immutable.
            return HashCode.Aggregate(base.GetHashCode(), (_role ?? 0).GetHashCode());
        }
    }
}
