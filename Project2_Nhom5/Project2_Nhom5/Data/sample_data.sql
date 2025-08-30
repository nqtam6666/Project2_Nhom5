-- Thêm dữ liệu mẫu cho hệ thống đặt vé

-- Thêm rạp chiếu
INSERT INTO RapPhim (TenRap, DiaDiem, SoPhong) VALUES 
('CineMax Hà Đông', '28A Đ. Lê Trọng Tấn, Hà Cầu, Hà Đông, Hà Nội', 1),
('CineMax Mỹ Đình', 'Mỹ Đình, Nam Từ Liêm, Hà Nội', 2),
('CineMax Times City', '458 Minh Khai, Hai Bà Trưng, Hà Nội', 3);

-- Thêm ghế cho rạp 1 (8 hàng x 10 cột)
INSERT INTO Ghe (MaRap, MaSoGhe, LoaiGhe) VALUES 
-- Hàng A (VIP)
(1, 'A1', 'VIP'), (1, 'A2', 'VIP'), (1, 'A3', 'VIP'), (1, 'A4', 'VIP'), (1, 'A5', 'VIP'),
(1, 'A6', 'VIP'), (1, 'A7', 'VIP'), (1, 'A8', 'VIP'), (1, 'A9', 'VIP'), (1, 'A10', 'VIP'),
-- Hàng B (VIP)
(1, 'B1', 'VIP'), (1, 'B2', 'VIP'), (1, 'B3', 'VIP'), (1, 'B4', 'VIP'), (1, 'B5', 'VIP'),
(1, 'B6', 'VIP'), (1, 'B7', 'VIP'), (1, 'B8', 'VIP'), (1, 'B9', 'VIP'), (1, 'B10', 'VIP'),
-- Hàng C (VIP)
(1, 'C1', 'VIP'), (1, 'C2', 'VIP'), (1, 'C3', 'VIP'), (1, 'C4', 'VIP'), (1, 'C5', 'VIP'),
(1, 'C6', 'VIP'), (1, 'C7', 'VIP'), (1, 'C8', 'VIP'), (1, 'C9', 'VIP'), (1, 'C10', 'VIP'),
-- Hàng D (Thường)
(1, 'D1', 'thuong'), (1, 'D2', 'thuong'), (1, 'D3', 'thuong'), (1, 'D4', 'thuong'), (1, 'D5', 'thuong'),
(1, 'D6', 'thuong'), (1, 'D7', 'thuong'), (1, 'D8', 'thuong'), (1, 'D9', 'thuong'), (1, 'D10', 'thuong'),
-- Hàng E (Thường)
(1, 'E1', 'thuong'), (1, 'E2', 'thuong'), (1, 'E3', 'thuong'), (1, 'E4', 'thuong'), (1, 'E5', 'thuong'),
(1, 'E6', 'thuong'), (1, 'E7', 'thuong'), (1, 'E8', 'thuong'), (1, 'E9', 'thuong'), (1, 'E10', 'thuong'),
-- Hàng F (Thường)
(1, 'F1', 'thuong'), (1, 'F2', 'thuong'), (1, 'F3', 'thuong'), (1, 'F4', 'thuong'), (1, 'F5', 'thuong'),
(1, 'F6', 'thuong'), (1, 'F7', 'thuong'), (1, 'F8', 'thuong'), (1, 'F9', 'thuong'), (1, 'F10', 'thuong'),
-- Hàng G (Thường)
(1, 'G1', 'thuong'), (1, 'G2', 'thuong'), (1, 'G3', 'thuong'), (1, 'G4', 'thuong'), (1, 'G5', 'thuong'),
(1, 'G6', 'thuong'), (1, 'G7', 'thuong'), (1, 'G8', 'thuong'), (1, 'G9', 'thuong'), (1, 'G10', 'thuong'),
-- Hàng H (Thường)
(1, 'H1', 'thuong'), (1, 'H2', 'thuong'), (1, 'H3', 'thuong'), (1, 'H4', 'thuong'), (1, 'H5', 'thuong'),
(1, 'H6', 'thuong'), (1, 'H7', 'thuong'), (1, 'H8', 'thuong'), (1, 'H9', 'thuong'), (1, 'H10', 'thuong');

