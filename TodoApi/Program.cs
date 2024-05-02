using static Microsoft.EntityFrameworkCore.ServerVersion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoApi;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
                      Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
}
app.UseSwaggerUI();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
  

});
app.UseCors(policy =>
{
    policy.AllowAnyOrigin();
    policy.AllowAnyMethod(); 
    policy.AllowAnyHeader(); 
});

app.MapGet("/item", async (ToDoDbContext dbContext) =>
{
    var tasks = await dbContext.Items.ToListAsync(); 
    return Results.Ok(tasks); 
});
app.MapPost("/item", async (ToDoDbContext dbContext, Item item) =>
{
    await dbContext.Items.AddAsync(item);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/item/{item.Id}", item);
});

app.MapPut("/item/{id}", async (ToDoDbContext dbContext, int id, Item item) =>
{
    var existingItem = await dbContext.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    existingItem.Name = item.Name;
    existingItem.IsComplete = item.IsComplete;

    await dbContext.SaveChangesAsync();
    return Results.Ok();
});

app.MapDelete("/item/{id}", async (ToDoDbContext dbContext, int id) =>
{
    var existingItem = await dbContext.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    dbContext.Items.Remove(existingItem);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

