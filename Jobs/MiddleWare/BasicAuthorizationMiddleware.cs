using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;
public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string authHeader = context.Request.Headers["Authorization"];

        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            string decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            string[] credentials = decodedCredentials.Split(':', 2);

            if (credentials.Length == 2 && AuthenticateUser(credentials[0], credentials[1]))
            {
                await _next(context);
                return;
            }
        }

        // Authentication failed: send 401 Unauthorized with WWW-Authenticate header
        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Your Realm\"";
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
    }

    private bool AuthenticateUser(string username, string password)
    {
        // Replace with your actual authentication logic (e.g., database lookup)
        return username == "admin" && password == "password";
    }
}