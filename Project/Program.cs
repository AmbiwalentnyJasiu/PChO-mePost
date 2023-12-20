using Neo4j.Driver;
using Project;
using Project.DataAccess;
using Project.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
});

builder.Services.Configure<NeO4jConnectionSettings>(builder.Configuration.GetSection("NeO4jConnectionSettings"));

var settings = new NeO4jConnectionSettings();
builder.Configuration.GetSection("NeO4jConnectionSettings").Bind(settings);

builder.Services.AddSingleton(GraphDatabase.Driver(settings.Neo4jConnection,
    AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)))
    
    .AddScoped<INeo4jDataAccess, Neo4jDataAccess>()
    .AddTransient<IAccountRepository, AccountRepository>()
    .AddTransient<IUserRepository, UserRepository>()
    .AddTransient<IPostRepository, PostRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();