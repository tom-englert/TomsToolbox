namespace TomsToolbox.Wpf.Tests.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class BinaryOperationConverterTests
    {
        [TestMethod]
        public void BinaryOperationConverter_Add_Double_Integer_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(1.5, null, 2, null);
            Assert.AreEqual(1.5 * 2, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Add_Integer_Integer_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(1, null, 2, null);
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Integer_Double_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(3, null, 2.5, null);
            Assert.AreEqual(3 * 2.5, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Vector_Number_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Vector(1, 2), null, 2.5, null);
            Assert.AreEqual(new Vector(2.5, 5), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Vector_NumberString_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Vector(1, 2), null, "2.5", null);
            Assert.AreEqual(new Vector(2.5, 5), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Vector_VectorString_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Vector(1, 2), null, "2.5,1.5", null);
            Assert.AreEqual(5.5, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Vector_Vector_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Vector(1, 2), null, new Vector(2.5, 1.5), null);
            Assert.AreEqual(5.5, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Vector_Unknown_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Vector(1, 2), null, target, null);
            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Size_Double_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Size(1, 2), null, 2, null);
            Assert.AreEqual(new Vector(2, 4), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Unknown_Number_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(target, null, 2, null);
            Assert.AreEqual(DependencyProperty.UnsetValue, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Add_Point_VectorString_Test()
        {
            var target = BinaryOperationConverter.Addition;
            var result = target.Convert(new Point(2, 5), null, "2,3", null);
            Assert.AreEqual(new Point(4, 8), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Substract_Vector_VectorString_Test()
        {
            var target = BinaryOperationConverter.Subtraction;
            var result = target.Convert(new Vector(2, 5), null, "2,3", null);
            Assert.AreEqual(new Vector(0, 2), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Substract_DateTime_DateTime_Test()
        {
            var target = BinaryOperationConverter.Subtraction;
            var diff = TimeSpan.FromMinutes(10);
            var op1 = DateTime.Now;
            var op2 = op1 - diff;
            var result = target.Convert(op1, null, op2, null);
            Assert.AreEqual(diff, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Vector_MatrixString_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new Vector(2, 5), null, "0,1,-1,0,0,0", null);
            Assert.AreEqual(new Vector(-5, 2), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Rect_MatrixString_Test()
        {
            var target = BinaryOperationConverter.Multiply; // => Transform
            var result = target.Convert(new Rect(0, 0, 2, 5), null, "0,1,-1,0,0,0", null);
            Assert.AreEqual(new Rect(-5, 0, 5, 2), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Add_Rect_VectorString_Test()
        {
            var target = BinaryOperationConverter.Addition; // => Offset
            var result = target.Convert(new Rect(1, 2, 3, 4), null, "1,2", null);
            Assert.AreEqual(new Rect(2, 4, 3, 4), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Rect_String_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(new Rect(1, 2, 3, 4), null, "1,2,3,4", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Double_String_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(3.14, null, "3.14", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Integer_String_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(3, null, "3", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Boolean_String_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(true, null, "true", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Boolean_String2_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(true, null, "false", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_TimeSpan_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(TimeSpan.FromHours(2), null, "2:00:00", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Rect_UnmatchedString_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(new Rect(1, 2, 3, 4), null, "1,4,3,4", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_String_String_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert("Test", null, "Test", null);
            Assert.AreEqual(true, result);
            result = target.Convert("Test1", null, "Test", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Equality_Custom_String_Test()
        {
            var target = BinaryOperationConverter.Equality;
            var result = target.Convert(new TestClass(42), null, "42", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Inequality_Custom_String_Test()
        {
            var target = BinaryOperationConverter.Inequality;
            var result = target.Convert(new TestClass(42), null, "43", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Multiply_Custom_String_Test()
        {
            var target = BinaryOperationConverter.Multiply;
            var result = target.Convert(new TestClass(4), null, "5", null);
            Assert.AreEqual(new TestClass(20), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Division_Custom_String_Test()
        {
            var target = BinaryOperationConverter.Division;
            var result = target.Convert(new TestClass(20), null, "5", null);
            Assert.AreEqual(new TestClass(4), result);
        }

        [TestMethod]
        public void BinaryOperationConverter_LessThan_Custom_String_Test()
        {
            var target = BinaryOperationConverter.LessThan;
            var result = target.Convert(new TestClass(4), null, "5", null);
            Assert.AreEqual(true, result);
            result = target.Convert(new TestClass(5), null, "5", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Inequality_Rect_String_Test()
        {
            var target = BinaryOperationConverter.Inequality;
            var result = target.Convert(new Rect(1, 2, 3, 4), null, "1,2,3,4", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_Inequality_Rect_UnmatchedString_Test()
        {
            var target = BinaryOperationConverter.Inequality;
            var result = target.Convert(new Rect(1, 2, 3, 4), null, "1,4,3,4", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationConverter_GreaterThan_TimeSpan_Test()
        {
            var target = BinaryOperationConverter.GreaterThan;
            var result = target.Convert(TimeSpan.FromHours(2), null, "1:59:00", null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationMultiConverter_GreaterThan_Test()
        {
            var target = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThan };
            var result = target.Convert(new object[] { 3, 2.0 }, null, null, null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void BinaryOperationMultiConverter_NotGreaterThan_Test()
        {
            var target = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThan };
            var result = target.Convert(new object[] { 1, 2.0 }, null, null, null);
            Assert.AreEqual(false, result);
        }


        [TestMethod]
        public void BinaryOperationMultiConverter_GreaterThanOrEqual_Test()
        {
            var target = new BinaryOperationConverter { Operation = BinaryOperation.GreaterThanOrEqual };
            var result = target.Convert(new object[] { 2, 2.0 }, null, null, null);
            Assert.AreEqual(true, result);
        }

        class TestClassTypeConverter : TypeConverter
        {
            [CanBeNull]
            public override object? ConvertFrom([CanBeNull] ITypeDescriptorContext context, CultureInfo culture, [CanBeNull] object value)
            {
                return new TestClass(Convert.ToInt32(value, culture));
            }

            [CanBeNull]
            public override object? ConvertTo([CanBeNull] ITypeDescriptorContext context, [CanBeNull] CultureInfo culture, [CanBeNull] object value, Type destinationType)
            {
                return value?.ToString();
            }
        }

        [TypeConverter(typeof(TestClassTypeConverter))]
        struct TestClass : IEquatable<TestClass>
        {
            private readonly int _value;

            public TestClass(int value) : this()
            {
                _value = value;
            }

            public static TestClass operator *(TestClass x, int y)
            {
                return new TestClass(x._value * y);
            }
            public static TestClass operator /(TestClass x, int y)
            {
                return new TestClass(x._value / y);
            }

            public static bool operator >(TestClass x, TestClass y)
            {
                return x._value > y._value;
            }
            public static bool operator <(TestClass x, TestClass y)
            {
                return x._value < y._value;
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
                return _value;
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
            public override bool Equals([CanBeNull] object obj)
            {
                if (!(obj is TestClass))
                    return false;

                return Equals((TestClass)obj);
            }

            /// <summary>
            /// Determines whether the specified <see cref="TestClass"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="TestClass"/> to compare with this instance.</param>
            /// <returns><c>true</c> if the specified <see cref="TestClass"/> is equal to this instance; otherwise, <c>false</c>.</returns>
            public bool Equals(TestClass obj)
            {
                return InternalEquals(this, obj);
            }

            private static bool InternalEquals(TestClass left, TestClass right)
            {
                return left._value == right._value;
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            public static bool operator ==(TestClass left, TestClass right)
            {
                return InternalEquals(left, right);
            }
            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            public static bool operator !=(TestClass left, TestClass right)
            {
                return !InternalEquals(left, right);
            }

            #endregion

            public override string ToString()
            {
                return _value.ToString();
            }
        }
    }
}
