using Microsoft.EntityFrameworkCore;

namespace StudentSystem.Models
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
            
        }

        public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
            :base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homework { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=StudentSystem;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
