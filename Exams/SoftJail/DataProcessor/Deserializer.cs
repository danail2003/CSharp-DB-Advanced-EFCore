namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var departmentsDto = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);

            List<Department> departments = new List<Department>();

            foreach (var dto in departmentsDto)
            {
                if (!IsValid(dto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Department department = new Department
                {
                    Name = dto.Name
                };

                bool isInvalidCell = true;

                foreach (var cellDto in dto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        isInvalidCell = false;
                        break;
                    }

                    Cell cell = new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    };

                    department.Cells.Add(cell);
                }

                if (!isInvalidCell)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (department.Cells.Count == 0)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                departments.Add(department);
                stringBuilder.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var prisonersDto = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);

            List<Prisoner> prisoners = new List<Prisoner>();

            foreach (var dto in prisonersDto)
            {
                if (!IsValid(dto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                bool isIncarcerationDateValid = DateTime.TryParseExact(dto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validIncarDate);

                if (!isIncarcerationDateValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? nullReleaseDate;

                if (!string.IsNullOrEmpty(dto.ReleaseDate))
                {
                    bool isReleaseDateValid = DateTime.TryParseExact(dto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate);

                    if (!isReleaseDateValid)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    nullReleaseDate = validDate;
                }
                else
                {
                    nullReleaseDate = null;
                }

                Prisoner prisoner = new Prisoner
                {
                    FullName = dto.FullName,
                    Nickname = dto.Nickname,
                    Age = dto.Age,
                    IncarcerationDate = validIncarDate,
                    ReleaseDate = nullReleaseDate,
                    Bail = dto.Bail,
                    CellId = dto.CellId
                };

                bool isMailValid = true;

                foreach (var mailDto in dto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        isMailValid = false;
                        break;
                    }

                    Mail mail = new Mail
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    };

                    prisoner.Mails.Add(mail);
                }

                if (!isMailValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                prisoners.Add(prisoner);
                stringBuilder.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.AddRange(prisoners);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));

            var officersDto = (ImportOfficerDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Officer> officers = new List<Officer>();

            foreach (var dto in officersDto)
            {
                if (!IsValid(dto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                bool isPositionValid = Enum.TryParse<Position>(dto.Position, out Position validPosition);

                if (!isPositionValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                bool isWeaponValid = Enum.TryParse<Weapon>(dto.Weapon, out Weapon validWeapon);

                if (!isWeaponValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Department department = context.Departments.FirstOrDefault(x => x.Id == dto.DepartmentId);

                Officer officer = new Officer
                {
                    FullName = dto.FullName,
                    Salary = dto.Salary,
                    Position = validPosition,
                    Weapon = validWeapon,
                    Department = department
                };

                foreach (var prisonerDto in dto.Prisoners)
                {
                    Prisoner prisoner = context.Prisoners.FirstOrDefault(x => x.Id == prisonerDto.Id);

                    officer.OfficerPrisoners.Add(new OfficerPrisoner { Officer = officer, Prisoner = prisoner });
                }

                officers.Add(officer);
                stringBuilder.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}