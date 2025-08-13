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
                CompressionLevel = WriterCompressionLevel.High, PrettyPrint = true, UseDtd = true
            }));

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
