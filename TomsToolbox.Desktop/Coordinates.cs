namespace TomsToolbox.Desktop
{
    using System;
    using System.Windows;

    using TomsToolbox.Core;

    /// <summary>
    /// WGS-84 coordinates in degrees.
    /// </summary>
    public struct Coordinates : IEquatable<Coordinates>
    {
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;


        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> structure.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public Coordinates(double latitude, double longitude)
            : this()
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Gets or sets the latitude in degrees.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets or sets the longitude in degrees.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Converts a point from WGS-84 coordinates (in degrees) into logical XY coordinates in the range 0..1.
        /// </summary>
        /// <param name="coordinates">The WGS-84 coordinates.</param>
        /// <returns>The logical coordinates</returns>
        public static implicit operator Point(Coordinates coordinates)
        {
            return CoordinatesToPoint(coordinates);
        }

        /// <summary>
        /// Converts a point from logical coordinates in the range 0..1 into latitude/longitude WGS-84 coordinates (in degrees).
        /// </summary>
        /// <param name="point">The logical coordinates.</param>
        /// <returns>The WGS-84 coordinates</returns>
        public static implicit operator Coordinates(Point point)
        {
            return PointToCoordinates(point);
        }

        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees) into logical XY coordinates in the range 0..1
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>The logical point</returns>
        public static Point CoordinatesToPoint(Coordinates coordinates)
        {
            var latitude = coordinates.Latitude.Clip(MinLatitude, MaxLatitude);
            var longitude = coordinates.Longitude.Clip(MinLongitude, MaxLongitude);

            var x = (longitude + 180) / 360;
            var sinLatitude = Math.Sin(latitude * Math.PI / 180);
            var y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            return new Point(x.Clip(0, 1), y.Clip(0, 1));
        }

        /// <summary>
        /// Converts a point from logical coordinates in the range 0..1 into latitude/longitude WGS-84 coordinates (in degrees).
        /// </summary>
        /// <param name="value">The logical point value.</param>
        /// <returns>The coordinates</returns>
        public static Coordinates PointToCoordinates(Point value)
        {
            var x = value.X.Clip(0, 1) - 0.5;
            var y = 0.5 - value.Y.Clip(0, 1);

            var latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            var longitude = 360 * x;

            return new Coordinates(latitude, longitude);
        }



        #region IEquatable implementation

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Latitude + Longitude).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && Equals((Coordinates)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Coordinates"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Coordinates"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Coordinates"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(Coordinates other)
        {
            return InternalEquals(this, other);
        }

        private static bool InternalEquals(Coordinates left, Coordinates right)
        {
            return Math.Abs(left.Latitude - right.Latitude) < double.Epsilon
                   && Math.Abs(left.Longitude - right.Longitude) < double.Epsilon;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Coordinates left, Coordinates right)
        {
            return InternalEquals(left, right);
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Coordinates left, Coordinates right)
        {
            return !InternalEquals(left, right);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return new Point(Latitude, Longitude).ToString();
        }
    }
}
