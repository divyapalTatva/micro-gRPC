using AllProtos.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add controllers for REST API
builder.Services.AddControllers();

builder.Services.AddGrpcClient<ProductService.ProductServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:5001"); // ProductService endpoint
});

// Swagger (for REST API testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map REST controllers
app.MapControllers();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
