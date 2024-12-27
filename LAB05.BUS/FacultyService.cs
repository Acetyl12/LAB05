using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAB05.DAL.EntitiesDB;

namespace LAB05.BUS
{
    public class FacultyService
    {
        public List<Faculty> GetFaculties()
        {
            Model1 context = new Model1();
            return context.Faculties.ToList();
        }
    }
}
