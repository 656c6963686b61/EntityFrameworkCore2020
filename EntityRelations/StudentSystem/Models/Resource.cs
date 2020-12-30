using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(MAX)")]
        public string Url { get; set; }
        
        public ResourceType ResourceTypes { get; set; }
        
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