-- Thêm ghế cho rạp 2 (6 hàng x 8 cột)
INSERT INTO Ghe (MaRap, MaSoGhe, LoaiGhe) VALUES 
-- Hàng A (VIP)
(2, 'A1', 'VIP'), (2, 'A2', 'VIP'), (2, 'A3', 'VIP'), (2, 'A4', 'VIP'), (2, 'A5', 'VIP'), (2, 'A6', 'VIP'), (2, 'A7', 'VIP'), (2, 'A8', 'VIP'),
-- Hàng B (VIP)
(2, 'B1', 'VIP'), (2, 'B2', 'VIP'), (2, 'B3', 'VIP'), (2, 'B4', 'VIP'), (2, 'B5', 'VIP'), (2, 'B6', 'VIP'), (2, 'B7', 'VIP'), (2, 'B8', 'VIP'),
-- Hàng C (Thường)
(2, 'C1', 'thuong'), (2, 'C2', 'thuong'), (2, 'C3', 'thuong'), (2, 'C4', 'thuong'), (2, 'C5', 'thuong'), (2, 'C6', 'thuong'), (2, 'C7', 'thuong'), (2, 'C8', 'thuong'),
-- Hàng D (Thường)
(2, 'D1', 'thuong'), (2, 'D2', 'thuong'), (2, 'D3', 'thuong'), (2, 'D4', 'thuong'), (2, 'D5', 'thuong'), (2, 'D6', 'thuong'), (2, 'D7', 'thuong'), (2, 'D8', 'thuong'),
-- Hàng E (Thường)
(2, 'E1', 'thuong'), (2, 'E2', 'thuong'), (2, 'E3', 'thuong'), (2, 'E4', 'thuong'), (2, 'E5', 'thuong'), (2, 'E6', 'thuong'), (2, 'E7', 'thuong'), (2, 'E8', 'thuong'),
-- Hàng F (Thường)
(2, 'F1', 'thuong'), (2, 'F2', 'thuong'), (2, 'F3', 'thuong'), (2, 'F4', 'thuong'), (2, 'F5', 'thuong'), (2, 'F6', 'thuong'), (2, 'F7', 'thuong'), (2, 'F8', 'thuong');

-- Thêm suất chiếu cho ngày mai
INSERT INTO SuatChieu (MaPhim, MaRap, NgayChieu, GioChieu) VALUES 
-- Avengers: Endgame (Phim 1)
(1, 1, DATEADD(day, 1, GETDATE()), '09:00:00'),
(1, 1, DATEADD(day, 1, GETDATE()), '12:00:00'),
(1, 1, DATEADD(day, 1, GETDATE()), '15:00:00'),
(1, 1, DATEADD(day, 1, GETDATE()), '18:00:00'),
(1, 1, DATEADD(day, 1, GETDATE()), '21:00:00'),

(1, 2, DATEADD(day, 1, GETDATE()), '10:00:00'),
(1, 2, DATEADD(day, 1, GETDATE()), '13:00:00'),
(1, 2, DATEADD(day, 1, GETDATE()), '16:00:00'),
(1, 2, DATEADD(day, 1, GETDATE()), '19:00:00'),

(1, 3, DATEADD(day, 1, GETDATE()), '11:00:00'),
(1, 3, DATEADD(day, 1, GETDATE()), '14:00:00'),
(1, 3, DATEADD(day, 1, GETDATE()), '17:00:00'),
(1, 3, DATEADD(day, 1, GETDATE()), '20:00:00'),

