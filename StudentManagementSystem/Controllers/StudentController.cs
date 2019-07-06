using System;
using System.Collections.Generic;
using System.Web.Mvc;
using StudentManagementSystem.Models;
using System.Data;
using System.Data.SqlClient;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private string _connectionString = @"Data Source=NMC305L01\SQLEXPRESS;Initial Catalog=School;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // GET: Student
        public ActionResult Index()
        {
            string queryString = "Select * From Students";
            var students = new List<Student>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var student = new Student();
                    student.ID = Convert.ToInt32(reader["ID"]);
                    student.FirstName = reader["FirstName"].ToString().Trim();
                    student.LastName = reader["LastName"].ToString().Trim();
                    students.Add(student);
                }
                connection.Close();
            }
            return View(students);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Student student)
        {
            string queryString = @"Insert into Students (FirstName, LastName) Values (@FirstName, @LastName)";
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
            var student = new Student();
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
            var student = new Student();
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
        public ActionResult Edit(Student student)
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
    }
}