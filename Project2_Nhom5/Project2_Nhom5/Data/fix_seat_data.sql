-- Script để sửa dữ liệu ghế cho đúng
-- Chạy script này để đảm bảo dữ liệu ghế chính xác

USE BanVeXemPhim;
GO

-- Xóa dữ liệu ghế cũ (nếu có)
DELETE FROM Ghe;
GO

-- Thêm ghế cho rạp 1 (CGV Vincom) - 8 hàng x 10 cột
INSERT INTO Ghe (MaRap, MaSoGhe, LoaiGhe) VALUES 
-- Hàng A (VIP) - 10 ghế
(1, 'A1', 'VIP'), (1, 'A2', 'VIP'), (1, 'A3', 'VIP'), (1, 'A4', 'VIP'), (1, 'A5', 'VIP'),
(1, 'A6', 'VIP'), (1, 'A7', 'VIP'), (1, 'A8', 'VIP'), (1, 'A9', 'VIP'), (1, 'A10', 'VIP'),
-- Hàng B (VIP) - 10 ghế  
(1, 'B1', 'VIP'), (1, 'B2', 'VIP'), (1, 'B3', 'VIP'), (1, 'B4', 'VIP'), (1, 'B5', 'VIP'),
(1, 'B6', 'VIP'), (1, 'B7', 'VIP'), (1, 'B8', 'VIP'), (1, 'B9', 'VIP'), (1, 'B10', 'VIP'),
-- Hàng C (VIP) - 10 ghế
(1, 'C1', 'VIP'), (1, 'C2', 'VIP'), (1, 'C3', 'VIP'), (1, 'C4', 'VIP'), (1, 'C5', 'VIP'),
(1, 'C6', 'VIP'), (1, 'C7', 'VIP'), (1, 'C8', 'VIP'), (1, 'C9', 'VIP'), (1, 'C10', 'VIP'),
-- Hàng D (Thường) - 10 ghế
(1, 'D1', 'thuong'), (1, 'D2', 'thuong'), (1, 'D3', 'thuong'), (1, 'D4', 'thuong'), (1, 'D5', 'thuong'),
(1, 'D6', 'thuong'), (1, 'D7', 'thuong'), (1, 'D8', 'thuong'), (1, 'D9', 'thuong'), (1, 'D10', 'thuong'),
-- Hàng E (Thường) - 10 ghế
(1, 'E1', 'thuong'), (1, 'E2', 'thuong'), (1, 'E3', 'thuong'), (1, 'E4', 'thuong'), (1, 'E5', 'thuong'),
(1, 'E6', 'thuong'), (1, 'E7', 'thuong'), (1, 'E8', 'thuong'), (1, 'E9', 'thuong'), (1, 'E10', 'thuong'),
-- Hàng F (Thường) - 10 ghế
(1, 'F1', 'thuong'), (1, 'F2', 'thuong'), (1, 'F3', 'thuong'), (1, 'F4', 'thuong'), (1, 'F5', 'thuong'),
(1, 'F6', 'thuong'), (1, 'F7', 'thuong'), (1, 'F8', 'thuong'), (1, 'F9', 'thuong'), (1, 'F10', 'thuong'),
-- Hàng G (Thường) - 10 ghế
(1, 'G1', 'thuong'), (1, 'G2', 'thuong'), (1, 'G3', 'thuong'), (1, 'G4', 'thuong'), (1, 'G5', 'thuong'),
(1, 'G6', 'thuong'), (1, 'G7', 'thuong'), (1, 'G8', 'thuong'), (1, 'G9', 'thuong'), (1, 'G10', 'thuong'),
-- Hàng H (Thường) - 10 ghế
(1, 'H1', 'thuong'), (1, 'H2', 'thuong'), (1, 'H3', 'thuong'), (1, 'H4', 'thuong'), (1, 'H5', 'thuong'),
(1, 'H6', 'thuong'), (1, 'H7', 'thuong'), (1, 'H8', 'thuong'), (1, 'H9', 'thuong'), (1, 'H10', 'thuong');

