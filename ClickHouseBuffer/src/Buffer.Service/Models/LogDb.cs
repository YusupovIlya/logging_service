using Monq.Core.ClickHouseBuffer.Attributes;

namespace Buffer.Service.Models
{
    public class LogDb
    {
        [ClickHouseColumn("_id")]
        public Guid Id { get; set; }

        [ClickHouseColumn("_rawJson")]
        public string RawJson { get; set; }

        [ClickHouseColumn("_labelsRawJson")]
        public string Labels { get; set; } = string.Empty;

        [ClickHouseColumn("@timestamp")]
        public DateTimeOffset TimeStamp { get; set; }

        [ClickHouseColumn("_date")]
        public DateTime Date { get; set; }

        [ClickHouseColumn("source.user_id")]
        public Guid UserId { get; set; }

        [ClickHouseColumn("source.endpoint")]
        public string Endpoint { get; set; }

        [ClickHouseColumn("source.ip")]
        public string Ip { get; set; }

        [ClickHouseColumn("source.action_description")]
        public string Action { get; set; }
    }
}
