using BlazorOidc;
using BlazorOidc.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Net.Http.Headers;

const string _commonName = "http://schemas.xmlsoap.org/claims/CommonName";

var builder = WebApplication.CreateBuilder(args);

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
}).CreateLogger("Program");

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.MetadataAddress = builder.Configuration["OpenIdConnect:MetadataAddress"];
    options.ClientId = builder.Configuration["OpenIdConnect:ClientId"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.Scope.Clear();
    options.Scope.Add("openid");
    // *********** DANGER: CANNOT USE IN PROD *****************
    //options.BackchannelHttpHandler = new HttpClientHandler
    //{
    //    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    //};
    // *********** DANGER: CANNOT USE IN PROD *****************

    options.Events.OnTokenValidated = context =>
    {
        if (context.Principal != null)
        {
            var userName = $"{context.Principal.FindFirst(c => c.Type == _commonName)?.Value}";
            logger.LogInformation($"User Name: {userName}");
        }
        else
        {
            logger.LogError("Principal is NULL");
        }

        return Task.CompletedTask;
    };
    options.Events.OnAuthenticationFailed = context =>
    {
        logger.LogError($"OnAuthenticationFailed: {context.Exception.Message}");

        if (context.Exception.InnerException != null)
        {
            logger.LogError($"OnAuthenticationFailed(Inner Exception): {context.Exception.InnerException.Message}");
        }

        return Task.CompletedTask;
    };
    options.Events.OnRemoteFailure = context =>
    {
        logger.LogError($"OnRemoteFailure: {context?.Failure?.Message}");

        if (context != null && context.Failure != null && context.Failure.InnerException != null)
        {
            logger.LogError($"OnRemoteFailure(Inner Exception): {context.Failure.InnerException.Message}");
        }

        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<LoginService>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
app.UseDeveloperExceptionPage();

app.Use((ctx, next) =>
{
    ctx.Response.Headers[HeaderNames.CacheControl] = "no-cache,no-store,no-transform,max-age=0";
    ctx.Request.Scheme = "https";
    return next();
});

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();