using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.Oss;
using Autodesk.SDKManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly OssClient _ossClient;



    public static string ClientId = "Your Client ID";
    public static string ClientSecret = "Your Client Secret";

   
    private static SDKManager _sdkManager = SdkManagerBuilder.Create().Build();

    private static AuthenticationClient _authenticationClient = new AuthenticationClient(_sdkManager);

    private const string callbackurl = "https://localhost:7244/api/auth/callback";

    private static readonly List<Scopes> scopes = new List<Scopes>
    {
         Scopes.DataCreate,
          Scopes.DataRead,
          Scopes.DataWrite,
          Scopes.AccountRead,
          Scopes.BucketCreate,
          Scopes.BucketRead
    };


    [HttpGet("Login")]

    public IActionResult Login()
    {
      string authorizationUrl = _authenticationClient.Authorize(
        clientId: ClientId,
        ResponseType.Code,
        redirectUri: callbackurl,
        scopes: scopes

      );
      return Redirect(authorizationUrl);
    }


    [HttpGet("callback")]

    public async Task<IActionResult> Callback([FromQuery] string code)
    {

      if (string.IsNullOrEmpty(code))
      {
        return BadRequest("Authorization code is missing.");
      }

      try
      {
        var tokenResponse = await _authenticationClient.GetThreeLeggedTokenAsync(
    clientId: ClientId,
    code: code,
    redirectUri: callbackurl,
    clientSecret: ClientSecret
);
        // Store the access token securely (e.g., in a database or session)
        TokenStore.AccessToken = tokenResponse.AccessToken;
        TokenStore.ExpireAt = DateTime.UtcNow.AddSeconds((double)(tokenResponse.ExpiresIn));
        return Ok("Authentication successful. Access token obtained.");
      }
      catch (Exception ex)
      {
        return BadRequest($"Authentication failed: {ex.Message}");
      }

    }

    public static string GetStoredToken()
    {
      if (string.IsNullOrEmpty(TokenStore.AccessToken))
      {
        return null;
      }
      else
      {
        return TokenStore.AccessToken;
      }
    }
    [HttpGet("public-token")]
    public async Task<IActionResult> GetpublicToken()
    {
      try
      {
        var auth = await _authenticationClient.GetTwoLeggedTokenAsync(clientId: ClientId, clientSecret: ClientSecret, new List<Scopes> { Scopes.ViewablesRead });
        return Ok(new
        {
            access_token = auth.AccessToken,
            expires_in = auth.ExpiresIn
        });
      }
      catch (Exception)
      {

       return BadRequest("Failed to get public token.");
      }

    }
    
    public static class TokenStore
    {
      public static string AccessToken { get; set; }

      public static DateTime ExpireAt { get; set; }


    }
  }
}
