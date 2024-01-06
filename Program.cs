using ExpressionGeneratorApp;
using ExpressionGeneratorApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

Console.WriteLine("Application started");
var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=transactions.db").Options;

int departmentCount = 24, teamCount = 96, employeeCount = 2000, projectCount = 192;

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

    //var predicate = parser.ParsePredicateOf<Employee>(jsonDocument);

    var model = RuleModelBuilder.GetModelRule(typeof(Employee));

    //var listQuery = employees.Where(predicate);
    var query = context.Employees.AsQueryable();
    query = query.ProcessByProperty(jsonDocument);
    var expression = parser.ParseExpressionOf<Employee>(jsonDocument);

    var results = query.Where(expression).ToList();
}


