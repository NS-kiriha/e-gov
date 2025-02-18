using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Linq;

namespace project1.Pages
{
    public class ticketModel : PageModel
    {
        [BindProperty] public string FullName { get; set; }
        [BindProperty] public int Age { get; set; }
        [BindProperty] public string Gender { get; set; }
        [BindProperty] public string Contact { get; set; }
        [BindProperty] public DateTime AppointmentDate { get; set; }
        [BindProperty] public string Department { get; set; }
        public string Message { get; set; } = "";

        private readonly string _connectionString = "server=localhost;user=root;password=;database=egov_project";

        public void OnGet()
        {
            Message = "";
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Please correct the errors and try again.";
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Message += $" Error: {error.ErrorMessage}";
                }
                return Page();
            }

            // Generate a secure 8-character random ticket ID
            string ticketId = GenerateRandomId(8);

            // Insert the patient record with 'Waiting' status
            string query = @"INSERT INTO patients (Id, fullName, Age, Gender, Contact, AppointmentDate, Department, status)
                             VALUES (@TicketId, @FullName, @Age, @Gender, @Contact, @AppointmentDate, @Department, 'Waiting');";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TicketId", ticketId);
                        cmd.Parameters.AddWithValue("@FullName", FullName);
                        cmd.Parameters.AddWithValue("@Age", Age);
                        cmd.Parameters.AddWithValue("@Gender", Gender);
                        cmd.Parameters.AddWithValue("@Contact", Contact);
                        cmd.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                        cmd.Parameters.AddWithValue("@Department", Department);

                        int result = cmd.ExecuteNonQuery(); // Execute insert query

                        if (result > 0)
                        {
                            return RedirectToPage("./TicketDetails", new { id = ticketId });
                            // Redirect to ticket details
                        }
                        else
                        {
                            Message = "There was an issue generating your ticket. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}, StackTrace: {ex.StackTrace}";
            }

            return Page();
        }

        // Generate a secure alphanumeric random ticket ID
        private string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
