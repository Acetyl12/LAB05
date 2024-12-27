using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAB05.DAL.EntitiesDB;

namespace LAB05.BUS
{
    public class StudentService
    {
        public List<Student> GetStudents()
        {
            Model1 context = new Model1();
            return context.Students.ToList();
        }

        public List<Student> GetStudentsHaveNoMajor()
        {
            Model1 context = new Model1();
            return context.Students.Where(s => s.MajorID == null).ToList();
        }

        public List<Student> GetStudentsHaveNoMajor(int facultyID)
        {
            Model1 context = new Model1();
            return context.Students.Where(s => s.MajorID == null && s.FacultyID == facultyID).ToList();
        }

        public Student FindByID(string studentID)
        {
            Model1 context = new Model1();
            return context.Students.FirstOrDefault(s => s.StudentID == studentID);
        }

        public void InsertStudent(Student student)
        {
            Model1 context = new Model1();
            context.Students.Add(student);
            context.SaveChanges();
        }

        public void DeleteStudent(Student student)
        {
            Model1 context = new Model1();
            context.Students.Remove(student);
            context.SaveChanges();
        }

        public void UpdateStudent(Student student)
        {
            Model1 context = new Model1();
            var studentUpdate = context.Students.FirstOrDefault(s => s.StudentID == student.StudentID);
            studentUpdate.FullName = student.FullName;
            studentUpdate.AverageScore = student.AverageScore;
            studentUpdate.FacultyID = student.FacultyID;
            studentUpdate.MajorID = student.MajorID;
            studentUpdate.Avatar = student.Avatar;
            context.SaveChanges();
        }
    }
}
