using Logging.Server.Models.StreamData.Api.Schemas;

namespace Logging.Server.Service.StreamData.Configuration
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    public static class AppConstants
    {
        public const string SourcePrefix = "source.";
        public const string LabelsPrefix = "_labels.";
        public const string SourceRawJson = "_rawJson";
        public const string LabelsRawJson = "_labelsRawJson";
        public const string FalseCondition = "0";

        public static readonly StreamDataSchemaColumnViewModel[] ServiceFields =
        {
            new("_id", StreamDataSchemaColumnType.Guid),
            new("_rawId", StreamDataSchemaColumnType.Guid),
            new("_aggregatedAt", StreamDataSchemaColumnType.Date),
            new("_userspaceId", StreamDataSchemaColumnType.Integer),
            new("_stream.id", StreamDataSchemaColumnType.Integer),
            new("_stream.name", StreamDataSchemaColumnType.String),
            new("@timestamp", StreamDataSchemaColumnType.Date)
        };

        public static class MicroserviceBaseUris
        {
            public const string Cl = "api/cl";
        }

        public static class Configuration
        {
            public const string Swagger = "Swagger";
            public const string ConnectionString = "PostgreSQL:DefaultConnection:ConnectionString";
            public const string ClickHouseConnectionString = "ClickHouseHttp:DefaultConnection:ConnectionString";
        }

        public static class ClickHouse
        {
            public const string StreamDataTablePrefix = "stream";
            public const ushort DateTimePrecision = 3;
        }

        public static class Localization
        {
            public static class CultureIds
            {
                public const string Ru = "ru-RU";
                public const string En = "en-US";
            }
        }

        /// <summary>
        /// Пакеты прав.
        /// </summary>
        public static class Grants
        {
            public const string ConnectorGet = "base-system.integrations.streams-read";
            public const string ConnectorWrite = "base-system.integrations.streams-write";

            /// <summary>
            /// Пакеты прав на коннекторы.
            /// </summary>
            public static readonly string[] Connector = { ConnectorGet, ConnectorWrite };
        }
    }
}
