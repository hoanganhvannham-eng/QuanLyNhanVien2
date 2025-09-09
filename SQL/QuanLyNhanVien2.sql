
-- Tạo mới database
CREATE DATABASE QuanLyNhanVien2;
GO

USE QuanLyNhanVien2;
GO

UPDATE tblNhanVien SET DeletedAt = 1 WHERE MaNV = 3;
SELECT * FROM tblNhanVien WHERE DeletedAt = 0;






CREATE TABLE tblPhongBan (
    MaPB INT PRIMARY KEY IDENTITY(1,1),
    TenPB NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    SoDienThoai VARCHAR(20),
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL -- 0: chưa xóa, 1: đã xóa
);

CREATE TABLE tblChucVu (
    MaCV INT PRIMARY KEY IDENTITY(1,1),
    TenCV NVARCHAR(100) NOT NULL,
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL
);

CREATE TABLE tblNhanVien (
    MaNV INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(200),
    SoDienThoai VARCHAR(20),
    Email VARCHAR(100) UNIQUE, -- không được trùng 
    MaPB INT NOT NULL,
    MaCV INT NOT NULL,
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL,
    FOREIGN KEY (MaPB) REFERENCES tblPhongBan(MaPB),
    FOREIGN KEY (MaCV) REFERENCES tblChucVu(MaCV)
);

CREATE TABLE tblHopDong (
    MaHopDong INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT UNIQUE NOT NULL,   -- Mỗi nhân viên chỉ có 1 hợp đồng
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE,
    LoaiHopDong NVARCHAR(50),
    LuongCoBan DECIMAL(18,2) NOT NULL,
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV)
);

CREATE TABLE tblLuong (
    MaLuong INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT NOT NULL,
    Thang INT CHECK (Thang BETWEEN 1 AND 12),
    Nam INT,
    LuongCoBan DECIMAL(18,2) NOT NULL, -- ########.## (tối đa 8 số nguyên + 2 số thập phân).
    SoNgayCong INT,
    PhuCap DECIMAL(18,2) DEFAULT 0,
    KhauTru DECIMAL(18,2) DEFAULT 0,
    Ghichu NVARCHAR(255),
    TongLuong AS (LuongCoBan + PhuCap - KhauTru) PERSISTED, -- cột tính toán
    DeletedAt INT NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV)
);

CREATE TABLE tblDuAn (
    MaDA INT PRIMARY KEY IDENTITY(1,1),
    TenDA NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(500),
    NgayBatDau DATE,
    NgayKetThuc DATE,
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL
);

CREATE TABLE tblChiTietDuAn (
    MaNV INT NOT NULL,
    MaDA INT NOT NULL,
    VaiTro NVARCHAR(100),
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL,
    PRIMARY KEY (MaNV, MaDA),
    FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV),
    FOREIGN KEY (MaDA) REFERENCES tblDuAn(MaDA)
);

CREATE TABLE tblChamCong (
    MaChamCong INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT NOT NULL,
    Ngay DATE NOT NULL,
    GioVao TIME NOT NULL,     -- Giờ bắt đầu làm
    GioVe TIME NOT NULL,      -- Giờ kết thúc làm
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV)
);

CREATE TABLE tblTaiKhoan (
    MaTK INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT UNIQUE NOT NULL,
    TenDangNhap VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(255) NOT NULL, 
    Quyen NVARCHAR(50) DEFAULT 'User', -- auto nếu không chỉ định hệ thống sẽ gán quyền User.
    Ghichu NVARCHAR(255),
    DeletedAt INT NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES tblNhanVien(MaNV)
);


-- Phòng Ban
INSERT INTO tblPhongBan (TenPB, DiaChi, SoDienThoai, Ghichu, DeletedAt)
VALUES
(N'Phòng Hành Chính', N'Hà Nội', '0241234567', N'Quản lý nhân sự & hành chính', 0),
(N'Phòng Kỹ Thuật', N'Hồ Chí Minh', '0287654321', N'Phát triển & bảo trì hệ thống', 0),
(N'Phòng Kinh Doanh', N'Đà Nẵng', '0236789123', N'Tìm kiếm khách hàng và bán hàng', 0);

-- Chức vụ
INSERT INTO tblChucVu (TenCV, Ghichu, DeletedAt)
VALUES
(N'Giám đốc', N'Quản lý toàn công ty', 0),
(N'Trưởng phòng', N'Quản lý phòng ban', 0),
(N'Nhân viên', N'Thực hiện công việc được giao', 0);

