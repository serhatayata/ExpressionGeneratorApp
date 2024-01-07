using ExpressionGeneratorApp;
using ExpressionGeneratorApp.Entities;
using ExpressionGeneratorApp.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

Console.WriteLine("Application started");
var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=transactions.db").Options;

int companyCount = 6, departmentCount = 24, teamCount = 96, employeeCount = 2000, projectCount = 192;

var companies = Company.GetList(companyCount);
var departments = Department.GetList(departmentCount, companyCount);
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
    context.Companies.AddRange(companies);

    await context.SaveChangesAsync();
}

using (var context = new AppDbContext(options))
{
    var parser = new JsonExpressionParser();

    var ruleFile = await File.ReadAllTextAsync("databaseRules.json");
    var jsonDocument = JsonDocument.Parse(ruleFile);

    var model = RuleModelBuilder.GetModelRule(typeof(Employee));

    var query = context.Employees.AsQueryable();
    query = query.ProcessByProperty(jsonDocument);
    var predicate = parser.ParsePredicateOf<Employee>(jsonDocument);
    var results = query.Where(predicate).ToList();
}


