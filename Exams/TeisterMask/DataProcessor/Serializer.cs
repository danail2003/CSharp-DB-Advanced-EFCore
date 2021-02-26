namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects
                .Where(x => x.Tasks.Count > 0)
                .ToArray()
                .Select(x => new ExportProjectDto()
                {
                    TasksCount = x.Tasks.Count,
                    ProjectName = x.Name,
                    HasEndDate = x.DueDate == null ? "No" : "Yes",
                    Tasks = x.Tasks
                    .Select(y => new ExportTaskDto()
                    {
                        Name = y.Name,
                        Label = y.LabelType.ToString()
                    })
                    .ToArray()
                    .OrderBy(y => y.Name)
                    .ToArray()
                })
                .OrderByDescending(x => x.TasksCount)
                .ThenBy(x => x.ProjectName)
                .ToArray();

            StringBuilder stringBuilder = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportProjectDto[]), new XmlRootAttribute("Projects"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(stringBuilder), projects, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees.Where(x => x.EmployeesTasks.Any(y => y.Task.OpenDate >= date))
                .ToArray()
                .Select(x => new
                {
                    x.Username,
                    Tasks = x.EmployeesTasks
                    .Where(y => y.Task.OpenDate >= date)
                    .OrderByDescending(y => y.Task.DueDate)
                    .ThenBy(y => y.Task.Name)
                    .Select(y => new
                    {
                        TaskName = y.Task.Name,
                        OpenDate = y.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = y.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = y.Task.LabelType.ToString(),
                        ExecutionType = y.Task.ExecutionType.ToString()
                    })
                    .ToArray()
                })
                .OrderByDescending(x => x.Tasks.Length)
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();

            var json = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return json;
        }
    }
}