-- The Dark Knight (Phim 2)
(2, 1, DATEADD(day, 2, GETDATE()), '09:30:00'),
(2, 1, DATEADD(day, 2, GETDATE()), '12:30:00'),
(2, 1, DATEADD(day, 2, GETDATE()), '15:30:00'),
(2, 1, DATEADD(day, 2, GETDATE()), '18:30:00'),

(2, 2, DATEADD(day, 2, GETDATE()), '10:30:00'),
(2, 2, DATEADD(day, 2, GETDATE()), '13:30:00'),
(2, 2, DATEADD(day, 2, GETDATE()), '16:30:00'),
(2, 2, DATEADD(day, 2, GETDATE()), '19:30:00'),

-- Inception (Phim 3)
(3, 1, DATEADD(day, 3, GETDATE()), '10:00:00'),
(3, 1, DATEADD(day, 3, GETDATE()), '13:00:00'),
(3, 1, DATEADD(day, 3, GETDATE()), '16:00:00'),
(3, 1, DATEADD(day, 3, GETDATE()), '19:00:00'),

(3, 3, DATEADD(day, 3, GETDATE()), '11:00:00'),
(3, 3, DATEADD(day, 3, GETDATE()), '14:00:00'),
(3, 3, DATEADD(day, 3, GETDATE()), '17:00:00'),
(3, 3, DATEADD(day, 3, GETDATE()), '20:00:00');

-- Thêm mã giảm giá
INSERT INTO GiamGia (MaCode, MoTa, LoaiGiamGia, GiaTri, NgayHetHan) VALUES 
('WELCOME10', 'Giảm giá 10% cho khách hàng mới', 'phantram', 10.00, DATEADD(month, 3, GETDATE())),
('VIP20', 'Giảm giá 20% cho ghế VIP', 'phantram', 20.00, DATEADD(month, 6, GETDATE())),
('FLAT50K', 'Giảm giá cố định 50,000 VNĐ', 'codinh', 50000.00, DATEADD(month, 1, GETDATE())),
('WEEKEND15', 'Giảm giá 15% cho cuối tuần', 'phantram', 15.00, DATEADD(month, 2, GETDATE()));

-- Thêm một số vé đã đặt để test
INSERT INTO Ve (MaNguoiDung, MaSuatChieu, MaGhe, GiaVe, TrangThai) VALUES 
-- Vé đã thanh toán
(1, 1, 1, 95000, 'dathanhtoan'),  -- Ghế A1 VIP
(1, 1, 2, 95000, 'dathanhtoan'),  -- Ghế A2 VIP
(2, 1, 21, 75000, 'dathanhtoan'), -- Ghế B1 thường
(2, 1, 22, 75000, 'dathanhtoan'), -- Ghế B2 thường

-- Vé chờ xử lý
(3, 2, 41, 75000, 'choxuly'),     -- Ghế C1 thường
(3, 2, 42, 75000, 'choxuly'),     -- Ghế C2 thường

-- Vé đã hủy
(1, 3, 61, 75000, 'dahuy'),       -- Ghế D1 thường
(2, 3, 62, 75000, 'dahuy');       -- Ghế D2 thường

-- Thêm thanh toán cho vé đã thanh toán
INSERT INTO ThanhToan (MaVe, SoTien, PhuongThucThanhToan, NgayThanhToan) VALUES 
(1, 95000, 'momo', DATEADD(day, -1, GETDATE())),
(2, 95000, 'zalopay', DATEADD(day, -1, GETDATE())),
(3, 75000, 'banking', DATEADD(day, -2, GETDATE())),
(4, 75000, 'cash', DATEADD(day, -2, GETDATE()));

PRINT 'Đã thêm dữ liệu mẫu thành công!'
PRINT 'Bao gồm:'
PRINT '- 3 rạp chiếu với tổng cộng 200+ ghế'
PRINT '- 25+ suất chiếu cho các phim'
PRINT '- 4 mã giảm giá'
PRINT '- 8 vé mẫu (đã thanh toán, chờ xử lý, đã hủy)'
PRINT '- 4 giao dịch thanh toán mẫu' 