-- Thêm ghế cho rạp 2 (BHD Star Cineplex) - 6 hàng x 8 cột
INSERT INTO Ghe (MaRap, MaSoGhe, LoaiGhe) VALUES 
-- Hàng A (VIP) - 8 ghế
(2, 'A1', 'VIP'), (2, 'A2', 'VIP'), (2, 'A3', 'VIP'), (2, 'A4', 'VIP'), (2, 'A5', 'VIP'), (2, 'A6', 'VIP'), (2, 'A7', 'VIP'), (2, 'A8', 'VIP'),
-- Hàng B (VIP) - 8 ghế
(2, 'B1', 'VIP'), (2, 'B2', 'VIP'), (2, 'B3', 'VIP'), (2, 'B4', 'VIP'), (2, 'B5', 'VIP'), (2, 'B6', 'VIP'), (2, 'B7', 'VIP'), (2, 'B8', 'VIP'),
-- Hàng C (Thường) - 8 ghế
(2, 'C1', 'thuong'), (2, 'C2', 'thuong'), (2, 'C3', 'thuong'), (2, 'C4', 'thuong'), (2, 'C5', 'thuong'), (2, 'C6', 'thuong'), (2, 'C7', 'thuong'), (2, 'C8', 'thuong'),
-- Hàng D (Thường) - 8 ghế
(2, 'D1', 'thuong'), (2, 'D2', 'thuong'), (2, 'D3', 'thuong'), (2, 'D4', 'thuong'), (2, 'D5', 'thuong'), (2, 'D6', 'thuong'), (2, 'D7', 'thuong'), (2, 'D8', 'thuong'),
-- Hàng E (Thường) - 8 ghế
(2, 'E1', 'thuong'), (2, 'E2', 'thuong'), (2, 'E3', 'thuong'), (2, 'E4', 'thuong'), (2, 'E5', 'thuong'), (2, 'E6', 'thuong'), (2, 'E7', 'thuong'), (2, 'E8', 'thuong'),
-- Hàng F (Thường) - 8 ghế
(2, 'F1', 'thuong'), (2, 'F2', 'thuong'), (2, 'F3', 'thuong'), (2, 'F4', 'thuong'), (2, 'F5', 'thuong'), (2, 'F6', 'thuong'), (2, 'F7', 'thuong'), (2, 'F8', 'thuong');

-- Thêm ghế cho rạp 3 (Lotte Cinema) - 6 hàng x 8 cột
INSERT INTO Ghe (MaRap, MaSoGhe, LoaiGhe) VALUES 
-- Hàng A (VIP) - 8 ghế
(3, 'A1', 'VIP'), (3, 'A2', 'VIP'), (3, 'A3', 'VIP'), (3, 'A4', 'VIP'), (3, 'A5', 'VIP'), (3, 'A6', 'VIP'), (3, 'A7', 'VIP'), (3, 'A8', 'VIP'),
-- Hàng B (VIP) - 8 ghế
(3, 'B1', 'VIP'), (3, 'B2', 'VIP'), (3, 'B3', 'VIP'), (3, 'B4', 'VIP'), (3, 'B5', 'VIP'), (3, 'B6', 'VIP'), (3, 'B7', 'VIP'), (3, 'B8', 'VIP'),
-- Hàng C (Thường) - 8 ghế
(3, 'C1', 'thuong'), (3, 'C2', 'thuong'), (3, 'C3', 'thuong'), (3, 'C4', 'thuong'), (3, 'C5', 'thuong'), (3, 'C6', 'thuong'), (3, 'C7', 'thuong'), (3, 'C8', 'thuong'),
-- Hàng D (Thường) - 8 ghế
(3, 'D1', 'thuong'), (3, 'D2', 'thuong'), (3, 'D3', 'thuong'), (3, 'D4', 'thuong'), (3, 'D5', 'thuong'), (3, 'D6', 'thuong'), (3, 'D7', 'thuong'), (3, 'D8', 'thuong'),
-- Hàng E (Thường) - 8 ghế
(3, 'E1', 'thuong'), (3, 'E2', 'thuong'), (3, 'E3', 'thuong'), (3, 'E4', 'thuong'), (3, 'E5', 'thuong'), (3, 'E6', 'thuong'), (3, 'E7', 'thuong'), (3, 'E8', 'thuong'),
-- Hàng F (Thường) - 8 ghế
(3, 'F1', 'thuong'), (3, 'F2', 'thuong'), (3, 'F3', 'thuong'), (3, 'F4', 'thuong'), (3, 'F5', 'thuong'), (3, 'F6', 'thuong'), (3, 'F7', 'thuong'), (3, 'F8', 'thuong');

-- Kiểm tra dữ liệu
SELECT 
    g.MaGhe,
    g.MaSoGhe,
    g.LoaiGhe,
    r.TenRap,
    CASE 
        WHEN g.LoaiGhe = 'VIP' THEN 95000
        ELSE 75000
    END AS GiaVe
FROM Ghe g
INNER JOIN RapPhim r ON g.MaRap = r.MaRap
ORDER BY g.MaRap, g.MaSoGhe;

-- Hiển thị thống kê
DECLARE @TotalSeats INT, @VIPSeats INT, @RegularSeats INT;

SELECT @TotalSeats = COUNT(*) FROM Ghe;
SELECT @VIPSeats = COUNT(*) FROM Ghe WHERE LoaiGhe = 'VIP';
SELECT @RegularSeats = COUNT(*) FROM Ghe WHERE LoaiGhe = 'thuong';

PRINT 'Đã cập nhật dữ liệu ghế thành công!';
PRINT 'Tổng số ghế: ' + CAST(@TotalSeats AS NVARCHAR(10));
PRINT 'Ghế VIP: ' + CAST(@VIPSeats AS NVARCHAR(10));
PRINT 'Ghế thường: ' + CAST(@RegularSeats AS NVARCHAR(10));
