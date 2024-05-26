namespace Logging.Server.Service.StreamData.Services.Implementation
{
    /// <summary>
    /// Фабрика по созданию сервисов формирования файлов.
    /// </summary>
    public static class FileServiceFactory
    {
        /// <summary>
        /// Создать сервис формирования файлов определённого типа.
        /// </summary>
        /// <param name="type">Тип файла.</param>
        public static FileService? Create(string? type) =>
            type switch
            {
                "csv" => new CsvFileService("text/csv"),
                "xlsx" => new XlsxFileService("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                _ => null
            };
    }
}
