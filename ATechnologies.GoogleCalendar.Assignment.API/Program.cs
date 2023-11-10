using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

#region Services

builder.Services.Configure<GoogleOauthSettings>(builder.Configuration.GetSection(nameof(GoogleOauthSettings)));
builder.Services.Configure<GooglCalendarSettings>(builder.Configuration.GetSection(nameof(GooglCalendarSettings)));
builder.Services.AddSingleton<GoogleOauthSettings>();
builder.Services.AddSingleton<GooglCalendarSettings>();
builder.Services.AddScoped<IGoogleOauthService, GoogleOauthService>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddSingleton<ICachedGoogleOauthSevice, CachedGoogleOauthSevice>();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", ploicyBuilder =>
    {
        ploicyBuilder.AllowAnyMethod();
        ploicyBuilder.AllowAnyHeader();
        ploicyBuilder.AllowAnyOrigin();
    });
});
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
