using Microsoft.EntityFrameworkCore;
using Logging.Server.Service.StreamData.Models;
using Newtonsoft.Json;

namespace Logging.Server.Service.StreamData.Configuration
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена

    //public class StreamDataContext : DbContext
    //{
    //    public virtual DbSet<Language> Languages { get; set; }
    //    public virtual DbSet<Resource> Resources { get; set; }
    //    public virtual DbSet<Alias> Aliases { get; set; }

    //    static readonly JsonSerializerSettings _jsonSerializerSettings = new() { NullValueHandling = NullValueHandling.Ignore };

    //    public StreamDataContext(DbContextOptions<StreamDataContext> options) : base(options) {}

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Language>(entity =>
    //        {
    //            entity.HasKey(val => val.Id);

    //            entity.Property(val => val.Id)
    //                .HasMaxLength(sbyte.MaxValue);

    //            entity
    //                .HasMany(val => val.Resources)
    //                .WithOne(val => val.Language!)
    //                .OnDelete(DeleteBehavior.Restrict);
    //        });

    //        modelBuilder.Entity<Resource>(entity =>
    //        {
    //            entity.HasKey(val => new { val.LanguageId, val.Key });

    //            entity.Property(val => val.Key)
    //                .HasMaxLength(sbyte.MaxValue)
    //                .IsRequired();

    //            entity.Property(val => val.Value)
    //                .HasMaxLength(byte.MaxValue);
    //        });

    //        SeedData(modelBuilder);

    //        modelBuilder.Entity<Alias>(entity =>
    //        {
    //            entity.HasKey(val => new {val.UserspaceId, val.FieldName, val.Value });

    //            entity.Property(val => val.Value)
    //                .HasMaxLength(byte.MaxValue)
    //                .IsRequired();

    //            entity.Property(val => val.FieldName)
    //                .HasMaxLength(byte.MaxValue)
    //                .IsRequired();

    //            entity.Property(val => val.IsDefault)
    //                .HasDefaultValue(false)
    //                .IsRequired();

    //            entity.Property(val => val.UserspaceId)
    //                .IsRequired();

    //            entity.HasIndex(val => new { val.UserspaceId, val.IsDefault });
    //        });
    //    }

    //    static void SeedData(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Language>(entity =>
    //            entity.HasData(
    //                new Language { Id = "ru-RU" },
    //                new Language { Id = "en-US" })
    //        );
    //        modelBuilder.Entity<Resource>(entity =>
    //            entity.HasData(
    //                new Resource { LanguageId = "en-US", Key = "alias_is_not_found", Value = "Alias is not found." },
    //                new Resource { LanguageId = "ru-RU", Key = "alias_is_not_found", Value = "Алиас не найден." },
    //                new Resource { LanguageId = "en-US", Key = "alias_already_exists", Value = "Alias already exists." },
    //                new Resource { LanguageId = "ru-RU", Key = "alias_already_exists", Value = "Алиас уже существует." },
    //                new Resource { LanguageId = "en-US", Key = "file_type_is_incorrect", Value = "File type is incorrect." },
    //                new Resource { LanguageId = "ru-RU", Key = "file_type_is_incorrect", Value = "Некорректный тип файла." },
    //                new Resource { LanguageId = "en-US", Key = "query_is_required", Value = "Query is required." },
    //                new Resource { LanguageId = "ru-RU", Key = "query_is_required", Value = "Строка запроса является обязательным полем." },
    //                new Resource { LanguageId = "en-US", Key = "query_is_invalid", Value = "Query is invalid." },
    //                new Resource { LanguageId = "ru-RU", Key = "query_is_invalid", Value = "Строка запроса невалидна." },
    //                new Resource { LanguageId = "en-US", Key = "stream_schema_is_not_found", Value = "Stream schema with identifier {0} is not found." },
    //                new Resource { LanguageId = "ru-RU", Key = "stream_schema_is_not_found", Value = "Схема потока данных с идентификатором {0} не найдена." },
    //                new Resource { LanguageId = "en-US", Key = "no_permissions_to_access_streams", Value = "The user does not have sufficient rights to access the streams." },
    //                new Resource { LanguageId = "ru-RU", Key = "no_permissions_to_access_streams", Value = "У пользователя нет необходимых прав для доступа к выбранным потокам данных." }));
    //    }
    //}
}
