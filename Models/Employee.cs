namespace ExpressionGeneratorApp.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Department { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }

    public static string GetOne(Random random, string[] list)
        => list[(int)Math.Floor(random.NextDouble() * list.Length)];

    public static Gender GetAge(Random random)
        => Enum.GetValues(typeof(Gender))
               .Cast<Gender>()
               .ToArray()[random.Next(0,2)];

    public static string[] Names = new[]
    { "Mehmet", "Gizem", "Hasan", "Ayşe", "Gülsüm", "Serhat", "Fehmi", "Sinem", "Beyza", "Kemal", "Esra", "Emir" };

    public static string[] Surnames = new[]
    { "Kaya", "Tekin", "Gündüz", "Taş", "Yılmaz", "Sevim", "Derya", "Hikmet", "Şen", "Suna", "Esen", "Reşit" };

    public static string[] Departments = new[]
    { "Technology", "Marketing", "Finance", "Human Resources" };

    public static List<Employee> GetList(int count)
    {
        var random = new Random();
        var id = 1;
        var result = new List<Employee>
            {
                new Employee
                {
                    Id = id++,
                    Name = Names.First(),
                    Surname = Surnames.First(),
                    Department = "Technology",
                    Gender = Models.Gender.Male,
                    Age = 27
                }
            };
        while (--count > 0)
        {
            var newEmployee = new Employee
            {
                Id = id++,
                Name = GetOne(random, Names),
                Surname = GetOne(random, Surnames),
                Department = GetOne(random, Departments),
                Age = random.Next(18,65),
                Gender = GetAge(random)
            };
            result.Add(newEmployee);
        }
        return result;
    }

    public override string ToString()
    {
        return $"Id:{Id} Name:{Name} Department:{Department} Gender:{Gender} Age:{Age}";
    }
}
