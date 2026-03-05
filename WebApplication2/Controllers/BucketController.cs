using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.Oss;
using Autodesk.Oss.Model;
using Autodesk.SDKManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BucketController : ControllerBase
  {
    private readonly OssClient _ossClient;

    private readonly AuthenticationClient _authenticationClient;

  


    public BucketController()
    {
      var sdkManager = SdkManagerBuilder.Create().Build();

      _authenticationClient = new AuthenticationClient(sdkManager);

      _ossClient = new OssClient(sdkManager);


    }


    [HttpPost("bucket")]
    public async Task<IActionResult> CreateBucket([FromQuery] string bucketKey)
    {
      try
      {
        string token = await GetToken();


        await _ossClient.CreateBucketAsync(
          accessToken: token,
          xAdsRegion: Autodesk.Oss.Model.Region.US,
          bucketsPayload:new CreateBucketsPayload
          {
            BucketKey= bucketKey.ToLower(),
            PolicyKey=PolicyKey.Transient

          }

          );

        return Ok($"Bucket '{bucketKey}' created successfully.");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
    [HttpPost("Upload")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<IActionResult> UploadFile([FromQuery] string bucketKey, IFormFile file)
    {
      if (file == null || file.Length == 0)
        return BadRequest("Please select a file.");

      try
      {
        string token = await GetToken();
        using var stream = file.OpenReadStream();
        var result = await _ossClient.UploadObjectAsync(
            bucketKey: bucketKey.ToLower(),
            objectKey: file.FileName,
            sourceToUpload: stream,
            accessToken: token
        );
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest($"Upload failed: {ex.Message}");
      }
    }

    private async Task<string> GetToken()
    {
      var auth = await _authenticationClient.GetTwoLeggedTokenAsync(AuthController.ClientId, AuthController.ClientSecret,
          new List<Scopes> {
            Scopes.DataRead,       
            Scopes.DataCreate,
            Scopes.DataWrite,
            Scopes.AccountRead,
            Scopes.BucketCreate,
            Scopes.BucketRead
          }
      );
      return auth.AccessToken;
    }
    [HttpGet("files")]
    public async Task<IActionResult> GetFiles([FromQuery] string bucketKey)
    {
      try
      {
        string token = await GetToken();
        var objects = await _ossClient.GetObjectsAsync(
            accessToken: token,
            bucketKey: bucketKey.ToLower()
        );
        var result = objects.Items.Select(obj => new
        {
          name = obj.ObjectKey,
          urn = obj.ObjectId,  // This is the URN you pass to Model Derivative
          size = obj.Size
        });
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
