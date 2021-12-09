using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TestWebAppCore.Controllers
{
    public class MyClaimsController : Controller
    {

        [Authorize]
        [Route("/me/claims")]
        public IActionResult Index()
        {
            // The NameIdentifier
            var nameIdentifier = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
