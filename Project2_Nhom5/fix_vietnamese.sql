-- Script sửa lỗi encoding tiếng Việt
-- Chạy từng lệnh một trong SQL Server Management Studio

-- 1. Cập nhật collation cho database
ALTER DATABASE BanVeXemPhim COLLATE Vietnamese_CI_AS;

-- 2. Cập nhật collation cho bảng GiamGia
ALTER TABLE GiamGia ALTER COLUMN MoTa NVARCHAR(MAX) COLLATE Vietnamese_CI_AS;

-- 3. Test cập nhật mô tả với tiếng Việt
UPDATE GiamGia 
SET MoTa = N'Giảm giá 10% cho tất cả vé' 
WHERE MaCode = 'WELCOME10';

-- 4. Kiểm tra kết quả
SELECT MaCode, MoTa FROM GiamGia WHERE MaCode = 'WELCOME10'; 