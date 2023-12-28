using ExpressionGeneratorApp;
using ExpressionGeneratorApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

Console.WriteLine("Application started");
var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=transactions.db").Options;

using (var context = new AppDbContext(options))
{
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
    context.Employees.AddRange(Employee.GetList(1000));
    await context.SaveChangesAsync();

    var count = await context.Employees.CountAsync();
    Console.WriteLine($"Insert count: {count}.");
    Console.WriteLine("Parsing expression started");
    var parser = new JsonExpressionParser();

    var ruleFile = await File.ReadAllTextAsync("databaseRules.json");
    var jsonDocument = JsonDocument.Parse(ruleFile);
    var expression = parser.ParseExpressionOf<Employee>(jsonDocument);
    var predicate = parser.ParsePredicateOf<Employee>(jsonDocument);
    Console.WriteLine("Started getting data from database...");

    var listQuery = Employee.GetList(1000).Where(predicate);
    var query = context.Employees.Where(expression)
                                 .OrderBy(t => t.Id);

    var results = await query.ToListAsync();
    Console.WriteLine($"Retrieved {results.Count}");
}


