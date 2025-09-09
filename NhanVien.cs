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
                    string sql = @"SELECT  MaNV,HoTen, NgaySinh, GioiTinh, DiaChi,  SoDienThoai,  Email, MaPB, MaCV,  GhiChu
                                FROM tblNhanVien
                                WHERE DeletedAt = 0  ORDER BY MaNV"; 

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
            

            try
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
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    // Câu lệnh SQL chèn dữ liệu vào bảng tblNhanVien
                    string sql = @"INSERT INTO tblNhanVien 
                           ( HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaPB, MaCV, GhiChu, DeletedAt)
                           VALUES ( @HoTen, @NgaySinh, @GioiTinh, @DiaChi, @SoDienThoai, @Email, @MaPB, @MaCV, @GhiChu, 0)";

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
                // 1. Kiểm tra xem đã chọn nhân viên nào chưa
                if (string.IsNullOrEmpty(tbmaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn hoặc nhập mã nhân viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Xác nhận người dùng trước khi xóa
                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa nhân viên này không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        // DELETE FROM tblNhanVien WHERE MaNV = @MaNV / UPDATE tblNhanVien SET DeletedAt = 1 WHERE MaNV = @MaNV
                        string query = "UPDATE tblNhanVien SET DeletedAt = 1 WHERE MaNV = @MaNV";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaNV", tbmaNV.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 3. Load lại danh sách sau khi xóa
                            LoadDataNhanVien();

                            // 4. Xóa trắng các ô nhập liệu
                            ClearAllInputs(this);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhân viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtGridViewNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                tbmaNV.Text = dtGridViewNhanVien.Rows[i].Cells[0].Value.ToString();
                tbHoTen.Text = dtGridViewNhanVien.Rows[i].Cells[1].Value.ToString();
                dateTimePickerNgaySinh.Value = Convert.ToDateTime(dtGridViewNhanVien.Rows[i].Cells[2].Value);
                cbBoxGioiTinh.Text = dtGridViewNhanVien.Rows[i].Cells[3].Value.ToString();
                tbDiaChi.Text = dtGridViewNhanVien.Rows[i].Cells[4].Value.ToString();
                tbSoDienThoai.Text = dtGridViewNhanVien.Rows[i].Cells[5].Value.ToString();
                tbEmail.Text = dtGridViewNhanVien.Rows[i].Cells[6].Value.ToString();
                tbMaPhongBan.Text = dtGridViewNhanVien.Rows[i].Cells[7].Value.ToString();
                tbMaChucVu.Text = dtGridViewNhanVien.Rows[i].Cells[8].Value.ToString();
                tbGhiChu.Text = dtGridViewNhanVien.Rows[i].Cells[9].Value.ToString();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbmaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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

                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc chắn muốn sửa nhân viên này không?",
                    "Xác nhận sửa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                    conn.Open();
                    // Câu lệnh SQL chèn dữ liệu vào bảng tblNhanVien
                    string sql = @"UPDATE tblNhanVien SET HoTen = @HoTen, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, DiaChi = @DiaChi, SoDienThoai = @SoDienThoai, 
                             Email = @Email, MaPB = @MaPB, MaCV = @MaCV, GhiChu= @GhiChu, DeletedAt = 0 WHERE MaNV = @MaNV";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        // Gán giá trị từ các ô nhập liệu vào tham số SQL
                        cmd.Parameters.AddWithValue("@MaNV", tbmaNV.Text.Trim());
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
                            MessageBox.Show("Cập nhật thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDataNhanVien(); 
                            ClearAllInputs(this);
                        }
                        else
                        {
                            MessageBox.Show("Sửa nhân viên thất bại!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("loi" + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbHoTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên nhân viên để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string TenTimKiem = tbHoTen.Text.Trim();  // Lấy nội dung ô tìm kiếm
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    string sql = @"SELECT MaNV, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaPB, MaCV, GhiChu
                           FROM tblNhanVien
                           WHERE DeletedAt = 0 AND HoTen LIKE @TenTimKiem
                           ORDER BY MaNV";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@TenTimKiem", "%" + TenTimKiem + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dtGridViewNhanVien.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("loi " + ex.Message);
            }
        }

        private void btnNVDaNghiViec_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))  
                {
                    conn.Open();
                    string query = @"SELECT  MaNV ,HoTen, NgaySinh, GioiTinh, DiaChi,  SoDienThoai,  Email, MaPB, MaCV,  GhiChu
                                FROM tblNhanVien
                                WHERE DeletedAt != 0 ORDER BY MaNV";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dtGridViewNhanVien.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnKhoiPhucNhanVien_Click(object sender, EventArgs e)
        {

            try
            {
                // 1. Kiểm tra xem đã chọn nhân viên nào chưa
                if (string.IsNullOrEmpty(tbmaNV.Text))
                {
                    MessageBox.Show("Vui lòng chọn hoặc nhập mã nhân viên cần khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Xác nhận người dùng trước khi xóa
                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc chắn muốn khôi phục nhân viên này không?",
                    "Xác nhận khôi phục",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(constr))
                    {
                        conn.Open();
                        // DELETE FROM tblNhanVien WHERE MaNV = @MaNV / UPDATE tblNhanVien SET DeletedAt = 1 WHERE MaNV = @MaNV
                        string query = "UPDATE tblNhanVien SET DeletedAt = 0 WHERE MaNV = @MaNV";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaNV", tbmaNV.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("khôi phục nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 3. Load lại danh sách sau khi xóa
                            LoadDataNhanVien();

                            // 4. Xóa trắng các ô nhập liệu
                            ClearAllInputs(this);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhân viên để khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show("loi " + ex.Message);
            }
        }

        private void btnrestar_Click(object sender, EventArgs e)
        {
            LoadDataNhanVien();
        }
    }
}
