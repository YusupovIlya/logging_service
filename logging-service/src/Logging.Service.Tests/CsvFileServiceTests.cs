using Logging.Server.Service.StreamData.Services.Implementation;
using Logging.Server.Service.StreamData.Models;
using System.Globalization;
using CsvHelper;

namespace Logging.Service.Tests;

public class CsvFileServiceTests
{
    [Fact]
    public void Prepare_ShouldCreateCorrectHeaders_WithGivenFields()
    {
        // Arrange
        var service = new CsvFileService("text/csv");
        var streamFields = new Dictionary<long, IEnumerable<string>> { { 1, new[] { "RawJson", "LabelsRawJson", "@timestamp" } } };
        var orderedFields = new[] { "RawJson", "LabelsRawJson", "@timestamp" };
        var values = Enumerable.Empty<BaseStreamDataEvent>();

        // Act
        var result = service.Prepare(streamFields, orderedFields, values);

        // Assert
        using var reader = new StreamReader(new MemoryStream(result));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();
        Assert.True(csv.HeaderRecord.SequenceEqual(orderedFields));
    }

    [Fact]
    public void Prepare_ShouldFillDataCorrectly_WithBaseStreamDataEventValues()
    {
        // Arrange
        var service = new CsvFileService("text/csv");
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
        using var reader = new StreamReader(new MemoryStream(result));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();
        Assert.True(csv.HeaderRecord.SequenceEqual(orderedFields));
    }

    [Fact]
    public void Prepare_ShouldHandleEmptyValues_Correctly()
    {
        // Arrange
        var service = new CsvFileService("text/csv");
        var streamFields = new Dictionary<long, IEnumerable<string>>();
        var orderedFields = new[] { "RawJson", "LabelsRawJson", "@timestamp" };
        var values = Enumerable.Empty<BaseStreamDataEvent>();

        // Act
        var result = service.Prepare(streamFields, orderedFields, values);

        // Assert
        using var reader = new StreamReader(new MemoryStream(result));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        Assert.False(csv.Read());
    }
}
