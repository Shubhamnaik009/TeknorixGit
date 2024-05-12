using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    string authHeader = context.Request.Headers["Authorization"];

    if (authHeader != null && authHeader.StartsWith("Basic "))
    {
        string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
        string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        string[] credentials = decodedCredentials.Split(':', 2);

        if (credentials.Length == 2 && AuthenticateUser(credentials[0], credentials[1]))
        {
            await next(); 
            return;
        }
    }

    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Your Realm\"";
    context.Response.StatusCode = 401;
    await context.Response.WriteAsync("Unauthorized");
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});


app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
bool AuthenticateUser(string username, string password)
{
    return username == "admin" && password == "password";
}

