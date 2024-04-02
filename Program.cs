using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace StudentManagementSystem
{
    // Department enum
    internal enum Department
    {
        CSE, BBA, EEE
    }

    // Degree enum
    internal enum Degree
    {
        BSC, BBA, MSC
    }

    // Course class
    public class Course
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public int TotalCredits { get; set; }
    }

    // Semester class
    public class Semester
    {
        public string SemesterName { get; set; }
        public string Year { get; set; }
        public List<Course> Courses { get; set; }
    }

    // Student class
    public class Student
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
        public string JoiningBatch { get; set; }
        public dynamic Department { get; set; }
        public dynamic Degree { get; set; }
        public List<Semester> SemestersAttended { get; set; }
    }

    // Delegate and generics
    public delegate void StudentAction<Thing>(List<Student> students, Thing parameter);

    // MainCourses class
    public static class MainCourses
    {
        public static List<Course> Courses = new List<Course>
        {
            new Course { CourseID = "CSC 101", CourseName = "Introduction to Computer Science", TeacherName = "Prof. Smith", TotalCredits = 3 },
            new Course { CourseID = "ENG 201", CourseName = "English Literature", TeacherName = "Prof. Johnson", TotalCredits = 4 },
        };
    }

    // Extension class
    public static class Extensions
    {
        public static void SortCoursesAlphabetically(this List<Course> courses)
        {
            courses.Sort((x, y) => string.Compare(x.CourseName, y.CourseName));
        }
    }

    class Program
    {
        private const string studentPath = "students.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Student Management System");

            // Load previous students
            List<Student> students = LoadStudents();

            // Implement dashboard
            while (true)
            {
                Console.WriteLine("\nDash Board:");
                Console.WriteLine("1. Add New Student");
                Console.WriteLine("2. View Student Details");
                Console.WriteLine("3. Delete Student");
                Console.WriteLine("4. Add New Semester");
                Console.WriteLine("5. View All Students");
                Console.WriteLine("6. Exit");

                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                if (option == "1")
                {
                    AddNewStudent(students);
                }
                else if (option == "2")
                {
                    ViewStudentDetails(students);
                }
                else if (option == "3")
                {
                    DeleteStudent(students);
                }
                else if (option == "4")
                {
                    AddNewSemester(students);
                }
                else if (option == "5")
                {
                    try
                    {
                        ViewAllStudents(students);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
                else if (option == "6")
                {
                    SaveStudents(students);
                    return;
                }
                else
                {
                    Console.WriteLine("Choose Valid Option!");
                }
            }
        }

        static void AddNewStudent(List<Student> students)
        {
            Console.WriteLine("\nAdd New Student:");

            // User input
            Student newStudent = new Student();

            Console.Write("First Name: ");
            newStudent.FirstName = Console.ReadLine();
            Console.Write("Middle Name: ");
            newStudent.MiddleName = Console.ReadLine();
            Console.Write("Last Name: ");
            newStudent.LastName = Console.ReadLine();
            Console.Write("Student ID (XXX): ");
            newStudent.StudentID = Console.ReadLine();
            Console.Write("Joining Batch (XX): ");
            newStudent.JoiningBatch = Console.ReadLine();

            Console.Write("Department (CSE, BBA, EEE): ");
            newStudent.Department = Enum.Parse(typeof(Department), Console.ReadLine());

            Console.Write("Degree (BSC, BBA, MSC): ");
            newStudent.Degree = Enum.Parse(typeof(Degree), Console.ReadLine());

            // Semesters attended list
            newStudent.SemestersAttended = new List<Semester>();

            students.Add(newStudent);

            Console.WriteLine("Student added successfully!");
        }

        static void ViewStudentDetails(List<Student> students)
        {
            Console.WriteLine("\nView Student Details:");
            Console.Write("Enter Student ID: ");
            string studentID = Console.ReadLine();

            Student student = students.Find(s => s.StudentID == studentID);
            if (student != null)
            {
                Console.WriteLine(JsonConvert.SerializeObject(student, Formatting.Indented));

                // Sorting courses for each semester using extension method
                foreach (var semester in student.SemestersAttended)
                {
                    semester.Courses.SortCoursesAlphabetically();
                }

                // Display sorted courses
                Console.WriteLine("\nSorted Courses:");
                foreach (var semester in student.SemestersAttended)
                {
                    Console.WriteLine($"Semester: {semester.SemesterName}, Year: {semester.Year}");
                    foreach (var course in semester.Courses)
                    {
                        Console.WriteLine($"{course.CourseID} - {course.CourseName}");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        static void DeleteStudent(List<Student> students)
        {
            Console.WriteLine("\nDelete Student:");
            Console.Write("Enter Student ID: ");
            string studentID = Console.ReadLine();

            Student student = students.Find(s => s.StudentID == studentID);
            if (student != null)
            {
                students.Remove(student);
                Console.WriteLine("Student deleted successfully!");
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        static void AddNewSemester(List<Student> students)
        {
            Console.WriteLine("\nAdd New Semester:");

           
            Console.Write("Enter Student ID: ");
            string studentID = Console.ReadLine();

           
            Student student = students.Find(s => s.StudentID == studentID);
            if (student != null)
            {
                // Display available courses that student not taken 
                Console.WriteLine("Available Courses:");
                foreach (var course in MainCourses.Courses)
                {
                    bool hasTaken = student.SemestersAttended.Any(semester => semester.Courses.Any(c => c.CourseID == course.CourseID));
                    if (!hasTaken)
                    {
                        Console.WriteLine($"{course.CourseID} - {course.CourseName}");
                    }
                }

                
                Console.Write("Enter course IDs to add (comma-separated): ");
                string[] courseIDsToAdd = Console.ReadLine().Split(',');

                // Add courses to new semester
                Semester newSemester = new Semester();
                newSemester.Courses = new List<Course>();
                foreach (var courseID in courseIDsToAdd)
                {
                    Course courseToAdd = MainCourses.Courses.Find(c => c.CourseID == courseID.Trim());
                    if (courseToAdd != null)
                    {
                        newSemester.Courses.Add(courseToAdd);
                    }
                    else
                    {
                        Console.WriteLine($"Course with ID {courseID} not found.");
                    }
                }

               
                student.SemestersAttended.Add(newSemester);

                Console.WriteLine("Semester added successfully!");
            }
            else
            {
                Console.WriteLine("Student not found.");
            }
        }

        static void ViewAllStudents(List<Student> students)
        {
            // LINQ to find all students
            var cseStudents = from student in students
                              where student.FirstName.StartsWith("a", StringComparison.OrdinalIgnoreCase)
                              select student;

            Console.WriteLine("\nStudents in CSE Department:");
            foreach (var student in cseStudents)
            {
                Console.WriteLine($"Name: {student.FirstName} {student.LastName}, Student ID: {student.StudentID}");
            }
        }

        static List<Student> LoadStudents()
        {
            if (File.Exists(studentPath))
            {
                string json = File.ReadAllText(studentPath);
                return JsonConvert.DeserializeObject<List<Student>>(json);
            }
            else
            {
                return new List<Student>();
            }
        }

        static void SaveStudents(List<Student> students)
        {
            string json = JsonConvert.SerializeObject(students, Formatting.Indented);
            File.WriteAllText(studentPath, json);
        }
    }
}
