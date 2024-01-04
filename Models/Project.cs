namespace ExpressionGeneratorApp.Models;

public class Project
{
    /// <summary>
    /// Id of the project
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of the project
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Description of the project
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Url of the project
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Team id of the project
    /// </summary>
    public int TeamId { get; set; }
    /// <summary>
    /// Team of the project
    /// </summary>
    public Team Team { get; set; }

    public static List<Project> GetList(int count, int teamCount)
    {
        var random = new Random();
        var id = 1;
        var teams = new List<int>();
        for (int i = 1; i <= teamCount; i++)
            teams.Add(i);

        var result = new List<Project>
        {
            new Project
            {
                Id = id,
                Name = $"Project-{id}",
                Description = $"Description-{id}",
                Url = $"Url-{id}",
                TeamId = RandomData.GetOne(random, teams.ToArray())
            }
        };

        id++;
        while (--count > 0)
        {
            var newProject = new Project
            {
                Id = id,
                Description = $"Description-{id}",
                Url = $"Url-{id}",
                Name = $"Project-{id}",
                TeamId = RandomData.GetOne(random, teams.ToArray())
            };

            id++;
            result.Add(newProject);
        }

        return result;
    }
}
