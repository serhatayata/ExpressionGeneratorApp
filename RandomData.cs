using ExpressionGeneratorApp.Models;

namespace ExpressionGeneratorApp;

public  static class RandomData
{
    public static string GetOne(Random random, string[] list)
    => list[(int)Math.Floor(random.NextDouble() * list.Length)];

    public static int GetOne(Random random, int[] list)
    => list[(int)Math.Floor(random.NextDouble() * list.Length)];

    public static Gender GetGender(Random random)
        => Enum.GetValues(typeof(Gender))
               .Cast<Gender>()
               .ToArray()[random.Next(0, 2)];

    public static bool GetStatus(Random random) => random.Next(0, 2) == 1;

    public static string[] Names = new[]
    { "Mehmet", "Gizem", "Hasan", "Ayşe", "Gülsüm", "Serhat", "Fehmi", "Sinem", "Beyza", "Kemal", "Esra", "Emir" };

    public static string[] Surnames = new[]
    { "Kaya", "Tekin", "Gündüz", "Taş", "Yılmaz", "Sevim", "Derya", "Hikmet", "Şen", "Suna", "Esen", "Reşit" };

    public static string[] Titles = new[]
    { "Technology", "Marketing", "Finance", "Human Resources" };

    public static string[] CompanyNames = new[]
    { "Tech", "ArtDic", "Normal", "HRS", "AS", "VR", "Artificial", "New", "Data", "ES", "MB", "HTech", "Soft", "Comp", "System", "Network", "CS", "WD", "HC", "COMP" };

    public static string GetCompanyName(Random random)
    {
        var name1 = GetOne(random, CompanyNames);
        var name2 = GetOne(random, CompanyNames);
        while(string.Equals(name1, name2))
            name2 = GetOne(random, CompanyNames);

        return string.Join(" ", name1, name2);
    }

    public static int GetRandomYear(Random random) => random.Next(1990, 2020);

    public static DateTime GetBirthdate(Random random)
    {
        var startDate = new DateTime(1955, 1, 1);
        var endDate = new DateTime(2000, 1, 1);

        TimeSpan timeSpan = endDate - startDate;
        TimeSpan newSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
        DateTime newDate = startDate + newSpan;

        return newDate.Date;
    }
}
