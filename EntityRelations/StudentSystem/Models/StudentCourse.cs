using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Models
{
    public class StudentCourse
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [Required]
        public Student Student { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }

        [Required]
        public Course Course { get; set; }
    }
}
