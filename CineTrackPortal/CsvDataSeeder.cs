using CineTrackPortal.Data;
using CineTrackPortal.Models;
using CsvHelper;
using System.Globalization;

public static class CsvDataSeeder
{
    public static void SeedMoviesFromCsv(ApplicationDbContext context, string csvPath)
    {
        if (!File.Exists(csvPath)) return;

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>();

        foreach (var record in records)
        {
            string? title = record.names;
            string? dateStr = record.date_x?.Trim();
            string? actorsStr = record.crew; // CSV column with actor names

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(dateStr))
                continue;

            if (!DateTime.TryParseExact(dateStr, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                if (!DateTime.TryParse(dateStr, out date))
                    continue;
            }

            // Avoid duplicates (by title and date)
            if (context.Movies.Any(m => m.Title == title && m.Date == date))
                continue;

            var movie = new MovieModel
            {
                MovieId = Guid.NewGuid(),
                Title = title,
                Date = date,
                Actors = new List<ActorModel>()
            };

            if (!string.IsNullOrWhiteSpace(actorsStr))
            {
                var actorNames = actorsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var actorFullName in actorNames)
                {
                    // Split full name into first and last name
                    var nameParts = actorFullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (nameParts.Length < 2)
                        continue; // Skip if not both first and last name

                    string firstName = nameParts[0];
                    string lastName = nameParts[1];

                    // Check if actor already exists
                    var actor = context.Actors.FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName);
                    if (actor == null)
                    {
                        actor = new ActorModel
                        {
                            ActorId = Guid.NewGuid(),
                            FirstName = firstName,
                            LastName = lastName
                        };
                        context.Actors.Add(actor);
                    }
                    movie.Actors.Add(actor);
                }
            }

            context.Movies.Add(movie);
        }
        context.SaveChanges();
    }
}