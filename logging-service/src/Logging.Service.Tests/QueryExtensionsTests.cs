using Xunit;
using Logging.Server.Service.StreamData.Extensions;
using System.Text;
using Monq.Models.Abstractions.v2;

namespace Logging.Service.Tests;

public class QueryExtensionsTests
{
    [Fact]
    public void AppendPreWhereExpression_ShouldCorrectlyAppendSingleCondition()
    {
        // Arrange
        var sb = new StringBuilder();
        var preWhereExpressions = new[] { "condition1 = true" };

        // Act
        sb.AppendPreWhereExpression(preWhereExpressions);

        // Assert
        var expected = "PREWHERE (condition1 = true)";
        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendPreWhereExpression_ShouldCorrectlyAppendMultipleConditions()
    {
        // Arrange
        var sb = new StringBuilder();
        var preWhereExpressions = new[] { "condition1 = true", "condition2 = false" };

        // Act
        sb.AppendPreWhereExpression(preWhereExpressions);

        // Assert
        var expected = "PREWHERE (condition1 = true)" + Environment.NewLine + " AND (condition2 = false)";
        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void GetSqlExpression_ShouldReturnCorrectExpressionForSingleDate()
    {
        // Arrange
        var date = new DateRangePostViewModel
        {
            Start = new DateTimeOffset(new DateTime(2023, 1, 1)),
            End = new DateTimeOffset(new DateTime(2023, 1, 1))
        };
        var fieldName = "@timestamp";
        Func<DateTimeOffset, string> format = (dt) => $"'{dt:yyyy-MM-dd}'";

        // Act
        var result = date.GetSqlExpression(fieldName, format);

        // Assert
        var expected = "@timestamp = '2023-01-01'";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSqlExpression_ShouldReturnCorrectExpressionForDateRange()
    {
        // Arrange
        var date = new DateRangePostViewModel
        {
            Start = new DateTimeOffset(new DateTime(2023, 1, 1)),
            End = new DateTimeOffset(new DateTime(2023, 1, 2))
        };
        var fieldName = "@timestamp";
        Func<DateTimeOffset, string> format = (dt) => $"'{dt:yyyy-MM-dd}'";

        // Act
        var result = date.GetSqlExpression(fieldName, format);

        // Assert
        var expected = "@timestamp BETWEEN '2023-01-01' AND '2023-01-02'";
        Assert.Equal(expected, result);
    }
}
