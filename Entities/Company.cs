namespace ExpressionGeneratorApp.Entities;

public class Company
{
    /// <summary>
    /// Id of the company
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of the company
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Founder name and surname of the company
    /// </summary>
    public string Founder { get; set; }
    /// <summary>
    /// Year founded at
    /// </summary>
    public int FoundedAtYear { get; set; }
    /// <summary>
    /// Departments of the company
    /// </summary>
    public ICollection<Department> Departments { get; set; }

    public static List<Company> GetList(int count)
    {
        var random = new Random();
        var id = 1;

        var result = new List<Company>
        {
            new Company
            {
                Id = id,
                Name = RandomData.GetCompanyName(random),
                FoundedAtYear = RandomData.GetRandomYear(random),
                Founder = RandomData.GetOne(random, RandomData.Names) +
                          RandomData.GetOne(random, RandomData.Surnames)
            }
        };

        id++;
        while (--count > 0)
        {
            var newDepartment = new Company
            {
                Id = id,
                Name = RandomData.GetCompanyName(random),
                FoundedAtYear = RandomData.GetRandomYear(random),
                Founder = RandomData.GetOne(random, RandomData.Names) + 
                          RandomData.GetOne(random, RandomData.Surnames)
            };

            id++;
            result.Add(newDepartment);
        }
        return result;
    }

    public override string ToString()
    {
        return $"Id:{Id} Name:{Name} Founder: {Founder} FoundedAtYear: {FoundedAtYear} DepartmentCount: {Departments.Count()}";
    }
}
