using OSLC4Net.Server.Providers;
using VDS.RDF.Writing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new OslcRdfInputFormatter());
    options.OutputFormatters.Insert(0,
        new OslcRdfOutputFormatter(
            new OslcOutputFormatConfig
            {
                // CompressionLevel = WriterCompressionLevel.Minimal,
                // PrettyPrint = false,
                CompressionLevel = WriterCompressionLevel.High,
                PrettyPrint = true,
                // UseDtd = true
            }));

});
// Learn more about configuring OpenAPI at https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0
builder.Services.AddOpenApi();
builder.WebHost.UseKestrel();
builder.WebHost.ConfigureKestrel(options =>
{
    // due to DotNetRdf not supporting async parsing
    options.AllowSynchronousIO = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
