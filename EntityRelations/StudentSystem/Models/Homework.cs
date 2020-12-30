using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Models
{
    public class Homework
    {
        [Key]
        public int HomeworkId { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(MAX)")]
        public string Content { get; set; }

        [Required]
        public ContentType ContentType { get; set; }

        [Required]
        public DateTime SubmissionTime { get; set; }
        
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
