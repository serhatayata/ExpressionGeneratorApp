using ExpressionGeneratorApp;
using ExpressionGeneratorApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

Console.WriteLine("Application started");
var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=transactions.db").Options;

int departmentCount = 4, teamCount = 8, employeeCount = 1000, projectCount = 32;

var departments = Department.GetList(departmentCount);
var teams = Team.GetList(teamCount, departmentCount);
var employees = Employee.GetList(employeeCount, teamCount);
var projects = Project.GetList(projectCount, teamCount);

using (var context = new AppDbContext(options))
{
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    context.Departments.AddRange(departments);
    context.Teams.AddRange(teams);
    context.Projects.AddRange(projects);
    context.Employees.AddRange(employees);

    await context.SaveChangesAsync();
}

using (var context = new AppDbContext(options))
{
    var parser = new JsonExpressionParser();

    var ruleFile = await File.ReadAllTextAsync("databaseRules.json");
    var jsonDocument = JsonDocument.Parse(ruleFile);

    var expression = parser.ParseExpressionOf<Employee>(jsonDocument);
    var predicate = parser.ParsePredicateOf<Employee>(jsonDocument);

    var model = RuleModelBuilder.GetModelRule<Employee>();
    Console.WriteLine("Started getting data from database...");

    var listQuery = employees.Where(predicate);
    var query = context.Employees.Where(expression)
                                 .OrderBy(t => t.Id);

    var results = await query.ToListAsync();
    Console.WriteLine($"Retrieved {results.Count}");
}


