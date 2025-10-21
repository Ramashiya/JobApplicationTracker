using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JobApplicationTracker
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } // Applied, Interview, Rejected, Offered
        public string ApplicationMethod { get; set; } // Email, Online, Post
        public DateTime ClosingDate { get; set; }
        public string Notes { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
    }

    public class ApplicationManager
    {
        private List<JobApplication> applications;
        private readonly string dataFile = "jobApplications.json";
        private int nextId = 1;

        public ApplicationManager()
        {
            applications = new List<JobApplication>();
            LoadApplications();
        }

        // Load applications from JSON file
        private void LoadApplications()
        {
            if (File.Exists(dataFile))
            {
                try
                {
                    string json = File.ReadAllText(dataFile);
                    applications = JsonSerializer.Deserialize<List<JobApplication>>(json);
                    nextId = applications.Count > 0 ? applications.Max(a => a.Id) + 1 : 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading data: {ex.Message}");
                    applications = new List<JobApplication>();
                }
            }
        }

        // Save applications to JSON file
        private void SaveApplications()
        {
            try
            {
                string json = JsonSerializer.Serialize(applications, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        // Add new application
        public void AddApplication()
        {
            Console.WriteLine("\n=== ADD NEW JOB APPLICATION ===");

            var application = new JobApplication();
            application.Id = nextId++;

            Console.Write("Company: ");
            application.Company = Console.ReadLine();

            Console.Write("Position: ");
            application.Position = Console.ReadLine();

            Console.Write("Reference Number: ");
            application.ReferenceNumber = Console.ReadLine();

            Console.Write("Application Date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime appDate))
                application.ApplicationDate = appDate;
            else
                application.ApplicationDate = DateTime.Now;

            Console.Write("Closing Date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime closeDate))
                application.ClosingDate = closeDate;

            Console.Write("Application Method (Email/Online/Post): ");
            application.ApplicationMethod = Console.ReadLine();

            Console.Write("Status (Applied/Interview/Rejected/Offered): ");
            application.Status = Console.ReadLine();

            Console.Write("Contact Person: ");
            application.ContactPerson = Console.ReadLine();

            Console.Write("Contact Email: ");
            application.ContactEmail = Console.ReadLine();

            Console.Write("Notes: ");
            application.Notes = Console.ReadLine();

            applications.Add(application);
            SaveApplications();
            Console.WriteLine("Application added successfully!");
        }

        // View all applications
        public void ViewAllApplications()
        {
            Console.WriteLine("\n=== ALL JOB APPLICATIONS ===");

            if (!applications.Any())
            {
                Console.WriteLine("No applications found.");
                return;
            }

            foreach (var app in applications)
            {
                DisplayApplication(app);
                Console.WriteLine("------------------------");
            }
        }

        // View applications by status
        public void ViewApplicationsByStatus()
        {
            Console.Write("Enter status to filter (Applied/Interview/Rejected/Offered): ");
            string status = Console.ReadLine();

            var filtered = applications.Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

            Console.WriteLine($"\n=== APPLICATIONS WITH STATUS: {status.ToUpper()} ===");

            if (!filtered.Any())
            {
                Console.WriteLine("No applications found with this status.");
                return;
            }

            foreach (var app in filtered)
            {
                DisplayApplication(app);
                Console.WriteLine("------------------------");
            }
        }

        // Update application status
        public void UpdateApplicationStatus()
        {
            Console.Write("Enter Application ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var application = applications.FirstOrDefault(a => a.Id == id);
                if (application != null)
                {
                    Console.Write("New Status (Applied/Interview/Rejected/Offered): ");
                    application.Status = Console.ReadLine();

                    Console.Write("Update Notes: ");
                    application.Notes = Console.ReadLine();

                    SaveApplications();
                    Console.WriteLine("Application updated successfully!");
                }
                else
                {
                    Console.WriteLine("Application not found!");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID!");
            }
        }

        // Search applications
        public void SearchApplications()
        {
            Console.Write("Search term (company/position/reference): ");
            string searchTerm = Console.ReadLine().ToLower();

            var results = applications.Where(a =>
                a.Company.ToLower().Contains(searchTerm) ||
                a.Position.ToLower().Contains(searchTerm) ||
                a.ReferenceNumber.ToLower().Contains(searchTerm)).ToList();

            Console.WriteLine($"\n=== SEARCH RESULTS FOR: '{searchTerm}' ===");

            if (!results.Any())
            {
                Console.WriteLine("No applications found.");
                return;
            }

            foreach (var app in results)
            {
                DisplayApplication(app);
                Console.WriteLine("------------------------");
            }
        }

        // Display statistics
        public void ShowStatistics()
        {
            Console.WriteLine("\n=== APPLICATION STATISTICS ===");
            Console.WriteLine($"Total Applications: {applications.Count}");

            var statusGroups = applications.GroupBy(a => a.Status)
                                         .Select(g => new { Status = g.Key, Count = g.Count() });

            foreach (var group in statusGroups)
            {
                Console.WriteLine($"{group.Status}: {group.Count}");
            }

            // Show upcoming deadlines (within 7 days)
            var upcoming = applications.Where(a => a.ClosingDate >= DateTime.Now &&
                                                 a.ClosingDate <= DateTime.Now.AddDays(7))
                                     .OrderBy(a => a.ClosingDate)
                                     .ToList();

            if (upcoming.Any())
            {
                Console.WriteLine("\n=== UPCOMING DEADLINES (Next 7 days) ===");
                foreach (var app in upcoming)
                {
                    Console.WriteLine($"{app.Company} - {app.Position} - Due: {app.ClosingDate:yyyy-MM-dd}");
                }
            }
        }

        // Display single application
        private void DisplayApplication(JobApplication app)
        {
            Console.WriteLine($"ID: {app.Id}");
            Console.WriteLine($"Company: {app.Company}");
            Console.WriteLine($"Position: {app.Position}");
            Console.WriteLine($"Ref No: {app.ReferenceNumber}");
            Console.WriteLine($"Applied: {app.ApplicationDate:yyyy-MM-dd}");
            Console.WriteLine($"Closes: {app.ClosingDate:yyyy-MM-dd}");
            Console.WriteLine($"Method: {app.ApplicationMethod}");
            Console.WriteLine($"Status: {app.Status}");
            Console.WriteLine($"Contact: {app.ContactPerson} ({app.ContactEmail})");
            Console.WriteLine($"Notes: {app.Notes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ApplicationManager manager = new ApplicationManager();
            bool exit = false;

            Console.WriteLine("=== JOB APPLICATION TRACKER ===");

            while (!exit)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.AddApplication();
                        break;
                    case "2":
                        manager.ViewAllApplications();
                        break;
                    case "3":
                        manager.ViewApplicationsByStatus();
                        break;
                    case "4":
                        manager.UpdateApplicationStatus();
                        break;
                    case "5":
                        manager.SearchApplications();
                        break;
                    case "6":
                        manager.ShowStatistics();
                        break;
                    case "7":
                        exit = true;
                        Console.WriteLine("Good luck with your job search!");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("=== JOB APPLICATION TRACKER MENU ===");
            Console.WriteLine("1. Add New Application");
            Console.WriteLine("2. View All Applications");
            Console.WriteLine("3. View Applications by Status");
            Console.WriteLine("4. Update Application Status");
            Console.WriteLine("5. Search Applications");
            Console.WriteLine("6. Show Statistics");
            Console.WriteLine("7. Exit");
            Console.Write("Choose an option (1-7): ");
        }
    }
}