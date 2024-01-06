namespace ExpressionGeneratorApp.Entities;

public class Department
{
    /// <summary>
    /// Id of the department
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of the department
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Description of the department
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Employees of the department
    /// </summary>
    public ICollection<Team> Teams { get; set; }

    public static List<Department> GetList(int count)
    {
        var random = new Random();
        var id = 1;

        var result = new List<Department>
        {
            new Department
            {
                Id = id,
                Title = $"Department-{id}",
                Description = $"Description-{id}"
            }
        };

        id++;
        while (--count > 0)
        {
            var newDepartment = new Department
            {
                Id = id,
                Title = $"Department-{id}",
                Description = $"Description-{id}"
            };

            id++;
            result.Add(newDepartment);
        }
        return result;
    }

    public override string ToString()
    {
        return $"Id:{Id} Title:{Title} Description: {Description}";
    }
}
