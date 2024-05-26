using Logging.Server.Models.StreamData.Api.Schemas;

namespace Logging.Server.StreamData.Validator.Configuration
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    public static class AppConstants
    {
        public const string SourcePrefix = "source.";
        public const string LabelsPrefix = "_labels.";

        public const string SourceRawJson = "_rawJson";
        public const string LabelsRawJson = "_labelsRawJson";
        public const string FalseCondition = "0";
        public const string TrueCondition = "1";

        public const string SourceObjName = "source";
        public const string LabelsObjName = "_labels";

        public static readonly string[] Occurs =
        {
            "AND", "OR"
        };

        public static class ClickHouse
        {
            public const ushort DateTimePrecision = 3;
        }

        public static readonly StreamDataSchemaColumnViewModel[] ServiceFields =
        {
            new("_id", StreamDataSchemaColumnType.Guid),
            //new("_rawId", StreamDataSchemaColumnType.Guid),
            //new("_aggregatedAt", StreamDataSchemaColumnType.Date),
            //new("_userspaceId", StreamDataSchemaColumnType.Integer),
            //new("_stream.id", StreamDataSchemaColumnType.Integer),
            //new("_stream.name", StreamDataSchemaColumnType.String),
            new("@timestamp", StreamDataSchemaColumnType.Date),
        };

        public static class Symbols
        {
            public const char Space = ' ';
            public const char Backslash = '\\';
            public const char DoubleQuote = '\"';
            public const char Colon = ':';
            public const char LeftBrace = '(';
            public const char RightBrace = ')';
        }
    }
}
