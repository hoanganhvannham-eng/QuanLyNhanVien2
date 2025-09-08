using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyNhanVien2
{
    public partial class NhanVien: Form
    {
        string constr = "Data Source=DESKTOP-10V42VO\\SQLEXPRESS;Initial Catalog=QuanLyNhanVien2;Integrated Security=True;";
        public NhanVien()
        {
            InitializeComponent();
        }
        private void ClearAllInputs(Control parent)
        {
            foreach (Control ctl in parent.Controls)
            {
                if (ctl is TextBox)
                    ((TextBox)ctl).Clear();
                else if (ctl is ComboBox)
                    ((ComboBox)ctl).SelectedIndex = -1;
                else if (ctl is DateTimePicker)
                    ((DateTimePicker)ctl).Value = DateTime.Now;
                else if (ctl.HasChildren)
                    ClearAllInputs(ctl);
            }
        }
        private void LoadDataNhanVien()
        {
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT  MaNV,HoTen,CONVERT(VARCHAR(10), NgaySinh, 103) AS NgaySinh, -- Định dạng dd/MM/yyyy
                                    GioiTinh, DiaChi  SoDienThoai,  Email, MaPB, MaCV,  GhiChu
                                FROM tblNhanVien
                                WHERE DeletedAt IS NULL ORDER BY MaNV;
";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dtGridViewNhanVien.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void NhanVien_Load(object sender, EventArgs e)
        {
            LoadDataNhanVien();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu nhập vào    string.IsNullOrWhiteSpace(tbmaNV.Text) ||
            if (
                string.IsNullOrWhiteSpace(tbHoTen.Text) ||
                cbBoxGioiTinh.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(tbDiaChi.Text) ||
                string.IsNullOrWhiteSpace(tbSoDienThoai.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text) ||
                string.IsNullOrWhiteSpace(tbMaPhongBan.Text) ||
                string.IsNullOrWhiteSpace(tbMaChucVu.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    // Câu lệnh SQL chèn dữ liệu vào bảng tblNhanVien
                    string sql = @"INSERT INTO tblNhanVien 
                           ( HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaPB, MaCV, GhiChu, DeletedAt)
                           VALUES ( @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SoDienThoai, @Email, @MaPB, @MaCV, @GhiChu, NULL)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Gán giá trị từ các ô nhập liệu vào tham số SQL
                        cmd.Parameters.AddWithValue("@HoTen", tbHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgaySinh", dateTimePickerNgaySinh.Value);
                        cmd.Parameters.AddWithValue("@GioiTinh", cbBoxGioiTinh.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@DiaChi", tbDiaChi.Text.Trim());
                        cmd.Parameters.AddWithValue("@SoDienThoai", tbSoDienThoai.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", tbEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@MaPB", tbMaPhongBan.Text.Trim());
                        cmd.Parameters.AddWithValue("@MaCV", tbMaChucVu.Text.Trim());
                        cmd.Parameters.AddWithValue("@GhiChu", tbGhiChu.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Thêm nhân viên thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDataNhanVien(); // Hàm load lại dữ liệu DataGridView
                            ClearAllInputs(this); // Xóa dữ liệu trên form
                        }
                        else
                        {
                            MessageBox.Show("Thêm nhân viên thất bại!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(tbmaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                if (MessageBox.Show("Bạn có muốn xóa dữ liệu?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        string query = "DELETE FROM SanPham WHERE Id=@id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", tbId.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Không tìm thấy khách hàng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        LoadDataSanPham();
                    }
                }
                ClearAllInputs(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void dtGridViewNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtGridViewNhanVien.Rows[e.RowIndex];

                // Lấy dữ liệu từ DataGridView gán vào các TextBox
                tbmaNV.Text = row.Cells["MaNV"].Value.ToString();
                tbHoTen.Text = row.Cells["HoTen"].Value.ToString();
                dateTimePickerNgaySinh.Text = row.Cells["NgaySinh"].Value.ToString(); 
                cbBoxGioiTinh.SelectedItem = row.Cells["GioiTinh"].Value.ToString();
                tbDiaChi.Text = row.Cells["DiaChi"].Value.ToString();
                tbSoDienThoai.Text = row.Cells["SoDienThoai"].Value.ToString();
                tbEmail.Text = row.Cells["Email"].Value.ToString();
                tbMaPhongBan.Text = row.Cells["MaPB"].Value.ToString();
                tbMaChucVu.Text = row.Cells["MaCV"].Value.ToString();
                tbGhiChu.Text = row.Cells["Ghichu"].Value.ToString();
            }
        }
    }
}
