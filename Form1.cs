using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyNhanVien2
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void chiTietDuAnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChiTietDuAn CTDA = new ChiTietDuAn();
            CTDA.MdiParent = this;
            CTDA.Show();
        }

        private void nhanVienToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NhanVien NV = new NhanVien();
            NV.MdiParent = this;
            NV.Show();
        }

        private void thongTinDuAnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ThongTinDuAnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DuAn DA = new DuAn();
            DA.MdiParent = this;
            DA.Show();
        }

        private void phongBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PhongBan PB = new PhongBan();
            PB.MdiParent = this;
            PB.Show();
        }

        private void chucVuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChucVu CV = new ChucVu();
            CV.MdiParent = this;
            CV.Show();
        }

        private void taiKhoanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HopDong HD = new HopDong();
            HD.MdiParent = this;
            HD.Show();
        }

        private void longToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaiKhoan TK = new TaiKhoan();
            TK.MdiParent = this;
            TK.Show();
        }

        private void luongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Luong L = new Luong();
            L.MdiParent = this;
            L.Show();
        }

        private void chamConngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChamCong CC = new ChamCong();
            CC.MdiParent = this;
            CC.Show();
        }
    }
}
