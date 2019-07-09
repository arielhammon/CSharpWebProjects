using System;
using System.Collections.Generic;
using System.Web.Mvc;
using StudentManagementSystem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _connectionString = @"Data Source=NMC305L01\SQLEXPRESS;Initial Catalog=School;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // GET: Student
        public ActionResult Index()
        {
            var students = new List<StudentManagementSystem.Models.Student>();
            using (var db = new SchoolEntities())
            {
                //method 1: create new list from full list returned from database with only the desired records
                students = db.Students.Where(x => x.Removed == null).ToList();

                //method 2: query the database for only the desired records
                students = (from s in db.Students
                            where s.Removed == null
                            select s).ToList();
            }
            var studentVMs = new List<StudentVM>();
            foreach (Student student in students)
            {
                var studentVM = new StudentVM();
                studentVM.ID = student.ID;
                studentVM.FirstName = student.FirstName;
                studentVM.LastName = student.LastName;
                studentVMs.Add(studentVM);
            }
            return View(studentVMs);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(StudentManagementSystem.Models.StudentVM student)
        {
            string queryString = @"Insert into Students (FirstName, LastName, Removed) Values (@FirstName, @LastName, Null)";
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                command.Parameters.Add("@LastName", SqlDbType.VarChar);
                command.Parameters["@FirstName"].Value = student.FirstName;
                command.Parameters["@LastName"].Value = student.LastName;

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Details(int ID)
        {
            string queryString = "Select * From Students where ID = @ID";
            var student = new StudentManagementSystem.Models.StudentVM();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = ID;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    student.ID = Convert.ToInt32(reader["ID"]);
                    student.FirstName = reader["FirstName"].ToString().Trim();
                    student.LastName = reader["LastName"].ToString().Trim();
                }
                connection.Close();
            }
            return View(student);
        }
        public ActionResult Edit(int ID)
        {
            string queryString = "Select * From Students Where ID = @ID";
            var student = new StudentManagementSystem.Models.StudentVM();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = ID;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    student.ID = Convert.ToInt32(reader["ID"]);
                    student.FirstName = reader["FirstName"].ToString().Trim();
                    student.LastName = reader["LastName"].ToString().Trim();
                }
                connection.Close();
            }
            return View(student);
        }
        [HttpPost]
        public ActionResult Edit(StudentManagementSystem.Models.StudentVM student)
        {
            string queryString = @"Update Students set FirstName=@FirstName, LastName=@LastName Where ID=@ID";
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters.Add("@FirstName", SqlDbType.VarChar);
                command.Parameters.Add("@LastName", SqlDbType.VarChar);
                command.Parameters["@ID"].Value = student.ID;
                command.Parameters["@FirstName"].Value = student.FirstName;
                command.Parameters["@LastName"].Value = student.LastName;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Remove(int ID)
        {
            using (var db = new SchoolEntities())
            {
                StudentManagementSystem.Models.Student student = db.Students.Find(ID);
                student.Removed = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}