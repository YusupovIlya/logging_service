using Logging.Server.Service.StreamData.Configuration;
using Logging.Server.Service.StreamData.HttpServices;
using Logging.Server.Service.StreamData.Services;
using Logging.Server.Service.StreamData.Services.Implementation;
using Logging.Server.StreamData.Validator.Services;
using Logging.Server.StreamData.Validator.Services.Implementation;
using Microsoft.AspNetCore.Mvc;
using Monq.Core.BasicDotNetMicroservice.GlobalExceptionFilters.Filters;
using Monq.Core.HttpClientExtensions.Exceptions;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    }));
var constr = configuration.GetValue<string>(AppConstants.Configuration.ClickHouseConnectionString);
builder.Services.AddGlobalExceptionFilter()
                .AddExceptionHandler<ResponseException>(ex =>
                    new ObjectResult(JsonConvert.DeserializeObject(ex.ResponseData)) { StatusCode = (int)ex.StatusCode })
                .AddDefaultExceptionHandlers();

builder.Services.Configure<ClickHouseOptions>(options => options.ConnectionString = configuration.GetValue<string>(AppConstants.Configuration.ClickHouseConnectionString));

builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IStreamDataSchemasRepository, StreamDataSchemasRepository>();
builder.Services.AddScoped<IStreamDataRepository, StreamDataRepositoryImpl>();
//builder.Services.AddScoped<IAliasRepository, AliasRepository>();
builder.Services.AddSingleton<IMqlQueryParserBuilder, MqlQueryParserBuilder>();

//builder.Services.AddScoped<IStringLocalizer, EfCoreStringLocalizer>();

builder.Services.AddScoped<IStreamsHttpService, StreamsHttpServiceImpl>();

//var connectionString = configuration.GetValue<string>(AppConstants.Configuration.ClickHouseConnectionString)
//builder.Services
//    .AddEntityFrameworkNpgsql()
//    .AddDbContext<StreamDataContext>(o => o.UseNpgsql(connectionString));

builder.Services
    .AddControllers(opt => opt.Filters.Add(typeof(GlobalExceptionFilter)))
    .AddNewtonsoftJson(opt => opt.UseCamelCasing(true));

builder.Services.AddAutoMapper(typeof(Program).Assembly);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.Run();
