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
using System.Xml.Linq;

namespace QuanLyNhanVien2
{
    public partial class DangNhap: Form
    {
        string constr = "Data Source=DESKTOP-10V42VO\\SQLEXPRESS;Initial Catalog=QuanLyNhanVien2;Integrated Security=True;";
        public DangNhap()
        {
            InitializeComponent();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {
            tbpassword.UseSystemPasswordChar = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void btndangnhap_Click(object sender, EventArgs e)
        {

            string username = tbusename.Text.Trim();
            string password = tbpassword.Text.Trim();

            // 1. Kiểm tra bỏ trống
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tài khoản và mật khẩu!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra với cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM tblTaiKhoan WHERE TenDangNhap = @username AND MatKhau = @password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = (int)cmd.ExecuteScalar();

                    // 3. Xử lý kết quả
                    if (count > 0)
                    {
                        MessageBox.Show("Đăng nhập thành công!",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close(); // Đóng form đăng nhập
                    }
                    else
                    {
                        MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!",
                                        "Lỗi",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message,
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("ban co muon thoat khong?", "tieu de thoat",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkshowpassword.Checked)
            {
                // Hiển thị mật khẩu
                tbpassword.UseSystemPasswordChar = false;
            }
            else
            {
                // Ẩn mật khẩu
                tbpassword.UseSystemPasswordChar = true;
            }
        }
    }
}
