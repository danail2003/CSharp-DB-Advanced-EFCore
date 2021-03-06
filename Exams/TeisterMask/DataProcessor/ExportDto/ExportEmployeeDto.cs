﻿namespace TeisterMask.DataProcessor.ExportDto
{
    public class ExportEmployeeDto
    {
        public string Username { get; set; }

        public ExportTaskDto[] Tasks { get; set; }
    }

    public class ExportTaskDto
    {
        public string TaskName { get; set; }

        public string OpenDate { get; set; }

        public string DueDate { get; set; }

        public string LabelType { get; set; }

        public string ExecutionType { get; set; }
    }
}
