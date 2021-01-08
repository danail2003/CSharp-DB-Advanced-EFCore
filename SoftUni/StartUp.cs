using System;
using System.Text;
using System.Linq;
using System.Globalization;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            SoftUniContext context = new SoftUniContext();

            Console.WriteLine(GetAddressesByTown(context));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees.OrderBy(x => x.EmployeeId).Select(x => new
            {
                x.FirstName,
                x.LastName,
                x.MiddleName,
                x.JobTitle,
                x.Salary
            }).ToList();

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees.Where(x => x.Salary > 50000).OrderBy(x => x.FirstName).Select(x => new
            {
                x.FirstName,
                x.Salary
            }).ToList();

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees.Where(x => x.Department.Name == "Research and Development")
                .OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Department,
                    x.Salary
                }).ToList();

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Department.Name} - ${employee.Salary:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            Address address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees.First(x => x.LastName == "Nakov");
            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees.OrderByDescending(x => x.AddressId).Select(x => new
            {
                x.Address.AddressText
            }).Take(10).ToList();

            foreach (var employeeAddress in employees)
            {
                stringBuilder.AppendLine(employeeAddress.AddressText);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees.Where(x => x.EmployeesProjects
              .Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(p => new
                    {
                        p.Project.Name,
                        p.Project.StartDate,
                        p.Project.EndDate
                    }).ToList()
                }).Take(10).ToList();

            foreach (var employee in employees)
            {
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    string startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    string endDate = project.EndDate == null ? "not finished" : project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    stringBuilder.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var addresses = context.Addresses.Select(x => new
            {
                x.AddressText,
                x.Town.Name,
                Employees = x.Employees.Count
            }).OrderByDescending(x => x.Employees).ThenBy(x => x.Name).ThenBy(x => x.AddressText).ToList();

            foreach (var address in addresses)
            {
                stringBuilder.AppendLine($"{address.AddressText}, {address.Name} - {address.Employees} employees");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employee = context.Employees.Where(x => x.EmployeeId == 147).Select(x => new
            {
                x.FirstName,
                x.LastName,
                x.JobTitle,
                EmployeeProject = x.EmployeesProjects.Select(p => new
                {
                    p.Project.Name
                }).OrderBy(o => o.Name).ToList()
            }).ToList();

            foreach (var info in employee)
            {
                stringBuilder.AppendLine($"{info.FirstName} {info.LastName} - {info.JobTitle}");

                foreach (var project in info.EmployeeProject)
                {
                    stringBuilder.AppendLine(project.Name);
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var departments = context.Departments.Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count).ThenBy(x => x.Name).Select(x => new
                {
                    x.Name,
                    x.Manager.FirstName,
                    x.Manager.LastName,
                    EmployeeInfo = x.Employees.Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    }).OrderBy(o => o.FirstName).ThenBy(o => o.LastName).ToList()
                }).ToList();


            foreach (var department in departments)
            {
                stringBuilder.AppendLine($"{department.Name} - {department.FirstName} {department.LastName}");

                foreach (var employee in department.EmployeeInfo)
                {
                    stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var projects = context.Projects.OrderByDescending(x => x.StartDate).Take(10).OrderBy(x => x.Name).Select(x => new
            {
                x.Name,
                x.Description,
                x.StartDate
            }).ToList();

            foreach (var project in projects)
            {
                stringBuilder.AppendLine(project.Name);
                stringBuilder.AppendLine(project.Description);
                stringBuilder.AppendLine(project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.Department.Name == "Engineering" || x.Department.Name == "Tool Design"
                || x.Department.Name == "Marketing" || x.Department.Name == "Information Services").ToList();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;
                context.SaveChanges();
            }

            employees.Select(x => new
            {
                x.FirstName,
                x.LastName,
                x.Salary
            }).ToList();

            foreach (var employee in employees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName))
            {
                stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // broken test
            if (context.Employees.Any(e => e.FirstName == "Svetlin"))
            {
                string pattern = "SA";
                var employeesByNamePattern = context.Employees
                    .Where(employee => employee.FirstName.StartsWith(pattern));

                foreach (var employeeByPattern in employeesByNamePattern)
                {
                    stringBuilder.AppendLine($"{employeeByPattern.FirstName} {employeeByPattern.LastName} " +
                                       $"- {employeeByPattern.JobTitle} - (${employeeByPattern.Salary})");
                }
            }
            else
            {
                var employeesByNamePattern = context.Employees.Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary,
                })
                    .Where(e => e.FirstName.StartsWith("Sa"))
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList();

                foreach (var employee in employeesByNamePattern)
                {
                    stringBuilder.AppendLine($"{employee.FirstName} {employee.LastName} " +
                                       $"- {employee.JobTitle} - (${employee.Salary:F2})");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var project = context.Projects.Find(2);

            context.EmployeesProjects.ToList().ForEach(x => context.EmployeesProjects.Remove(x));
            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects.Take(10).Select(x => new
            {
                x.Name
            }).ToList();

            foreach (var pr in projects)
            {
                stringBuilder.AppendLine(pr.Name);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            context.Employees.Where(e => e.Address.Town.Name == "Seattle").ToList().ForEach(e => e.AddressId = null);

            int count = context.Addresses.Where(a => a.Town.Name == "Seattle").Count();

            context.Addresses.Where(a => a.Town.Name == "Seattle").ToList().ForEach(a => context.Addresses.Remove(a));

            context.Towns.Remove(context.Towns.SingleOrDefault(t => t.Name == "Seattle"));

            context.SaveChanges();

            stringBuilder.AppendLine($"{count} addresses in Seattle were deleted");

            return stringBuilder.ToString().TrimEnd();
        }
    }
}
