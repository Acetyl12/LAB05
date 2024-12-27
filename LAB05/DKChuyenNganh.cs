using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LAB05.BUS;
using LAB05.DAL.EntitiesDB;
using LAB05.DAL;

namespace LAB05
{
    public partial class DKChuyenNganh : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public DKChuyenNganh()
        {
            InitializeComponent();
        }

        private void DKChuyenNganh_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacultys = facultyService.GetFaculties();
                FillFalcultyCombobox(listFacultys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void FillMajorCombobox(List<Major> listMajor)
        {
            this.cmbMajor.DataSource = listMajor;
            this.cmbMajor.DisplayMember = "Name";
            this.cmbMajor.ValueMember = "MajorID";
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = (Faculty)cmbFaculty.SelectedItem;
            if(selectedFaculty != null)
            {
                var listMajors = majorService.GetMajors(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajors);
                var listStudents = studentService.GetStudentsHaveNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }

        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[1].Value = item.StudentID;
                dataGridView1.Rows[index].Cells[2].Value = item.FullName;
                if (item.Faculty != null)
                    dataGridView1.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[4].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dataGridView1.Rows[index].Cells[5].Value = item.Major.Name + "";
            }
        }

        //đăng ký được chuyên ngành có các sinh viên được chọn(checked) ở DataGridView.
        private void btnDK_Click(object sender, EventArgs e)
        {
            Faculty selectedFaculty = (Faculty)cmbFaculty.SelectedItem;
            Major selectedMajor = (Major)cmbMajor.SelectedItem;
            if (selectedFaculty == null || selectedMajor == null)
            {
                MessageBox.Show("Chưa chọn khoa hoặc chuyên ngành");
                return;
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                {
                    string studentID = row.Cells[1].Value.ToString();
                    Student student = studentService.FindByID(studentID);
                    student.MajorID = selectedMajor.MajorID;
                    studentService.UpdateStudent(student);
                }
            }
            MessageBox.Show("Đăng ký thành công");
            var listStudents = studentService.GetStudentsHaveNoMajor(selectedFaculty.FacultyID);
            BindGrid(listStudents);
        }
    }
}
