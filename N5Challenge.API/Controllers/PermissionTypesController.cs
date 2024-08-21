using Microsoft.AspNetCore.Mvc;
using N5Challenge.Application.Interfaces;

namespace N5Challenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionTypesController : ControllerBase
    {
        private readonly IPermissionTypeServices _permissionTypeServices;

        #region Constructor
        public PermissionTypesController(IPermissionTypeServices permissionTypeServices)
        {
            _permissionTypeServices = permissionTypeServices;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            var result = await _permissionTypeServices.GetPermissionTypesAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermission(int id)
        {
            var result = await _permissionTypeServices.GetPermissionTypeByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

    }


}
