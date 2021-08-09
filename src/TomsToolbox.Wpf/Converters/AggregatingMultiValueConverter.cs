namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// A converter that aggregates the inner converters for all values, overcoming the restriction of .Net that <see cref="IMultiValueConverter"/> can't be nested.
    /// </summary>
    /// <remarks>
    /// This converter aggregates the result by calling each of the <see cref="Converters"/> with the aggregated value as input and the next value as parameter, 
    /// i.e. the first converter will aggregate value[0] and value[1], the second converter will aggregate the result of the first and value[2], etc.<para/>
    /// If there are less converters than parameters-1, and the last converter is an <see cref="IValueConverter"/>, the last converter is used to aggregate the remainder of values.<para/>
    /// If there are less converters than parameters-1, and the last converter is an <see cref="IMultiValueConverter"/>, the <see cref="IMultiValueConverter"/> is invoked with the aggregated value and all remaining values.
    /// </remarks>
    /// <example>
    /// 
    /// <code language="XAML"><![CDATA[
    /// <Window.Resources>
    ///   <toms:CompositeMultiValueConverter  x:Key="ThresholdConverter">
    ///     <toms:CompositeMultiValueConverter.MultiValueConverter>
    ///       <toms:AggregatingMultiValueConverter>
    ///         <toms:BinaryOperationConverter Operation="Subtraction"/>
    ///         <toms:BinaryOperationConverter Operation="LessThanOrEqual"/>
    ///       </toms:AggregatingMultiValueConverter>
    ///     </toms:CompositeMultiValueConverter.MultiValueConverter>
    ///     <toms:BooleanToVisibilityConverter/>
    ///   </toms:CompositeMultiValueConverter>
    /// </Window.Resources>
    /// <TextBlock Text="The elapsed time is less than the minimum required time!">
    ///   <TextBlock.Visibility>
    ///     <MultiBinding Converter="{StaticResource ThresholdConverter}">
    ///         <Binding Path="Now" Source="{x:Static toms:DateTimeSource.Default}"/>
    ///         <Binding Path="OperationStartTime"/>
    ///         <Binding Path="MinimumOperationTime"/>
    ///     </MultiBinding>
    ///   </TextBlock.Visibility>
    /// </TextBlock>
    /// ]]></code>
    /// </example>
    [ContentProperty("Converters")]
    public class AggregatingMultiValueConverter : MultiValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="values">The values produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        protected override object? Convert(object?[] values, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (!Converters.Any())
                throw new InvalidOperationException("Need at least one converter");

            var numberOfValues = values.Length;
            if (numberOfValues == 0)
                return null;

            var aggregated = values[0];
            var converter = Converters[0];

            var nextConverterIndex = 1;
            var nextValueIndex = 1;

            for (; nextValueIndex < numberOfValues; nextValueIndex++)
            {
                if (converter is IValueConverter valueConverter)
                {
                    aggregated = valueConverter.Convert(aggregated, targetType, values[nextValueIndex], culture);

                    if (nextConverterIndex < Converters.Count)
                    {
                        converter = Converters[nextConverterIndex];
                        nextConverterIndex += 1;
                    }
                }
                else if (nextConverterIndex >= Converters.Count)
                {
                    if (converter is IMultiValueConverter multiValueConverter)
                    {
                        return multiValueConverter.Convert(new[] { aggregated }.Concat(values.Skip(nextValueIndex)).ToArray(), targetType, parameter, culture);
                    }

                    throw new InvalidOperationException("All converters must implement IValueConverter, except the last may be an IMultiValueConverter");
                }
            }

            return aggregated;
        }

        /// <summary>
        /// Gets the aggregating converters. Must be all <see cref="IValueConverter"/>, only the last might be a <see cref="IMultiValueConverter"/>.
        /// </summary>
        public Collection<object> Converters { get; } = new();
    }
}
