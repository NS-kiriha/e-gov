using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace project1.Pages.admin
{
    public class dashboardModel : PageModel
    {
        public List<Doctor> AvailableDoctorsList { get; set; } = new List<Doctor>();
        public List<Patient> QueueList { get; set; } = new List<Patient>();
        public List<Patient> Patients { get; set; } = new List<Patient>();

        private readonly string _connectionString = "server=localhost;user=root;password=;database=egov_project";

        public void OnGet()
        {
            Console.WriteLine("OnGet Executed"); // Debugging log
            LoadAvailableDoctors();
            LoadQueueList();
            LoadPatientData();
        }

        // 🔹 Load Available Doctors
        private void LoadAvailableDoctors()
        {
            AvailableDoctorsList.Clear(); // Prevent duplication
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Name FROM doctors WHERE available = 1";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AvailableDoctorsList.Add(new Doctor
                                {
                                    Name = reader["Name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading doctors: {ex.Message}");
            }
        }

        // 🔹 Load Queue List (Waiting Patients)
        private void LoadQueueList()
        {
            QueueList.Clear(); // Prevent duplication
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
                                QueueList.Add(new Patient
                                {
                                    TicketId = reader["Id"].ToString(),
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
                Console.WriteLine($"Error loading queue list: {ex.Message}");
            }
        }

        // 🔹 Load Patient List
        private void LoadPatientData()
        {
            Patients.Clear(); // Prevent duplication
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, FullName, Age, Gender, Contact, Department FROM patients";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Patients.Add(new Patient
                                {
                                    TicketId = reader["Id"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? 0 : Convert.ToInt32(reader["Age"]),
                                    Gender = reader["Gender"].ToString(),
                                    Contact = reader["Contact"].ToString(),
                                    Department = reader["Department"].ToString()
                                });
                            }
                        }
                    }
                }
                Console.WriteLine($"Loaded {Patients.Count} patients"); // Debugging log
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patient data: {ex.Message}");
            }
        }

        // 🔹 Handle Call Patient
        public IActionResult OnPostCallPatient(string ticketId)
        {
            try
            {
                // Update the status of the patient to 'Called' in the database
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE patients SET Status = 'Called' WHERE Id = @ticketId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ticketId", ticketId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reload the queue list after the update
                LoadQueueList();

                return RedirectToPage(); // Refresh the page to reflect the changes
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling patient: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Delete method for removing patient (existing functionality)
        public IActionResult OnPostDelete(string ticketId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM patients WHERE Id = @ticketId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ticketId", ticketId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadPatientData();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting patient: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    // Patient class
    public class Patient
    {
        public string TicketId { get; set; }
        public string FullName { get; set; }
        public string Contact { get; set; }
        public string Department { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
    }

    // Doctor class
    public class Doctor
    {
        public string Name { get; set; }
    }
}
