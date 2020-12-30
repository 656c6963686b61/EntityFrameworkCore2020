using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Models
{
    public class Student
    {
        public Student()
        {
            this.Courses = new List<StudentCourse>();
        }
        
        [Key]
        public int StudentId { get; set; }

        public DateTime? Birthday { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "varchar(10)")]
        public int? PhoneNumber { get; set; }

        [Required]
        public DateTime RegisteredOn { get; set; }

        public ICollection<StudentCourse> Courses { get; set; }
    }
}
