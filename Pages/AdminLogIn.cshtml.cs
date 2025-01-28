using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace project1.Pages
{
    public class AdminLogInModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Employee ID is required.")]
        public string E_Id { get; set; }

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

            // Regex pattern for employee ID (can be adjusted as needed)
            string eidPattern = "^\\+?[1-9][0-9]{9,14}$";

            // Validate the E_Id format
            bool isValidEid = Regex.IsMatch(E_Id, eidPattern);

            if (!isValidEid)
            {
                ModelState.AddModelError("E_Id", "Enter a valid employee ID.");
                return Page();
            }

            try
            {
                // Authenticate the user with the provided credentials
                bool isAuthenticated = AuthenticateUser(E_Id, Password);

                if (isAuthenticated)
                {
                    Message = "Login successful!";
                    return RedirectToPage("/Index"); // Replace with your actual dashboard page
                }

                ModelState.AddModelError("", "Invalid Employee ID or password.");
            }
            catch (Exception ex)
            {
                // Log the exception and inform the user
                Message = "An error occurred during login. Please try again later.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Page();
        }

        private bool AuthenticateUser(string eId, string password)
        {
            const string connectionString = "server=localhost;user=root;password=;database=e-gov";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to fetch the password for the given E_Id
                    string query = "SELECT password FROM admin WHERE E_Id = @E_Id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@E_Id", eId);

                        // Execute the query and fetch the result
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            // Compare the provided password directly with the stored password
                            string storedPassword = result.ToString();
                            return password == storedPassword;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                Console.WriteLine($"Database error: {ex.Message}");
                throw;
            }

            return false;
        }
    }
}
