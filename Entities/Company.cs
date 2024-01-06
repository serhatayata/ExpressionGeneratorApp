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

    public override string ToString()
    {
        return $"Id:{Id} Name:{Name} Founder: {Founder} FoundedAtYear: {FoundedAtYear} DepartmentCount: {Departments.Count()}";
    }
}
