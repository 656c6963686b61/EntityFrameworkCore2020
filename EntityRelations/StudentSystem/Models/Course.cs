using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Models
{
    public class Course
    {
        public Course()
        {
            this.Students = new List<StudentCourse>();
        }
        
        [Key]
        public int CourseId { get; set; }

        [Column(TypeName = "nvarchar(80)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }
        
        public ICollection<StudentCourse> Students { get; set; }
    }
}
