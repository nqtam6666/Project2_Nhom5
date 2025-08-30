# 🎬 Hệ Thống Đặt Vé Xem Phim

## 📋 Mô tả

Hệ thống đặt vé xem phim hoàn chỉnh với giao diện hiện đại, hỗ trợ đặt vé trực tuyến, chọn ghế, thanh toán và quản lý lịch sử đặt vé.

## ✨ Tính năng chính

### 🎯 Cho Khách Hàng (Guest Area)

- **Xem phim**: Danh sách phim đang chiếu và sắp chiếu
- **Đặt vé**: Chỉ cho phim "Đang chiếu"
- **Chọn ghế**: Sơ đồ rạp trực quan với ghế VIP và thường
- **Thanh toán**: 4 phương thức (MoMo, ZaloPay, Banking, Tiền mặt)
- **Mã giảm giá**: Hỗ trợ giảm giá theo % và số tiền cố định
- **Lịch sử**: Xem và quản lý vé đã đặt
- **Xác nhận**: QR Code và mã vé duy nhất

### 🔧 Cho Admin (Admin Area)

- **Quản lý phim**: Thêm, sửa, xóa phim
- **Quản lý rạp**: Cấu hình rạp chiếu và ghế ngồi
- **Quản lý suất chiếu**: Lập lịch chiếu phim
- **Quản lý vé**: Xem và xử lý đặt vé
- **Quản lý thanh toán**: Theo dõi giao dịch
- **Báo cáo doanh thu**: Thống kê theo thời gian
- **Quản lý người dùng**: Phân quyền và tài khoản
- **Mã giảm giá**: Tạo và quản lý khuyến mãi

## 🚀 Cài đặt và Chạy

### Yêu cầu hệ thống

- .NET 8.0
- SQL Server
- Visual Studio 2022 hoặc VS Code

### Bước 1: Clone và Build

```bash
git clone <repository-url>
cd Project2_Nhom5/Project2_Nhom5/Project2_Nhom5
dotnet restore
dotnet build
```

### Bước 2: Cấu hình Database

1. Tạo database mới trong SQL Server
2. Cập nhật connection string trong `appsettings.json`
3. Chạy migration:

```bash
dotnet ef database update
```

### Bước 3: Thêm dữ liệu mẫu

Chạy file SQL: `Data/sample_data.sql`

### Bước 4: Chạy ứng dụng

```bash
dotnet run
```

## 📊 Cấu trúc Database

### Bảng chính

- **NguoiDung**: Thông tin người dùng
- **Phim**: Thông tin phim
- **RapPhim**: Thông tin rạp chiếu
- **Ghe**: Cấu hình ghế ngồi
- **SuatChieu**: Lịch chiếu phim
- **Ve**: Thông tin vé đặt
- **ThanhToan**: Giao dịch thanh toán
- **GiamGia**: Mã giảm giá

## 🎨 Giao diện

### Theme

- **Light/Dark mode** tự động
- **Responsive design** cho mọi thiết bị
- **Modern UI** với gradient và animation
- **Bootstrap 5** framework

### Màu sắc chủ đạo

- Primary: `#ff6b35` (Orange)
- Secondary: `#1a1a2e` (Dark Blue)
- Accent: `#16213e` (Navy)

## 🔐 Bảo mật

### Authentication

- **Cookie-based** authentication
- **Session 30 ngày** tự động
- **Role-based** authorization (Admin/Guest)

### Validation

- **Server-side** validation
- **Client-side** validation với jQuery
- **Anti-forgery** tokens

## 💳 Thanh toán

### Phương thức hỗ trợ

1. **MoMo** - Ví điện tử
2. **ZaloPay** - Ví điện tử
3. **Banking** - Chuyển khoản ngân hàng
4. **Cash** - Tiền mặt

### Mã giảm giá

- **WELCOME10**: Giảm 10% cho khách mới
- **VIP20**: Giảm 20% cho ghế VIP
- **FLAT50K**: Giảm 50,000 VNĐ
- **WEEKEND15**: Giảm 15% cuối tuần

## 📱 Responsive Design

### Breakpoints

- **Mobile**: < 576px
- **Tablet**: 576px - 992px
- **Desktop**: > 992px

### Features

- **Touch-friendly** buttons
- **Swipe gestures** support
- **Optimized** images
- **Fast loading** times

## 🛠️ Công nghệ sử dụng

### Backend

- **ASP.NET Core 8.0**
- **Entity Framework Core**
- **SQL Server**
- **C# 12.0**

### Frontend

- **Bootstrap 5**
- **jQuery 3.7**
- **SweetAlert2**
- **QR Code Generator**

### Tools

- **Visual Studio 2022**
- **SQL Server Management Studio**
- **Git**

## 📈 Performance

### Optimization

- **Lazy loading** images
- **Minified** CSS/JS
- **Caching** strategies
- **Database** indexing

### Monitoring

- **Error logging**
- **Performance metrics**
- **User analytics**

## 🔄 Workflow đặt vé

1. **Chọn phim** → Xem danh sách phim đang chiếu
2. **Chọn suất** → Xem lịch chiếu và rạp
3. **Chọn ghế** → Sơ đồ rạp với ghế trống/đã đặt
4. **Thanh toán** → Nhập thông tin và chọn phương thức
5. **Xác nhận** → Nhận mã vé và QR Code
6. **Lịch sử** → Xem và quản lý vé đã đặt

## 🎯 Roadmap

### V1.1 (Sắp tới)

- [ ] Tích hợp thanh toán thực tế
- [ ] Push notifications
- [ ] Mobile app
- [ ] Loyalty program

### V1.2 (Tương lai)

- [ ] AI recommendation
- [ ] Social features
- [ ] Advanced analytics
- [ ] Multi-language support

## 📞 Hỗ trợ

### Liên hệ

- **Email**: support@cinema.com
- **Phone**: 1900-xxxx
- **Website**: www.cinema.com

### Documentation

- **API Docs**: `/swagger`
- **User Guide**: `/help`
- **Admin Guide**: `/admin/help`

## 📄 License

MIT License - Xem file `LICENSE` để biết thêm chi tiết.

---

**Made with ❤️ by Team 5**
