using ClosedXML.Excel;
using Logging.Server.Service.StreamData.Services.Implementation;
using Logging.Server.Service.StreamData.Models;

namespace Logging.Service.Tests;

public class XlsxFileServiceTests
{
    [Fact]
    public void Prepare_ShouldCreateCorrectHeaders_WithGivenFields()
    {
        // Arrange
        var service = new XlsxFileService("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var streamFields = new Dictionary<long, IEnumerable<string>> { { 1, new[] { "RawJson", "LabelsRawJson", "@timestamp" } } };
        var orderedFields = new[] { "RawJson", "LabelsRawJson", "@timestamp" };
        var values = Enumerable.Empty<BaseStreamDataEvent>();

        // Act
        var result = service.Prepare(streamFields, orderedFields, values);

        // Assert
        using var workbook = new XLWorkbook(new MemoryStream(result));
        var worksheet = workbook.Worksheets.First();
        Assert.Equal("RawJson", worksheet.Cell(1, 1).Value);
        Assert.Equal("LabelsRawJson", worksheet.Cell(1, 2).Value);
        Assert.Equal("@timestamp", worksheet.Cell(1, 3).Value);
    }

    [Fact]
    public void Prepare_ShouldFillDataCorrectly_WithBaseStreamDataEventValues()
    {
        // Arrange
        var service = new XlsxFileService("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var streamFields = new Dictionary<long, IEnumerable<string>> { { 1, new[] { "RawJson", "LabelsRawJson", "@timestamp" } } };
        var orderedFields = new[] { "RawJson", "LabelsRawJson", "@timestamp" };
        var values = new List<BaseStreamDataEvent>
        {
            new BaseStreamDataEvent
            {
                RawJson = "{\"property\":\"value\"}",
                LabelsRawJson = "{\"label\":\"value\"}",
                Timestamp = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            }
        };

        // Act
        var result = service.Prepare(streamFields, orderedFields, values);

        // Assert
        using var workbook = new XLWorkbook(new MemoryStream(result));
        var worksheet = workbook.Worksheets.First();
        Assert.True(!worksheet.Row(1).IsEmpty());
    }

    [Fact]
    public void Prepare_ShouldHandleEmptyValues_Correctly()
    {
        // Arrange
        var service = new XlsxFileService("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var streamFields = new Dictionary<long, IEnumerable<string>>();
        var orderedFields = new[] { "RawJson", "LabelsRawJson", "@timestamp" };
        var values = Enumerable.Empty<BaseStreamDataEvent>();

        // Act
        var result = service.Prepare(streamFields, orderedFields, values);

        // Assert
        using var workbook = new XLWorkbook(new MemoryStream(result));
        var worksheet = workbook.Worksheets.First();
        Assert.True(worksheet.Row(2).IsEmpty());
    }
}
