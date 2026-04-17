// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Math.Intervals.Tests;

partial class IntervalCoreTests
{
    TypeConverter GetTypeConverter()
    {
        var interval = EmptyInterval<int>();
        return TypeDescriptor.GetConverter(interval);
    }

    [TestMethod]
    public void Interval_Core_TypeConverter_CloneConversion()
    {
        var typeConverter = GetTypeConverter();

        var intervalToConvert = NewInterval(ValueInterval.Inclusive(1, 2));

        object? convertedInterval = typeConverter.ConvertFrom(intervalToConvert);
        Assert.AreNotSame(intervalToConvert, convertedInterval);
        Assert.AreEqual(intervalToConvert, convertedInterval);

        convertedInterval = typeConverter.ConvertTo(intervalToConvert, intervalToConvert.GetType());
        Assert.AreNotSame(intervalToConvert, convertedInterval);
        Assert.AreEqual(intervalToConvert, convertedInterval);
    }
}
