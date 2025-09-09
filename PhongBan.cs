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
    public partial class PhongBan: Form
    {
        string constr = "Data Source=DESKTOP-10V42VO\\SQLEXPRESS;Initial Catalog=QuanLyNhanVien2;Integrated Security=True;";
        public PhongBan()
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
        private void LoadDataPhongBan()
        {
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT  MaPB, TenPB, DiaChi, SoDienThoai, Ghichu FROM tblPhongBan WHERE DeletedAt = 0 ORDER BY MaPB"; // 

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewPhongBan.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void PhongBan_Load(object sender, EventArgs e)
        {
            LoadDataPhongBan();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if(
                  string.IsNullOrWhiteSpace(tbTenPB.Text) ||
                  string.IsNullOrWhiteSpace(tbDiaChi.Text) ||
                  string.IsNullOrWhiteSpace(tbSoDienThoai.Text) ||
                  string.IsNullOrWhiteSpace(tbGhiChu.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    // ktra ten pong ban
                    string checkMaPBSql = "SELECT COUNT(*) FROM tblPhongBan  WHERE TenPB  = @TenPB AND DiaChi  = @DiaChi";
                    using (SqlCommand cmd = new SqlCommand(checkMaPBSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenPB", tbTenPB.Text.Trim());
                        cmd.Parameters.AddWithValue("@DiaChi", tbDiaChi.Text.Trim());
                        int MaPBCount = (int)cmd.ExecuteScalar();

                        if (MaPBCount > 0)
                        {
                            MessageBox.Show("phòng ban nay da tồn tại trong hệ thống!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Dừng lại, không thêm nhân viên
                        }
                    }

                    string sql = @"INSERT INTO tblPhongBan 
                           ( TenPB, DiaChi, SoDienThoai, Ghichu, DeletedAt) VALUES ( @TenPB, @DiaChi, @SoDienThoai, @Ghichu, 0)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Gán giá trị từ các ô nhập liệu vào tham số SQL
                        cmd.Parameters.AddWithValue("@TenPB", tbTenPB.Text.Trim());
                        cmd.Parameters.AddWithValue("@DiaChi", tbDiaChi.Text.Trim());
                        cmd.Parameters.AddWithValue("@SoDienThoai", tbSoDienThoai.Text.Trim());
                        cmd.Parameters.AddWithValue("@GhiChu", tbGhiChu.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Thêm phòng ban thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDataPhongBan(); // Hàm load lại dữ liệu DataGridView
                            ClearAllInputs(this); // Xóa dữ liệu trên form
                        }
                        else
                        {
                            MessageBox.Show("Thêm phòng ban thất bại!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            } catch(Exception ex)
            {
                MessageBox.Show("loi " + ex.Message);
            }
        }

        private void dataGridViewPhongBan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            if (i >= 0)
            {
                tbmaPB.Text = dataGridViewPhongBan.Rows[i].Cells[0].Value.ToString();
                tbTenPB.Text = dataGridViewPhongBan.Rows[i].Cells[1].Value.ToString();
                tbDiaChi.Text = dataGridViewPhongBan.Rows[i].Cells[2].Value.ToString();
                tbSoDienThoai.Text = dataGridViewPhongBan.Rows[i].Cells[3].Value.ToString();
                tbGhiChu.Text = dataGridViewPhongBan.Rows[i].Cells[4].Value.ToString();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra xem đã chọn nhân viên nào chưa
                if (string.IsNullOrEmpty(tbmaPB.Text))
                {
                    MessageBox.Show("Vui lòng chọn hoặc nhập mã phòng ban cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Xác nhận người dùng trước khi xóa
                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa phòng ban này không?",
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
                        string query = "UPDATE tblPhongBan SET DeletedAt = 1 WHERE MaPB = @MaPB";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaPB", tbmaPB.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 3. Load lại danh sách sau khi xóa
                            LoadDataPhongBan();

                            // 4. Xóa trắng các ô nhập liệu
                            ClearAllInputs(this);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy phòng ban để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(tbmaPB.Text))
                {
                    MessageBox.Show("Vui lòng chọn phòng ban cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Kiểm tra dữ liệu nhập vào    string.IsNullOrWhiteSpace(tbmaNV.Text) ||
                if (
                    string.IsNullOrWhiteSpace(tbTenPB.Text) ||
                    string.IsNullOrWhiteSpace(tbDiaChi.Text) ||
                    string.IsNullOrWhiteSpace(tbSoDienThoai.Text) ||
                    string.IsNullOrWhiteSpace(tbGhiChu.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc chắn muốn sửa phòng ban này không?",
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
                        string sql = @"UPDATE tblPhongBan SET TenPB = @TenPB, DiaChi = @DiaChi, SoDienThoai = @SoDienThoai, GhiChu= @GhiChu, DeletedAt = 0 WHERE MaPB = @MaPB";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        // Gán giá trị từ các ô nhập liệu vào tham số SQL
                        cmd.Parameters.AddWithValue("@MaPB", tbmaPB.Text.Trim());
                        cmd.Parameters.AddWithValue("@TenPB", tbTenPB.Text.Trim());
                        cmd.Parameters.AddWithValue("@DiaChi", tbDiaChi.Text.Trim());
                        cmd.Parameters.AddWithValue("@SoDienThoai", tbSoDienThoai.Text.Trim());
                        cmd.Parameters.AddWithValue("@GhiChu", tbGhiChu.Text.Trim());

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Cập nhật thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDataPhongBan();
                            ClearAllInputs(this);
                        }
                        else
                        {
                            MessageBox.Show("Sửa phòng ban thất bại!", "Lỗi",
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
                if (string.IsNullOrEmpty(tbTenPB.Text) ) //&& string.IsNullOrEmpty(tbmaPB.Text)
                {
                    MessageBox.Show("Vui lòng nhập tên phòng ban để tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); // hoặc mã
                    return;
                }
                string TenTimKiem = tbTenPB.Text.Trim();  // Lấy nội dung ô tìm kiếm
                //string MaPBtimkiem = tbmaPB.Text.Trim();  // Lấy nội dung ô tìm kiếm
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    string sql = @"SELECT MaPB, TenPB, DiaChi, SoDienThoai, Ghichu
                                    FROM tblPhongBan
                                    WHERE DeletedAt = 0 AND TenPB LIKE @TenTimKiem
                                    ORDER BY MaPB"; //(TenPB LIKE @TenTimKiem OR MaPB LIKE @MaPBtimkiem)
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@TenTimKiem", "%" + TenTimKiem + "%");
                    //cmd.Parameters.AddWithValue("@MaPBtimkiem", "%" + MaPBtimkiem + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewPhongBan.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("loi " + ex.Message);
            }
        }

        private void btnrestar_Click(object sender, EventArgs e)
        {
            LoadDataPhongBan();
        }

        private void btnHienThiPhongBanCu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    string query = @"SELECT  MaPB, TenPB, DiaChi, SoDienThoai, Ghichu FROM tblPhongBan WHERE DeletedAt = 1 ORDER BY MaPB";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewPhongBan.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnKhoiPhucPhongBanCu_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra xem đã chọn nhân viên nào chưa
                if (string.IsNullOrEmpty(tbmaPB.Text))
                {
                    MessageBox.Show("Vui lòng chọn hoặc nhập mã phòng ban cần khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Xác nhận người dùng trước khi xóa
                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc chắn muốn khôi phòng ban viên này không?",
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
                        string query = "UPDATE tblPhongBan SET DeletedAt = 0 WHERE MaPB = @MaPB";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaPB", tbmaPB.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("khôi phục phòng ban thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 3. Load lại danh sách sau khi xóa
                            LoadDataPhongBan();

                            // 4. Xóa trắng các ô nhập liệu
                            ClearAllInputs(this);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy phòng ban để khôi phục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("loi " + ex.Message);
            }

        }
    }
}
