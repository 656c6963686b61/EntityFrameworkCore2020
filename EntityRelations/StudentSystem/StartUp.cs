using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using StudentSystem.Models;

namespace StudentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new StudentSystemContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var student = new Student()
            {
                Name = "Ivan",
                RegisteredOn = new DateTime(2020, 10, 9),
                Courses = new List<StudentCourse>()
                {
                    new StudentCourse()
                    {
                        Course = new Course()
                        {
                            Name = "Java",
                            StartDate = new DateTime(2020, 6, 8),
                            EndDate = new DateTime(2020, 8, 10),
                            Price = 200
                        }
                    },
                    new StudentCourse()
                    {
                        Course = new Course()
                        {
                            Name = "CPP",
                            StartDate = new DateTime(2020, 6, 8),
                            EndDate = new DateTime(2020, 8, 10),
                            Price = 100
                        }
                    }
                }
            };
            db.Students.Add(student);
            
            db.SaveChanges();
        }
    }
}
