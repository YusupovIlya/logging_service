using Xunit;
using System;
using Monq.Core.ClickHouseBuffer.Attributes;
using Monq.Core.ClickHouseBuffer.Extensions;

public class ClickHouseBulkModelExtensionsTests
{
    [Fact]
    public void CreateDbValues_ShouldCorrectlyHandleProperties()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = model.CreateDbValues();

        // Assert
        Assert.Equal("DefaultName", result["name"]);
        Assert.Equal(25, result["custom_column"]);
        Assert.False(result.ContainsKey("ignoredProperty"));
        Assert.True(result.ContainsKey("dateOfBirth"));
        Assert.Equal("Active", result["status"]);
    }

    [Fact]
    public void CreateDbValues_ShouldUseCamelCaseByDefault()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = model.CreateDbValues();

        // Assert
        Assert.True(result.ContainsKey("name"));
    }

    [Fact]
    public void CreateDbValues_ShouldConvertDateTimeOffsetToUtcDateTime()
    {
        // Arrange
        var model = new TestModel { DateOfBirth = DateTimeOffset.Now };

        // Act
        var result = model.CreateDbValues();

        // Assert
        Assert.IsType<DateTime>(result["dateOfBirth"]);
    }
}



public class TestModel
{
    public string Name { get; set; } = "DefaultName";

    [ClickHouseColumn(name: "custom_column")]
    public int Age { get; set; } = 25;

    [ClickHouseIgnore]
    public bool IgnoredProperty { get; set; } = true;

    public DateTimeOffset DateOfBirth { get; set; } = DateTimeOffset.Now;

    public TestEnum Status { get; set; } = TestEnum.Active;

    public enum TestEnum
    {
        Inactive,
        Active
    }
}
