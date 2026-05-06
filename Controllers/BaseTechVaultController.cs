using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechVault.API.Controllers
{
    public abstract class BaseTechVaultController : ControllerBase
    {
        protected Guid CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Guid.Empty;
                return Guid.Parse(userIdClaim.Value);
            }
        }

        protected bool IsAdmin => User.IsInRole("Admin");
        protected bool IsTechnician => User.IsInRole("Technician");
    }
}
