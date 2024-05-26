using Xunit;
using Monq.Core.ClickHouseBuffer.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("TestString", "testString")]
    [InlineData("testString", "testString")]
    [InlineData("T", "t")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToCamelCase_ShouldConvertCorrectly(string input, string expected)
    {
        // Act
        var result = input.ToCamelCase();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToCamelCase_ShouldNotModifyString_IfAlreadyInCamelCase()
    {
        // Arrange
        var input = "camelCaseString";

        // Act
        var result = input.ToCamelCase();

        // Assert
        Assert.Equal(input, result);
    }
}
