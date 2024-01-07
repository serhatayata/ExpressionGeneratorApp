using ExpressionGeneratorApp.Models;
using System;

namespace ExpressionGeneratorApp.Entities;

public class Team
{
    /// <summary>
    /// Id of the team
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of the team
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Department Id of the team
    /// </summary>
    public int DepartmentId { get; set; }
    /// <summary>
    /// Department of the team
    /// </summary>
    public Department Department { get; set; }
    /// <summary>
    /// Projects of the team
    /// </summary>
    public ICollection<Project> Projects { get; set; }
    /// <summary>
    /// Employees of the team
    /// </summary>
    public ICollection<Employee> Employees { get; set; }

    public static List<Team> GetList(int count, int departmentCount)
    {
        var random = new Random();
        var id = 1;
        var departments = new List<int>();
        for (int i = 1; i <= departmentCount; i++)
            departments.Add(i);

        var result = new List<Team>
        {
            new Team
            {
                Id = id,
                Name = $"Team-{id}",
                DepartmentId = RandomData.GetOne(random, departments.ToArray())
            }
        };

        id++;
        while (--count > 0)
        {
            var newTeam = new Team
            {
                Id = id,
                Name = $"Team-{id}",
                DepartmentId = RandomData.GetOne(random, departments.ToArray())
            };

            id++;
            result.Add(newTeam);
        }
        return result;
    }

    public override string ToString()
    {
        return $"Id:{Id} Name:{Name} DepartmentId:{DepartmentId} ProjectCount: {Projects.Count()} EmployeeCount: {Employees.Count()}";
    }
}