-- Nhân viên
INSERT INTO tblNhanVien (HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email, MaPB, MaCV, Ghichu, DeletedAt)
VALUES
(N'Nguyễn Văn A', '1985-05-20', N'Nam', N'Hà Nội', '0912345678', 'ana@example.com', 1, 1, N'Giám đốc công ty', 0),
(N'Trần Thị B', '1990-09-15', N'Nữ', N'Hồ Chí Minh', '0987654321', 'hib@example.com', 2, 2, N'Trưởng phòng kỹ thuật', 0),
(N'Lê Văn C', '1995-12-01', N'Nam', N'Đà Nẵng', '0934567890', 'vnc@example.com', 3, 3, N'Nhân viên kinh doanh', 0),
(N'Phạm Thị D', '1997-07-07', N'Nữ', N'Hải Phòng', '0978123456', 'thi@example.com', 2, 3, N'Nhân viên kỹ thuật', 0);

-- Hợp đồng
INSERT INTO tblHopDong (MaNV, NgayBatDau, NgayKetThuc, LoaiHopDong, LuongCoBan, Ghichu, DeletedAt)
VALUES
(1, '2020-01-01', NULL, N'Không xác định thời hạn', 30000000, N'Hợp đồng Giám đốc', 0),
(2, '2021-03-01', '2026-03-01', N'Xác định thời hạn 5 năm', 20000000, N'Hợp đồng Trưởng phòng', 0),
(3, '2022-06-15', '2025-06-15', N'Xác định thời hạn 3 năm', 12000000, N'Hợp đồng nhân viên KD', 0),
(4, '2023-02-01', '2026-02-01', N'Xác định thời hạn 3 năm', 10000000, N'Hợp đồng nhân viên KT', 0);

-- Lương
INSERT INTO tblLuong (MaNV, Thang, Nam, LuongCoBan, SoNgayCong, PhuCap, KhauTru, Ghichu, DeletedAt)
VALUES
(1, 7, 2025, 30000000, 22, 5000000, 1000000, N'Lương tháng 7 Giám đốc', 0),
(2, 7, 2025, 20000000, 22, 3000000, 500000, N'Lương tháng 7 Trưởng phòng', 0),
(3, 7, 2025, 12000000, 21, 2000000, 200000, N'Lương tháng 7 NV KD', 0),
(4, 7, 2025, 10000000, 20, 1500000, 100000, N'Lương tháng 7 NV KT', 0);

-- Dự án
INSERT INTO tblDuAn (TenDA, MoTa, NgayBatDau, NgayKetThuc, Ghichu, DeletedAt)
VALUES
(N'Hệ thống ERP', N'Xây dựng hệ thống quản lý doanh nghiệp', '2023-01-01', '2025-12-31', N'Dự án quan trọng', 0),
(N'Website TMĐT', N'Phát triển website thương mại điện tử', '2024-03-01', '2025-03-01', N'Dự án thương mại', 0),
(N'Ứng dụng Mobile', N'Ứng dụng di động bán hàng', '2025-01-01', NULL, N'Dự án mới', 0);

-- Chi tiết dự án
INSERT INTO tblChiTietDuAn (MaNV, MaDA, VaiTro, Ghichu, DeletedAt)
VALUES
(1, 1, N'Quản lý dự án', N'Giám sát toàn bộ', 0),
(2, 1, N'Chủ trì kỹ thuật', N'Thiết kế hệ thống', 0),
(3, 2, N'Kinh doanh', N'Tìm khách hàng', 0),
(4, 3, N'Lập trình viên', N'Phát triển ứng dụng', 0);

-- Chấm công
INSERT INTO tblChamCong (MaNV, Ngay, GioVao, GioVe, Ghichu, DeletedAt)
VALUES
(1, '2025-08-01', '08:00:00', '17:00:00', N'Đi làm đúng giờ', 0),
(2, '2025-08-01', '08:15:00', '17:30:00', N'Đi muộn 15 phút', 0),
(3, '2025-08-01', '08:00:00', '17:00:00', N'Làm việc đầy đủ', 0),
(4, '2025-08-01', '08:30:00', '17:00:00', N'Đi muộn 30 phút', 0);

-- Tài khoản
INSERT INTO tblTaiKhoan (MaNV, TenDangNhap, MatKhau, Quyen, Ghichu, DeletedAt)
VALUES
(1, 'admin', '123456', N'Admin', N'Tài khoản quản trị', 0),
(2, 'thib', '123456', N'Manager', N'Tài khoản trưởng phòng', 0),
(3, 'vanc', '123456', N'User', N'Tài khoản nhân viên KD', 0),
(4, 'thid', '123456', N'User', N'Tài khoản nhân viên KT', 0);
