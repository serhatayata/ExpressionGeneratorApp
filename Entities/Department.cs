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
    /// Id of the company
    /// </summary>
    public int CompanyId { get; set; }
    /// <summary>
    /// Company of department
    /// </summary>
    public Company Company { get; set; }
    /// <summary>
    /// Employees of the department
    /// </summary>
    public ICollection<Team> Teams { get; set; }

    public static List<Department> GetList(int count, int companyCount)
    {
        var random = new Random();
        var id = 1;
        var companies = new List<int>();
        for (int i = 1; i <= companyCount; i++)
            companies.Add(i);

        var result = new List<Department>
        {
            new Department
            {
                Id = id,
                Title = $"Department-{id}",
                Description = $"Description-{id}",
                CompanyId = RandomData.GetOne(random, companies.ToArray())
            }
        };

        id++;
        while (--count > 0)
        {
            var newDepartment = new Department
            {
                Id = id,
                Title = $"Department-{id}",
                Description = $"Description-{id}",
                CompanyId = RandomData.GetOne(random, companies.ToArray())
            };

            id++;
            result.Add(newDepartment);
        }
        return result;
    }

    public override string ToString()
    {
        return $"Id:{Id} Title:{Title} Description: {Description} TeamsCount: {Teams.Count()}";
    }
}
