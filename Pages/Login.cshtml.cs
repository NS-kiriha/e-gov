using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace project1.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Phone or Email is required.")]
        public string Credential { get; set; }

        [BindProperty, Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Message { get; set; } = "";

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Please correct the errors.";
                return Page();
            }

            string phonePattern = @"^\+?[1-9][0-9]{9,14}$"; // Supports international numbers
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            bool isPhone = Regex.IsMatch(Credential, phonePattern);
            bool isEmail = Regex.IsMatch(Credential, emailPattern);

            if (!isPhone && !isEmail)
            {
                ModelState.AddModelError("Credential", "Enter a valid phone number or email address.");
                return Page();
            }

            try
            {
                bool isAuthenticated = AuthenticateUser(Credential, Password, isPhone);

                if (isAuthenticated)
                {
                    Message = "Login successful!";
                    return RedirectToPage("/Index"); // Replace with the actual dashboard page
                }

                ModelState.AddModelError("", "Invalid phone/email or password.");
            }
            catch (Exception ex)
            {
                Message = "An error occurred during login. Please try again later.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Page();
        }

        private bool AuthenticateUser(string credential, string password, bool isPhone)
        {
            string connectionString = "server=localhost;user=root;password=;database=e-gov";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = isPhone
                    ? "SELECT password FROM patient WHERE phone = @Credential"
                    : "SELECT password FROM patient WHERE email = @Credential";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Credential", credential);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string hashedPassword = result.ToString();
                        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    }
                }
            }

            return false;
        }
    }
}
