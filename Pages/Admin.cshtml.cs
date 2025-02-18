using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace project1.Pages
{
    public class AdminModel : PageModel
    {


        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; } = false;

        public IActionResult OnPost()
        {
            if (Email == "yagraj123@gmail.com" && Password == "yagraj123")
            {
               HttpContext.Session.SetString("Login", "true"); // Set session value
                return RedirectToPage("/admin/dashboard");
            }

               HttpContext.Session.SetString("Login", "false"); // Set session value
               return Page();
        }

        //logout 

        public IActionResult OnPostLogout()
        {
          
           HttpContext.Session.Clear();

           // Redirect to Admin or Login page after logout
           return RedirectToPage("/index");
         }
        public IActionResult OnGet()
        {

            // Retrieve login status from session
                       //var loginStatus = HttpContext.Session.GetString("Login");
                        //IsLoggedIn = loginStatus == "true"; // Set property for use in the view
                        var loginStatus = HttpContext.Session.GetString("Login");
                       if (loginStatus == "true")
                        {
                            IsLoggedIn = true;
                        }
                        //Redirects user to Dashboard if already logged in
                       if (IsLoggedIn == true)
                        {
                            return RedirectToPage("/admin/dashboard");
                        }
                        return Page();

        }
    }
}

