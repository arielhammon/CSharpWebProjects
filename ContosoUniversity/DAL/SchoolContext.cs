using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContosoUniversity.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoUniversity.DAL
{
    public class SchoolContext : DbContext
    {
        public SchoolContext() : base("SchoolContext")
        {
            //the following line would disable lazy loading for the entire context
            //this.Configuration.LazyLoadingEnabled = false;

            //Alternatively, removing the virtual key word from the navigation property in the class defintion
            //would disable lazy loading on a per property basis.

            //Using the Include statement forces eager loading on a per query basis, like this:
            //departments = context.Departments.Include(x => x.Courses);

            //Using the Collection(...).Load() command defers loading navigation properties for a deparment d, like this:
            //context.Entry(d).Collection(x => x.Courses).Load();
            //Or for single entity like this:
            //context.Entry(dept).Reference(x => x.Administrator).Load();
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors).WithMany(i => i.Courses)
                .Map(t => t.MapLeftKey("CourseID")
                .MapRightKey("InstructorID")
                .ToTable("CourseInstructor"));
            modelBuilder.Entity<Department>().MapToStoredProcedures();
        }
    }
}