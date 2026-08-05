
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new("https+http://catalog");
    client.Timeout =  TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient<BasketApiClient>(client =>
{
    client.BaseAddress = new("https+http://basket");
    client.Timeout =  TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient<OrderingApiClient>(client =>
{
    client.BaseAddress = new("https+http://ordering");
    client.Timeout =  TimeSpan.FromMinutes(10);
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
