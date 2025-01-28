using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;

namespace project1.Pages
{
    public class CreatePatientModel : PageModel
    {
        [BindProperty, Required, StringLength(100)]
        public string Name { get; set; }

        [BindProperty, Required, Phone]
        public string Phone { get; set; }

        [BindProperty, Required, EmailAddress]
        public string Email { get; set; }

        [BindProperty, Required]
        public string DoB { get; set; }

        [BindProperty, Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty, Required]
        public string Gender { get; set; }

        public string Message { get; set; } = "";

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Please correct the errors and try again.";
                return Page();
            }

            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            string constr = "server=localhost;user=root;password=;database=e-gov";

            try
            {
                using (var con = new MySqlConnection(constr))
                {
                    con.Open();

                    string sql = @"INSERT INTO patient (name, phone, email, dob, password, gender)
                                   VALUES (@Name, @Phone, @Email, @DoB, @Password, @Gender)";

                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Phone", Phone);
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@DoB", DoB);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Gender", Gender);

                        int result = cmd.ExecuteNonQuery();

                        Message = result > 0 ? "Saved Successfully." : "Saving Failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
            }

            return Page();
        }

        public void OnGet()
        {
            // Optionally load data for dropdowns or checkboxes
        }
    }
}
