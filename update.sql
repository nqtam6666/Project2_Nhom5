CREATE DATABASE BanVeXemPhim;
GO

USE BanVeXemPhim;
GO

-- Bảng NguoiDung
CREATE TABLE NguoiDung (
    MaNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    MatKhau NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    SoDienThoai NVARCHAR(20),
    VaiTro NVARCHAR(50) CHECK (VaiTro IN ('Khach', 'NguoiDung', 'Admin', 'DaiLy')) DEFAULT 'NguoiDung',
    TrangThai NVARCHAR(50) CHECK (TrangThai IN ('hoatdong', 'khonghoatdong')) DEFAULT 'hoatdong'
);

-- Bảng Phim
CREATE TABLE Phim (
    MaPhim INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    TheLoai NVARCHAR(255),
    ThoiLuong INT, -- phút
    MoTa TEXT,
    AnhBia NVARCHAR(255),
    Trailer NVARCHAR(255),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN ('dangchieu', 'sapchieu', 'ngungchieu')) DEFAULT 'sapchieu'
);

-- Bảng RapPhim
CREATE TABLE RapPhim (
    MaRap INT IDENTITY(1,1) PRIMARY KEY,
    TenRap NVARCHAR(100) NOT NULL,
    DiaDiem NVARCHAR(255),
    SoPhong INT NOT NULL
);

-- Bảng SuatChieu
CREATE TABLE SuatChieu (
    MaSuatChieu INT IDENTITY(1,1) PRIMARY KEY,
    MaPhim INT,
    MaRap INT,
    NgayChieu DATE NOT NULL,
    GioChieu TIME NOT NULL,
    FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim) ON DELETE CASCADE,
    FOREIGN KEY (MaRap) REFERENCES RapPhim(MaRap) ON DELETE CASCADE
);

-- Bảng Ghe
CREATE TABLE Ghe (
    MaGhe INT IDENTITY(1,1) PRIMARY KEY,
    MaRap INT,
    MaSoGhe NVARCHAR(10) NOT NULL, -- A1, A2,...
    LoaiGhe NVARCHAR(50) CHECK (LoaiGhe IN ('thuong', 'VIP')) DEFAULT 'thuong',
    FOREIGN KEY (MaRap) REFERENCES RapPhim(MaRap) ON DELETE CASCADE
);

-- Bảng Ve
CREATE TABLE Ve (
    MaVe INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoiDung INT,
    MaSuatChieu INT,
    MaGhe INT,
    GiaVe DECIMAL(10,2),
    TrangThai NVARCHAR(50) CHECK (TrangThai IN ('choxuly', 'dathanhtoan', 'dahuy')) DEFAULT 'choxuly',
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung) ON DELETE SET NULL,
    FOREIGN KEY (MaSuatChieu) REFERENCES SuatChieu(MaSuatChieu) ON DELETE CASCADE,
    FOREIGN KEY (MaGhe) REFERENCES Ghe(MaGhe) ON DELETE NO ACTION
);

-- Bảng Giamgia
CREATE TABLE GiamGia (
    MaGiamGia INT IDENTITY(1,1) PRIMARY KEY,
    MaCode NVARCHAR(50) UNIQUE NOT NULL,
    MoTa TEXT,
    LoaiGiamGia NVARCHAR(50) CHECK (LoaiGiamGia IN ('phantram', 'codinh')) NOT NULL,
    GiaTri DECIMAL(10,2) NOT NULL,
    NgayHetHan DATE NOT NULL
);

-- Bảng Thanhtoan
CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    MaVe INT UNIQUE,
    SoTien DECIMAL(10,2) NOT NULL,
    PhuongThucThanhToan NVARCHAR(50),
    NgayThanhToan DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaVe) REFERENCES Ve(MaVe) ON DELETE CASCADE
);

-- Bảng Doanhthu
CREATE TABLE DoanhThu (
    MaDoanhThu INT IDENTITY(1,1) PRIMARY KEY,
    MaSuatChieu INT,
    TongTien DECIMAL(15,2) DEFAULT 0,
    HoaHongDaiLy DECIMAL(5,2) DEFAULT 0, -- %
    FOREIGN KEY (MaSuatChieu) REFERENCES SuatChieu(MaSuatChieu) ON DELETE CASCADE
);

