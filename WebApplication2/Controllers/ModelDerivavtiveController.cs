using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.ModelDerivative;
using Autodesk.ModelDerivative.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ModelDerivavtiveController : ControllerBase
  {
    private ModelDerivativeClient _mdClient;

    private readonly AuthenticationClient _authenticationClient;

    public ModelDerivavtiveController()
    {
      var sdkManager = Autodesk.SDKManager.SdkManagerBuilder.Create().Build();
      _authenticationClient = new AuthenticationClient(sdkManager);
      _mdClient = new ModelDerivativeClient(sdkManager);

    }

    [HttpPost("translate")]
    public async Task<IActionResult> Translate([FromQuery] string urn)
    {
      try
      {
        string safeUrn = Base64Encode(urn);
        string Token = await GetToken(); // 2 -legged token for server-side operations
        var job = new JobPayload
        {
          Input = new JobPayloadInput { Urn = safeUrn },

          Output = new JobPayloadOutput
          {
            Formats = new List<IJobPayloadFormat>
            {
              new JobPayloadFormatSVF2
              {
               Views = [View._2d, View._3d]

              },

            }
           
          }

        };

        var result = await _mdClient.StartJobAsync(accessToken: Token, jobPayload: job);

        return Ok(result);
      }
      catch (Exception ex)
      {

        return BadRequest($"Translation failed: {ex.Message}");
      }

    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus([FromQuery] string urn)
    {

      try
      {
        string safeUrn = Base64Encode(urn);
        string token = await GetToken();

        var manifest = await _mdClient.GetManifestAsync(accessToken: token, urn: safeUrn);

        return Ok(new
        {
          status = manifest.Status,
          progress = manifest.Progress,

        });
      }

      catch (Exception ex)
      {

        return BadRequest($"Failed to get status: {ex.Message}");
      }


    }
    [HttpGet("manifest")]
    public async Task<IActionResult> GetManifest([FromQuery] string urn)
    {
      try
      {
        string token = await GetToken();
        string safeUrn = Base64Encode(urn);

        var manifest = await _mdClient.GetManifestAsync(
            accessToken: token,
            urn: safeUrn
        );

        // Return full manifest to see all available views
        return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(manifest));
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
    private async Task<string> GetToken()
    {
      var auth = await _authenticationClient.GetTwoLeggedTokenAsync(
          AuthController.ClientId, AuthController.ClientSecret,
          new List<Scopes>
          {
            Scopes.DataRead,
            Scopes.DataWrite,
            Scopes.DataCreate,
            Scopes.BucketCreate,
            Scopes.BucketRead,
            Scopes.ViewablesRead
          }
      );
      return auth.AccessToken;
    }

    private string Base64Encode(string plainText)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
  }
}
