using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{

    public RedirectToPageResult OnGet()
    {
        return RedirectToPage("/Providers/Index");
    }
}