-- Chèn dữ liệu vào bảng Nguoidung
INSERT INTO NguoiDung (TenDangNhap, MatKhau, Email, SoDienThoai, VaiTro, TrangThai)
VALUES 
    ('doanquan', 'password1', 'doanquan6805@gmail.com', '0359271424', 'Admin', 'hoatdong'),
    ('vanthinh', 'password1', 'thinhvannguyen113@gmail.com', '0343841426', 'Admin', 'hoatdong'),
    ('quangtam', 'password1', 'nguyenquangtam179@gmail.com', '0961138440', 'Admin', 'hoatdong');

-- Chèn dữ liệu vào bảng Phim
INSERT INTO Phim (TieuDe, TheLoai, ThoiLuong, MoTa, AnhBia, Trailer, TrangThai)
VALUES 
    ('Avengers: Endgame', 'Hanh_dong, Khoa_hoc_vien_tuong', 181, 'Mot dinh cao cua 22 bo phim lien ket.', 'https://example.com/avengers.jpg', 'https://example.com/avengers_trailer', 'dangchieu'),
    ('The Lion King', 'Hoat_hinh, Chinh_kich', 118, 'Ban lam lai cua bo phim hoat hinh kinh dien.', 'https://example.com/lionking.jpg', 'https://example.com/lionking_trailer', 'sapchieu'),
    ('Inception', 'Khoa_hoc_vien_tuong, Gay_can', 148, 'Mot bo phimFAC phim cuop giat tam tri.', 'https://example.com/inception.jpg', 'https://example.com/inception_trailer', 'ngungchieu');

-- Chèn dữ liệu vào bảng Rapphim
INSERT INTO RapPhim (TenRap, DiaDiem, SoPhong)
VALUES 
    ('CGV Vincom', 'Ha Noi, Viet Nam', 1),
    ('BHD Star Cineplex', 'TP Ho Chi Minh, Viet Nam', 2),
    ('Lotte Cinema', 'Da Nang, Viet Nam', 3);

-- Chèn dữ liệu vào bảng SuatChieu
INSERT INTO SuatChieu (MaPhim, MaRap, NgayChieu, GioChieu)
VALUES 
    (1, 1, '2025-08-26', '14:30:00'),
    (2, 2, '2025-08-27', '18:00:00'),
    (3, 3, '2025-08-28', '20:15:00');

-- Chèn dữ liệu vào bảng Ghe
INSERT INTO Ghe (MaRap, MaSoGhe, LoaiGhe)
VALUES 
    (1, 'A1', 'thuong'),
    (1, 'A2', 'VIP'),
    (2, 'B1', 'thuong'),
    (2, 'B2', 'VIP'),
    (3, 'C1', 'thuong');

-- Chèn dữ liệu vào bảng Ve
INSERT INTO Ve (MaNguoiDung, MaSuatChieu, MaGhe, GiaVe, TrangThai)
VALUES 
    (1, 1, 1, 120000.00, 'dathanhtoan'),
    (2, 2, 3, 150000.00, 'dathanhtoan'),
    (3, 3, 5, 100000.00, 'choxuly');

-- Chèn dữ liệu vào bảng GiamGia
INSERT INTO GiamGia (MaCode, MoTa, LoaiGiamGia, GiaTri, NgayHetHan)
VALUES 
    ('DISCOUNT10', 'Giam 10% cho lan dau mua ve', 'phantram', 10.00, '2025-09-01'),
    ('FIXED50K', 'Giam 50,000 VND', 'codinh', 50000.00, '2025-08-31');

-- Chèn dữ liệu vào bảng ThanhToan
INSERT INTO ThanhToan (MaVe, SoTien, PhuongThucThanhToan)
VALUES 
    (1, 120000.00, 'TheTinDung'),
    (2, 150000.00, 'TienMat');

-- Chèn dữ liệu vào bảng DoanhThu
INSERT INTO DoanhThu (MaSuatChieu, TongTien, HoaHongDaiLy)
VALUES 
    (1, 120000.00, 5.00), -- Doanh thu từ suất chiếu Avengers
    (2, 150000.00, 5.00); -- Doanh thu từ suất chiếu The Lion King