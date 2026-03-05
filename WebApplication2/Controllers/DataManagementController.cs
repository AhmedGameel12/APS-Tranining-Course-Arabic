using Autodesk.Authentication;
using Autodesk.DataManagement;
using Autodesk.SDKManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APS.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataManagementController : ControllerBase
  {
    private static AuthenticationClient _authenticationClient;
    private readonly DataManagementClient _dmClient;
    public DataManagementController()
    {
      var sDKManager = SdkManagerBuilder.Create().Build();
      _authenticationClient = new AuthenticationClient(sDKManager);

      _dmClient = new DataManagementClient(sDKManager);

    }

    [HttpGet("hubs")]

    public async Task<IActionResult> GetHubs()
    {
      try
      {
        string token = AuthController.GetStoredToken();
        var hubs = await _dmClient.GetHubsAsync(accessToken:token);

        if (hubs.Data != null)
        {
          var results = hubs.Data.Select(hub => new
          {
            Id = hub.Id,
            Name = hub.Attributes.Name,
            Type = hub.Type
          });

          return Ok(results);
        }
        else
        {
          return Ok("No hubs found for the authenticated user.");
        }

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("hubs/{hubId}/projects")]
    public async Task<IActionResult> GetProjects(string hubId)
    {
      try
      {
        string token = AuthController.GetStoredToken();
        var projects = await _dmClient.GetHubProjectsAsync(
           hubId: hubId,
           accessToken: token
       );
        if (projects.Data != null)
        {
          var results = projects.Data.Select(project => new
          {
            Id = project.Id,
            Name = project.Attributes.Name,
            Type = project.Type
          });
          return Ok(results);
        }
        else
        {
          return Ok("No projects found for the specified hub.");
        }
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }

    }
  }
}
