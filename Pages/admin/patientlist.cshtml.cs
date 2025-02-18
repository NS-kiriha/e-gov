using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace project1.Pages.admin
{
    public class patientlistModel : PageModel
    {
        public List<Patient> Patients { get; set; } = new List<Patient>();

        private readonly string _connectionString = "server=localhost;user=root;password=;database=egov_project";

        public void OnGet()
        {
            LoadPatientData();
        }

        private void LoadPatientData()
        {
            Patients.Clear(); // 🛠️ Prevent duplicate records

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, FullName, Contact, Department FROM patients WHERE Status = 'Waiting' ORDER BY AppointmentDate ASC";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Patients.Add(new Patient
                                {
                                    TicketId = reader["TicketId"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Contact = reader["Contact"].ToString(),
                                    Department = reader["Department"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patient data: {ex.Message}");
            }
        }

        public IActionResult OnPostDelete(string id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM patients WHERE TicketId = @TicketId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TicketId", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting patient: {ex.Message}");
                return Page();
            }
        }

        public class Patient
        {
            public string TicketId { get; set; }
            public string FullName { get; set; }
            public string Contact { get; set; }
            public string Department { get; set; }
        }
    }
}
