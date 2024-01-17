// ReSharper disable UnusedVariable
namespace TomsToolbox.Essentials.Tests;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

using Xunit;

public class BinaryOperationProcessorTests
{
    [Fact]
    public void BinaryOperationProcessor_Multiply_Double_Number_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(1.5, 2.5);
        Assert.Equal(1.5 * 2.5, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Vector_Number_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new Vector(1, 2), 2.5);
        Assert.Equal(new Vector(2.5, 5), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Vector_NumberString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new Vector(1, 2), "2.5");
        Assert.Equal(new Vector(2.5, 5), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Vector_VectorString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new Vector(1, 2), "2.5,1.5");
        Assert.Equal(5.5, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Vector_Vector_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new Vector(1, 2), new Vector(2.5, 1.5));
        Assert.Equal(5.5, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Vector_Unknown_Test()
    {
        Assert.Throws<InvalidCastException>(() =>
        {
            var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
            var result = target.Execute(new Vector(1, 2), target);
        });
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Size_Double_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new Size(1, 2), 2);
        Assert.Equal(new Vector(2, 4), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Unknown_Number_Test()
    {
        Assert.Throws<InvalidCastException>(() =>
        {
            var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
            var result = target.Execute(target, 2);
        });
    }

    [Fact]
    public void BinaryOperationProcessor_Add_Point_VectorString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Addition);
        var result = target.Execute(new Point(2, 5), "2,3");
        Assert.Equal(new Point(4, 8), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Subtract_Vector_VectorString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Subtraction);
        var result = target.Execute(new Vector(2, 5), "2,3");
        Assert.Equal(new Vector(0, 2), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Vector_MatrixString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new Vector(2, 5), "0,1,-1,0,0,0");
        Assert.Equal(new Vector(-5, 2), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Rect_MatrixString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply); // => Transform
        var result = target.Execute(new Rect(0, 0, 2, 5), "0,1,-1,0,0,0");
        Assert.Equal(new Rect(-5, 0, 5, 2), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Add_Rect_VectorString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Addition); // => Offset
        var result = target.Execute(new Rect(1, 2, 3, 4), "1,2");
        Assert.Equal(new Rect(2, 4, 3, 4), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_Rect_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(new Rect(1, 2, 3, 4), "1,2,3,4");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_Double_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(3.14, "3.14");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_Integer_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(3, "3");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_Boolean_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(true, "true");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_Boolean_String2_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(true, "false");
        Assert.Equal(false, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_TimeSpan_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(TimeSpan.FromHours(2), "2:00:00");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_LessThanOrEqual_TimeSpan_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.LessThanOrEqual);
        var result = target.Execute(TimeSpan.FromHours(2), "2:00:00");
        Assert.Equal(true, result);
    }


    [Fact]
    public void BinaryOperationProcessor_Equality_Rect_UnmatchedString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(new Rect(1, 2, 3, 4), "1,4,3,4");
        Assert.Equal(false, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_String_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute("Test", "Test");
        Assert.Equal(true, result);
        result = target.Execute("Test1", "Test");
        Assert.Equal(false, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Equality_Custom_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Equality);
        var result = target.Execute(new TestClass(42), "42");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Inequality_Custom_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Inequality);
        var result = target.Execute(new TestClass(42), "43");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Multiply_Custom_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Multiply);
        var result = target.Execute(new TestClass(4), "5");
        Assert.Equal(new TestClass(20), result);
    }

    [Fact]
    public void BinaryOperationProcessor_Division_Custom_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Division);
        var result = target.Execute(new TestClass(20), "5");
        Assert.Equal(new TestClass(4), result);
    }

    [Fact]
    public void BinaryOperationProcessor_LessThan_Custom_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.LessThan);
        var result = target.Execute(new TestClass(4), "5");
        Assert.Equal(true, result);
        result = target.Execute(new TestClass(5), "5");
        Assert.Equal(false, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Inequality_Rect_String_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Inequality);
        var result = target.Execute(new Rect(1, 2, 3, 4), "1,2,3,4");
        Assert.Equal(false, result);
    }

    [Fact]
    public void BinaryOperationProcessor_Inequality_Rect_UnmatchedString_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.Inequality);
        var result = target.Execute(new Rect(1, 2, 3, 4), "1,4,3,4");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationProcessor_GreaterThan_TimeSpan_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.GreaterThan);
        var result = target.Execute(TimeSpan.FromHours(2), "1:59:00");
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationMultiConverter_GreaterThan_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.GreaterThan);
        var result = target.Execute(3, 2.0);
        Assert.Equal(true, result);
    }

    [Fact]
    public void BinaryOperationMultiConverter_NotGreaterThan_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.GreaterThan);
        var result = target.Execute(1, 2.0);
        Assert.Equal(false, result);
    }


    [Fact]
    public void BinaryOperationMultiConverter_GreaterThanOrEqual_Test()
    {
        var target = new BinaryOperationProcessor(BinaryOperation.GreaterThanOrEqual);
        var result = target.Execute(2, 2.0);
        Assert.Equal(true, result);
    }

    class TestClassTypeConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
        {
            return new TestClass(Convert.ToInt32(value, culture));
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return value?.ToString();
        }
    }

    [TypeConverter(typeof(TestClassTypeConverter))]
    readonly struct TestClass : IEquatable<TestClass>
    {
        private readonly int _value;

        public TestClass(int value)
            : this()
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
        public override bool Equals(object? obj)
        {
            return obj is TestClass testClass && Equals(testClass);
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
