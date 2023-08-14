using System.Data.SqlClient;
using CourseWebApi.Results;
using Microsoft.AspNetCore.Mvc;

namespace CourseWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CourseController : ControllerBase
{
    [Route("GetAllCourses")]
    [HttpGet]
    public JsonResult GetAllCourses()
    {
        string connectionString = "data source=.;database=SinaDB;integrated security=SSPI";
        SqlConnection con = new SqlConnection(connectionString);
        using (con)
        {
            con.Open();
            String query = "SELECT * From Courses";
            using (SqlCommand command = new SqlCommand(query, con))
            {
                List<ResultCourses> courses = new List<ResultCourses>();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int.TryParse(reader["Instructor_Id"].ToString(), out int d);



                        courses.Add(new()
                        {
                            Id = (int)reader["Id"],
                            courseName = reader["Name"].ToString(),
                            instructorId = d == 0 ? null : d
                        });
                    }
                }
                return new JsonResult(courses);
            }
        }
    }

    [Route("GetSpecific")]
    [HttpGet]
    public JsonResult GetSpecific([FromQuery]GetSpecificViewModel _getSpecificViewModel)
    {
        string connectionString = "data source=.;database=SinaDB;integrated security=SSPI";
        SqlConnection con = new SqlConnection(connectionString);
        using (con)
        {
            con.Open();
            string query = "select c.Id as 'Course Id',c.Name as 'Course name',I.InstructorName as 'instructor name'  ,s.Id as 'Student Id', s.Name as 'Student name'  from courses as c" +
                  " left join instructors as I on c.Instructor_id=I.Id" +
                  " left join enrollments as e on c.Id = e.CourseId" +
                  " left join students as s on s.Id = e.StudentId" +
                  " where c.Id = @id";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@id", _getSpecificViewModel.CourseId);
                List<ResultGSC> courses = new List<ResultGSC>();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        int.TryParse(reader["Student Id"].ToString(), out int d);

                        string test = (string)reader["Student name"].ToString();
                        courses.Add(new()
                        {
                            CourseId = (int)reader["Course Id"],
                            CourseName = (string)reader["Course name"],
                            InstructorName = (string)reader["instructor name"],
                            StudentId =  d == 0 ? null : d,
                            StudentName = test == "" ? null : test,
                        });
                    }
                }
                return new JsonResult(courses);
            }
        }
    }

    [Route("CreateCourse")]
    [HttpPost]
    public JsonResult InsertCourse(CourseViewModel _courseViewModel)
    {
        string connectionString = "data source=.;database=SinaDB;integrated security=SSPI";
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "insert into courses (Name) values (@name)";
            SqlCommand cmd = new SqlCommand(query, con);
            using (cmd)
            {
                cmd.Parameters.AddWithValue("@name", _courseViewModel.courseName);
                int rowsaffected = cmd.ExecuteNonQuery();
            }
            return new JsonResult(_courseViewModel);
        }
    }

    [Route("EnrollStudent")]
    [HttpPost]
    public JsonResult EnrollStudent(EnrollStudentViewModel _enrollStudentViewModel)
    {
        string connectionString = "data source=.;database=SinaDB;integrated security=SSPI";
        SqlConnection con = new SqlConnection(connectionString);
        using (con)
        {
            con.Open();
            string query = "INSERT INTO Enrollments ( StudentId, CourseId) VALUES ( @StudentId, @CourseId)";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@StudentId", _enrollStudentViewModel.studentId);
                command.Parameters.AddWithValue("@CourseId", _enrollStudentViewModel.courseId);

                int rowsAffected = command.ExecuteNonQuery();

                return new JsonResult(_enrollStudentViewModel);
            }
        }

    }

    [Route("UpdateCourse")]
    [HttpPut]
    public JsonResult UpdateCourse(UpdateCourseViewModel _updateCourseViewModel)
    {
        string connectionString = "data source=.;database=SinaDB;integrated security=SSPI";
        SqlConnection con = new SqlConnection(connectionString);
        using (con)
        {
            con.Open();
            string query = "UPDATE courses SET Name = @courseName, Instructor_Id = @InstructorId WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@courseName", _updateCourseViewModel.courseName);
                command.Parameters.AddWithValue("@InstructorId", _updateCourseViewModel.InstructorId);
                command.Parameters.AddWithValue("@Id", _updateCourseViewModel.Id);

                int rowsAffected = command.ExecuteNonQuery();

                return new JsonResult(_updateCourseViewModel);
            }
        }
    }

    [Route("DeleteCourse")]
    [HttpDelete]
    public JsonResult DeleteCourse([FromQuery]DeleteCourseViewModel _deleteCourseViewModel)
    {
        string connectionString = "data source=.;database=SinaDB;integrated security=SSPI";
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "delete from enrollments "
                + "where CourseId = @Id1;"
                + "delete from courses "
                + " where Id = @Id2;";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Id1", _deleteCourseViewModel.Id);
                cmd.Parameters.AddWithValue("@Id2", _deleteCourseViewModel.Id);
                int rowsaffected = cmd.ExecuteNonQuery();
            }
            return new JsonResult(_deleteCourseViewModel);
        }
    }









}
