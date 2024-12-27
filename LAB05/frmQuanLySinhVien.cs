using LAB05.BUS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LAB05.DAL.EntitiesDB;
using LAB05.DAL;
using LAB05.BUS;
using System.IO;

namespace LAB05
{
    public partial class frmQuanLySinhVien : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private string avatarfilePath = "";
        public frmQuanLySinhVien()
        {
            InitializeComponent();
        }



        private void frmQuanLySinhVien_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudentManagement);
                var listFaculties = facultyService.GetFaculties();
                var listStudents = studentService.GetStudents();
                FillFalcultyCombobox(listFaculties);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            cmbFaculty.DataSource = listFacultys;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudentManagement.Rows.Clear();
            foreach (var item in listStudent)
            {
                //int index = dgvStudentManagement.Rows.Add();
/*                dgvStudentManagement.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudentManagement.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudentManagement.Rows[index].Cells[2].Value =
                    item.Faculty.FacultyName;
                dgvStudentManagement.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dgvStudentManagement.Rows[index].Cells[4].Value = item.Major.Name + "";*/
                string majorName = item.MajorID == null ? "" : item.Major.Name;
                dgvStudentManagement.Rows.Add(item.StudentID, item.FullName, item.Faculty.FacultyName, item.AverageScore, majorName);
                LoadAvatar(item.StudentID);
            }
        }


        private void LoadAvatar(string studentID)
        {
            const string ImageFolderName = "Images";
            string folderPath = Path.Combine(Application.StartupPath, ImageFolderName);
            var student = studentService.FindByID(studentID);  
            if(student != null || string.IsNullOrEmpty(student.Avatar))
            {
                picAvatar.Image = null;
                return;
            }
            string avatarPath = Path.Combine(folderPath, student.Avatar);
            if (File.Exists(avatarPath))
            {
                try
                {
                    using (var avatarImage = Image.FromFile(avatarPath))
                    {
                        picAvatar.Image = new Bitmap(avatarImage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    picAvatar.Image = null;
                }
            }
            else
            {
                picAvatar.Image = null;
            }
        }

        private string SaveAvatar(string sourcePath, string studentID)
        {
            const string ImageFolderName = "Images";
            try
            {
                if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(studentID))
                {
                    throw new Exception("Source path or student ID is invalid");
                }

                if(!File.Exists(sourcePath))
                {
                    throw new Exception("Source path is not exist");
                }
                string folderPath = Path.Combine(Application.StartupPath, ImageFolderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string fileExtension = Path.GetExtension(sourcePath);
                string targetFileName = $"{studentID}{fileExtension}";
                string targetPath = Path.Combine(folderPath, targetFileName);
                File.Copy(sourcePath, targetPath, true);
                return targetFileName;
            }
            catch
            {
                MessageBox.Show("Lỗi khi lưu ảnh", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void btnPicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg; *jeg; *.png) | *.jpg; *jeg; *.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picAvatar.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var student = studentService.FindByID(txtID.Text) ?? new Student();
                student.StudentID = txtID.Text.Trim();
                student.FullName = txtName.Text.Trim();
                student.AverageScore = double.Parse(txtScore.Text);
                student.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());
                if (!string.IsNullOrEmpty(avatarfilePath))
                {
                    string avatarFileName = SaveAvatar(avatarfilePath, txtID.Text);
                    if (!string.IsNullOrEmpty(avatarFileName))
                    {
                        student.Avatar = avatarFileName;
                    }
                }
                studentService.InsertStudent(student);
                BindGrid(studentService.GetStudents());
                avatarfilePath = string.Empty;
            }
            catch
            {
                MessageBox.Show("Lỗi thêm dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Student student = studentService.FindByID(txtID.Text);
                if (student != null)
                {
                    studentService.DeleteStudent(student);
                    BindGrid(studentService.GetStudents());
                    picAvatar.Image = null;
                    txtID.Text = "";
                    txtName.Text = "";
                    txtScore.Text = "";
                    cmbFaculty.SelectedIndex = 0;
                    MessageBox.Show("Xóa dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStudentManagement_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudentManagement.Rows[e.RowIndex];
                txtID.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                txtScore.Text = row.Cells[3].Value.ToString();
                cmbFaculty.Text = row.Cells[2].Value.ToString();
            }
        }

        private void ckbChuyenNganh_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.ckbChuyenNganh.Checked)
                listStudents = studentService.GetStudentsHaveNoMajor();
            else
                listStudents = studentService.GetStudents();
            BindGrid(listStudents);
        }

        private void đăngKýChuyênNgànhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DKChuyenNganh dKChuyenNganh = new DKChuyenNganh();
            dKChuyenNganh.ShowDialog();
        }
    }
}
