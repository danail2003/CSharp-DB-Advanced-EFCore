namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners.Where(x => ids.Contains(x.Id)).ToArray()
                .Select(x => new
                {
                    x.Id,
                    Name = x.FullName,
                    x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(y => new
                    {
                        OfficerName = y.Officer.FullName,
                        Department = y.Officer.Department.Name
                    })
                    .ToArray()
                    .OrderBy(y => y.OfficerName),
                    TotalOfficerSalary = x.PrisonerOfficers.Sum(y => y.Officer.Salary)
                })
                .ToArray()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var json = JsonConvert.SerializeObject(prisoners, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] names = prisonersNames.Split(",");

            StringBuilder stringBuilder = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(typeof(ExportPrisonerDto[]), new XmlRootAttribute("Prisoners"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var prisoners = context.Prisoners.Where(x => names.Contains(x.FullName)).ToArray()
                .Select(x => new ExportPrisonerDto()
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Messages = x.Mails.Select(y => new MailDto()
                    {
                        Description = y.Description.Reverse().ToString()
                    })
                    .ToArray()
                })
                .ToArray()
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Id)
                .ToArray();

            serializer.Serialize(new StringWriter(stringBuilder), prisoners, namespaces);

            return stringBuilder.ToString().TrimEnd();
        }
    }
}