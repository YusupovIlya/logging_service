using Logging.Server.Models.StreamData.Api.Schemas;
using Logging.Server.StreamData.Validator.Services.Implementation;

namespace Logging.Service.Tests;

public class QueryParserTests
{
    private MqlQueryParser mqlQueryParser => new MqlQueryParserBuilder().Create();

    private new List<StreamDataSchemaColumnViewModel> columns => new()
    {
        new StreamDataSchemaColumnViewModel("source.endpoint", StreamDataSchemaColumnType.String),
        new StreamDataSchemaColumnViewModel("source.ip", StreamDataSchemaColumnType.String),
        new StreamDataSchemaColumnViewModel("source.action_description", StreamDataSchemaColumnType.String),
    };

    [Theory]
    [InlineData("like(upperUTF8(visitParamExtractString(_rawJson, 'endpoint')), {value1:String})", "source.endpoint:getFacility")]
    [InlineData("like(upperUTF8(visitParamExtractString(_rawJson, 'ip')), {value1:String})", "source.ip:185")]
    [InlineData("like(upperUTF8(visitParamExtractString(_rawJson, 'action_description')), {value1:String})", "source.action_description:GetList")]

    [InlineData("like(upperUTF8(visitParamExtractString(_rawJson, 'endpoint')), {value1:String}) AND like(upperUTF8(visitParamExtractString(_rawJson, 'ip')), CONCAT('%', {value2:String}, '%'))", "source.endpoint:getFacility AND source.ip:*185*")]
    [InlineData("like(upperUTF8(visitParamExtractString(_rawJson, 'endpoint')), {value1:String}) OR like(upperUTF8(visitParamExtractString(_rawJson, 'ip')), CONCAT('%', {value2:String}, '%'))", "source.endpoint:getFacility OR source.ip:*185*")]

    [InlineData("like(upperUTF8(visitParamExtractString(_rawJson, 'endpoint')), {value1:String})\r\n\t OR like(upperUTF8(visitParamExtractString(_rawJson, 'ip')), {value1:String})\r\n\t OR like(upperUTF8(visitParamExtractString(_rawJson, 'action_description')), {value1:String})", "Разработчик")]
    public void ToSqlQuery_ValidQueryWithFullSearchTerm_ReturnsExpectedResult(string expectedSql, string query)
    {
        // Act
        var result = mqlQueryParser.ToSqlQuery(query, columns);

        // Assert
        Assert.Equal(expectedSql, result);
    }
}

