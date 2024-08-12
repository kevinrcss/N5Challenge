using Microsoft.AspNetCore.Mvc;
using N5Challenge.Application.DTOs;
using N5Challenge.Application.Interfaces;

namespace N5Challenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission(PermissionDto permissionDto)
        {
            var result = await _permissionService.RequestPermissionAsync(permissionDto);
            if (result.Success) 
            { 
                return Ok(result); 
            }
            return BadRequest(result);
        }

        [HttpPut("modify")]
        public async Task<IActionResult> ModifyPermission(PermissionDto permissionDto)
        {
            var result = await _permissionService.ModifyPermissionAsync(permissionDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions([FromQuery] string searchTerm)
        {
            var result = await _permissionService.GetPermissionsAsync(searchTerm);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermission(int id)
        {
            var result = await _permissionService.GetPermissionByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
