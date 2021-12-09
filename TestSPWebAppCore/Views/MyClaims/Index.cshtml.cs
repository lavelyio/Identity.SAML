using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestWebAppCore.Views.MyClaims
{
    public class IndexModel : PageModel
    {
        private bool isVerifyingClaims { get; set; }

        private bool shouldRender { get; set; }

        public void OnGet()
        {
        }

    }
}
