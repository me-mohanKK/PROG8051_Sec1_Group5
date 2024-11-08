var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable serving static files
app.UseStaticFiles(); // Add this line

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Optional: Map routes for your success and cancel pages
app.MapGet("/success", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/success.html"); // Adjust path if necessary
});

app.MapGet("/cancel", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/cancel.html"); // Adjust path if necessary
});

// Ensure the app runs
app.Run();
