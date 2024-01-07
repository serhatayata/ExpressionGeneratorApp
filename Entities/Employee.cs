using System;
using ExpressionGeneratorApp.Models;

namespace ExpressionGeneratorApp.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public DateTime Birthdate { get; set; }
    public bool Status { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; }

    public static List<Employee> GetList(int count, int teamCount)
    {
        var random = new Random();
        var id = 1;
        var teams = new List<int>();
        for (int i = 1; i <= teamCount; i++)
            teams.Add(i);

        var result = new List<Employee>
        {
            new Employee
            {
                Id = id++,
                Name = RandomData.Names.First(),
                Surname = RandomData.Surnames.First(),
                Gender = Gender.Male,
                Age = 27,
                Birthdate = new DateTime(1996, 2, 2),
                Status = RandomData.GetStatus(random),
                TeamId = RandomData.GetOne(random, teams.ToArray())
            }
        };
        while (--count > 0)
        {
            var birthdate = RandomData.GetBirthdate(random);

            var newEmployee = new Employee
            {
                Id = id++,
                Name = RandomData.GetOne(random, RandomData.Names),
                Surname = RandomData.GetOne(random, RandomData.Surnames),
                Age = GetAge(birthdate),
                Gender = RandomData.GetGender(random),
                Birthdate = birthdate,
                Status = RandomData.GetStatus(random),
                TeamId = RandomData.GetOne(random, teams.ToArray())
            };
            result.Add(newEmployee);
        }
        return result;
    }

    public static int GetAge(DateTime birthdate)
    {
        TimeSpan span = DateTime.Now - birthdate;
        return new DateTime(span.Ticks).Year - 1;
    }

    public override string ToString()
    {
        return $"Id:{Id} Name:{Name} Gender:{Gender} Age:{Age}";
    }
}
