# JobApplicationTracker
Track your job applications directly from the terminal! Built with C# and JSON, this console app helps you stay organized while applying for jobs.

## Features

- Add new job applications (company, position, reference number, dates, status, contact info, notes)
- View all applications
- Filter applications by status (Applied, Interview, Rejected, Offered)
- Update application status and notes
- Search applications by company, position, or reference number
- View statistics:
  - Total applications
  - Applications by status
  - Upcoming deadlines
- Persistent storage in a JSON file (`jobApplications.json`)

---

## Technologies

- **C#** (.NET 6 or later recommended)
- Console Application
- JSON storage (`System.Text.Json`)
