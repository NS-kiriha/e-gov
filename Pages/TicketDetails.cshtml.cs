using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;

namespace project1.Pages
{
    public class TicketDetailsModel : PageModel
    {
        public string TicketId { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Contact { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Department { get; set; }

        private readonly string _connectionString = "server=localhost;user=root;password=;database=egov_project";

        public IActionResult OnGet(string id) // ✅ Use string instead of int
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("/Error"); // Redirect if ID is missing
            }

            string query = "SELECT * FROM patients WHERE Id = @TicketId"; // Make sure the column name is correct

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TicketId", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TicketId = id;
                                FullName = reader["fullName"].ToString();
                                Age = Convert.ToInt32(reader["Age"]);
                                Gender = reader["Gender"].ToString();
                                Contact = reader["Contact"].ToString();
                                AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]);
                                Department = reader["Department"].ToString();
                            }
                            else
                            {
                                return RedirectToPage("/Error"); // Redirect if ticket not found
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToPage("/Error"); // Redirect on error
            }

            return Page();
        }
    }
}
