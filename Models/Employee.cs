namespace ExpressionGeneratorApp.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Department { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public DateTime Birthdate { get; set; }
    public bool Status { get; set; }

    public static List<Employee> GetList(int count)
    {
        var random = new Random();
        var id = 1;
        var result = new List<Employee>
            {
                new Employee
                {
                    Id = id++,
                    Name = RandomData.Names.First(),
                    Surname = RandomData.Surnames.First(),
                    Department = "Technology",
                    Gender = Models.Gender.Male,
                    Age = 27,
                    Birthdate = new DateTime(1996, 2, 2),
                    Status = RandomData.GetStatus(random)
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
                Department = RandomData.GetOne(random, RandomData.Departments),
                Age = GetAge(birthdate),
                Gender = RandomData.GetGender(random),
                Birthdate = birthdate,
                Status = RandomData.GetStatus(random)
            };
            result.Add(newEmployee);
        }
        return result;
    }

    public static int GetAge(DateTime birthdate)
    {
        TimeSpan span = DateTime.Now - birthdate;
        return (DateTime.Now + span).Year - 1;
    }

    public override string ToString()
    {
        return $"Id:{Id} Name:{Name} Department:{Department} Gender:{Gender} Age:{Age}";
    }
}
