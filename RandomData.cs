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
