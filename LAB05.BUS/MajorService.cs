using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAB05.DAL.EntitiesDB;

namespace LAB05.BUS
{
    public class MajorService
    {
        public List<Major> GetMajors()
        {
            Model1 context = new Model1();
            return context.Majors.ToList();
        }

        public List<Major> GetMajors(int facultyID)
        {
            Model1 context = new Model1();
            return context.Majors.Where(m => m.FacultyID == facultyID).ToList();
        }
    }
}
