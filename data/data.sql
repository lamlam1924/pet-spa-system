
INSERT INTO Roles (RoleName, Description) VALUES
    ('Admin', 'Quản trị viên hệ thống, có quyền quản lý toàn bộ dữ liệu và chức năng'),
    ('Customer', 'Khách hàng, có thể đặt lịch hẹn, mua sản phẩm và viết đánh giá'),
    ('Employee', 'Nhân viên spa, thực hiện các dịch vụ chăm sóc thú cưng');

INSERT INTO Status_Appointment (StatusName, Description) VALUES
    ('Pending', 'Lịch hẹn đang chờ xác nhận từ nhân viên'),
    ('Confirmed', 'Lịch hẹn đã được xác nhận và sẵn sàng thực hiện'),
    ('InProgress', 'Lịch hẹn đang được thực hiện tại spa'),
    ('Completed', 'Lịch hẹn đã hoàn thành'),
    ('Cancelled', 'Lịch hẹn đã bị hủy bởi khách hàng hoặc nhân viên');

INSERT INTO StatusOrder (StatusName, Description) VALUES
    ('Pending', 'Đơn hàng đang chờ xử lý'),
    ('Processing', 'Đơn hàng đang được chuẩn bị và đóng gói'),
    ('Shipped', 'Đơn hàng đã được giao cho đơn vị vận chuyển'),
    ('Delivered', 'Đơn hàng đã được giao đến khách hàng'),
    ('Cancelled', 'Đơn hàng đã bị hủy');

-- Chèn phương thức thanh toán
INSERT INTO PaymentMethods (MethodName, Description) VALUES
    ('VNPay', 'Thanh toán qua VNPay'),
    ('PayOS', 'Thanh toán qua PayOS'),
    ('Cash', 'Thanh toán tiền mặt');

-- Chèn trạng thái thanh toán
INSERT INTO PaymentStatuses (StatusName, Description) VALUES
    ('Pending', 'Chờ thanh toán'),
    ('Completed', 'Đã thanh toán'),
    ('Failed', 'Thanh toán thất bại');

select * from ProductCategories
--Table Cate_Product
INSERT INTO ProductCategories (Name)
VALUES
    (N'Cho chó'),
    (N'Cho mèo'),
    (N'Phụ kiện thú cưng'),
    (N'Sản phẩm điều trị'),
    (N'Thực phẩm chức năng');
INSERT INTO ProductCategories (Name, Cate_parent)
VALUES
    (N'Sản phẩm vệ sinh', 1),
    (N'Thức ăn dinh dưỡng', 1),
    (N'Sản phẩm vệ sinh', 2),
    (N'Thức ăn dinh dưỡng', 2),
    (N'Chuổng & Balo', 3),
	(N'Dụng cụ ăn uống', 3),
	(N'Đồ chơi', 3),
	(N'Nệm ngủ', 3),
	(N'Vòng cổ & dây dắt', 3);
INSERT INTO ProductCategories (Name, Cate_parent)
VALUES
    (N'Bánh thưởng', 7),
    (N'Thức ăn hạt', 7),
    (N'Thức ăn hỗ trợ điều trị', 7),
    (N'Thức ăn ướt', 7),
	(N'Bánh thưởng', 9),
    (N'Hạt cho mèo', 9),
    (N'Pate & Sốt', 9),
    (N'Thức ăn hỗ trợ điều trị', 9);

-- Chèn dữ liệu mẫu vào bảng Users (không có ảnh đại diện)
select * from Users
INSERT INTO Users (Username, Email, PasswordHash, FullName, Phone, Address, RoleID, ProfilePictureUrl, UpdatedAt)
VALUES 
-- Admin
('admin_pet', 'admin@petservice.vn', 'hashed_password_admin123', N'Nguyễn Văn Quản Trị', '0909123456', N'12 Nguyễn Trãi, Hà Nội', 1, NULL, NULL),
-- Customer
('khachhang01', 'kh01@gmail.com', 'hashed_password_cust01', N'Lê Thị Khách', '0911122233', N'45 Hai Bà Trưng, TP.HCM', 2, NULL, null),
('khachhang02', 'kh02@gmail.com', 'hashed_password_cust02', N'Trần Văn Mua', '0988765432', N'21 Lý Thường Kiệt, Đà Nẵng', 2, NULL, NULL),
-- Employee
('nhanvien01', 'nvspa01@petspa.vn', 'hashed_password_emp01', N'Phạm Thị Spa', '0933344455', N'67 Nguyễn Huệ, Hà Nội', 3, NULL, null),
('nhanvien02', 'nvspa02@petspa.vn', 'hashed_password_emp02', N'Hoàng Văn Cạo Lông', '0966123456', N'89 Trần Phú, TP.HCM', 3, NULL, NULL);

INSERT INTO Species (SpeciesName) VALUES
(N'Dog'),
(N'Cat');
-- Pet của khách hàng 1
INSERT INTO Pets (UserID, Name, SpeciesID, Breed, Age, Gender, HealthCondition, SpecialNotes, LastSpaVisit)
VALUES 
(2, N'Milu', 1, N'Poodle', 3, 'Male', N'Khỏe mạnh', NULL, '2025-05-20 09:00:00'),
(2, N'Mèo Mun', 2, N'Mèo ta', 2, 'Female', NULL, N'Rất hiếu động, sợ nước', NULL),
-- Pet của khách hàng 2
(3, N'LuLu', 1, N'Chihuahua', 1, 'Female', NULL, NULL, NULL),
(3, N'Xám', 2, NULL, 4, 'Male', N'Từng bị bệnh da liễu', NULL, '2025-04-15 15:00:00');

