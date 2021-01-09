namespace P01_StudentSystem.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;

    public class StudentCourse
    {
        [Required, ForeignKey("Student")]
        public int StudentId { get; set; }

        public Student Student { get; set; }

        [Required, ForeignKey("Course")]
        public int CourseId { get; set; }

        public Course Course { get; set; }
    }
}
