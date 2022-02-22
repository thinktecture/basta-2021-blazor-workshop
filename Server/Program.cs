using ConfTool.Server.GrpcServices;
using ConfTool.Server.Hubs;
using ConfTool.Server.Models;
using ConfTool.Server.Utils;
using ConfTool.Shared.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddDbContext<ConferencesDbContext>(
    options => options.UseInMemoryDatabase(databaseName: "ConfTool"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerGen();

builder.Services.AddMvc().AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<ConferenceDetailsValidator>());

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddGrpc();
builder.Services.AddCodeFirstGrpc(config => { config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal; });
builder.Services.AddCodeFirstGrpcReflection();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    builder.Configuration.Bind("Oidc", options);
                    options.RefreshOnIssuerKeyNotFound = true;
                });

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("api", builder =>
    {
        builder.RequireAuthenticatedUser();
        builder.RequireScope("api");
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = scope.ServiceProvider.GetRequiredService<ConferencesDbContext>();
    DataGenerator.Initialize(services);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conferences API V1");
});


app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseGrpcWeb();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<ConferencesServiceCodeFirst>().EnableGrpcWeb();
    endpoints.MapHub<ConferencesHub>("/conferencesHub");
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("index.html");
});



app.Run();
