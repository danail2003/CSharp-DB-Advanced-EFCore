namespace MiniORM.App
{
    using Data;
    using System.Linq;
    using Data.Entities;

    public class StartUp
    {
        public static void Main()
        {
            string connectionString = "Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MiniORM;" + "Integrated Security=true;";

            SoftUniDbContext context = new SoftUniDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true,
            });

            Employee employee = context.Employees.Last();
            employee.FirstName = "Modified";

            context.SaveChanges();
        }
    }
}
