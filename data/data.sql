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


INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Cây lăn lông quần áo – M', N'Cây lăn lông quần áo
Đặc điểm:

Chất liệu: cán bằng nhựa (phần màu hồng) + lõi lăn bụi (phần màu trắng) 60 tờ.
Kích thước: size M (lõi dài 10cm)
Lõi lăn bụi gồm cuộn giấy lăn bụi 60 tờ tiết kiệm


Cây lăn lông quần áo dùng loại keo dán cao cấp, không độc hại, không dính lên bề mặt quần áo, sofa, gối, giường… khi lăn.
Thanh lăn bụi nhỏ gọn, có thể mang theo khi đi du lịch, công tác…
Keo dính chắc chắn, tay cầm nhẹ. Dễ dàng loại bỏ các sợi vải thừa, lông chó mèo, bụi bẩn, phù hợp với tất cả các bề mặt

Hướng dẫn sử dụng:

Dùng cuộn lăn bụi lăn trực tiếp trên bề mặt muốn vệ sinh.
Sau khi lăn, bóc bỏ lớp đã sử dụng.
Nếu dùng hết, bạn có thể mua lõi lăn bụi rời để thay thế rồi dùng tiếp.

Chú ý:

Cán là phần nhựa màu hồng, lõi là phần màu trắng.
Lần đầu tiên sử dụng, quý khách nên chọn phân loại “Cán + lõi” để sử dụng ngay.
Sau khi sử dụng hết lõi, quý khách giữ lại cán và mua phân loại “Lõi riêng” để thay thế
tại : https://petservicehcm.com/store/loi-lan-long-quan-ao-10cm', 20000, 'https://petservicehcm.com/wp-content/uploads/2022/06/400G-36-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Dung dịch vệ sinh tai Bio Clean Ears', N'Dung dịch vệ sinh tai BIO CLEAN EARS
-Công dụng: Làm sạch tai, làm sạch các mô h.oại t.ử, các mảnh vụn của vùng tai bị tổn t.hương của chó mèo
-Liều lượng và cách dùng
+Mở nắp chai, bóp nhẹ, nhỏ dung dịch vào tai
+Giữ vành tai ngửa lên để ổng tai ở vị trí thẳng đứng.
+Xoa nhẹ phần gốc tai, có thể dùng bông gòn thấm dung dịch bẩn ở phần trên của vành tai và quanh ống tai.
+Để làm sạch vệ sinh tai, loại bỏ mùi hôi tai, giúp phòng ngừa các tác nhân gây b.ệnh v.iêm tai có thể sử dụng 2-3 lần /tuần.
Làm sạch tay trước khi tiến hành các biện pháp điều t.rị hoặc nhỏ thuốc t.rị b.ệnh tai khác
-Lưu ý:
Chị dùng tại chỗ cho chó mèo thú cảnh
Tránh sản phẩm tiếp xúc trực tiếp với mắt
Để sản phẩm cách xa tầm tay trẻ em
Chống chỉ định: không dùng trong trường hợp thú bị t.hủng màng nhĩ', 75000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-23-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Găng tay chải lông rụng thú cưng', N'Găng tay chải lông rụng thú cưng
Ưu điểm sản phẩm:

Dễ dùng
Vải lưới thoáng khí và mau khô
Dây đeo băng gai dính có thể điều chỉnh phù hợp với mọi kích thước bàn tay

Công dụng:

Lấy lông rụng trên chó mèo trước khi tắm, trong thời kỳ rụng lông.
Chải lông rối, massage cho thú cưng
Kỳ lông và người cho thú cưng khi tắm
Tránh nhiệt độ cao và ánh sáng mặt trời trực tiếp để không gây biến dạng sản phẩm.

– Xuất xứ: Trung Quốc
– Tên sản phẩm: Găng tay lấy lông rụng chó mèo
– Sản xuất: Sinofiz Cat Litter Products (Dalian) Co., Ltd', 32000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Gang-tay-chai-long-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Kìm cắt móng nhỏ', N'1. Thông tin sản phẩm
☄️KỀM CẮT MÓNG với chất liệu thép không ghỉ, bền và dễ sử dụng giúp bạn dễ dàng cắt móng cho thú cưng.
☄️Làm nail cho thú cưng cũng là niềm vui của bao bạn trẻ.
☄️Việc chăm sóc bộ móng cho thú cưng cũng là cách giúp bạn bảo vệ bộ sofa trong nhà.
☄️Kềm cắt móng chất liệu thép không gỉ, bền và dễ sử dụng giúp bạn dễ dàng cắt móng cho thú cưng.
2. Hướng dẫn sử dụng
– Cầm chân thú cưng nhẹ nhàng. Cắt phần móng thừa ra theo 1 góc 45 độ. Điểm cắt cuối cùng của kềm hướng về phần cuối cùng của móng, điều này sẽ giúp bạn cắt bỏ được phần nhọn của móng chân thú cưng.
– Đẩy nhẹ mu bàn chân cún lên để lộ ra phần móng thừa và cắt bỏ. Bấm kềm nhẹ nhàng, giữ chắc tay tránh làm gãy móng.
– CẮT từng phần nhỏ của móng KHÔNG cắt 1 lần cả đoạn dài. Đặc biệt chú ý hơn với những thú cưng có móng chân tối màu.
– Sau mỗi lần cắt, bạn nhìn thật kỹ móng vừa cắt NẾU thấy phần chấm đen, đậm màu hơn ở giữa móng – là điểm bắt đầu của phần thịt / mạch máu. Đến đây thì KHÔNG cắt nữa.
– Kiểm tra xem móng của thú cưng có bị giòn không hoặc thấy móng vẫn còn sắc thì giũa móng lại cho thú cưng.
– Thưởng cho thú cưng bánh thưởng, xương thưởng, snack để giúp chó biết nó vừa hợp tác đúng mức.
**CHÍNH SÁCH CỦA PET SERVICE**
– Sản phẩm cam kết giống 100% mô tả.
– Mỗi sản phẩm khi được bán ra đều được kiểm tra cẩn thận trước khi gửi tới Quý khách.
– Hàng có sẵn, giao hàng ngay khi shop nhận được đơn.
– Hỗ trợ đổi trả, hoàn tiền đối với sản phẩm lỗi theo chính sách Shopee.
– Vui lòng quay lại video quá trình mở sản phẩm để được Pet Service hỗ trợ nhanh nhất
trong các trường hợp phát sinh vấn đề về đơn hàng.', 55000, 'https://petservicehcm.com/wp-content/uploads/2023/02/Khung-Shopee-II-6-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Lõi lăn lông quần áo – 10cm', N'Lõi lăn lông quần áo
Đặc điểm:

Kích thước: size M (lõi dài 10cm)
Lõi lăn bụi gồm cuộn giấy lăn bụi 60 tờ tiết kiệm


Cây lăn bụi dùng loại keo dán cao cấp, không độc hại, không dính lên bề mặt quần áo, sofa, gối, giường… khi lăn.
Kích cỡ nhỏ gọn, có thể mang theo khi đi du lịch, công tác…
Keo dính chắc chắn, dễ dàng loại bỏ các sợi vải thừa, lông chó mèo, bụi bẩn, phù hợp với tất cả các bề mặt.

Hướng dẫn sử dụng:

Dùng cuộn lăn bụi lăn trực tiếp trên bề mặt muốn vệ sinh.
Sau khi lăn, bóc bỏ lớp đã sử dụng.
Nếu dùng hết, bạn có thể mua lõi lăn bụi rời để thay thế rồi dùng tiếp.

Chú ý:

Cán là phần nhựa màu hồng, lõi là phần màu trắng.
Lần đầu tiên sử dụng, quý khách nên chọn phân loại “Cán + lõi” để sử dụng ngay.
Sau khi sử dụng hết lõi, quý khách giữ lại cán và mua phân loại “Lõi riêng” để thay thế.', 15000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Thiet-ke-chua-co-ten-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Lược chải lông tròn nút bấm Pet Service', N'N/A', 70000, 'https://petservicehcm.com/wp-content/uploads/2023/02/Khung-background-san-pham-shopee-mau-10.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Sữa tắm OLIVE ESSENCE giúp dưỡng lông cho chó chai 450ml', N'1. MÔ TẢ SẢN PHẨM
– Dầu tắm Olive với hương thơm dễ chịu, có nhiều công dụng khác nhau để phù hợp với thú cưng.
– Sữa tắm Olive Dưỡng lông: là sản phẩm dưỡng lông tối ưu, bổ sung các tinh chất dưỡng giúp thẩm thấu sâu vào bề mặt da & lông, mang đến cho vật nuôi một bộ lông sáng óng, mềm mượt.
2. HƯỚNG DẪN SỬ DỤNG
– Làm ướt lông vật nuôi và xoa đều dầu gội lên toàn bộ cơ thể chúng ( tránh để sản phẩm tiếp xúc trực tiếp với mắt )
– Massage cơ thể thú cưng trong 5 -10 phút
– Xả lại bằng nước sạch cho đến khi hết bọt xà phòng.
– Lặp lại quy trình trên nếu vật nuôi vẫn còn bẩn.
– Dùng khăn và máy sấy làm khô lông thú cưng.', 60000, 'https://petservicehcm.com/wp-content/uploads/2024/04/gan-2.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Sữa tắm Oliver hỗ trợ trị Nấm, Da 300ml PET SERVICE.', N'1. THÔNG TIN SẢN PHẨM.
1.1. MÔ TẢ SẢN PHẨM.
– Sữa tắm Oliver hỗ trợ về da cho chó mèo trên 8 tháng.
1.2. ƯU ĐIỂM.
– Sữa tắm Oliver hỗ trợ p.hục h.ồi da do v.i k.huẩn và n.ấm
– Sữa tắm Oliver có mùi phấn nhẹ dễ chịu.
– Thúc đẩy sự làm l.ành da, phòng n.gừa da bị khô và n.gứa.
– Oliver lý tưởng để điều t.rị da bị tróc vảy sừng và tẩy nhờn cho da bị r.ối loạn tiết nhờn.
– Điều t.rị các rối loạn ngoài da kết hợp với g.hẻ Demodex, v.iêm da tăng tiết chất nhờn, m.ụn nước trên chó mèo.
2. HƯỚNG DẪN SỬ DỤNG
– Lắc kỹ chai trước khi sử dụng. Hòa dầu gội với nước ấm.
– Làm ướt lông thú và xoa đều dầu gội lên toàn bộ cơ thể thú cưng ( tránh để dầu gội tiếp xúc với phần mắt )
– Mát xa nhẹ nhàng cơ thể thú cưng trong 5 – 10 phút
– Xả lại bằng nước ấm cho đến khi hết bọt xà phòng.
– Lặp lại quy trình nếu thú cưng vẫn còn bẩn.
– Làm khô lông thú cưng bằng khăn và máy sấy', 170000, 'https://petservicehcm.com/wp-content/uploads/2024/07/z5641413190068_cf445cb37cd2ff68b2522d6c2d02d794.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Sữa tắm Oliver hỗ trợ trị Ve, Ghẻ và Bọ Chét cho Thú Cưng chai 300ml PET SERVICE', N'1. THÔNG TIN SẢN PHẨM.
1.1. MÔ TẢ SẢN PHẨM.
– Sữa tắm Oliver hỗ trợ về da cho chó mèo trên 8 tháng.
1.2. ƯU ĐIỂM.
– Sữa tắm Oliver hỗ trợ p.hục h.ồi da do v.i k.huẩn và n.ấm
– Sữa tắm Oliver có mùi phấn nhẹ dễ chịu.
– Thúc đẩy sự làm l.ành da, phòng n.gừa da bị khô và n.gứa.
– Oliver lý tưởng để điều t.rị da bị tróc vảy sừng và tẩy nhờn cho da bị r.ối loạn tiết nhờn.
– Điều t.rị các rối loạn ngoài da kết hợp với g.hẻ Demodex, v.iêm da tăng tiết chất nhờn, m.ụn nước trên chó mèo.
2. HƯỚNG DẪN SỬ DỤNG
– Lắc kỹ chai trước khi sử dụng. Hòa dầu gội với nước ấm.
– Làm ướt lông thú và xoa đều dầu gội lên toàn bộ cơ thể thú cưng. ( tránh để dầu gội tiếp xúc với phần mắt )
– Mát xa nhẹ nhàng cơ thể thú cưng trong 5 – 10 phút – Xả lại bằng nước ấm cho đến khi hết bọt xà phòng.
– Lặp lại quy trình nếu thú cưng vẫn còn bẩn.
– Làm khô lông thú cưng bằng khăn và máy sấy.', 170000, 'https://petservicehcm.com/wp-content/uploads/2024/07/z5641413190017_9ccfcea0192d24e9e720b16bfae621da.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (6, N'Sữa tắm Yú', N'🎀🎀🎀SỮA TẮM YU CHO CHÓ MÈO – GIỮ MÙI THƠM LÂU- 400ml  🎀🎀🎀
Dòng sản phẩm Yu, dầu tắm độc đáo cho chó mèo từ các loại hương hoa và thảo mộc phương đông, nâng niu làn da nhạy cảm, giúp bộ lông thú cưng trở nên mềm mượt, thơm quyến rũ đến không ngờ.
🌺SỮA TẮM YU HƯƠNG HOA LAN với chất chống oxi hóa nhầm phục hồi cân bằng độ ẩm và làm trẻ hóa làn da của thú cưng, Hương thơm quý phải, mạnh mẽ
🌺SỮA TẮM YU HƯƠNG HOA TRÀ giúp sợi lông chắc khỏe từ bên trong, hương thơm đặc biệt quyến rũ
🌺SỮA TẮM YU CHO LÔNG TRẮNG với các dưỡng chất làm sạch, loại bỏ lớp tích tụ dưới lông, mang lại vẻ đẹp tự nhiên cuarbooj lông trắng, làm cho chúng sáng bóng và mềm mịn.
Thành phần: Chinese bellflower essence + Witch Hazel + Allantoin + Vitamin B6 + Epilobium fleischeri extrack
🌺SỮA TẮM YU HƯƠNG HOA ANH ĐÀO hương thơm quyến rũ! Chiết xuất hoa anh đào nhẹ dịu làm giảm kích ứng, giúp làn da của thú cưng trở nên mềm mại và thoáng mát
🌺SỮA TẮM YU HƯƠNG HOA SEN được chiết xuất từ tinh chất hoa sen, đây quà tặng đặc biệt cho các bé bị rối vón lông, với công thức tạo hình 3D dành cho tất cả các giống chó mèo gồm 5 ưu điểm đặc biệt:
✅ Làm cho lông của thú cưng xù bông hơn, dễ tạo kiểu khi cắt tỉa lông.
✅ Loại bỏ triệt để hiện tượng thắt nút lông, lông rối, lông vón cục.
✅ Lưu lại mùi hương thơm mát bền lâu.
✅ 100% từ nguyên liệu tự nhiên với những hạt axit lactic Niosome, kích thích các nang lông hoạt động hiệu quả, khiến cho lông bông xù tự nhiên, mang lại một bộ lông tuyệt đẹp.
Xuất xứ: Đài Loan', 390000, 'https://petservicehcm.com/wp-content/uploads/2023/02/bat-an-inox-11-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Chân Gà Sấy PAWO By PET SERVICE 100g', N'Sản phẩm độc quyền, treat sấy khô PAWO Chân Gà:
Treat chân gà sấy khô PAWO là món ăn vặt lý tưởng cho chó mèo, vừa hấp dẫn vừa giàu dinh dưỡng với nhiều lợi ích cho sức khỏe. Chân gà sấy giảm mùi tanh nhưng vẫn giữ được vị ngon tự nhiên của chân gà tươi, kích thích vị giác của thú cưng. Chân gà sấy khô PAWO cung cấp lượng protein dồi dào, cùng với canxi và collagen, giúp hỗ trợ sức khỏe xương khớp, răng miệng, và lông mượt.
Không chỉ cung cấp dinh dưỡng, món treat này còn giúp thú cưng nhai gặm, giảm căng thẳng, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả. Đặc biệt, sản phẩm không chứa chất bảo quản, an toàn và vệ sinh tuyệt đối cho chó mèo.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi trở lên. Chỉ dùng làm món ăn vặt, không vượt quá 10% khẩu phần ăn hàng ngày.', 86000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m0ppg5efk32ndc-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Chân Gà Sấy PAWO By PET SERVICE 50g', N'Sản phẩm độc quyền, treat sấy khô PAWO Chân Gà:
Treat chân gà sấy khô PAWO là món ăn vặt lý tưởng cho chó mèo, vừa hấp dẫn vừa giàu dinh dưỡng với nhiều lợi ích cho sức khỏe. Chân gà sấy giảm mùi tanh nhưng vẫn giữ được vị ngon tự nhiên của chân gà tươi, kích thích vị giác của thú cưng. Chân gà sấy khô PAWO cung cấp lượng protein dồi dào, cùng với canxi và collagen, giúp hỗ trợ sức khỏe xương khớp, răng miệng, và lông mượt.
Không chỉ cung cấp dinh dưỡng, món treat này còn giúp thú cưng nhai gặm, giảm căng thẳng, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả. Đặc biệt, sản phẩm không chứa chất bảo quản, an toàn và vệ sinh tuyệt đối cho chó mèo.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi trở lên. Chỉ dùng làm món ăn vặt, không vượt quá 10% khẩu phần ăn hàng ngày.', 53000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m0ppg5efk32ndc-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Chóp Cánh Gà sấy khô PAWO By PET SERVICE 100g', N'Sản phẩm độc quyền, treat sấy khô PAWO Chóp Cánh Gà:
Treat chóp cánh gà sấy khô PAWO là món ăn vặt thơm ngon, được yêu thích bởi chó mèo với nhiều lợi ích sức khỏe. Sản phẩm không chỉ giảm mùi hôi khó chịu mà còn giữ lại hương vị tự nhiên, hấp dẫn của chóp cánh gà tươi.
Lợi ích dinh dưỡng: Chóp cánh gà sấy khô PAWO chứa hàm lượng protein cao, cung cấp dinh dưỡng thiết yếu cho thú cưng. Bên cạnh đó, sản phẩm còn bổ sung collagen và canxi, hỗ trợ sức khỏe xương khớp và phát triển toàn diện cho bé yêu.
Tăng cường sức khỏe: Treat này giúp cải thiện sức khỏe răng miệng, làm mượt lông và chắc khỏe xương khớp. Việc nhai gặm cũng giúp thú cưng giảm căng thẳng, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả.
Đặc điểm nổi bật:

Không chứa chất bảo quản
An toàn và vệ sinh tuyệt đối cho thú cưng

Hướng dẫn sử dụng: Chóp cánh gà sấy khô PAWO phù hợp cho chó mèo từ 3 tháng tuổi trở lên. Chỉ nên dùng làm món ăn vặt, không vượt quá 10% khẩu phần ăn hàng ngày.', 109000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7r98o-lwguteubxcyhf5-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Heo Phi Lê sấy khô PAWO By PET SERVICE 100g', N'Treat phi lê heo sấy khô là món ăn vặt hấp dẫn, được yêu thích bởi chó mèo nhờ vào hương vị thơm ngon và nhiều lợi ích cho sức khỏe. Sản phẩm giảm mùi hôi khó chịu nhưng vẫn giữ được vị ngọt tự nhiên của thịt heo tươi.
Phi lê heo sấy khô PAWO giữ hàm lượng protein cao, đảm bảo cung cấp dinh dưỡng thiết yếu cho thú cưng. Sản phẩm còn bổ sung collagen, canxi và Omega 3, hỗ trợ sức khỏe xương khớp và tăng cường sức khỏe tổng thể cho bé yêu.
Ngoài ra, treat này giúp cải thiện sức khỏe răng miệng, làm mượt lông và chắc khỏe xương khớp. Việc nhai gặm phi lê heo còn giúp giảm stress, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả.
Đặc biệt, sản phẩm không chứa chất bảo quản, đảm bảo an toàn và vệ sinh tuyệt đối cho thú cưng của bạn. Phù hợp cho chó mèo từ 3 tháng tuổi trở lên, chỉ nên dùng làm món ăn vặt và không vượt quá 10% khẩu phần ăn hàng ngày.', 130000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m1asi289opp8c4.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Heo Phi Lê sấy khô PAWO By PET SERVICE 50g', N'Treat phi lê heo sấy khô là món ăn vặt hấp dẫn, được yêu thích bởi chó mèo nhờ vào hương vị thơm ngon và nhiều lợi ích cho sức khỏe. Sản phẩm giảm mùi hôi khó chịu nhưng vẫn giữ được vị ngọt tự nhiên của thịt heo tươi.
Phi lê heo sấy khô PAWO giữ hàm lượng protein cao, đảm bảo cung cấp dinh dưỡng thiết yếu cho thú cưng. Sản phẩm còn bổ sung collagen, canxi và Omega 3, hỗ trợ sức khỏe xương khớp và tăng cường sức khỏe tổng thể cho bé yêu.
Ngoài ra, treat này giúp cải thiện sức khỏe răng miệng, làm mượt lông và chắc khỏe xương khớp. Việc nhai gặm phi lê heo còn giúp giảm stress, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả.
Đặc biệt, sản phẩm không chứa chất bảo quản, đảm bảo an toàn và vệ sinh tuyệt đối cho thú cưng của bạn. Phù hợp cho chó mèo từ 3 tháng tuổi trở lên, chỉ nên dùng làm món ăn vặt và không vượt quá 10% khẩu phần ăn hàng ngày.', 76000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m1asi289opp8c4.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – Đồ gặm da bò cho chó nhỏ và vừa dạng xoắn trung size M.1 – 30 gram', N'Da bò dạng xoắn trung cho chó nhỏ, chó vừa hoặc làm phần thưởng.
√ 100% Da bò tự nhiên
√ Làm hoàn toàn thủ công, không hóa chất và sấy khô ở nhiệt độ thích hợp giúp giữ nguyên dưỡng chất.
√ Cấu tạo xoắn giống như một chiếc bàn chải giúp nhẹ nhàng len sâu chải sạch từng kẽ răng và quanh viền nướu.
√ Collagen trong da bò giúp lông cún bóng mượt, khỏe mạnh, hạn chế rụng lông, làm chậm sự lão hóa của các tế bào cơ thể
√ Dễ tiêu hóa.
√ Món quà thơm ngon thỏa mãn tập tính gặm nhai của cún yêu.', 75000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-6.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – Đồ gặm da bò cho chó nhỏ, tập nhai size S.2 – 30 gram', N'TELLME® Da bò dạng xoắn cho chó nhỏ, tập nhai
√ 100% Da bò tự nhiên
√ Làm hoàn toàn thủ công, không hóa chất và sấy khô ở nhiệt độ thích hợp giúp giữ nguyên dưỡng chất.
√ Độ dai vừa phải giúp cún mài răng và tránh gặm nhấm đồ đạc, hỗ trợ thay răng dễ dàng.
√ Collagen trong da bò giúp lông cún bóng mượt, khỏe mạnh, hạn chế rụng lông, làm chậm sự lão hóa của các tế bào cơ thể
√ Dễ tiêu hóa.
√ Phù hợp cho các bé từ 02 tháng tuổi.', 75000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-4.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – Đồ gặm da bò cho chó nhỏ, vừa dạng xoắn trung size M.3 – 90 gram', N'TELLME® Da bò dạng xoắn trung cho chó nhỏ, chó vừa hoặc làm phần thưởng.
√ 100% Da bò tự nhiên
√ Làm hoàn toàn thủ công, không hóa chất và sấy khô ở nhiệt độ thích hợp giúp giữ nguyên dưỡng chất.
√ Cấu tạo xoắn giống như một chiếc bàn chải giúp nhẹ nhàng len sâu chải sạch từng kẽ răng và quanh viền nướu.
√ Collagen trong da bò giúp lông cún bóng mượt, khỏe mạnh, hạn chế rụng lông, làm chậm sự lão hóa của các tế bào cơ thể
√ Dễ tiêu hóa.
√ Món quà thơm ngon thỏa mãn tập tính gặm nhai của cún yêu.', 166000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-5.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – Đồ gặm da bò cho chó vừa, lớn dạng gậy lớn size L.1 – 80 gram', N'TELLME® Da bò dạng gậy lớn dành riêng cho chó vừa, chó lớn hoặc chó có cấu tạo hàm đặc biệt.
√ 100% Da bò tự nhiên
√ Làm hoàn toàn thủ công, không hóa chất và sấy khô ở nhiệt độ thích hợp giúp giữ nguyên dưỡng chất.
√ Giúp răng chắc khỏe và sạch sẽ
√ Collagen trong da bò giúp lông cún bóng mượt, khỏe mạnh, hạn chế rụng lông, làm chậm sự lão hóa của các tế bào cơ thể
√ Dễ tiêu hóa.
√ Món ăn thơm ngon, dinh dưỡng thỏa mãn tập tính gặm nhai tự nhiên của chó.', 159000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-7.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – vị Cá Ngừ gói 40g', N'TELLME Cá ngừ khô cho chó chỉ với một thành phần: 100% cá ngừ đại dương tự nhiên nguyên chất, Ít chất béo và giàu protein, sản phẩm Cá ngừ khô TELLME hoàn toàn tự nhiên và có giá trị dinh dưỡng cao, không chất bảo quản hoặc chất độc ảnh hưởng đến hệ tiêu hóa của thú cưng của bạn.
Nguồn cung cấp tuyệt vời vitamin A, vitamin B, sắt, đồng, phốt pho và kẽm, cũng như các axit béo thiết yếu.
Nguồn cung cấp axit béo Omega 3 và Omega 6 tuyệt vời: Các axit béo này không chỉ tăng cường hệ miễn dịch hoạt động đầy đủ mà còn giúp da và lông của cún yêu khỏe mạnh và sáng bóng.
Hương vị thơm ngon, hấp dẫn, sản phẩm Cá ngừ khô TELLME, rất phù hợp được sử dụng làm phần thưởng, dùng cho công việc huấn luyện chó.', 75000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-2.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – vị Da bò hình nơ – gói 50g', N'TELLME® Da bò dạng xương nơ dành cho chó nhỏ, chó vừa hoặc làm phần thưởng.
√ 100% Da bò tự nhiên
√ Làm hoàn toàn thủ công, không hóa chất và sấy khô ở nhiệt độ thích hợp giúp giữ nguyên dưỡng chất.
√ Giúp răng chắc khỏe và sạch sẽ
√ Collagen trong da bò giúp lông cún bóng mượt, khỏe mạnh, hạn chế rụng lông, làm chậm sự lão hóa của các tế bào cơ thể
√ Dễ tiêu hóa.
√ Món ăn thơm ngon, dinh dưỡng thỏa mãn tập tính gặm nhai tự nhiên của chó.', 120000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-8.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – vị Da heo – gói 130g', N'TELLME® Thanh gặm da heo được làm từ 100% da heo tươi, hoàn toàn an toàn như một món ăn tự nhiên cho chó.
Da heo giàu collagen và chất keo protein giúp cho da, lông và các khớp khỏe mạnh.
Tốt cho răng. Với hành động gặm thanh da heo giúp loại bỏ mảng bám trên răng, làm sạch răng một cách tự nhiên, một giải pháp thay thế tuyệt vời, tự nhiên và không ngũ cốc (grain free) cho các sản phẩm truyền thống.
Sản phẩm được đóng gói trong túi giấy có zíp an toàn với môi trường, rất tiện lợi bảo quản khi túi được mở, chưa dùng hết.
Không chứa chất tạo màu, hương vị và chất bảo quản.
Cún yêu của bạn sẽ hoàn toàn hài lòng với sản phẩm Thanh gặm da heo TELLME.', 102000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – vị Gan bò sấy – gói 75g', N'TELLME® Gan bò sấy cho chó mèo được lựa chọn từ nguồn nguyên liệu gan bò tươi ngon và sấy bằng công nghệ sấy hiện đại giúp chó mèo:

Tiêu hóa dễ dàng, chỉ với một thành phần 100% gan bò tự nhiên.
Ít chất béo và giàu protein, sản phẩm Gan bò sấy hoàn toàn tự nhiên và có giá trị dinh dưỡng cao, không chất bảo quản hoặc chất độn ảnh hưởng đến hệ tiêu hóa của thú cưng của bạn.
Nguồn cung cấp tuyệt vời vitamin A, sắt, đồng, phốt pho và kẽm cũng như các Vitamins B và các axit béo thiết yếu, rất cần thiết cho thú cưng.
Nguồn cung cấp axit béo Omega 3 và Omega 6 tuyệt vời: Các axit béo này không chỉ tăng cường hệ miễn dịch hoạt động đầy đủ mà còn giúp da và lông của chó mèo khỏe mạnh và sáng bóng.
Hương vị thơm ngon, hấp dẫn, sản phẩm Gan bò sấy, rất phù hợp được sử dụng làm phần thưởng, dùng cho công việc huấn luyện chó.', 55000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-3.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Snack Tell Me cho chó – Xúc xích khô Thịt Heo và Cà rốt tươi dạng thanh gặm – 85 gram', N'Thực phẩm giàu Protein và chất xơ, ít béo tốt cho tiêu hóa, giúp cún yêu hạn chế tiêu chảy.
Cà rốt giàu Biotin, vitamin A, tốt cho răng và mắt, giúp lông mềm mượt.
Thanh gặm khuyến khích cún yêu nhai giúp loại bỏ mảng bám có hại và cao răng giúp cải thiện về sức khỏe răng miệng.
Tăng cường sự khéo léo của chân.
Hạn chế tối đa mùi hôi của chất thải.
Món ăn thưởng thơm ngon khiến cún không ngừng quẫy đuôi', 88000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-9.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Ức gà sấy Pawo by Pet Service 100g', N'Sản phẩm độc quyền, treat sấy khô Pawo:
– Treat sấy khô là món ăn vặt yêu thích của các bé với nhiều lợi ích tuyệt vời cho sức khỏe
– Treat sấy khô giảm mùi tanh khó chịu nhưng vẫn giữ được vị hấp dẫn của thịt tươi
– Treat sấy khô Pawo giữ được hàm lượng protein lên tới hơn 80%
– Cung cấp protein, chất xơ, canxi và Omega 3 cho bé ngay trong khẩu phần hàng ngày.
– Giúp làm mượt lông, chắc khỏe xương khớp và răng.
– Kích thích các bé nhai gặm, hỗ trợ giảm stress, luyện hàm và vệ sinh răng miệng hiệu quả.
Đặc biệt: Không chất bảo quản, an toàn, đảm bảo vệ sinh cho bé.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi. Chỉ cho ăn vặt, không vượt quá 10% khẩu phần hàng ngày.', 105000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7r98o-lwguteubujtl7f-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (15, N'Ức gà sấy Pawo by Pet Service 50g', N'Sản phẩm độc quyền, treat sấy khô Pawo:
– Treat sấy khô là món ăn vặt yêu thích của các bé với nhiều lợi ích tuyệt vời cho sức khỏe
– Treat sấy khô giảm mùi tanh khó chịu nhưng vẫn giữ được vị hấp dẫn của thịt tươi
– Treat sấy khô Pawo giữ được hàm lượng protein lên tới hơn 80%
– Cung cấp protein, chất xơ, canxi và Omega 3 cho bé ngay trong khẩu phần hàng ngày.
– Giúp làm mượt lông, chắc khỏe xương khớp và răng.
– Kích thích các bé nhai gặm, hỗ trợ giảm stress, luyện hàm và vệ sinh răng miệng hiệu quả.
Đặc biệt: Không chất bảo quản, an toàn, đảm bảo vệ sinh cho bé.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi. Chỉ cho ăn vặt, không vượt quá 10% khẩu phần hàng ngày.', 55000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7r98o-lwguteubujtl7f-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (16, N'Hạt Ganador Adult cho chó trưởng thành vị Bò gói 3kg', N'1. ĐẶC ĐIỂM SẢN PHẨM
Thức ăn cho chó Ganador Thịt Bò là sản phẩm của thương hiệu Ganador, một thương hiệu thức ăn cho chó nổi tiếng đến từ Pháp. Sản phẩm được thiết kế dành cho chó trưởng thành trên 12 tháng tuổi, với công thức dinh dưỡng cân bằng, giúp chó phát triển khỏe mạnh và năng động.
2. CÔNG DỤNG SẢN PHẨM
– Giúp chó phát triển khỏe mạnh: Sản phẩm cung cấp đầy đủ các chất dinh dưỡng cần thiết cho sự phát triển của chó, giúp chó có thể phát triển khỏe mạnh về thể chất và tinh thần.
– Tăng cường sức đề kháng: Các vitamin và khoáng chất trong sản phẩm giúp tăng cường sức đề kháng, giúp chó chống lại các tác nhân gây bệnh.
– Giúp chó duy trì năng lượng: Sản phẩm cung cấp năng lượng cần thiết cho chó hoạt động cả ngày.
– Giúp chó có bộ lông khỏe mạnh: Omega 3 và 6 trong sản phẩm giúp da và lông chó khỏe mạnh, mềm mượt.', 150000, 'https://petservicehcm.com/wp-content/uploads/2023/12/Pot-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (16, N'Hạt Ganador Adult vị Gà gói 3kg', N'Hạt Ganador Adult vị Gà nướng 3KG
Mô tả sản phẩm:
– Ganador là nhãn hiệu thức ăn cho chó cưng được sản xuất bởi Tập đoàn Neovia với gần 60 năm kinh nghiệm trong lĩnh vực dinh dưỡng và chăm sóc thú cưng. Công thức sản phẩm là tâm huyết nghiên cứu của các chuyên gia dinh dưỡng vật nuôi hàng đầu tại Pháp. Ganador cung cấp cho chó cưng hàm lượng dinh dưỡng cân bằng và đầy đủ nhất, giúp chúng luôn khỏe mạnh và năng động. Sản phẩm được sản xuất từ những nguyên liệu chất lượng cao, tuân thủ nghiêm ngặt hệ thống tiêu chuẩn quốc tế (AAFCO).
Đặc điểm nổi bật:

Hạt Ganador vị gà nướng Adult giúp tăng đề kháng, hỗ trợ tiêu hóa cho chó trưởng thành.
Bổ sung Omega 3 và 6 cho làn da và bộ lông khỏe mạnh.
Công thức cân bằng protein và khoáng chất, giúp xương và cơ chắc khỏe.
Chiết xuất Yucca Schidigera kiểm soát mùi hôi chất thải.

Thành phần:
6 khoáng chất , 9 loại Vitamin, Canxi D, chiết xuất Yucca Schidigera, ngũ cốc nguyên hạt, bột gia cầm, bã nành, cám lúa mì, mỡ gia cầm, hương gà nướng
Dinh dưỡng

ĐẠM THÔ (Tối thiểu) 23.0%
BÉO THÔ (Tối thiểu) 10.0%
XƠ THÔ (Tối đa) 4.0%
ĐỘ ẨM (Tối đa) 12.0%', 150000, 'https://petservicehcm.com/wp-content/uploads/2022/08/Khung-Shopee-50-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (16, N'Hạt Ganador cho chó vị cừu gói 1,5kg', N'1. ĐẶC ĐIỂM SẢN PHẨM
– Thức ăn Hỗn Hợp Ganador Thịt Cừu được chế biến đặc biệt cho thú cưng của bạn nhằm đảm bảo một chế độ dinh dưỡng toàn diện và cân bằng.
– Bổ sung Vitamin E và Selen giúp tăng cường hệ thống miễn dịch
– Không chứa hương vị nhân tạo
– Cung cấp năng lượng, Vitamin & khoáng, Omega 3&6
– Phân rắn và ít mùi
2. THÀNH PHẦN
Gạo, lúa mì, bột thịt gia cầm, bã nành, mỡ gia cầm (nguồn tự nhiên của Omega 3&6), bột thịt cừu, khoáng chất (Sắt, Đồng, Mangan, Kẽm, Iốt, Selen), các vitamin (A, D3, K3, B1, B2, B6, B12, PP, E (Tocopherol), Calcium D-Pantothenate, Biotin, Axit folic, Choline), Dicalcium Phosphate, Calcium Carbonate, muối, chất bảo quản, chất chống oxi hóa, chất làm ngon miệng, chiết xuất Yucca Schidigera.



ĐẠM THÔ (Tối thiểu)
21.0%


BÉO THÔ (Tối thiểu)
6.0%


XƠ THÔ (Tối đa)
12.0%


ĐỘ ẨM (Tối đa)
12.0%', 90000, 'https://petservicehcm.com/wp-content/uploads/2023/02/gan.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (16, N'Hạt Ganador Puppy Vị Sữa & DHA gói 3kg', N'Hạt Ganador Puppy Vị Sữa & DHA 3KG
Mô tả sản phẩm:
– Ganador là nhãn hiệu thức ăn cho chó cưng được sản xuất bởi Tập đoàn Neovia với gần 60 năm kinh nghiệm trong lĩnh vực dinh dưỡng và chăm sóc thú cưng. Công thức sản phẩm là tâm huyết nghiên cứu của các chuyên gia dinh dưỡng vật nuôi hàng đầu tại Pháp. Ganador cung cấp cho chó cưng hàm lượng dinh dưỡng cân bằng và đầy đủ nhất, giúp chúng luôn khỏe mạnh và năng động. Sản phẩm được sản xuất từ những nguyên liệu chất lượng cao, tuân thủ nghiêm ngặt hệ thống tiêu chuẩn quốc tế (AAFCO).
Đặc điểm nổi bật:

Hạt Ganador Puppy Vị Sữa & DHA giúp hoàn thiện hệ tiêu hóa và đề kháng cho chó đang phát triển
Bổ sung Omega 3 và 6 theo tỷ lệ hợp lý giúp tạo bộ lông khỏe mạnh cho chó con
Công thức cân bằng protein và khoáng chất, giúp xương và cơ chó con chắc khỏe.
Chiết xuất Yucca Schidigera kiểm soát mùi hôi chất thải.

Thành phần:
6 khoáng chất , 9 loại Vitamin, Canxi D, Các Axit amin thiết yếu, chiết xuất Yucca Schidigera, các sản phẩm về sữa, Gluten lúa mì, bột Thịt và Xương, ngũ cốc nguyên hạt, bột gia cầm, bã nành, cám lúa mì, mỡ gia cầm, hương sữa
Dinh dưỡng

ĐẠM THÔ (Tối thiểu) 23.0%
BÉO THÔ (Tối thiểu) 10.0%
XƠ THÔ (Tối đa) 4.0%
ĐỘ ẨM (Tối đa) 12.0%', 228000, 'https://petservicehcm.com/wp-content/uploads/2022/08/Khung-Shopee-49-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (16, N'REFLEX SKIN CARE – Thức ăn khô cho mèo chăm sóc, tái tạo và phục hồi lông da', N'ĐẶC ĐIỂM SẢN PHẨM
Reflex Plus Skin Care With Salmon
Thức ăn khô cho Mèo chăm sóc phục hồi và tái tạo lông da
Xuất xứ: Thổ Nhĩ Kỳ
CÔNG DỤNG SẢN PHẨM

Giúp chăm sóc phục hồi và tái tạo lông da
Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi.
Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
Xylo-oligosaccharides (XOS) giúp giảm các vấn đề như chất béo không mong muốn và đường huyết
Cải thiện quá trình tiêu hóa và chuyển hóa thức ăn bằng cách cải thiện hệ vi khuẩn đường ruột
Bổ sung Vitamin A, D3, E, C và khoáng chất

HƯỚNG DẪN BẢO QUẢN
Bảo quản nơi khô ráo, tránh tiếp xúc trực tiếp ánh nắng mặt trời.', 252000, 'https://petservicehcm.com/wp-content/uploads/2023/08/bat-an-inox-86-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (16, N'Thức ăn hạt cao cấp dành cho chó Pro Pet Grand Magic túi 1kg', N'1. ĐẶC ĐIỂM SẢN PHẨM
Hạt Pro-Pet Grandmagic được làm từ những nguyên liệu tự nhiên như thịt bò, gà và hải sản, đem đến cho cún cưng nguồn Protein chất lượng. Đây là dòng sản phẩm cao cấp đến từ thương hiệu Việt Nam Pro-Pet, được rất nhiều người tin dùng.
Không chỉ bổ sung các chất đa lượng như Protein hay tinh bột, hạt Grandmagic còn có rất nhiều chất khoáng vi lượng. Với lượng Vitamin và chất chống Oxi hóa mạnh, thú cưng sẽ có tuổi thọ được kéo dài hơn, khỏe mạnh hơn. Bên cạnh đó, lượng Canxi vượt trội cũng sẽ giúp xây dựng hệ xương và răng cún chắc khỏe.
2. CÔNG DỤNG SẢN PHẨM

Protein từ các nguồn thực phẩm tự nhiên giúp cún phát triển nhanh chóng
Bổ sung Canxi vượt trội giúp xương và răng cún chắc khỏe. Từ đó, các bé sẽ tránh được những vấn đề như hạ bàn, tai cụp, còi xương…
Thiết kế hạt dễ tiêu hóa, giúp loại bớt mảng bám trong quá trình nhai
Nhiều Vitamin và khoáng chất giúp cún khỏe mạnh, đề kháng tốt.
Thương hiệu Việt Nam với mức giá cực kỳ hợp lý', 100000, 'https://petservicehcm.com/wp-content/uploads/2023/12/Khung-background-san-pham-shopee-mau-8.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (17, N'Hũ BETA AMIN ECOPETS bổ sung dinh dưỡng tăng đề kháng tránh bệnh vặt tăng cân khoẻ tránh GBC ở mèo – 50g PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
Bột Dinh Dưỡng Tăng Đề Kháng Beta Amin cho chó mèo.
1.2. ƯU ĐIỂM
– Beta Amin giúp tăng cường hệ miễn dịch, phòng ngừa các bệnh vi khuẩn, virus.
– Chứa protein và axit amin thiết yếu, hỗ trợ sức khỏe toàn diện cho thú cưng.
– Tăng khả năng miễn dịch, bảo vệ thú cưng khỏi các bệnh truyền nhiễm nguy hiểm.
+ HƯỚNG DẪN SỬ DỤNG
– Trộn Beta Amin cùng thức ăn hạt, pate hoặc cho thú cưng ăn trực tiếp.
– Để đạt hiệu quả tối đa, có thể sử dụng gấp đôi liều lượng khi thú cưng đang bị bệnh hoặc vi khuẩn xâm nhập.
– Sử dụng đều đặn để bảo vệ sức khỏe lâu dài cho thú cưng.', 120000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-1.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Cá ngừ xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Cá Ngừ xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 40000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-3.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Cá ngừ xay rau củ HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Cá Ngừ xay rau củ HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 77000, 'https://petservicehcm.com/wp-content/uploads/2025/05/gan-3-600x600-1.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Pate gan heo TELLME cho chó', N'Pate gan heo TELLME cho chó
CÔNG DỤNG
• 100% làm từ nguồn nguyên liệu chất liệu cao như thịt bò tươi hoặc thịt gà tươi và phomai hảo hạng đi kèm rau củ tươi và Vitamin D, E, Omega 3, Omega 6, nước hầm xương tạo nên nước sốt Tellme đầy dinh dưỡng.
• Omega 3 và 6 giúp bảo vệ da lông toàn diện, làm giảm dấu hiệu của lão hoá, trẻ hoá các tế bào . Hồi phục các vùng bị thương nhanh chóng. Thúc đẩy quá trình trao đổi chất. Đồng thời hỗ trợ hệ tiêu hoá ổn định.
• Vitamin D, E giúp chắc khoẻ xương. Ngăn sự lão hoá của các tế bào và dây thần kinh.
• Protein từ thịt bò hay thịt gà đảm bảo cho thú cưng 1 sức khoẻ toàn diện.
THÀNH PHẦN
Thịt Bò, thịt ức gà, pho mai, nước hầm xương, vitamin D, E, Omega 3, Omega 6, canxi chiết xuất từ vỏ trứng
CÁCH DÙNG
•Cho cún yêu ăn trực tiếp hoặc trộn với cơm/ hạt.
•Ngon hơn khi hâm nóng. Bảo quản mát sau khi mở túi.
•Cho ăn từ 1-2 túi/ ngày, phụ thuộc vào trọng lượng và mức độ hoạt động của cún yêu.
ĐÓNG GÓI: gói 130g', 18000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Khung-Shopee-51-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Pate Ganador cho chó vị gà Salad gói 120g – PET SERVICE', N'Pate Ganador vị gà salad là lựa chọn hoàn hảo cho bữa ăn dinh dưỡng của chó, với hương vị gà tươi ngon kết hợp cùng salad hấp dẫn. Sản phẩm được thiết kế để cung cấp nguồn protein dồi dào giúp phát triển cơ bắp, đồng thời hỗ trợ sức khỏe toàn diện cho chó ở mọi độ tuổi.
Thành phần của pate giàu dinh dưỡng với vitamin và khoáng chất cần thiết, giúp tăng cường hệ miễn dịch, hỗ trợ tiêu hóa tốt hơn và mang lại bộ lông mượt mà, xương khớp chắc khỏe. Đặc biệt, sản phẩm không chứa chất bảo quản hay phụ gia độc hại, đảm bảo an toàn tuyệt đối cho thú cưng của bạn.
Phù hợp cho chó từ 2 tháng tuổi trở lên, Pate Ganador có thể được sử dụng như bữa ăn chính hoặc kết hợp trong khẩu phần hàng ngày. Với công thức cân bằng, sản phẩm giúp cung cấp đầy đủ năng lượng cho chó trong suốt cả ngày.', 25000, 'https://petservicehcm.com/wp-content/uploads/2024/10/gan-10.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Pate Tell Me Creamy sốt kem cho chó', N'SỐT TELLME DÀNH CHO CHÓ
• Bảo vệ da lông toàn diện, làm giảm dấu hiệu của lão hoá, trẻ hoá các tế bào
• Giúp chắc khoẻ xương
• Đảm bảo cho thú cưng 1 sức khoẻ toàn diện.
• Thúc đẩy quá trình trao đổi chất. Đồng thời hỗ trợ hệ tiêu hoá ổn định.
• Cung cấp 1 lượng chất xơ tự nhiên đồng thời cũng giàu vitamin và chất dinh dưỡng thiết yếu', 18000, 'https://petservicehcm.com/wp-content/uploads/2023/11/Pawo-chan-7-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Raw hỗn hợp HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service.', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Raw hỗn hợp xay rau HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá thịt tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 35000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Raw hỗn hợp HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Raw hỗn hợp xay rau HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 64000, 'https://petservicehcm.com/wp-content/uploads/2025/05/gan-600x600-1.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Súp thưởng Ciao Churu túi 4 thanh nhiều mùi vị', N'Có các mùi vị:

Ức gà
Cá ngừ sò điệp
Gà
Súp thịt ức gà
Cá ngừ
Cá ngừ hải sản
Cá ngừ gà
Cá ngừ cua

1. CÔNG DỤNG SẢN PHẨM
– Kích thích sự thèm ăn.
– Dạng soup dễ tiêu hóa và hấp thụ, bổ sung thêm nước giúp hạn chế tối đa các bệnh về sỏi thận, tiết niệu…
– Giàu taurine và chất dinh dưỡng khác giúp mắt mèo sáng hơn, lông bóng mượt hơn.
– Rất nhiều chất dinh dưỡng và các chất vi lượng giúp xương chắc khỏe, tăng sức đề kháng.
– Công thức chế biến ít chất béo giúp mèo duy trì cơ thể lý tưởng.
2. CHỈ TIÊU THÀNH PHẦN
Độ ẩm 85% (tối đa), đạm thô 5% (tối thiểu), năng lượng trao đổi (me) 500kcal/kg (tối thiểu), xơ thô 2,5% (tối đa), canxi 0,1-0,6% (tối thiểu-tối đa), phốt pho tổng số 0,5-2,5% (tối thiểu-tối đa), li-zin tổng số 0,14% (toois thiểu), methionine và cysteine tổng số 0,08% (tối thiểu), tro thô 2,3% (tối đa), chất béo tổng số 2% (tối thiểu), kháng sinh (mg/kg) – không có
3. BẢO QUẢN: bảo quản sản phẩm ở nởi khô ráo thoáng mát, tránh tiếp xúc trực tiếp với ánh sang mặt trời. Bảo quản trong ngăn mát tủ lạnh sau khi mở và sử dụng trong vòng 24h', 30000, 'https://petservicehcm.com/wp-content/uploads/2024/02/Shopee-94-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Bò xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service.', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Bò xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt bò tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 45000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-5.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Bò xay rau củ HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service.', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Bò xay rau củ HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt bò tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 90000, 'https://petservicehcm.com/wp-content/uploads/2025/05/gan-5-600x600-1-e1747217897859.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Chim cút xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Chim cút xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
– Lành tính, phù hợp với các bé bị viêm da, nấm, ngứa.
– Được làm từ 100% nguyên liệu tự nhiên từ thịt chim cút và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Bổ sung Vitamin A, B1, B2 và các khoáng chất cần thiết.
– Rau củ hỗ trợ bổ sung chất xơ cho thú cưng hiệu quả.
– Dễ dàng chế biến, hương vị thơm ngon phù hợp với mọi thể trạng của thú cưng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông thịt bằng cách ngâm vào nước 30 phút hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt đôi cây thịt cần chết biến hoặc cắt phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chính đều.
– Bước 6: Để nguội và cho các “Boss” thưởng thức.
3. BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
4. LƯU Ý
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không đun lại sản phẩm 2,3 lần.', 48000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-9-1.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Chim cút xay rau củ HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Chim cút xay rau củ HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
•  Lành tính, phù hợp với các bé bị viêm da, nấm, ngứa.
•  Được làm từ 100% nguyên liệu tự nhiên từ thịt chim cút và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 82000, 'https://petservicehcm.com/wp-content/uploads/2025/05/gan-9-1-600x600-2.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt gà dinh dưỡng HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt gà dinh dưỡng HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt gà tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường, mờ, sai.', 25000, 'https://petservicehcm.com/wp-content/uploads/2025/05/z6601177752139_b93fa4de422a145af732d0e7baa051ff-800x762.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt gà dinh dưỡng HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
Thịt gà dinh dưỡng HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ gà tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pet.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 45000, 'https://petservicehcm.com/wp-content/uploads/2025/05/z6601177752139_b93fa4de422a145af732d0e7baa051ff-800x762.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Nạc Gà nguyên bản HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
• Thịt nạc Gà nguyên bản HG Food cho thú cưng 400g
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 25000, 'https://petservicehcm.com/wp-content/uploads/2025/05/z6601177753457_db1212e4e47b8069de9d3ffec98aea5e-800x757.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt nạc gà nguyên bản HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt nạc gà nguyên bản HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt gà tươi.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 35000, 'https://petservicehcm.com/wp-content/uploads/2025/05/z6601177753457_db1212e4e47b8069de9d3ffec98aea5e-800x757.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Nạc heo xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Nạc heo xay rau củ HG Food cho thú cưng 400g
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt heo tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 48000, 'https://petservicehcm.com/wp-content/uploads/2025/05/gan-7-600x600-1-e1747217763467.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thịt Nạc heo xay rau củ HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Nạc heo xay rau củ HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
•  Lành tính, phù hợp với các bé bị viêm da, dị ứng.
•  Được làm từ 100% nguyên liệu tự nhiên từ thịt nạc heo và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 75000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-7.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Bò tuyệt đỉnh 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 60000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawt0x90e.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Gà siêu chất 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lb6smmn27.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim bò hảo hạng 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 60000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawvu25c6.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim Bò Hảo Hạng,300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 46000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawvu25c6.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim Heo Thượng Hạng 300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 40000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m44q3j16yojz23.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Bò tuyệt đỉnh 300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Pet food cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 46000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawt0x90e.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Gà siêu chất 300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 39000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lb6smmn27.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim heo thượng hạng 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawt18v81.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (18, N'Xốt dinh dưỡng cho chó TELLME', N'Xốt và Pate dinh dưỡng cho chó TELLME
CÔNG DỤNG
• 100% làm từ nguồn nguyên liệu chất liệu cao như thịt bò tươi hoặc thịt gà tươi và phomai hảo hạng đi kèm rau củ tươi và Vitamin D, E, Omega 3, Omega 6, nước hầm xương tạo nên nước sốt Tellme đầy dinh dưỡng.
• Omega 3 và 6 giúp bảo vệ da lông toàn diện, làm giảm dấu hiệu của lão hoá, trẻ hoá các tế bào . Hồi phục các vùng bị thương nhanh chóng. Thúc đẩy quá trình trao đổi chất. Đồng thời hỗ trợ hệ tiêu hoá ổn định.
• Vitamin D, E giúp chắc khoẻ xương. Ngăn sự lão hoá của các tế bào và dây thần kinh.
• Protein từ thịt bò hay thịt gà đảm bảo cho thú cưng 1 sức khoẻ toàn diện.
• Rau củ tươi cung cấp 1 lượng chất xơ tự nhiên đồng thời cũng giàu vitamin và chất dinh dưỡng thiết yếu.
• Sốt Tellme có 5 hương vị khác nhau cho boss thay đổi khẩu vị: vị gà-phomai-rau, vị bò và rau, vị cá ngừ-rau, vị cá hồi-gà-rau, vị vịt-rau củ
THÀNH PHẦN
Thịt Bò, thịt ức gà, pho mai, nước hầm xương, vitamin D, E, Omega 3, Omega 6, canxi chiết xuất từ vỏ trứng, cà rốt, đậu Hà Lan, khoai lang tươi.
HƯƠNG VỊ

Heo
Gà phô mai
Bò rau củ
Cà ngừ & Gà
Cá hồi & Gà
Vịt

CÁCH DÙNG
•Cho cún yêu ăn trực tiếp hoặc trộn với cơm/ hạt.
•Ngon hơn khi hâm nóng. Bảo quản mát sau khi mở túi.
•Cho ăn từ 1-2 túi/ ngày, phụ thuộc vào trọng lượng và mức độ hoạt động của cún yêu.
ĐÓNG GÓI: gói 130g', 21000, 'https://petservicehcm.com/wp-content/uploads/2022/07/khung-Shopee-3-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cá ngừ xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Cá Ngừ xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 40000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-3.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cát đậu nành Cature Natural Litter Tofu – 7L', N'Cát đậu nành Natural Litter Tofu – 7L
Tính năng mới nổi bật:
Thiết kế mới, bao bì đẹp hơn, chất lượng cũng nâng tầm lên với nguyên liệu từ tự nhiên: 100% từ bã đậu nành, không phẩm màu và hóa chất độc hại.
– Kiểm soát mùi cực mạnh.
– 100% phân hủy tự nhiên dễ dàng xả trong toilet và không gây hại cho môi trường.
– Hấp thụ 400%.
Mùi hương:

Trà xanh
Sữa đậu nành

Hướng dẫn sử dụng:
– Đổ cát vào khay vệ sinh với độ dày từ 5-7cm.
– Khi dọn vệ sinh khay cát, chỉ cần hốt phần chất thải đã vón cục trên bề mặt và cho vào thùng rác (Có thể cát vào toilet). Lượng cát còn lại trong khay có thể tiếp tục sử dụng.
– Thêm cát mới vào khay để duy trì độ dày từ 5-7cm.
– Sau khoảng một thời gian (từ 2-4 tuần), khi cát đã giảm độ vón cục, thấm hút và khử mùi, thì cần bỏ hết cát cũ trong khay và thay bằng cát mới.
Gợi ý: Nếu chăm sóc từ 1-2 bé, các bạn có thể thay cát mới sau 20 -30 ngày, còn nếu trên 3 bé thì nên thay cát mới sau 15 ngày.
—————————————————-
● Khối lượng: 7L
● Hạn sử dụng : Xem trên bao bì
Lưu ý: Bảo quản sản phẩm nơi khô ráo', 135000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Shopee-11-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cát gỗ cho Mèo CatFee 6L Hương Dừa – 100% từ gỗ tự nhiên – Bảo vệ môi trường và sức khỏe PET SERVICE', N'PETSERVICE – TRỌN VẸN TRẢI NGHIỆM.                                                                                           1. THÔNG TIN SẢN PHẨM.
1.1. MÔ TẢ SẢN PHẨM.
– CATFEE là sản phẩm cát vệ sinh cho mèo được làm từ 100% nguyên liệu tự nhiên, bao gồm sợi gỗ từ cây Pinaceae, sợi trấu, và sơ hạt guar. Những thành phần này mang lại nhiều ưu điểm vượt trội.
1.2. ƯU ĐIỂM.
– Sợi gỗ có khả năng thấm hút và khóa mùi xuất sắc, không cần sử dụng hương liệu hóa học để che giấu mùi chất thải của thú cưng.
– Hiệu quả kiểm soát mùi vượt trội nhờ vào đặc tính tự nhiên của gỗ, đảm bảo an toàn cho sức khỏe của cả vật nuôi và người dùng.
– Thân thiện với môi trường và tiện lợi:
– Sản phẩm có thể xả trực tiếp vào bồn cầu hoặc dùng làm phân bón cho cây, giúp giảm thiểu rác thải.
– Không chứa hương liệu hóa học gây hại, mang lại sự an toàn cho thú cưng.
– Mùi hương tự nhiên:
– Có các mùi hương từ dầu dừa và cám gạo, được chiết xuất từ tinh dầu tự nhiên, an toàn cho sức khỏe.
– Hạt gỗ lớn không dính vào chân mèo, giúp giữ cho ngôi nhà luôn sạch sẽ.
– Sản phẩm vón cục nhanh chóng, có khả năng thấm hút gấp 5 lần và tạo ra ít bụi, cải thiện chất lượng không khí.
– Trọng lượng nhẹ giúp tiết kiệm chi phí vận chuyển.
– Khả năng thấm hút cao và kiểm soát mùi hiệu quả kéo dài thời gian sử dụng sản phẩm, giúp tiết kiệm chi phí so với cát đất sét truyền thống.
2. HƯỚNG DẪN SỬ DỤNG.
– Đổ đầy khay vệ sinh với cát gỗ hữu cơ CATFEE dày khoảng 5-7 cm.
– Dọn dẹp chất thải trong khay mỗi ngày.
– Có thể xả trực tiếp vào bồn cầu hoặc sử dụng làm phân bón cho cây.
– Bổ sung thêm cát gỗ hữu cơ sau mỗi lần dọn vệ sinh.
3. SỬ DỤNG CÁT VỚI MÁY VỆ SINH TỰ ĐỘNG.
– Cát gỗ vón CATFEE có thể sử dụng bình thường trên các loại máy vệ sinh tự động. Tuy nhiên, đối với máy có đường kính lỗ nhỏ hơn 1-1.5 cm, sản phẩm có thể không phù hợp do hạt cát được thiết kế với kích cỡ lớn.
– Thời gian sử dụng: Với khả năng thấm hút và khử mùi tốt, một túi nhỏ (2.3 kg) có thể sử dụng trong khoảng 3-5 tuần cho một bé mèo, trong khi túi lớn (3.4 kg) có thể kéo dài từ 6-8 tuần, giúp tiết kiệm chi phí và mang lại trải nghiệm tốt hơn so với cát đất sét.
CHÍNH SÁCH CỦA PET SERVICE
– Sản phẩm cam kết giống 100% mô tả
– Mỗi sản phẩm khi được bán ra đều được kiểm tra cẩn thận trước khi gửi tới Quý khách.
– Hàng có sẵn, giao hàng ngay khi shop nhận được đơn
– Hỗ trợ đổi trả, hoàn tiền đối với sản phẩm lỗi theo chính sách Shopee
– Vui lòng quay lại video quá trình mở sản phẩm để được Pet Service hỗ trợ nhanh nhất trong các trường hợp phát sinh vấn đề về đơn hàng.', 145000, 'https://petservicehcm.com/wp-content/uploads/2024/06/40.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cát gỗ vón cho Mèo CatFee 6L Mùi hương tự nhiên 100% từ gỗ tự nhiên – Bảo vệ môi trường và sức khỏe PET SERVICE', N'PETSERVICE – TRỌN VẸN TRẢI NGHIỆM.                                                                                           1. THÔNG TIN SẢN PHẨM.
1.1. MÔ TẢ SẢN PHẨM.
– CATFEE là sản phẩm cát vệ sinh cho mèo được làm từ 100% nguyên liệu tự nhiên, bao gồm sợi gỗ từ cây Pinaceae, sợi trấu, và sơ hạt guar. Những thành phần này mang lại nhiều ưu điểm vượt trội.
1.2. ƯU ĐIỂM.
– Sợi gỗ có khả năng thấm hút và khóa mùi xuất sắc, không cần sử dụng hương liệu hóa học để che giấu mùi chất thải của thú cưng.
– Hiệu quả kiểm soát mùi vượt trội nhờ vào đặc tính tự nhiên của gỗ, đảm bảo an toàn cho sức khỏe của cả vật nuôi và người dùng.
– Thân thiện với môi trường và tiện lợi:
– Sản phẩm có thể xả trực tiếp vào bồn cầu hoặc dùng làm phân bón cho cây, giúp giảm thiểu rác thải.
– Không chứa hương liệu hóa học gây hại, mang lại sự an toàn cho thú cưng.
– Mùi hương tự nhiên:
– Có các mùi hương từ dầu dừa và cám gạo, được chiết xuất từ tinh dầu tự nhiên, an toàn cho sức khỏe.
– Hạt gỗ lớn không dính vào chân mèo, giúp giữ cho ngôi nhà luôn sạch sẽ.
– Sản phẩm vón cục nhanh chóng, có khả năng thấm hút gấp 5 lần và tạo ra ít bụi, cải thiện chất lượng không khí.
– Trọng lượng nhẹ giúp tiết kiệm chi phí vận chuyển.
– Khả năng thấm hút cao và kiểm soát mùi hiệu quả kéo dài thời gian sử dụng sản phẩm, giúp tiết kiệm chi phí so với cát đất sét truyền thống.
2. HƯỚNG DẪN SỬ DỤNG.
– Đổ đầy khay vệ sinh với cát gỗ hữu cơ CATFEE dày khoảng 5-7 cm.
– Dọn dẹp chất thải trong khay mỗi ngày.
– Có thể xả trực tiếp vào bồn cầu hoặc sử dụng làm phân bón cho cây.
– Bổ sung thêm cát gỗ hữu cơ sau mỗi lần dọn vệ sinh.
3. SỬ DỤNG CÁT VỚI MÁY VỆ SINH TỰ ĐỘNG.
– Cát gỗ vón CATFEE có thể sử dụng bình thường trên các loại máy vệ sinh tự động. Tuy nhiên, đối với máy có đường kính lỗ nhỏ hơn 1-1.5 cm, sản phẩm có thể không phù hợp do hạt cát được thiết kế với kích cỡ lớn.
– Thời gian sử dụng: Với khả năng thấm hút và khử mùi tốt, một túi nhỏ (2.3 kg) có thể sử dụng trong khoảng 3-5 tuần cho một bé mèo, trong khi túi lớn (3.4 kg) có thể kéo dài từ 6-8 tuần, giúp tiết kiệm chi phí và mang lại trải nghiệm tốt hơn so với cát đất sét.
CHÍNH SÁCH CỦA PET SERVICE
– Sản phẩm cam kết giống 100% mô tả
– Mỗi sản phẩm khi được bán ra đều được kiểm tra cẩn thận trước khi gửi tới Quý khách.
– Hàng có sẵn, giao hàng ngay khi shop nhận được đơn
– Hỗ trợ đổi trả, hoàn tiền đối với sản phẩm lỗi theo chính sách Shopee
– Vui lòng quay lại video quá trình mở sản phẩm để được Pet Service hỗ trợ nhanh nhất trong các trường hợp phát sinh vấn đề về đơn hàng.', 140000, 'https://petservicehcm.com/wp-content/uploads/2024/06/39.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cát vệ sinh mix than hoạt tính LAPAW cho mèo mùi CAFE túi 15L/8kg', N'Cát vệ sinh cho mèo laPaw than hoạt tính siêu vón cục, siêu khử mùi, thơm lâu, ít bụi 20L
1. THÔNG TIN SẢN PHẨM
– Mùi hương: Chanh, Táo, Cà phê
– Khối lượng: 15L/8kg
– Hình dạng: Viên tròn
– Cát vệ sinh được mix giữa Bentonite thông thường và than hoạt tính
2. ƯU ĐIỂM cát vệ sinh cho mèo than hoạt tính:
– Cát vệ sinh cho mèo tăng khả năng vón cục cứng so với những loại bentonite khác.
– Cát vệ sinh hạn chế đến 99% bụi trong quá trình sử dụng của mèo và khi người dùng dọn vệ sinh.
– Than hoạt tính (Activated Carbon): là thành phần giúp tăng khả năng hấp thụ nước và ít vỡ vụn trong quá trình sử dụng nên sẽ hạn chế bụi bay.
– Cát vệ sinh cho mèo laPaw than hoạt tính chứa các hạt lưu hương và carbon hoạt tính giúp khử mùi tuyệt đối.
– Tiết kiệm hơn cho Sen do giá cả phải chăng.
– Nhờ thấm hút cực nhanh và vón cục ngay lập tức nên việc dọn vệ sinh dễ dàng hơn rất nhiều vì không bị ướt lan xuống dưới đáy hộp, tiết kiệm và kinh tế hơn cho người sử dụng.
– Hạn chế lượng bụi: Cấu tạo hạt đặc biệt với thành phần tự nhiên ít bụi và thân thiện với môi trường hơn. Hạn chế được tối đa tình trạng dị ứng cho chủ và vật nuôi.
3. HDSD cát vệ sinh:
– Đổ cát vệ sinh sạch với lượng khoảng 5-6cm vào trong khay vệ sinh sạch sẽ.
– Dọn sạch khay cát vệ sinh mèo của bạn hằng ngày.
– Chỉ dọn các chất thải rắn và các điểm ướt thay thế bằng cát mới để luôn giữ cho khay sạch sẽ.
– Thời gian sử dụng khoảng 1 tuần/bé mèo tùy vào việc vệ sinh của bé mèo.
– Để tăng khả năng sử dụng của cát vệ sinh
– Nên thay thế 100% cát cũ trước khi cho cát mới vào, loại bỏ phần rác thải 2 lần/ ngày để không gây vi khuẩn trong cát.', 130000, 'https://petservicehcm.com/wp-content/uploads/2024/04/lapaw.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cát vệ sinh mix than hoạt tính LAPAW cho mèo mùi Chanh túi 15L/8kg', N'Cát vệ sinh cho mèo laPaw than hoạt tính siêu vón cục, siêu khử mùi, thơm lâu, ít bụi 20L
1. THÔNG TIN SẢN PHẨM
– Mùi hương: Chanh, Táo, Cà phê
– Khối lượng: 15L/8kg
– Hình dạng: Viên tròn
– Cát vệ sinh được mix giữa Bentonite thông thường và than hoạt tính
2. ƯU ĐIỂM cát vệ sinh cho mèo than hoạt tính:
– Cát vệ sinh cho mèo tăng khả năng vón cục cứng so với những loại bentonite khác.
– Cát vệ sinh hạn chế đến 99% bụi trong quá trình sử dụng của mèo và khi người dùng dọn vệ sinh.
– Than hoạt tính (Activated Carbon): là thành phần giúp tăng khả năng hấp thụ nước và ít vỡ vụn trong quá trình sử dụng nên sẽ hạn chế bụi bay.
– Cát vệ sinh cho mèo laPaw than hoạt tính chứa các hạt lưu hương và carbon hoạt tính giúp khử mùi tuyệt đối.
– Tiết kiệm hơn cho Sen do giá cả phải chăng.
– Nhờ thấm hút cực nhanh và vón cục ngay lập tức nên việc dọn vệ sinh dễ dàng hơn rất nhiều vì không bị ướt lan xuống dưới đáy hộp, tiết kiệm và kinh tế hơn cho người sử dụng.
– Hạn chế lượng bụi: Cấu tạo hạt đặc biệt với thành phần tự nhiên ít bụi và thân thiện với môi trường hơn. Hạn chế được tối đa tình trạng dị ứng cho chủ và vật nuôi.
3. HDSD cát vệ sinh:
– Đổ cát vệ sinh sạch với lượng khoảng 5-6cm vào trong khay vệ sinh sạch sẽ.
– Dọn sạch khay cát vệ sinh mèo của bạn hằng ngày.
– Chỉ dọn các chất thải rắn và các điểm ướt thay thế bằng cát mới để luôn giữ cho khay sạch sẽ.
– Thời gian sử dụng khoảng 1 tuần/bé mèo tùy vào việc vệ sinh của bé mèo.
– Để tăng khả năng sử dụng của cát vệ sinh
– Nên thay thế 100% cát cũ trước khi cho cát mới vào, loại bỏ phần rác thải 2 lần/ ngày để không gây vi khuẩn trong cát.', 130000, 'https://petservicehcm.com/wp-content/uploads/2024/04/lapaw.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cát vệ sinh mix than hoạt tính LAPAW cho mèo mùi táo túi 15L/8kg', N'Cát vệ sinh cho mèo laPaw than hoạt tính siêu vón cục, siêu khử mùi, thơm lâu, ít bụi 20L
1. THÔNG TIN SẢN PHẨM
– Mùi hương: Chanh, Táo, Cà phê
– Khối lượng: 15L/8kg
– Hình dạng: Viên tròn
– Cát vệ sinh được mix giữa Bentonite thông thường và than hoạt tính
2. ƯU ĐIỂM cát vệ sinh cho mèo than hoạt tính:
– Cát vệ sinh cho mèo tăng khả năng vón cục cứng so với những loại bentonite khác.
– Cát vệ sinh hạn chế đến 99% bụi trong quá trình sử dụng của mèo và khi người dùng dọn vệ sinh.
– Than hoạt tính (Activated Carbon): là thành phần giúp tăng khả năng hấp thụ nước và ít vỡ vụn trong quá trình sử dụng nên sẽ hạn chế bụi bay.
– Cát vệ sinh cho mèo laPaw than hoạt tính chứa các hạt lưu hương và carbon hoạt tính giúp khử mùi tuyệt đối.
– Tiết kiệm hơn cho Sen do giá cả phải chăng.
– Nhờ thấm hút cực nhanh và vón cục ngay lập tức nên việc dọn vệ sinh dễ dàng hơn rất nhiều vì không bị ướt lan xuống dưới đáy hộp, tiết kiệm và kinh tế hơn cho người sử dụng.
– Hạn chế lượng bụi: Cấu tạo hạt đặc biệt với thành phần tự nhiên ít bụi và thân thiện với môi trường hơn. Hạn chế được tối đa tình trạng dị ứng cho chủ và vật nuôi.
3. HDSD cát vệ sinh:
– Đổ cát vệ sinh sạch với lượng khoảng 5-6cm vào trong khay vệ sinh sạch sẽ.
– Dọn sạch khay cát vệ sinh mèo của bạn hằng ngày.
– Chỉ dọn các chất thải rắn và các điểm ướt thay thế bằng cát mới để luôn giữ cho khay sạch sẽ.
– Thời gian sử dụng khoảng 1 tuần/bé mèo tùy vào việc vệ sinh của bé mèo.
– Để tăng khả năng sử dụng của cát vệ sinh
– Nên thay thế 100% cát cũ trước khi cho cát mới vào, loại bỏ phần rác thải 2 lần/ ngày để không gây vi khuẩn trong cát.', 130000, 'https://petservicehcm.com/wp-content/uploads/2024/04/lapaw-1.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Cây lăn lông quần áo – M', N'Cây lăn lông quần áo
Đặc điểm:

Chất liệu: cán bằng nhựa (phần màu hồng) + lõi lăn bụi (phần màu trắng) 60 tờ.
Kích thước: size M (lõi dài 10cm)
Lõi lăn bụi gồm cuộn giấy lăn bụi 60 tờ tiết kiệm


Cây lăn lông quần áo dùng loại keo dán cao cấp, không độc hại, không dính lên bề mặt quần áo, sofa, gối, giường… khi lăn.
Thanh lăn bụi nhỏ gọn, có thể mang theo khi đi du lịch, công tác…
Keo dính chắc chắn, tay cầm nhẹ. Dễ dàng loại bỏ các sợi vải thừa, lông chó mèo, bụi bẩn, phù hợp với tất cả các bề mặt

Hướng dẫn sử dụng:

Dùng cuộn lăn bụi lăn trực tiếp trên bề mặt muốn vệ sinh.
Sau khi lăn, bóc bỏ lớp đã sử dụng.
Nếu dùng hết, bạn có thể mua lõi lăn bụi rời để thay thế rồi dùng tiếp.

Chú ý:

Cán là phần nhựa màu hồng, lõi là phần màu trắng.
Lần đầu tiên sử dụng, quý khách nên chọn phân loại “Cán + lõi” để sử dụng ngay.
Sau khi sử dụng hết lõi, quý khách giữ lại cán và mua phân loại “Lõi riêng” để thay thế
tại : https://petservicehcm.com/store/loi-lan-long-quan-ao-10cm', 20000, 'https://petservicehcm.com/wp-content/uploads/2022/06/400G-36-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Găng tay chải lông rụng thú cưng', N'Găng tay chải lông rụng thú cưng
Ưu điểm sản phẩm:

Dễ dùng
Vải lưới thoáng khí và mau khô
Dây đeo băng gai dính có thể điều chỉnh phù hợp với mọi kích thước bàn tay

Công dụng:

Lấy lông rụng trên chó mèo trước khi tắm, trong thời kỳ rụng lông.
Chải lông rối, massage cho thú cưng
Kỳ lông và người cho thú cưng khi tắm
Tránh nhiệt độ cao và ánh sáng mặt trời trực tiếp để không gây biến dạng sản phẩm.

– Xuất xứ: Trung Quốc
– Tên sản phẩm: Găng tay lấy lông rụng chó mèo
– Sản xuất: Sinofiz Cat Litter Products (Dalian) Co., Ltd', 32000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Gang-tay-chai-long-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Lõi lăn lông quần áo – 10cm', N'Lõi lăn lông quần áo
Đặc điểm:

Kích thước: size M (lõi dài 10cm)
Lõi lăn bụi gồm cuộn giấy lăn bụi 60 tờ tiết kiệm


Cây lăn bụi dùng loại keo dán cao cấp, không độc hại, không dính lên bề mặt quần áo, sofa, gối, giường… khi lăn.
Kích cỡ nhỏ gọn, có thể mang theo khi đi du lịch, công tác…
Keo dính chắc chắn, dễ dàng loại bỏ các sợi vải thừa, lông chó mèo, bụi bẩn, phù hợp với tất cả các bề mặt.

Hướng dẫn sử dụng:

Dùng cuộn lăn bụi lăn trực tiếp trên bề mặt muốn vệ sinh.
Sau khi lăn, bóc bỏ lớp đã sử dụng.
Nếu dùng hết, bạn có thể mua lõi lăn bụi rời để thay thế rồi dùng tiếp.

Chú ý:

Cán là phần nhựa màu hồng, lõi là phần màu trắng.
Lần đầu tiên sử dụng, quý khách nên chọn phân loại “Cán + lõi” để sử dụng ngay.
Sau khi sử dụng hết lõi, quý khách giữ lại cán và mua phân loại “Lõi riêng” để thay thế.', 15000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Thiet-ke-chua-co-ten-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Sữa tắm OLIVE ESSENCE giúp dưỡng lông cho mèo chai 450ml', N'1. MÔ TẢ SẢN PHẨM
– Dầu tắm Olive với hương thơm dễ chịu, có nhiều công dụng khác nhau để phù hợp với thú cưng.
– Sữa tắm Olive Dưỡng lông: là sản phẩm dưỡng lông tối ưu, bổ sung các tinh chất dưỡng giúp thẩm thấu sâu vào bề mặt da & lông, mang đến cho vật nuôi một bộ lông sáng óng, mềm mượt.
2. HƯỚNG DẪN SỬ DỤNG
– Làm ướt lông vật nuôi và xoa đều dầu gội lên toàn bộ cơ thể chúng ( tránh để sản phẩm tiếp xúc trực tiếp với mắt )
– Massage cơ thể thú cưng trong 5 -10 phút
– Xả lại bằng nước sạch cho đến khi hết bọt xà phòng.
– Lặp lại quy trình trên nếu vật nuôi vẫn còn bẩn.
– Dùng khăn và máy sấy làm khô lông thú cưng.', 60000, 'https://petservicehcm.com/wp-content/uploads/2024/04/gan-1.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Sữa tắm Oliver hỗ trợ trị Nấm, Da 300ml PET SERVICE.', N'1. THÔNG TIN SẢN PHẨM.
1.1. MÔ TẢ SẢN PHẨM.
– Sữa tắm Oliver hỗ trợ về da cho chó mèo trên 8 tháng.
1.2. ƯU ĐIỂM.
– Sữa tắm Oliver hỗ trợ p.hục h.ồi da do v.i k.huẩn và n.ấm
– Sữa tắm Oliver có mùi phấn nhẹ dễ chịu.
– Thúc đẩy sự làm l.ành da, phòng n.gừa da bị khô và n.gứa.
– Oliver lý tưởng để điều t.rị da bị tróc vảy sừng và tẩy nhờn cho da bị r.ối loạn tiết nhờn.
– Điều t.rị các rối loạn ngoài da kết hợp với g.hẻ Demodex, v.iêm da tăng tiết chất nhờn, m.ụn nước trên chó mèo.
2. HƯỚNG DẪN SỬ DỤNG
– Lắc kỹ chai trước khi sử dụng. Hòa dầu gội với nước ấm.
– Làm ướt lông thú và xoa đều dầu gội lên toàn bộ cơ thể thú cưng ( tránh để dầu gội tiếp xúc với phần mắt )
– Mát xa nhẹ nhàng cơ thể thú cưng trong 5 – 10 phút
– Xả lại bằng nước ấm cho đến khi hết bọt xà phòng.
– Lặp lại quy trình nếu thú cưng vẫn còn bẩn.
– Làm khô lông thú cưng bằng khăn và máy sấy', 170000, 'https://petservicehcm.com/wp-content/uploads/2024/07/z5641413190068_cf445cb37cd2ff68b2522d6c2d02d794.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Sữa tắm Oliver hỗ trợ trị Ve, Ghẻ và Bọ Chét cho Thú Cưng chai 300ml PET SERVICE', N'1. THÔNG TIN SẢN PHẨM.
1.1. MÔ TẢ SẢN PHẨM.
– Sữa tắm Oliver hỗ trợ về da cho chó mèo trên 8 tháng.
1.2. ƯU ĐIỂM.
– Sữa tắm Oliver hỗ trợ p.hục h.ồi da do v.i k.huẩn và n.ấm
– Sữa tắm Oliver có mùi phấn nhẹ dễ chịu.
– Thúc đẩy sự làm l.ành da, phòng n.gừa da bị khô và n.gứa.
– Oliver lý tưởng để điều t.rị da bị tróc vảy sừng và tẩy nhờn cho da bị r.ối loạn tiết nhờn.
– Điều t.rị các rối loạn ngoài da kết hợp với g.hẻ Demodex, v.iêm da tăng tiết chất nhờn, m.ụn nước trên chó mèo.
2. HƯỚNG DẪN SỬ DỤNG
– Lắc kỹ chai trước khi sử dụng. Hòa dầu gội với nước ấm.
– Làm ướt lông thú và xoa đều dầu gội lên toàn bộ cơ thể thú cưng. ( tránh để dầu gội tiếp xúc với phần mắt )
– Mát xa nhẹ nhàng cơ thể thú cưng trong 5 – 10 phút – Xả lại bằng nước ấm cho đến khi hết bọt xà phòng.
– Lặp lại quy trình nếu thú cưng vẫn còn bẩn.
– Làm khô lông thú cưng bằng khăn và máy sấy.', 170000, 'https://petservicehcm.com/wp-content/uploads/2024/07/z5641413190017_9ccfcea0192d24e9e720b16bfae621da.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Sữa tắm Yú', N'🎀🎀🎀SỮA TẮM YU CHO CHÓ MÈO – GIỮ MÙI THƠM LÂU- 400ml  🎀🎀🎀
Dòng sản phẩm Yu, dầu tắm độc đáo cho chó mèo từ các loại hương hoa và thảo mộc phương đông, nâng niu làn da nhạy cảm, giúp bộ lông thú cưng trở nên mềm mượt, thơm quyến rũ đến không ngờ.
🌺SỮA TẮM YU HƯƠNG HOA LAN với chất chống oxi hóa nhầm phục hồi cân bằng độ ẩm và làm trẻ hóa làn da của thú cưng, Hương thơm quý phải, mạnh mẽ
🌺SỮA TẮM YU HƯƠNG HOA TRÀ giúp sợi lông chắc khỏe từ bên trong, hương thơm đặc biệt quyến rũ
🌺SỮA TẮM YU CHO LÔNG TRẮNG với các dưỡng chất làm sạch, loại bỏ lớp tích tụ dưới lông, mang lại vẻ đẹp tự nhiên cuarbooj lông trắng, làm cho chúng sáng bóng và mềm mịn.
Thành phần: Chinese bellflower essence + Witch Hazel + Allantoin + Vitamin B6 + Epilobium fleischeri extrack
🌺SỮA TẮM YU HƯƠNG HOA ANH ĐÀO hương thơm quyến rũ! Chiết xuất hoa anh đào nhẹ dịu làm giảm kích ứng, giúp làn da của thú cưng trở nên mềm mại và thoáng mát
🌺SỮA TẮM YU HƯƠNG HOA SEN được chiết xuất từ tinh chất hoa sen, đây quà tặng đặc biệt cho các bé bị rối vón lông, với công thức tạo hình 3D dành cho tất cả các giống chó mèo gồm 5 ưu điểm đặc biệt:
✅ Làm cho lông của thú cưng xù bông hơn, dễ tạo kiểu khi cắt tỉa lông.
✅ Loại bỏ triệt để hiện tượng thắt nút lông, lông rối, lông vón cục.
✅ Lưu lại mùi hương thơm mát bền lâu.
✅ 100% từ nguyên liệu tự nhiên với những hạt axit lactic Niosome, kích thích các nang lông hoạt động hiệu quả, khiến cho lông bông xù tự nhiên, mang lại một bộ lông tuyệt đẹp.
Xuất xứ: Đài Loan', 390000, 'https://petservicehcm.com/wp-content/uploads/2023/02/bat-an-inox-11-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (8, N'Xẻng cán dài', N'– Xẻng xúc cát vệ sinh
– Xẻng xúc chất thải cho mèo
– Xẻng xúc cát vệ sinh cho mèo được làm bằng nhựa cứng, có những lỗ lọc những hạt cát chưa sử dụng. Giúp bạn vệ sinh cho mèo gọn gàng sạch sẽ và tiện dụng.
– Xẻng xúc cát cho mèo có nhiều màu sắc vì vậy bạn tha hồ lựa chọn nhé!
– Trọng lượng siêu nhẹ
– Rất tiện lợi
– Vứt bỏ chất thải đúng cách
– Dễ dàng sử dụng bằng một tay
– Đáp ứng nhu cầu của bạn vật nuôi
———————————————-
 PET SERVICE – Dịch vụ thú cưng tại nhà
 Hotline: 0898 520 760
 Address: 217 Lâm Văn Bền, Bình Thuận, Quận 7
 Website: Petservicehcm.com
 Shopee: https://shopee.vn/petservicehcm', 20000, 'https://petservicehcm.com/wp-content/uploads/2022/08/Bat-an-cua-pet-2-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (19, N'Chân Gà Sấy PAWO By PET SERVICE 100g', N'Sản phẩm độc quyền, treat sấy khô PAWO Chân Gà:
Treat chân gà sấy khô PAWO là món ăn vặt lý tưởng cho chó mèo, vừa hấp dẫn vừa giàu dinh dưỡng với nhiều lợi ích cho sức khỏe. Chân gà sấy giảm mùi tanh nhưng vẫn giữ được vị ngon tự nhiên của chân gà tươi, kích thích vị giác của thú cưng. Chân gà sấy khô PAWO cung cấp lượng protein dồi dào, cùng với canxi và collagen, giúp hỗ trợ sức khỏe xương khớp, răng miệng, và lông mượt.
Không chỉ cung cấp dinh dưỡng, món treat này còn giúp thú cưng nhai gặm, giảm căng thẳng, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả. Đặc biệt, sản phẩm không chứa chất bảo quản, an toàn và vệ sinh tuyệt đối cho chó mèo.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi trở lên. Chỉ dùng làm món ăn vặt, không vượt quá 10% khẩu phần ăn hàng ngày.', 86000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m0ppg5efk32ndc-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (19, N'Chân Gà Sấy PAWO By PET SERVICE 50g', N'Sản phẩm độc quyền, treat sấy khô PAWO Chân Gà:
Treat chân gà sấy khô PAWO là món ăn vặt lý tưởng cho chó mèo, vừa hấp dẫn vừa giàu dinh dưỡng với nhiều lợi ích cho sức khỏe. Chân gà sấy giảm mùi tanh nhưng vẫn giữ được vị ngon tự nhiên của chân gà tươi, kích thích vị giác của thú cưng. Chân gà sấy khô PAWO cung cấp lượng protein dồi dào, cùng với canxi và collagen, giúp hỗ trợ sức khỏe xương khớp, răng miệng, và lông mượt.
Không chỉ cung cấp dinh dưỡng, món treat này còn giúp thú cưng nhai gặm, giảm căng thẳng, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả. Đặc biệt, sản phẩm không chứa chất bảo quản, an toàn và vệ sinh tuyệt đối cho chó mèo.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi trở lên. Chỉ dùng làm món ăn vặt, không vượt quá 10% khẩu phần ăn hàng ngày.', 53000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m0ppg5efk32ndc-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (19, N'Chóp Cánh Gà sấy khô PAWO By PET SERVICE 100g', N'Sản phẩm độc quyền, treat sấy khô PAWO Chóp Cánh Gà:
Treat chóp cánh gà sấy khô PAWO là món ăn vặt thơm ngon, được yêu thích bởi chó mèo với nhiều lợi ích sức khỏe. Sản phẩm không chỉ giảm mùi hôi khó chịu mà còn giữ lại hương vị tự nhiên, hấp dẫn của chóp cánh gà tươi.
Lợi ích dinh dưỡng: Chóp cánh gà sấy khô PAWO chứa hàm lượng protein cao, cung cấp dinh dưỡng thiết yếu cho thú cưng. Bên cạnh đó, sản phẩm còn bổ sung collagen và canxi, hỗ trợ sức khỏe xương khớp và phát triển toàn diện cho bé yêu.
Tăng cường sức khỏe: Treat này giúp cải thiện sức khỏe răng miệng, làm mượt lông và chắc khỏe xương khớp. Việc nhai gặm cũng giúp thú cưng giảm căng thẳng, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả.
Đặc điểm nổi bật:

Không chứa chất bảo quản
An toàn và vệ sinh tuyệt đối cho thú cưng

Hướng dẫn sử dụng: Chóp cánh gà sấy khô PAWO phù hợp cho chó mèo từ 3 tháng tuổi trở lên. Chỉ nên dùng làm món ăn vặt, không vượt quá 10% khẩu phần ăn hàng ngày.', 109000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7r98o-lwguteubxcyhf5-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (19, N'Heo Phi Lê sấy khô PAWO By PET SERVICE 100g', N'Treat phi lê heo sấy khô là món ăn vặt hấp dẫn, được yêu thích bởi chó mèo nhờ vào hương vị thơm ngon và nhiều lợi ích cho sức khỏe. Sản phẩm giảm mùi hôi khó chịu nhưng vẫn giữ được vị ngọt tự nhiên của thịt heo tươi.
Phi lê heo sấy khô PAWO giữ hàm lượng protein cao, đảm bảo cung cấp dinh dưỡng thiết yếu cho thú cưng. Sản phẩm còn bổ sung collagen, canxi và Omega 3, hỗ trợ sức khỏe xương khớp và tăng cường sức khỏe tổng thể cho bé yêu.
Ngoài ra, treat này giúp cải thiện sức khỏe răng miệng, làm mượt lông và chắc khỏe xương khớp. Việc nhai gặm phi lê heo còn giúp giảm stress, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả.
Đặc biệt, sản phẩm không chứa chất bảo quản, đảm bảo an toàn và vệ sinh tuyệt đối cho thú cưng của bạn. Phù hợp cho chó mèo từ 3 tháng tuổi trở lên, chỉ nên dùng làm món ăn vặt và không vượt quá 10% khẩu phần ăn hàng ngày.', 130000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m1asi289opp8c4.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (19, N'Heo Phi Lê sấy khô PAWO By PET SERVICE 50g', N'Treat phi lê heo sấy khô là món ăn vặt hấp dẫn, được yêu thích bởi chó mèo nhờ vào hương vị thơm ngon và nhiều lợi ích cho sức khỏe. Sản phẩm giảm mùi hôi khó chịu nhưng vẫn giữ được vị ngọt tự nhiên của thịt heo tươi.
Phi lê heo sấy khô PAWO giữ hàm lượng protein cao, đảm bảo cung cấp dinh dưỡng thiết yếu cho thú cưng. Sản phẩm còn bổ sung collagen, canxi và Omega 3, hỗ trợ sức khỏe xương khớp và tăng cường sức khỏe tổng thể cho bé yêu.
Ngoài ra, treat này giúp cải thiện sức khỏe răng miệng, làm mượt lông và chắc khỏe xương khớp. Việc nhai gặm phi lê heo còn giúp giảm stress, rèn luyện hàm và cải thiện vệ sinh răng miệng hiệu quả.
Đặc biệt, sản phẩm không chứa chất bảo quản, đảm bảo an toàn và vệ sinh tuyệt đối cho thú cưng của bạn. Phù hợp cho chó mèo từ 3 tháng tuổi trở lên, chỉ nên dùng làm món ăn vặt và không vượt quá 10% khẩu phần ăn hàng ngày.', 76000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7ras8-m1asi289opp8c4.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (19, N'Ức gà sấy Pawo by Pet Service 100g', N'Sản phẩm độc quyền, treat sấy khô Pawo:
– Treat sấy khô là món ăn vặt yêu thích của các bé với nhiều lợi ích tuyệt vời cho sức khỏe
– Treat sấy khô giảm mùi tanh khó chịu nhưng vẫn giữ được vị hấp dẫn của thịt tươi
– Treat sấy khô Pawo giữ được hàm lượng protein lên tới hơn 80%
– Cung cấp protein, chất xơ, canxi và Omega 3 cho bé ngay trong khẩu phần hàng ngày.
– Giúp làm mượt lông, chắc khỏe xương khớp và răng.
– Kích thích các bé nhai gặm, hỗ trợ giảm stress, luyện hàm và vệ sinh răng miệng hiệu quả.
Đặc biệt: Không chất bảo quản, an toàn, đảm bảo vệ sinh cho bé.
Sử dụng: Phù hợp cho chó mèo từ 3 tháng tuổi. Chỉ cho ăn vặt, không vượt quá 10% khẩu phần hàng ngày.', 105000, 'https://petservicehcm.com/wp-content/uploads/2023/09/vn-11134207-7r98o-lwguteubujtl7f-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt khô Minino Yum vị Cá hồi gói 1,5kg', N'1. ĐẶC ĐIỂM SẢN PHẨM
Minino Yum Vị Cá Hồi có công thức từ cá hồi tươi ngon, tạo nên hương vị tuyệt vời.
Thành phần được kết hợp từ các nguyên liệu chất lượng cao đảm bảo tính ngon miệng, hấp dẫn và đáp ứng nhu cầu dinh dưỡng của những chú mèo.
2. CÔNG DỤNG SẢN PHẨM
– Taurine: Tốt cho mắt.
– Calcium & Vitamin D: Giúp khung xương khỏe mạnh.
– Omega 3 & 6: Giúp mượt lông và da khỏe.

– Gạo và Yucca: Giúp đóng khuôn phân và giảm mùi.
3. THÀNH PHẦN
Gạo, bột thịt gia cầm, lúa mì, bã nành, mỡ gia cầm (nguồn Omega 3-6 tự nhiên), bột cá hồi, dầu cá (chứa DHA), Taurine, khoáng chất (sắt, đồng, mangan, kẽm, I-ốt, selen), Vitamins (A, D3, K3, B1, B2, B6, B12, PP, E (Tocopherol), Calcium D-Pantothenate, Biotin, Folic Acid, Choline), Sodium Disulfate, Monocalcium Phosphate, Calcium Carbonate, muối, chất bảo quản, chất chống oxi hóa, chất làm ngon miệng, chiết xuất Yucca Schidigera.', 138000, 'https://petservicehcm.com/wp-content/uploads/2023/12/Shopee-95-1-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt Minino Yum hải sản 1,5kg', N'Đảm bảo cho mèo cưngmột chế độ dinh dưỡng toàn diện ,cân bằng và tăng trưởng khoẻ mạnh trong mọi giai đoạn sống.
Hương vị gà Irresistible, gan và cá ngon
Dễ tiêu hóa, chứa taurine duy trì sức khỏe các tế bào cơ tim, võng mạc
Tác động đến khả năng sinh sản mèo.
Có chứa các axit béo thiết yếu omega-3 và omega-6 giúp lông da khỏe mạnh và mềm mại, sáng bóng.
Sản phẩm chứa các khoáng chất và các axit amin cân bằng để kiểm soát tốt hơn PH nước tiểu.
Chất lượng đảm bảo một cuộc sống lâu dài, khỏe mạnh
Giúp giảm mùi và giảm khối lượng phân thải.', 135000, 'https://petservicehcm.com/wp-content/uploads/2023/05/Vong-co-36-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt Reflex cho mèo adult 1,5kg-Gà', N'– Thức ăn khô cho mèo siêu cao cấp với công thức cân bằng và hoàn chỉnh dành cho giống mèo trưởng thành.
– Dành cho tất cả giống mèo trên 12 tháng tuổi. Thể trọng từ 2kg – 10kg.
@ Công thức đặc biệt cao cấp dành cho các giống mèo trưởng thành.
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Xylo-oligosaccharides (XOS) giúp giảm các vấn đề như chất béo không mong muốn và đường huyết ở mèo với giá trị Calo xấp xỉ bằng không. Cải thiện quá trình tiêu hóa và chuyển hóa thức ăn bằng cách cải thiện hệ vi khuẩn đường ruột. Tác dụng chống oxy hóa tự nhiên.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất.', 176000, 'https://petservicehcm.com/wp-content/uploads/2023/05/Vong-co-31-1-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt Reflex cho mèo adult Chicken & Rice 2kg', N'– Thức ăn khô cho mèo với công thức cân bằng và hoàn chỉnh dành cho giống mèo trưởng thành.
– Dành cho tất cả giống mèo trên 12 tháng tuổi. Thể trọng từ 1kg – 8kg.
@ Công thức đặc biệt dành cho các giống mèo trưởng thành.
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất.', 197000, 'https://petservicehcm.com/wp-content/uploads/2023/05/Vong-co-33-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt Reflex cho mèo adult Sterilised 1.5kg- cho mèo triệt sản', N'– Thức ăn khô cho mèo với công thức cân bằng và hoàn chỉnh dành cho giống mèo trưởng thành.
– Dành cho tất cả giống mèo trên 12 tháng tuổi. Thể trọng từ 1kg – 8kg.
@ Công thức đặc biệt dành cho các giống mèo trưởng thành (đặc biệt giúp tiệt trùng đường ruột ở mèo).
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất.', 197000, 'https://petservicehcm.com/wp-content/uploads/2023/05/Vong-co-32-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt Reflex kitten chicken & rice 2kg', N'Hạt Reflex Kitten cho mèo con – 2kg
Thức ăn cho mèo con REFLEX KITTEN vị thịt gà là thức ăn cho mèo con với hương vị thơm ngon từ làm từ hỗn hợp toàn chỉnh phù hợp cho mèo con từ 0.5 – 5kg.
Công dụng:
– Công thức đặc biệt cao cấp dành cho các giống mèo con.
– Bổ sung Vitamin A, D3, E, C và khoáng chất.
– Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
– Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
Thành phần : Bắp, gạo, mỡ gà, vỏ hạt mã đề, xơ mía, bã củ dền, hương gan, vitamin và khoáng chất, di-methionine, men bia, hạt lanh, xylo-oligosaccharides, muối, l-carnitine, chiết xuất cây yucca schidigera, chất bảo quản chống oxy hoá.

Xuất xứ: Thổ Nhĩ Kỳ.
Thương hiệu REFLEX
HDSD: Dùng cho mèo con từ 0.5 – 5kg, cho mèo ăn trực tiếp
Lưu ý: Bảo quản ở nơi thoáng mát

Quy cách: Gói 2kg', 207000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Vong-co-30-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Hạt Reflex Urinary cho mèo – 1.5Kg', N'Hạt Reflex Urinary cho mèo (tiết niệu) – 1.5Kg
THỨC ĂN DÀNH CHO MÈO BỊ THẬN, HỖ TRỢ TIẾT NIỆU
Sử dụng cho: 
– Mèo bị viêm bàng quang do vi khuẩn
– Mèo cần điều trị sỏi Struvite, Canxi Oxalate sỏi niệu.
– Thức ăn khô cho mèo siêu cao cấp với công thức cân bằng và hoàn chỉnh dành cho giống mèo trưởng thành.
– Dành cho tất cả giống mèo trên 12 tháng tuổi. Thể trọng từ 2kg – 10kg.
Ưu điểm đặc biệt
@ Công thức đặc biệt cao cấp dành cho các giống mèo trưởng thành (đặc biệt trị bệnh tiết niệu ở mèo).
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Xylo-oligosaccharides (XOS) giúp giảm các vấn đề như chất béo không mong muốn và đường huyết ở mèo với giá trị Calo xấp xỉ bằng không. Cải thiện quá trình tiêu hóa và chuyển hóa thức ăn bằng cách cải thiện hệ vi khuẩn đường ruột. Tác dụng chống oxy hóa tự nhiên.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất. – Trọng lượng: 1,5kg / gói
– Xuất xứ: Thổ Nhĩ Kỳ – Lider Pet Food
– Hướng dẫn sử dụng: ghi trên bao bì.
– Cách cho ăn: Chia làm 2-3 bữa ăn / ngày', 209000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Vong-co-29-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Reflex Adult Hairball & Indoor cho mèo – 1.5Kg', N'Refex Adult Hairball & Indoor cho mèo (búi lông)
Công dụng
Thức ăn khô cho mèo từ 12 tháng tuổi, hỗ trợ tiêu búi lông và giảm mùi hôi phân với vị cá hồi hấp dẫn
Xuất sứ: Thổ Nhĩ Kỳ
Trọng Lượng: túi 1.5kg
Ưu điểm đặc biệt
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất', 240000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Vong-co-28-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'REFLEX SKIN CARE – Thức ăn khô cho mèo chăm sóc, tái tạo và phục hồi lông da', N'ĐẶC ĐIỂM SẢN PHẨM
Reflex Plus Skin Care With Salmon
Thức ăn khô cho Mèo chăm sóc phục hồi và tái tạo lông da
Xuất xứ: Thổ Nhĩ Kỳ
CÔNG DỤNG SẢN PHẨM

Giúp chăm sóc phục hồi và tái tạo lông da
Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi.
Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
Xylo-oligosaccharides (XOS) giúp giảm các vấn đề như chất béo không mong muốn và đường huyết
Cải thiện quá trình tiêu hóa và chuyển hóa thức ăn bằng cách cải thiện hệ vi khuẩn đường ruột
Bổ sung Vitamin A, D3, E, C và khoáng chất

HƯỚNG DẪN BẢO QUẢN
Bảo quản nơi khô ráo, tránh tiếp xúc trực tiếp ánh nắng mặt trời.', 252000, 'https://petservicehcm.com/wp-content/uploads/2023/08/bat-an-inox-86-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Thức ăn cao cấp cho mèo dạng viên Maximum túi 1kg', N'1. ĐẶC ĐIỂM SẢN PHẨM
– Ngon miệng: Nguyên liệu thượng hạng và cách chế biến tinh tế giúp chú mèo của bạn có bữa ăn rất ngon miệng
– Tim Mạch: Hỗn hợp Taurine và Vitamin E giúp duy trì lưu thông máu, khỏe mạnh
– Da & Lông: Dòng sản phẩm MAXIMUM chứa sự kết hợp của Omega 3: 6: 9, kẽm, Astaxanthin, B-Glucan nhằm giúp duy trì làn da và bộ lông bóng mượt cho các chú mèo
– Hệ miễn dịch: B-Glucan hỗ trợ hệ thống miễn dịch, giúp mèo của bạn duy trì một cuộc sống năng động, khỏe mạnh
– Hệ tiêu hóa: Prebiotic tự nhiên, inulin, giúp thúc đẩy sự phát triển của vi khuẩn có lợi trong ruột dẫn đến phân nhỏ, theo khuôn, Extract Yucca để giảm mùi hôi của chất thải, Ngoài ra, kiểm soát quá trình tiêu hóa của đường ruột và tối đa hóa sự hấp thụ chất dinh dưỡng
– Không màu và mùi hương nhân tạo: MAXIMUM không chứa them màu sắc hoặc hương vị nhân tạo
2. THÀNH PHẦN
Cá ngừ, Cà hồi, Cá các loại, thịt gà, thịt bò, ngũ cốc, protein ngũ cốc, muối, khoáng chất (kali clorua, kẽm sunfat đồng, kali iodua), vitamin (A, B1, B2, B3, B6, B9, B12, C, D3, E và choline) methionline, taurine, chất chống oxy hóa, inulin và yucaa
HƯỚNG DẪN CHO ĂN HÀNG NGÀY
– 1 KG – 2 KG: 15G – 30 G
– 2 KG – 3 KG: 30G – 45 G
– 3 KG – 4 KG: 45G – 60 G
– 4 KG – 5 KG: 60 G – 85 G
3. NHỮNG ĐIỀU LƯU Ý
Đừng quên cho mèo uống nước sạch và thường xuyên thay nước. Thức ăn chỉ sử dụng cho thú cưng. Đưa mèo đến thú y để khám định kỳ.
4. BẢO QUẢN
Bảo quản nơi khô ráo, thoáng mát, tránh ánh nắng trực tiếp. Nên đóng kín miệng bao sau khi cho ăn để bảo quản tốt nhất.', 100000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Shopee-34-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Thức ăn cao cấp dạng viên cho mèo Minimax túi 350g', N'1. ĐẶC ĐIỂM SẢN PHẨM
– MINIMAX cung cấp một sự cân bằng dinh dưỡng của protein, vitamin và khoáng chất để phát triển cho mèo từ giai đoạn 3 tháng tuổi đến khi trưởng thành.
– MINIMAX có hàm lượng protein cao ( 32%) với khẩu vị cá ngừ sẽ giúp cho mèo của bạn có bữa ăn đầy các chất dinh dưỡng và ngon miệng.
– TIM MẠCH
Hỗn hợp taurine và vitamin E giúp duy trì lưu thông máu khỏe mạnh.
– DA & LÔNG
Dòng sản phẩm MINIMAX chứa sự kết hợp của Omega 3-6-9 và kẽm giúp duy trì làn da khỏe mạnh và bộ lông bóng mượt.
– HỆ MIỄN DỊCH
β-glucan hỗ trợ hệ thống miễn dịch, giúp mèo của bạn duy trì một cuộc sống luôn năng động, khỏe mạnh.
– HỆ TIÊU HÓA
Prebiotic tự nhiên, inulin, giúp thúc đẩy sự phát triển của vi khuẩn có lợi trong ruột dẫn đến phân nhỏ, cứng hơn. Extract Yucca để giúp giảm mùi hôi của chất thải. Ngoài ra, kiểm soát quá trình đường ruột và tối đa hóa sự hấp thụ chất dinh dưỡng.
– KHÔNG MÀU VÀ MÙI HƯƠNG NHÂN TẠO
– MINIMAX không chứa thêm màu sắc hoặc hương vị nhân tạo.
THÀNH PHẦN:
Cá ngừ, các cá loại, thịt gà, thịt bò, ngũ cốc, protein ngũ cốc, muối, khoáng chất (kali clorua, kẽm sunfat, sunfat đồng, sunfat đồng, kali iodua), vitamin (A, B1, B2 , B3, B6, B9, B12, C, D3, E và choline), methionine, taurine, chất chống oxy hóa, inulin và yucca.
2. HƯỚNG DẪN SỬ DỤNG :
Trọng lượng của mèo Lượng thức ăn
1kg-2kg 15g – 30g
2kg-3kg 30g – 45g
3kg-4kg 45g – 60g
4kg-5kg 60g – 85g
– Lượng thức ăn có thể thay đổi tùy theo tình trạng hoạt động và thể chất của mèo.
– Thay đổi chế độ ăn uống : Chúng tôi khuyên bạn nên dần dần thay đổi MINIMAX trong khoảng thời gian bảy ngày để cho phép mèo của bạn có thời gian thích nghi với chế độ ăn mới.
3. NHỮNG ĐIỀU LƯU Ý :
– Đừng quên cho mèo uống nước sạch và thường xuyên thay nước.
– Thức ăn chỉ sử dụng cho thú cưng.
– Đưa mèo đến thú y để khám định kỳ.
4. BẢO QUẢN :
– Bảo quản nơi khô ráo, thoáng mát, tránh ánh nắng trực tiếp
– Nên đóng kín miệng bao sau khi cho ăn để bảo quản tốt nhất.', 40000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Shopee-37-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (20, N'Thức ăn khô cho mèo Minino vị Cá ngừ gói 1,3kg', N'1. ĐẶC ĐIỂM SẢN PHẨM
Thức ăn cho mèo Minino chứa các thành phần dưỡng chất cần thiết và cân bằng, công thức sản phẩm tạo nên từ nguyên liệu Cá Ngừ thật.
– 28% Protein tối thiểu
– 9% Chất béo thô tối thiểu
– 5% Chất xơ thô tối đa
– 12% Độ ẩm tối đa
– Thơm ngon hơn, dinh dưỡng hơn với cá ngừ thật
2. THÀNH PHẦN CHÍNH
Lúa mì, bắp, bột thịt gia cầm, bã nành, mỡ gia cầm, bột cá ngừ (nguồn protein và Omega 3 tự nhiên từ thành phần cá thật), Taurine, khoáng chất (Sắt, Đồng, Mangan, Kẽm, Iốt, Selen), Vitamins (A, D3, K3, B1, B2, B6, B12, PP, E (Tocopherol), Calcium D-Pantothenate, Biotin, Axit Folic, Choline), Sodium Disulfate, Monocalcium Phosphate, Calcium Carbonate, muối, Chất bảo quản, chất chống oxi hóa, chất làm ngon miệng, chiết xuất Yucca Schidigera', 90000, 'https://petservicehcm.com/wp-content/uploads/2023/12/Shopee-90-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Cá ngừ xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Cá Ngừ xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 40000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-3.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate gan heo TELLME cho mèo', N'Pate gan heo TELLME cho mèo
CÔNG DỤNG
• 100% làm từ nguồn nguyên liệu chất liệu cao như thịt bò tươi hoặc thịt gà tươi và phomai hảo hạng đi kèm rau củ tươi và Vitamin D, E, Omega 3, Omega 6, nước hầm xương tạo nên nước sốt Tellme đầy dinh dưỡng.
• Omega 3 và 6 giúp bảo vệ da lông toàn diện, làm giảm dấu hiệu của lão hoá, trẻ hoá các tế bào . Hồi phục các vùng bị thương nhanh chóng. Thúc đẩy quá trình trao đổi chất. Đồng thời hỗ trợ hệ tiêu hoá ổn định.
• Vitamin D, E giúp chắc khoẻ xương. Ngăn sự lão hoá của các tế bào và dây thần kinh.
• Protein từ thịt bò hay thịt gà đảm bảo cho thú cưng 1 sức khoẻ toàn diện.
rau củ
THÀNH PHẦN
Thịt Bò, thịt ức gà, pho mai, nước hầm xương, vitamin D, E, Omega 3, Omega 6, canxi chiết xuất từ vỏ trứng
CÁCH DÙNG
• Ăn trực tiếp hoặc trộn với hạt và cơm.
• Mèo lớn ngày 1-2 gói.
• Mèo bé ngày 1 gói.
ĐÓNG GÓI: gói 130g', 18000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Shopee-96-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate King’s pet cho mèo lon 80g', N'Nguyên liệu tự nhiên: Sử dụng nguồn cá biển tươi xuất xứ hoàn toàn Việt Nam, không sử dụng chất độn, chất tạo dày. Kết cấu hoàn toàn từ nguyên liệu thật xay nhuyễn.
Hương vị thơm ngon: Phát triển từ dòng sản phẩm paté tươi (Paté hỗn hợp cá) đã phổ biến với khách hàng của công ty. Sản phẩm là phiên bản thay thế hoàn hảo cho khách hàng cần sử dụng Pate tươi với định lượng nhỏ, cần bảo quản lâu, di chuyển xa,…
Siêu cấp nước cho hệ tiêu hoá: Cho thú cưng sử dụng thức ăn ướt là giúp ngăn ngừa sỏi thận do thói quen sử dụng thức ăn khô & lười uống nước.
Đầy đủ vitamin, khoáng chất cần thiết: Giàu vitamin A, E, nhóm B, sắt, phosphorus, magie, canxi… từ thịt cá tươi nguyên chất giúp chó mèo sáng mắt, khỏe xương, phát triển bền vững.', 31000, 'https://petservicehcm.com/wp-content/uploads/2023/08/bat-an-inox-80-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate mèo Happy100 70g', N'PATE CHO MÈO WANPY HAPPY 100 VỚI 6 HƯƠNG VỊ (NHIỀU MÀU)

Vàng: Cá ngừ + cá hồi
Xanh lá: Cá ngừ + cá petrel
Tím: Gà + cá trích
Hồng: Gà + cá hồi
Cam: Gà + cá ngừ
Hồng nhạt: Gà + cá petrel

– Sản phẩm hoàn toàn tự nhiên không chất phụ gia, không phẩm màu.
– Mỗi vị làm từ những nguyên liệu khoái khẩu khác nhau dành cho mèo.
– Protein và đạm trong thành phần hỗ trợ phát triển cân đối.
– Bổ sung Omega 3 & 6 giúp lông mèo mềm mượt hơn.
– Cung cấp đủ lượng Canxi cho răng và xương chắc khỏe.
– Hàm lượng Taurine trong sản phẩm cho đôi mắt thú cưng luôn tinh anh.
– Tăng cường hệ miễn dịch nhờ vào lượng Vitamin và khoáng chất.
– Thực phẩm ướt giúp bảo vệ hệ tiêu hóa, tránh các bệnh sỏi thận.
– Có thể sử dụng trực tiếp hoặc trộn với hạt nhằm kích thích vị giác.
– Phù hợp với mọi giống mèo ở mọi lứa tuổi.', 10000, 'https://petservicehcm.com/wp-content/uploads/2023/06/Khung-Shopee-55-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Minino cho mèo gói 70g – PET SERVICE', N'Pate Minino vị gà salad là món ăn dinh dưỡng được yêu thích bởi mèo, mang lại nhiều lợi ích cho sức khỏe. Pate không chỉ giữ được hương vị tươi ngon tự nhiên của thịt gà mà còn cung cấp dưỡng chất thiết yếu. Sản phẩm giàu protein, giúp mèo duy trì cơ bắp khỏe mạnh, đồng thời bổ sung vitamin và khoáng chất hỗ trợ hệ miễn dịch và tiêu hóa. Công thức cân bằng giúp mèo mượt lông, chắc khỏe xương khớp. Không chứa chất bảo quản, an toàn và đảm bảo vệ sinh tuyệt đối cho mèo. Phù hợp cho mèo từ 2 tháng tuổi trở lên. Thích hợp làm món ăn chính hoặc bổ sung trong khẩu phần ăn hàng ngày.', 25000, 'https://petservicehcm.com/wp-content/uploads/2024/10/gan-8.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Nekko gravy cho mèo con và mèo trưởng thành gói 70g', N'Pate mèo Nekko được sản xuất bởi công ty Unicord Public Co Nhật Bản với công nghệ tiên tiến và nguồn nguyên liệu tươi ngon chất lượng có nhà máy đặt tại Thái Lan
Xuất xứ: Thái Lan
Đối tượng sử dụng: Mèo mọi lứa tuổi
Công dụng: 
– Nuôi dưỡng làn da và duy trì một lớp lông mượt cho mèo.
– Nuôi dưỡng mắt và hệ thần kinh của mèo
– Tăng cường vitamin E và khoáng chất. Prebiotic giúp tốt hệ thống tiêu hóa.
– Omega 3 giúp cải thiện sự thèm ăn, hỗ trợ não, mắt và hệ miễn dịch.
– Không có thịt nhân tạo. Không chất bảo quản
– Đạt chất lượng: ISO9001, HACCP, GMP, IFS, EFIS
Hương vị: 

Cá tráp
Cá hồi
Gà
Tôm
Cá ngừ

Định lượng: 70gr
Hướng dẫn cho ăn: 
– Cho các bé ăn trực tiếp.
– Có thể trộn chung với hạt để kích thích khẩu vị các bé ăn dễ dàng hơn.
– Sau khi mở nên dùng hết trong 1 lần.
– Nếu sử dụng không hết, nên đậy kín nắp và bảo quản trong ngăn mát tủ lạnh tối đa 3 ngày. Khi cần dùng tiếp, hãy lấy ra khỏi tủ lạnh, để ở nhiệt độ phòng cho hết lạnh rồi mới cho ăn.
– Không cần hâm nóng.', 17000, 'https://petservicehcm.com/wp-content/uploads/2023/06/Khung-Shopee-54-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Nekko jelly cho mèo con và mèo trưởng thành gói 70g', N'Pate Nekko được làm từ 100% thực thịt và cá thật, chất lượng cao và đã được lựa chọn là tốt. Những chú mèo rất thích hương vị của cá, thịt và phô mai trong các loại pate Nekko. Sản phẩm chứa các vitamin và khoáng chất cần thiết cho mèo của bạn phát triển toàn diện.
Công dụng
Nuôi dưỡng làn da và duy trì một lớp lông mượt cho mèo.
Nuôi dưỡng mắt và hệ thần kinh của mèo
Tăng cường vitamin E và khoáng chất. Prebiotic giúp tốt hệ thống tiêu hóa.
Omega 3 giúp cải thiện sự thèm ăn, hỗ trợ não, mắt và hệ miễn dịch.
Không có thịt nhân tạo. Không chất bảo quản
Đạt chất lượng: ISO 9001, HACCP, GMP, IFS, EFIS
Bảo quản nơi khô ráo và mát mẻ. Sau khi mở, có thể sử dụng nhiều nhất trong vòng 3 ngày nếu bảo quản trong tủ lạnh.
Hương vị: 

Cá ngừ
Cá ngừ topping cá bào
Cá ngừ topping thanh cua
Cá ngừ topping thịt gà
Cá ngừ topping tôm
Cá ngừ topping cá cơm', 18000, 'https://petservicehcm.com/wp-content/uploads/2023/06/Khung-Shopee-53-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Snappy Tom Gourmers Series trái cây gói 70g', N'1. CÔNG DỤNG SẢN PHẨM
Dòng sản phẩm Snappy Tom® Gourmers™ được thiết kế rất đơn giản và ngon miệng; với 8 công thức chế biến giúp cung cấp các chất dinh dưỡng cần thiết khác nhau để duy trì sự phát triển thích hợp của mèo và khuyến khích mèo ăn nhiều hơn.
Có 8 loại:
– Cá ngừ dứa
– Cá ngừ trứng
– Cá ngừ táo
– Cá ngừ xoài
– Cá ngừ kiwi
– Cá ngừ lô hội
– Cá ngừ kỷ tử
– Cá ngừ trứng cá
2. THÀNH PHẦN: Cá ngừ, quả dứa, kiwi, trứng gà, lô hội. trứng cá, táo, kỷ tử, xoài, chất tạo gel, vitamin và các khoáng chất, taurine, nước
3. HDSD
Lượng thức ăn có thể được điều chỉnh tùy theo mức độ hoạt động của mèo, đảm bảo luôn cho mèo uống đủ nước.
Trọng lượng của mèo       Lượng thức ăn
0.5 – 1.0kg                        140-240g
1.0 – 3.0kg                        240-540g
3.0 – 4.0kg                        540-680g
> 4.0kg                              >680g', 0, 'https://petservicehcm.com/wp-content/uploads/2024/03/Khung-background-san-pham-shopee-mau-2024-03-29T133535.466.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Tell Me Creamy sốt kem cho mèo', N'SỐT TELLME DÀNH CHO MÈO
• Bảo vệ da lông toàn diện, làm giảm dấu hiệu của lão hoá, trẻ hoá các tế bào
• Giúp chắc khoẻ xương
• Đảm bảo cho thú cưng 1 sức khoẻ toàn diện.
• Thúc đẩy quá trình trao đổi chất. Đồng thời hỗ trợ hệ tiêu hoá ổn định.
• Cung cấp 1 lượng chất xơ tự nhiên đồng thời cũng giàu vitamin và chất dinh dưỡng thiết yếu', 18000, 'https://petservicehcm.com/wp-content/uploads/2023/11/Pawo-chan-8-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Tươi Googaga 160G Cho Chó và Mèo, Dinh Dưỡng', N'PETSERVICE – TRỌN VẸN TRẢI NGHIỆM.
1. THÔNG TIN SẢN PHẨM
– Pate tươi cho chó và mèo dạng gói 160g với nhiều hương vị đa dạng, phù hợp cho chó và mèo trên 6 tháng tuổi, đặc biệt là những bé kén ăn.
+ Pate tươi vị Heo + Bí : Heo + Bí và Dầu Cá
+ Pate tươi vị Bò + Bí : Bò + Bí Và Dầu Cá
+ Pate tươi vị Đà Điểu + Bí : Đà Điểu + Bí và dầu cá
+ Pate tươi vị Gà Bò : Gà và bò + dầu cá
– Dinh dưỡng tối ưu: Được chế biến từ nguyên liệu tươi như bò, gà, cá hồi, giúp cung cấp đầy đủ dưỡng chất cần thiết cho sự phát triển toàn diện của chó.
– Hỗ trợ tim mạch và lông mượt: Bổ sung Taurine và dầu cá, tăng cường sức khỏe tim mạch, hỗ trợ thị lực, và mang lại bộ lông mềm mượt, bóng khỏe.
– Tăng cường miễn dịch: Chứa vitamin và khoáng chất thiết yếu, giúp chó khỏe mạnh và năng động hơn.
– Dễ ăn, dễ tiêu hóa: Kết cấu mềm mịn, thơm ngon, phù hợp với chó kén ăn hoặc cần chế độ dinh dưỡng đặc biệt.
CHÍNH SÁCH CỦA PET SERVICE
– Sản phẩm cam kết giống 100% mô tả
– Mỗi sản phẩm khi được bán ra đều được kiểm tra cẩn thận trước khi gửi tới Quý khách.
– Hàng có sẵn, giao hàng ngay khi shop nhận được đơn
– Hỗ trợ đổi trả, hoàn tiền đối với sản phẩm lỗi theo chính sách Shopee
– Vui lòng quay lại video quá trình mở sản phẩm để được Pet Service hỗ trợ nhanh nhất trong các trường hợp phát sinh vấn đề về đơn hàng.', 22000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m40v0kbi3cp352_tn.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Wanpy cho mèo gói 80g', N'Pate cho mèo Wanpy cung cấp cho các bé mèo nhiều giá trị về dinh dưỡng

Mix nhiều vị
Cá Ngừ
Thịt Gà + Cá Tuyết
Thịt Gà + Thịt Tôm
Cá Cơm + Cá Ngừ
Thịt Cua + Thịt Gà
Cá Hồi + Cá Ngừ
Thịt Gà + Rau Củ

– Nguyên liệu được chọn lọc, 100% tự nhiên
– Cung cấp DHA
– Tăng cường hệ miễn dịch
– Nuôi dưỡng lông
– Với thành phần được làm từ thịt cá hồi, thịt ức gà, taurine, vitamin A,B2,B3, B5,D3,E…
– Tiện lợi không cần chế biến, sơ chế
– Bổ sung nước cho cơ thể mèo, phòng ngừa các bệnh sỏi thận, bàng quang
– Không chất phụ gia, chất bảo quản, phẩm màu
– Thành phần tự nhiên 100%, độ ẩm cao, dễ dàng pha trộn với các loại thức ăn khác giúp bé ăn ngon miệng hơn.
Kích thích thèm ăn, ăn ngon hơn
CÁCH BẢO QUẢN
– Nên cho ăn trong ngày
– Tránh ánh sáng mặt trời trực tiếp chiếu vào sản phẩm
– Tránh để sản phẩm ở gần nơi có nhiệt độ cao
– Nếu bao bì đã mở,sản phẩm cần được đóng kín,bảo quản trong ngăn mát tủ lạnh và hâm nóng trước khi cho thú cưng ăn vào lần sau', 13000, 'https://petservicehcm.com/wp-content/uploads/2023/06/khung-Shopee-2-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Pate Wanpy dạng nắp vặn tiện lợi gói 90g (giao vị ngẫu nhiên)', N'1. ĐẶC ĐIỂM SẢN PHẨM
Pate cho mèo Wanpy cung cấp cho các bé mèo nhiều giá trị về dinh dưỡng
Gồm các vị:
– Cá Ngừ
– Thịt Gà + Cá Tuyết
– Thịt Gà + Thịt Tôm
– Cá Hồi + Cá Ngừ
– Nguyên liệu được chọn lọc, 100% tự nhiên
– Cung cấp DHA
– Tăng cường hệ miễn dịch
– Nuôi dưỡng lông
– Với thành phần được làm từ thịt cá hồi, thịt ức gà, taurine, vitamin A,B2,B3, B5,D3,E…
– Tiện lợi không cần chế biến, sơ chế
– Bổ sung nước cho cơ thể mèo, phòng ngừa các bệnh sỏi thận, bàng quang
– Không chất phụ gia, chất bảo quản, phẩm màu
– Thành phần tự nhiên 100%, độ ẩm cao, dễ dàng pha trộn với các loại thức ăn khác giúp bé ăn ngon miệng hơn.
Kích thích thèm ăn, ăn ngon hơn
2. CÁCH BẢO QUẢN
– Nên cho ăn trong ngày
– Tránh ánh sáng mặt trời trực tiếp chiếu vào sản phẩm
– Tránh để sản phẩm ở gần nơi có nhiệt độ cao
– Nếu bao bì đã mở,sản phẩm cần được đóng kín,bảo quản trong ngăn mát tủ lạnh và hâm nóng trước khi cho thú cưng ăn vào lần sau', 23000, 'https://petservicehcm.com/wp-content/uploads/2023/08/bat-an-inox-88-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Raw hỗn hợp HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service.', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Raw hỗn hợp xay rau HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ cá thịt tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 35000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thịt Bò xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service.', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Bò xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
• Được làm từ 100% nguyên liệu tự nhiên từ thịt bò tươi và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 45000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-5.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thịt Chim cút xay rau củ HG Food 400g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Chim cút xay rau củ HG Food cho thú cưng 400g.
1.2. ƯU ĐIỂM SẢN PHẨM
– Lành tính, phù hợp với các bé bị viêm da, nấm, ngứa.
– Được làm từ 100% nguyên liệu tự nhiên từ thịt chim cút và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Bổ sung Vitamin A, B1, B2 và các khoáng chất cần thiết.
– Rau củ hỗ trợ bổ sung chất xơ cho thú cưng hiệu quả.
– Dễ dàng chế biến, hương vị thơm ngon phù hợp với mọi thể trạng của thú cưng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông thịt bằng cách ngâm vào nước 30 phút hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt đôi cây thịt cần chết biến hoặc cắt phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chính đều.
– Bước 6: Để nguội và cho các “Boss” thưởng thức.
3. BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
4. LƯU Ý
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không đun lại sản phẩm 2,3 lần.', 48000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-9-1.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thịt Nạc heo xay rau củ HG Food 800g cho thú cưng thơm ngon dinh dưỡng Pet Service', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thịt Nạc heo xay rau củ HG Food cho thú cưng 800g.
1.2. ƯU ĐIỂM SẢN PHẨM
•  Lành tính, phù hợp với các bé bị viêm da, dị ứng.
•  Được làm từ 100% nguyên liệu tự nhiên từ thịt nạc heo và rau củ sạch.
• Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
• Cải thiện khả năng sinh sản.
• Tăng cường hệ miễn dịch.
• Giúp mượt lông, khoẻ da.
• Dễ dàng kiểm soát cân nặng.
• Hạn chế triệu chứng dị ứng.
• Hỗ trợ bài tiết.
• Giảm mùi hôi của Pets.
• Làm sạch răng một cách tự nhiên.
• Dễ tiêu hoá.
Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG. Vì sản phẩm này thuần Heo, không pha thêm Gà hay nội tạng nên giúp Chó mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG
• Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
• Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
• Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
• Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
• Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
• Bước 6: Sau đó có thể sử dụng.
3. HƯỚNG DẪN BẢO QUẢN
– Phần thịt chưa chế biến hãy bảo quản trong tủ đông từ -18 độ C đến -10 độ C.
– Phần thịt đã chế biến hãy bảo quản trong ngăn mát tủ lạnh, có thể dùng cho các bữa tiếp theo.
– Không sử dụng thịt đã bảo quản quá 3 ngày.
– Không nấu lại 2-3 lần.
3. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
• Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
• Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
• Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
• Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
• Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
• Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
• Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 75000, 'https://petservicehcm.com/wp-content/uploads/2024/08/gan-7.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức Ăn Chó Mèo – Pate Bò Hầm Củ Dền 400g – HG FOOD – PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
+ Pate Bò hầm củ dền 400g.
**Sản phẩm bao bì mới hộp nhôm, phù hợp để quay lò vi sóng
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn Pate Heo Hầm Cải Đỏ của bên HG. Vì sản phẩm 100% thịt Heo không pha thêm Gà và các nội tạng khác nên sẽ hỗ trợ thú cưng mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông sản phẩm bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt phần Pate vừa đủ cho thú cưng thưởng thức liền hoặc hâm nóng lại đều được.
3. BẢO QUẢN
– Bảo quản sản phẩm trong tủ đông từ -18 độ C đến -10 độ C nếu chưa sử dụng.
– Bảo quản sản phẩm trong ngăn mát tủ lạnh tối đa 2 ngày sau khi rã đông.
4. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
– Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
– Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
– Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
– Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
– Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
– Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
– Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/BO-800x715.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức Ăn Chó Mèo – Pate Cá Ngừ Hầm Bông Cải 400g – HG FOOD – PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
+ Pate Cá Ngừ Hầm Bông Cải 400g.
**Sản phẩm bao bì mới hộp nhôm, phù hợp để quay lò vi sóng
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn Pate Heo Hầm Cải Đỏ của bên HG. Vì sản phẩm 100% thịt Heo không pha thêm Gà và các nội tạng khác nên sẽ hỗ trợ thú cưng mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông sản phẩm bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt phần Pate vừa đủ cho thú cưng thưởng thức liền hoặc hâm nóng lại đều được.
3. BẢO QUẢN
– Bảo quản sản phẩm trong tủ đông từ -18 độ C đến -10 độ C nếu chưa sử dụng.
– Bảo quản sản phẩm trong ngăn mát tủ lạnh tối đa 2 ngày sau khi rã đông.
4. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
– Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
– Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
– Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
– Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
– Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
– Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
– Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/CA-NGU-800x710.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức Ăn Chó Mèo – Pate Cá Ngừ Mix Bò 400g – HG FOOD – PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
+ Pate Cá Ngừ Mix Bò 400g.
**Sản phẩm bao bì mới hộp nhôm, phù hợp để quay lò vi sóng
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn Pate Heo Hầm Cải Đỏ của bên HG. Vì sản phẩm 100% thịt Heo không pha thêm Gà và các nội tạng khác nên sẽ hỗ trợ thú cưng mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông sản phẩm bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt phần Pate vừa đủ cho thú cưng thưởng thức liền hoặc hâm nóng lại đều được.
3. BẢO QUẢN
– Bảo quản sản phẩm trong tủ đông từ -18 độ C đến -10 độ C nếu chưa sử dụng.
– Bảo quản sản phẩm trong ngăn mát tủ lạnh tối đa 2 ngày sau khi rã đông.
4. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
– Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
– Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
– Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
– Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
– Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
– Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
– Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 50000, 'https://petservicehcm.com/wp-content/uploads/2025/05/MIX-BO-800x711.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức Ăn Chó Mèo – Pate Gà Hầm Rau Củ 400g – HG FOOD – PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
+ Pate Gà Hầm Rau Củ 400g.
**Sản phẩm bao bì mới hộp nhôm, phù hợp để quay lò vi sóng
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn Pate Heo Hầm Cải Đỏ của bên HG. Vì sản phẩm 100% thịt Heo không pha thêm Gà và các nội tạng khác nên sẽ hỗ trợ thú cưng mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông sản phẩm bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt phần Pate vừa đủ cho thú cưng thưởng thức liền hoặc hâm nóng lại đều được.
3. BẢO QUẢN
– Bảo quản sản phẩm trong tủ đông từ -18 độ C đến -10 độ C nếu chưa sử dụng.
– Bảo quản sản phẩm trong ngăn mát tủ lạnh tối đa 2 ngày sau khi rã đông.
4. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
– Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
– Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
– Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
– Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
– Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
– Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
– Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/GA-800x702.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức Ăn Chó Mèo – Pate Heo Hầm Cải Đỏ 400g – HG FOOD – PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
+ Pate Heo hầm cải đỏ 400g.
**Sản phẩm bao bì mới hộp nhôm, phù hợp để quay lò vi sóng
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn Pate Heo Hầm Cải Đỏ của bên HG. Vì sản phẩm 100% thịt Heo không pha thêm Gà và các nội tạng khác nên sẽ hỗ trợ thú cưng mau hết dị ứng.
2. HƯỚNG DẪN SỬ DỤNG.
– Bước 1: Rã đông sản phẩm bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, cắt phần Pate vừa đủ cho thú cưng thưởng thức liền hoặc hâm nóng lại đều được.
3. BẢO QUẢN
– Bảo quản sản phẩm trong tủ đông từ -18 độ C đến -10 độ C nếu chưa sử dụng.
– Bảo quản sản phẩm trong ngăn mát tủ lạnh tối đa 2 ngày sau khi rã đông.
4. PHÂN BIỆT SẢN PHẨM HG PETFOOD THẬT/GIẢ
+ Sản phẩm HG thật:
– Bao bì in sắc nét, logo HG Petfood nằm góc trái trên cùng, kèm ngày sản xuất (NSX) và hạn sử dụng (HSD) rõ ràng, không nhòe mờ.
– Sản phẩm chính hãng được phân phối qua các cửa hàng chính thức, đảm bảo nguồn gốc rõ ràng và được kiểm soát chặt chẽ.
– Bao bì có đầy đủ logo và chứng nhận quốc tế ISO 9001 và HACCP, in rõ ràng bên góc phải sản phẩm.
– Sản phẩm thật liệt kê chi tiết các thành phần dinh dưỡng.
+ Sản phẩm HG giả:
– Bao bì in mờ, logo không rõ, thông tin NSX và HSD thiếu hoặc in nhòe, khó đọc.
– Sản phẩm giả thường không có nguồn gốc xuất xứ rõ ràng, được bán trôi nổi trên thị trường mà không kiểm soát.
– Sản phẩm giả thường thiếu các chứng nhận quốc tế như ISO và HACCP, hoặc nếu có thường in mờ, sai.', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/HEO-800x708.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Bò tuyệt đỉnh 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 60000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawt0x90e.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Gà siêu chất 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lb6smmn27.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim bò hảo hạng 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 60000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawvu25c6.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim Bò Hảo Hạng,300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 46000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawvu25c6.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim Heo Thượng Hạng 300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 40000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m44q3j16yojz23.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Bò tuyệt đỉnh 300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Pet food cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 46000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawt0x90e.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Gà siêu chất 300g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 39000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lb6smmn27.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn dinh dưỡng cho chó và mèo con HG Petfood PET SERVICE – Tim heo thượng hạng 500g', N'1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Thức ăn dinh dưỡng HG Petfood cho MÈO VÀ CHÓ CON 300g và 500g.
+ Gà Siêu Chất
+ Tim Heo Thượng Hạng
+ Tim Bò Hảo Hạng
+ Bò Tuyệt Đỉnh
1.2. ƯU ĐIỂM SẢN PHẨM
– Được làm từ 100% nguyên liệu tự nhiên từ thịt tươi và rau củ sạch.
– Cung cấp nguồn Protein dồi dào, đảm bảo năng lượng và dưỡng chất.
– Cải thiện khả năng sinh sản.
– Tăng cường hệ miễn dịch.
– Giúp Mượt lông, khoẻ da.
– Dễ dàng kiểm soát cân nặng.
– Hạn chế triệu chứng dị ứng.
– Hổ trợ bài tiết.
– Giảm mùi hôi của Pets.
– Làm sạch răng một cách tự nhiên.
– Dễ tiêu hoá.
*Đặc biệt: Đối với Chó/Mèo đang bị dị ứng thì cho ăn cây Heo Dinh Dưỡng của bên HG.
2.HƯỚNG DẪN SỬ DỤNG
– Bước 1: Rã đông bằng cách ngâm vào nước 30 phút, hoặc bỏ vào ngăn mát tủ lạnh trước 12 tiếng.
– Bước 2: Sau khi rã đông, chia đôi cây thịt cần chế biến. Hoặc chia phần thịt cần nấu theo nhu cầu.
– Bước 3: Đun sôi một lượng nước vừa đủ để nấu thịt.
– Bước 4: Bỏ thịt đã rã đông vào nước đã đun sôi, khuấy đều tay để tránh bị khét.
– Bước 5: Khuấy đều thịt kết hợp với vớt bọt để đảm bảo thịt được chín đều.
– Bước 6: Sau đó có thể sử dụng', 55000, 'https://petservicehcm.com/wp-content/uploads/2025/05/vn-11134207-7ras8-m0jo2lawt18v81.webp', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Thức ăn ướt Whiskas 1+ nhiều vị cho mèo trưởng thành túi 80g', N'Có các mùi vị:
– Cá saba
– Cá biển
– Cá ngừ
– Cá trắng
– Cá thu
– Gà
1. ĐẶC ĐIỂM SẢN PHẨM
Mèo từ 1-6 tuổi cần có nhiều thời gian vui chơi và chế độ ăn uống cân bằng để luôn giữ được vóc dáng cân đối, khỏe mạnh. Mèo là loài ăn thịt trong khi con người là loài ăn tạp, vì vậy nhu cầu protein của mèo cao gấp 2 lần của chúng ta. Mèo cũng cần 41 dưỡng chất thiết yếu để có sức khỏe tối ưu. Hiểu rõ nhu cầu dinh dưỡng của mèo, WHISKAS luôn thiết kế các sản phẩm với công thức đặc biệt để mang lại hệ dưỡng chất toàn diện và cân bằng.
2. CÔNG DỤNG SẢN PHẨM
– Công thức đặc biệt dành cho mèo từ 1 tuổi trở lên.
– Bổ sung chất béo omega 3 & 6 và kẽm cho bộ lông khỏe mạnh và bóng mượt.
– Kết hợp thêm vitamin A và taurine cho thị lực khỏe mạnh.
– Giàu protein từ cá thật, bao gồm chất béo, vitamin và khoáng chất, giúp bé mèo phát triển cân đối và luôn vui vẻ.
– Chứa chất chống oxy hóa (vitamin E và selen) cho hệ miễn dịch khỏe mạnh.', 17000, 'https://petservicehcm.com/wp-content/uploads/2024/03/z6072559159737_7956433d10616a6128f2f94955860633.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (21, N'Xốt dinh dưỡng cho mèo TELLME', N'Xốt và Pate dinh dưỡng cho mèo TELLME
CÔNG DỤNG
• 100% làm từ nguồn nguyên liệu chất liệu cao như thịt bò tươi hoặc thịt gà tươi và phomai hảo hạng đi kèm rau củ tươi và Vitamin D, E, Omega 3, Omega 6, nước hầm xương tạo nên nước sốt Tellme đầy dinh dưỡng.
• Omega 3 và 6 giúp bảo vệ da lông toàn diện, làm giảm dấu hiệu của lão hoá, trẻ hoá các tế bào . Hồi phục các vùng bị thương nhanh chóng. Thúc đẩy quá trình trao đổi chất. Đồng thời hỗ trợ hệ tiêu hoá ổn định.
• Vitamin D, E giúp chắc khoẻ xương. Ngăn sự lão hoá của các tế bào và dây thần kinh.
• Protein từ thịt bò hay thịt gà đảm bảo cho thú cưng 1 sức khoẻ toàn diện.
• Rau củ tươi cung cấp 1 lượng chất xơ tự nhiên đồng thời cũng giàu vitamin và chất dinh dưỡng thiết yếu.
• Sốt Tellme có 5 hương vị khác nhau cho boss thay đổi khẩu vị: vị gà-phomai-rau, vị bò và rau, vị cá ngừ-rau, vị cá hồi-gà-rau, vị vịt-rau củ
THÀNH PHẦN
Thịt Bò, thịt ức gà, pho mai, nước hầm xương, vitamin D, E, Omega 3, Omega 6, canxi chiết xuất từ vỏ trứng, cà rốt, đậu Hà Lan, khoai lang tươi.
HƯƠNG VỊ

Heo
Gà phô mai
Bò rau củ
Cá ngừ & gà
Cá hồi & gà
Vịt

CÁCH DÙNG
• Ăn trực tiếp hoặc trộn với hạt và cơm.
• Mèo lớn ngày 1-2 gói.
• Mèo bé ngày 1 gói.
ĐÓNG GÓI: gói 130g', 21000, 'https://petservicehcm.com/wp-content/uploads/2022/07/khung-Shopee-5-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (22, N'Hạt Reflex cho mèo adult Sterilised 1.5kg- cho mèo triệt sản', N'– Thức ăn khô cho mèo với công thức cân bằng và hoàn chỉnh dành cho giống mèo trưởng thành.
– Dành cho tất cả giống mèo trên 12 tháng tuổi. Thể trọng từ 1kg – 8kg.
@ Công thức đặc biệt dành cho các giống mèo trưởng thành (đặc biệt giúp tiệt trùng đường ruột ở mèo).
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất.', 197000, 'https://petservicehcm.com/wp-content/uploads/2023/05/Vong-co-32-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (22, N'Hạt Reflex Urinary cho mèo – 1.5Kg', N'Hạt Reflex Urinary cho mèo (tiết niệu) – 1.5Kg
THỨC ĂN DÀNH CHO MÈO BỊ THẬN, HỖ TRỢ TIẾT NIỆU
Sử dụng cho: 
– Mèo bị viêm bàng quang do vi khuẩn
– Mèo cần điều trị sỏi Struvite, Canxi Oxalate sỏi niệu.
– Thức ăn khô cho mèo siêu cao cấp với công thức cân bằng và hoàn chỉnh dành cho giống mèo trưởng thành.
– Dành cho tất cả giống mèo trên 12 tháng tuổi. Thể trọng từ 2kg – 10kg.
Ưu điểm đặc biệt
@ Công thức đặc biệt cao cấp dành cho các giống mèo trưởng thành (đặc biệt trị bệnh tiết niệu ở mèo).
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Xylo-oligosaccharides (XOS) giúp giảm các vấn đề như chất béo không mong muốn và đường huyết ở mèo với giá trị Calo xấp xỉ bằng không. Cải thiện quá trình tiêu hóa và chuyển hóa thức ăn bằng cách cải thiện hệ vi khuẩn đường ruột. Tác dụng chống oxy hóa tự nhiên.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất. – Trọng lượng: 1,5kg / gói
– Xuất xứ: Thổ Nhĩ Kỳ – Lider Pet Food
– Hướng dẫn sử dụng: ghi trên bao bì.
– Cách cho ăn: Chia làm 2-3 bữa ăn / ngày', 209000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Vong-co-29-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (22, N'Hũ BETA AMIN ECOPETS bổ sung dinh dưỡng tăng đề kháng tránh bệnh vặt tăng cân khoẻ tránh GBC ở mèo – 50g PET SERVICE', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
Bột Dinh Dưỡng Tăng Đề Kháng Beta Amin cho chó mèo.
1.2. ƯU ĐIỂM
– Beta Amin giúp tăng cường hệ miễn dịch, phòng ngừa các bệnh vi khuẩn, virus.
– Chứa protein và axit amin thiết yếu, hỗ trợ sức khỏe toàn diện cho thú cưng.
– Tăng khả năng miễn dịch, bảo vệ thú cưng khỏi các bệnh truyền nhiễm nguy hiểm.
+ HƯỚNG DẪN SỬ DỤNG
– Trộn Beta Amin cùng thức ăn hạt, pate hoặc cho thú cưng ăn trực tiếp.
– Để đạt hiệu quả tối đa, có thể sử dụng gấp đôi liều lượng khi thú cưng đang bị bệnh hoặc vi khuẩn xâm nhập.
– Sử dụng đều đặn để bảo vệ sức khỏe lâu dài cho thú cưng.', 120000, 'https://petservicehcm.com/wp-content/uploads/2024/12/gan-1.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (22, N'Reflex Adult Hairball & Indoor cho mèo – 1.5Kg', N'Refex Adult Hairball & Indoor cho mèo (búi lông)
Công dụng
Thức ăn khô cho mèo từ 12 tháng tuổi, hỗ trợ tiêu búi lông và giảm mùi hôi phân với vị cá hồi hấp dẫn
Xuất sứ: Thổ Nhĩ Kỳ
Trọng Lượng: túi 1.5kg
Ưu điểm đặc biệt
@ Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
@ Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi. Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
@ Bổ sung Vitamin A, D3, E, C và khoáng chất', 240000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Vong-co-28-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (22, N'REFLEX SKIN CARE – Thức ăn khô cho mèo chăm sóc, tái tạo và phục hồi lông da', N'ĐẶC ĐIỂM SẢN PHẨM
Reflex Plus Skin Care With Salmon
Thức ăn khô cho Mèo chăm sóc phục hồi và tái tạo lông da
Xuất xứ: Thổ Nhĩ Kỳ
CÔNG DỤNG SẢN PHẨM

Giúp chăm sóc phục hồi và tái tạo lông da
Sự cân bằng của Omega 3 & Omega 6 đã đạt được bằng cách sử dụng hạt lanh giúp lông bóng mượt.
Chiết xuất cây Yucca giúp tăng khả năng hấp thụ dinh dưỡng, kiểm soát mùi.
Men bia giúp hệ thống miễn dịch được tăng cường & năng suất vật nuôi được cải thiện.
Xylo-oligosaccharides (XOS) giúp giảm các vấn đề như chất béo không mong muốn và đường huyết
Cải thiện quá trình tiêu hóa và chuyển hóa thức ăn bằng cách cải thiện hệ vi khuẩn đường ruột
Bổ sung Vitamin A, D3, E, C và khoáng chất

HƯỚNG DẪN BẢO QUẢN
Bảo quản nơi khô ráo, tránh tiếp xúc trực tiếp ánh nắng mặt trời.', 252000, 'https://petservicehcm.com/wp-content/uploads/2023/08/bat-an-inox-86-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (12, N'Bàn cào móng chữ M cho mèo', N'Bàn cào móng chữ M cho mèo
Công dụng:

Chất liệu không tổn thương ngón chân của mèo.
Làm bằng bảng lót sóng, giống như một chiếc giường nằm chơi cho bé
Bền và mang lại nhiều niềm vui cho mèo của bạn.
Giúp đồ đạc của bạn không bị trầy xước.
Thỏa mãn bản năng gãi tự nhiên của mèo.
Trọng lượng nhẹ với thiết kế sóng tổ ong.
Mặt hàng này có lỗ xỏ lỗ, có thể treo vào tường hoặc cửa.

Mô tả sản phẩm
100% hàng mới, chất lượng cao.
Chất liệu: Giấy gợn.
Kích thước: Xấp xỉ 45 * 24 * 4,5 cm', 65000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Anh-San-pham-Pet-Services-8-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (12, N'Cần câu dây thép cho mèo', N'THÔNG TIN SẢN PHẨM
THIẾT KẾ
– Cần câu đồ chơi cho mèo là một trong những sản phẩm rất được ưu chuộng bởi tính vui nhộn, khả năng giải trí cao.
– Được thiết kế gồm 1 dây thép dài sơn tĩnh điện, gắn lông vũ nhiều màu rủ xuống.
– Phần cán thiết kế thông minh có thể gấp tròn lại chỉ còn 35cm, tiện mang đi ra vườn hoặc dã ngoại.
CÔNG DỤNG
– Cần câu có lông vũ bay bay, chuông kêu nên rất bắt mắt, mèo dễ dàng bị thu hút và bạn có thể bắt đầu chơi đùa với những pha rượt đuổi rất ngộ nghĩnh.
– Đồng thời, với những trò chơi vận động như trên, thú cưng của bạn được rèn luyện kỹ năng phản xạ, giảm stress và tăng cường sức khỏe vật nuôi
– Có thể gấp gọn tiện mang đi
CÁCH SỬ DỤNG
– Có thể cho mèo chơi trong phòng nhỏ hoặc sân rộng. Chỉ cần bạn có thời gian giải trí với boss cưng là đủ
– Khi dây câu buông ra sẽ dài hơn 1m, cho boss yêu thỏa sức chơi đùa.', 30000, 'https://petservicehcm.com/wp-content/uploads/2023/02/Khung-Shopee-II-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (12, N'Đồ chơi cá nhồi bông', N'Đồ chơi cá nhồi bông cho Chó Mèo 
         Mô tả sản phẩm:

Chiều dài: khoảng 20cm
Cá bông cho mèo bằng vải, bên trong nhồi bông.
Chất liệu vải mịn được thiết kế 3D chân thật
Dày dặn khó rách mà vẫn mềm mại để các bé nhai thoải mái không đau răng.
Chất liệu cao cấp không chứa chất độc hại

Công dụng:
Giúp mèo giảm stress khi chơi cùng cá.
Cá bông cho mèo vừa là đồ chơi để nhai, để ôm của bé, vừa giúp bé hứng thú. Một món đồ chơi hoàn hảo cho bé mèo của bạn.', 45000, 'https://petservicehcm.com/wp-content/uploads/2022/06/30-ca-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (12, N'Tháp banh 3 tầng cho mèo', N'Tháp banh 3 tầng cho mèo
Đặc điểm
Màu: Xanh
Chất liệu: Nhựa
Tính năng: Dễ sử dụng
Đối tượng: Mèo
Size: 25x14x16cm
Công dụng
Khi bạn đang ở ngoài trời, mèo của bạn di chuyển về phía đồ chơi của chúng. Móng vuốt xoay những quả bóng và phát ra tiếng khiến thú cưng của bạn tự hỏi điều gì xảy ra. Sau đó, chúng có thể xoay các quả bóng khác nhau theo những cách khác nhau và thậm chí chơi cả ngày
Tháp banh với 3 tầng đồ chơi cùng với 3 quả bóng có thể thu hút thú cưng của bạn.
Bạn có thể để đồ ăn cho thú cưng trên đó
Đồ chơi có thể tháo rời có thể mang lại nhiều cách chơi và dễ dàng hơn để làm sạch và mang theo. Tách nó ra và sau đó bạn có thể làm sạch và lưu trữ nó theo ý muốn.
Màu sắc sống động có thể thu hút sự chú ý của thú cưng của bạn và sau đó chúng có thể đi về phía đồ chơi và chơi nó.
Xem thêm sản phẩm tại PET SERVICE:
Shopee: https://shopee.vn/petservicehcm', 80000, 'https://petservicehcm.com/wp-content/uploads/2022/06/80-thap-banh-meo-3-tang-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (12, N'Tuýp cỏ mèo Hahale cho mèo 40ml', N'Tuýp cỏ mèo Hahale cho mèo 40ml
Cỏ mèo (cỏ bạc hà mèo) còn được gọi với cái tên là Catnip.
Đây là loại cỏ có nguồn gốc từ miền Nam và miền Đông của Châu Âu, Trung Á,…loại thực vật này có sức hấp dẫn rất mãnh liệt với phần lớn loài mèo.
Đặc biệt chúng còn sở hữu rất nhiều công dụng đặc biệt có ích cho mèo. Chính vì vậy, cỏ mèo hiện đang là một trong những món đồ không thể thiếu trong bộ sưu tập của các boss.

Chứa nhiều chất xơ, giúp mèo dễ tiêu hóa.
Nhiều vitamin và khoáng chất, cân bằng dinh dưỡng
Kích thích mèo khạc lông ra ngoài. Hạn chế chướng hơi, sình bụng
Giảm stress cho mèo hiệu quả

Hướng dẫn sử dụng:

CÁCH 1: Cho ăn trực tiếp bằng cách trộn vào thức ăn hoặc cho mèo ăn trực tiếp.
CÁCH 2: Cho ngửi trực tiếp
CÁCH 3: Xay nhỏ vụn ra cho vào đồ chơi cho mèo.
Mèo con dưới 3 tháng có thể không có phản ứng với cỏ bạc hà.
Một số bé mèo không thích cỏ bạc hà nên sẽ không có phản ứng.', 35000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Anh-San-pham-Pet-Services-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Dây dắt chó mèo cuốn tự động 3m', N'1. ĐẶC ĐIỂM SẢN PHẨM
– Dây dắt Chó Mèo tự động có thể điều chỉnh độ dài ngắn theo hướng di chuyển của chó mèo với nút bấm thông minh; mà không bị xoắn, vướng.
– Sản phẩm có trọng lượng rất nhẹ, giúp các bạn cầm không bị mỏi tay.
– Sản phẩm được làm từ sợi polyester chắc chắn, dày dặn; vỏ làm từ nhựa ABS có độ bền cao; sẽ giúp bạn giữ chặt và theo sát thú cưng của mình mọi lúc mọi nơi.
– Sản phẩm phù hợp khi đưa những bé thú cưng ra ngoài đi dạo hoặc chạy bộ.
– Nguồn gốc xuất xứ: Trung Quốc
2. THÔNG TIN SẢN PHẨM
– Chất liệu: ABS, sợi polyester
– Độ dài : 3m Phù hợp sử dụng cho chó mèo từ 1 – 10kg.
– Màu sắc: Xanh dương/ Vàng.', 79000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Khung-background-san-pham-shopee-mau-9.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Dây dắt kèm vòng cổ – 1.0', N'N/A', 70000, 'https://petservicehcm.com/wp-content/uploads/2024/03/dAY-DAT-KEM-VONG-CO.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'DÂY DẮT KÈM VÒNG CỔ 1.5', N'N/A', 80000, 'https://petservicehcm.com/wp-content/uploads/2024/03/dAY-DAT-KEM-VONG-CO.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Dây dắt kèm yếm 1.0', N'N/A', 80000, 'https://petservicehcm.com/wp-content/uploads/2024/03/day-dat.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Dây dắt kèm yếm 1.5', N'N/A', 90000, 'https://petservicehcm.com/wp-content/uploads/2024/03/day-dat.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Dây dắt kèm yếm 2.0', N'N/A', 100000, 'https://petservicehcm.com/wp-content/uploads/2024/03/day-dat.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Gối chống liếm size L', N'Kích thước:
– XS: 10-18cm, ~1kg
– S: 12-22cm, ~2,5kg
– M: 16-29cm, ~5kg
– L: 19-36cm, ~7kg', 95000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Pawo-chan-chan-7-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Gối chống liếm size M', N'Kích thước:
– XS: 10-18cm, ~1kg
– S: 12-22cm, ~2,5kg
– M: 16-29cm, ~5kg
– L: 19-36cm, ~7kg', 85000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Pawo-chan-chan-7-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Gối chống liếm size S', N'Kích thước:
– XS: 10-18cm, ~1kg
– S: 12-22cm, ~2,5kg
– M: 16-29cm, ~5kg
– L: 19-36cm, ~7kg', 75000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Pawo-chan-chan-7-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Gối chống liếm size XS', N'Kích thước:
– XS: 10-18cm, ~1kg
– S: 12-22cm, ~2,5kg
– M: 16-29cm, ~5kg
– L: 19-36cm, ~7kg', 65000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Pawo-chan-chan-7-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm chó mèo – số 5', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 50000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm chó mèo- số 3', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 60000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm chó mèo- số 4', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 55000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm chó mèo-số 6', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 45000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm chó mèo-số 7', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 40000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm- số 1', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 70000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Loa đeo chống liếm- số 2', N'Loa đeo chống liếm chó mèo
           Công dụng:

Vòng chống liếm cho chó mèo dùng trong trường hợp chó mèo dữ hay cắn người lạ.
Bảo vệ khi tiêm thuốc sẽ không còn bị chó cưng quay lại cắn vào tay.
Ngăn thú cưng liếm vết thương.
Ngăn thú cưng ăn uống bậy ngoài đường.
Sản phẩm bằng chất liệu bằng nhựa, an toàn, vững chắc. Thiết kế thẩm mỹ.
Kích thước:
+ Số 1 Dành cho cổ 41-48cm Độ rộng 22cm
+ Số 2 Dành cho cổ 36-42cm Độ rộng 20cm
+ Số 3 Dành cho cổ 25-36cm Độ rộng 13.5cm
+ Số 4 Dành cho cổ 26-32cm Độ rộng 13cm
+ Số 5 Dành cho cổ 22-28cm Độ rộng 12cm
+ Số 6 Dành cho cổ 18-22cm Độ rộng 11cm
+ Số 7 Dành cho cổ 12-18cm Độ rộng 8cm', 65000, 'https://petservicehcm.com/wp-content/uploads/2022/06/Pawo-chan-chan-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Vòng cổ nhiều màu họa tiết – 1.5', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Vòng cổ cho chó mèo nhiều màu, nhiều size là phụ kiện thời trang phù hợp cho cả chó và mèo. Sản phẩm có nhiều kích cỡ từ nhỏ đến lớn, với thiết kế chắc chắn và chất liệu mềm mại, giúp thú cưng thoải mái khi đeo. Vòng cổ có nhiều màu sắc tươi sáng, phù hợp với sở thích và phong cách riêng của mỗi thú cưng.
1.2. ƯU ĐIỂM
– Đa dạng kích thước, dễ điều chỉnh để vừa vặn với cổ thú cưng.
– Chất liệu bền, mềm mại, không gây kích ứng da.
– Khóa an toàn chắc chắn, dễ tháo mở. – Màu sắc bắt mắt, giúp thú cưng trở nên nổi bật và phong cách.
2. HƯỚNG DẪN SỬ DỤNG
– Lựa chọn kích cỡ vòng cổ phù hợp với cổ của chó/mèo.
– Đeo vòng cổ sao cho vừa khít nhưng vẫn tạo sự thoải mái, không quá chặt.
– Điều chỉnh dây đeo để đảm bảo an toàn khi thú cưng hoạt động.
– Vệ sinh vòng cổ thường xuyên để giữ độ sạch sẽ và bền đẹp.', 30000, 'https://petservicehcm.com/wp-content/uploads/2024/03/z6072559131074_0f377e6ed6b5bc1c398a3f91455b5e61.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Vòng cổ nhiều màu họa tiết – 2.0', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Vòng cổ cho chó mèo nhiều màu, nhiều size là phụ kiện thời trang phù hợp cho cả chó và mèo. Sản phẩm có nhiều kích cỡ từ nhỏ đến lớn, với thiết kế chắc chắn và chất liệu mềm mại, giúp thú cưng thoải mái khi đeo. Vòng cổ có nhiều màu sắc tươi sáng, phù hợp với sở thích và phong cách riêng của mỗi thú cưng.
1.2. ƯU ĐIỂM
– Đa dạng kích thước, dễ điều chỉnh để vừa vặn với cổ thú cưng.
– Chất liệu bền, mềm mại, không gây kích ứng da.
– Khóa an toàn chắc chắn, dễ tháo mở. – Màu sắc bắt mắt, giúp thú cưng trở nên nổi bật và phong cách.
2. HƯỚNG DẪN SỬ DỤNG
– Lựa chọn kích cỡ vòng cổ phù hợp với cổ của chó/mèo.
– Đeo vòng cổ sao cho vừa khít nhưng vẫn tạo sự thoải mái, không quá chặt.
– Điều chỉnh dây đeo để đảm bảo an toàn khi thú cưng hoạt động.
– Vệ sinh vòng cổ thường xuyên để giữ độ sạch sẽ và bền đẹp.', 35000, 'https://petservicehcm.com/wp-content/uploads/2024/03/z6072559131074_0f377e6ed6b5bc1c398a3f91455b5e61.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (14, N'Vòng cổ nhiều màu họa tiết 1.0', N'PET SERVICE – TRỌN VẸN TRẢI NGHIỆM
1. THÔNG TIN SẢN PHẨM
1.1. MÔ TẢ SẢN PHẨM
– Vòng cổ cho chó mèo nhiều màu, nhiều size là phụ kiện thời trang phù hợp cho cả chó và mèo. Sản phẩm có nhiều kích cỡ từ nhỏ đến lớn, với thiết kế chắc chắn và chất liệu mềm mại, giúp thú cưng thoải mái khi đeo. Vòng cổ có nhiều màu sắc tươi sáng, phù hợp với sở thích và phong cách riêng của mỗi thú cưng.
1.2. ƯU ĐIỂM
– Đa dạng kích thước, dễ điều chỉnh để vừa vặn với cổ thú cưng.
– Chất liệu bền, mềm mại, không gây kích ứng da.
– Khóa an toàn chắc chắn, dễ tháo mở. – Màu sắc bắt mắt, giúp thú cưng trở nên nổi bật và phong cách.
2. HƯỚNG DẪN SỬ DỤNG
– Lựa chọn kích cỡ vòng cổ phù hợp với cổ của chó/mèo.
– Đeo vòng cổ sao cho vừa khít nhưng vẫn tạo sự thoải mái, không quá chặt.
– Điều chỉnh dây đeo để đảm bảo an toàn khi thú cưng hoạt động.
– Vệ sinh vòng cổ thường xuyên để giữ độ sạch sẽ và bền đẹp.', 25000, 'https://petservicehcm.com/wp-content/uploads/2024/03/z6072559131074_0f377e6ed6b5bc1c398a3f91455b5e61.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Alkin Mitecyn 50ml – Xịt trị viêm da, nấm, ghẻ cho chó mèo', N'Alkin Mitecyn 50ml – Xịt trị viêm da, nấm, ghẻ chó mèo
– THÀNH PHẦN: Moxidectin , Ivermectin , Miconazole Nitrate, Neomycin sulfate, Chlorhexidin acetate
+ Thành phần an toàn chống dị ứng
+ Tiêu diệt nhanh chóng và tận gốc các trường hợp như:

Ghẻ chó, ghẻ Demodex (xà mâu), ghẻ nặng , các bệnh nấm da, viêm da có mủ,…
Viêm da do vi khuẩn gây ngứa ngáy, mùi hôi, cản trở hoạt động của thú cưng

+ Áp dụng cho tất cả các giống chó và mèo từ 3 tháng tuổi trở lên
– CÁCH SỬ DỤNG :
+ Nếu có thể hãy cạo lông khu vực bị ảnh hưởng. Xịt thuốc đồng đều cả khu vực xung quanh . Nếu xít trúng vào mắt , cần lấy khăn bông ướt lau lại ngay sau khi xịt
+ Mỗi ngày xịt 1 lần , xịt 7-10 ngày liên tục', 140000, 'https://petservicehcm.com/wp-content/uploads/2022/06/bat-an-inox-16-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Điều trị rận tai Auriderm', N'Đường dùng thuốc: nhỏ vào tai
Liều lượng: nhỏ thuốc 1 lần vào buổi sáng và 1 lần vào buổi chiều trong suốt 7-14 ngày, tùy the tiến triển triệu chứng, thời gian điều trị trung bình là 10 ngày. Sau khi lau sạch tai, nhỏ thuốc trực tiếp vào tai bằng cách bóp trên lọ thuốc. Nhỏ đầy kênh tai. Đặt gạc bông gòn ở bên ngoài kênh tai để thấm phần dung dịch bị tràn ra, Lau loa tai với phần bông thấm này khi bị ghẻ tai. Khi bóp nhẹ chai sẽ đc 0,5ml thuốc. Thể tích cần cho mỗi trường hợp sẽ tùy thuộc vào kích thước kênh lỗ tai.
Lưu ý: Nên lắc lọ thuốc trước khi sử dụng để đạt được 1 huyền dịch đồng nhất. Không được dùng cho thú bị viêm tai có kèm tổn thương màng nhĩ.', 180000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-25-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Fabricil Alkin – Xịt nấm mủ, viêm da cho chó mèo – PET SERVICE', N'MÔ TẢ SẢN PHẨM
Fabricil Alkin 50ml _ Xịt nấm mủ, viêm da cho chó mèo
[Đối tượng áp dụng]
Sử dụng an toàn cho chó mèo 8 tuần tuổi và chó mèo lớn hơn.
[Chỉ định]
• Hỗ trợ các chứng do lây nhiễm nấm;
• Hỗ trợ chứng da có mủ, viêm da, mẩn ngứa, dị ứng.
• Hỗ trợ hiệu quả các chứng lây nhiễm tổng hợp ve, nấm.
• Loại trừ tình trạng tiết bả nhờn, mủ, ngứa, rụng lông và các triệu chứng khác
[Cách dùng]
Trước khi sử dụng, đề nghị cố gắng có thể rửa sạch chỗ bị thương và xung quanh vùng bị thương;
Nếu có thể, đề nghị cạo hoặc cắt ngắn phần lông che chỗ bị thương.
Phun vào chỗ bị thương và vùng xung quanh, để dung dịch thuốc phủ đều lên vết thương.
Nếu hỗ trợ ở vùng nhạy cảm như quanh mắt, có thể che phần mắt, hoặc bôi vết thương bằng tăm bông y tế chấm dung dịch.
[Lượng dùng và liệu trình]
• Vấn đề ngoài da ở mức độ nhẹ: dùng 1 lần/ngày trước khi ngủ, 7 ngày là 1 liệu trình, kéo dài hết 1 liệu trình;
• Vấn đềngoài da ở mức độ nặng: dùng 2 lần/ngày cách nhau 8 tiếng, 7 ngày là 1 liệu trình, kéo dài hết 2 liệu trình;
[Chống chỉ định]
* Không khuyến nghị sử dụng liều lượng lớn trong trường hợp không cần thiết.
* Sử dụng công cụ chuyên nghiệp (như vòng loa quanh cổ) để tránh PET liếm phải.
* Nếu có vết lở loét hoặc vết thương chưa liền, đề nghị sử dụng theo hướng dẫn của bác sĩ.
Xuất Xứ : Anh Quốc
Phân Phối bởi Công ty TNHH Phát Triển Thương Mại Kỳ Nam', 140000, 'https://petservicehcm.com/wp-content/uploads/2025/05/FABRICIL.jpg', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Nhỏ gáy Advocate trị ve rận cho mèo dưới 4kg – 1 tuýp', N'Công dụng: Tiêu diệt, điều trị ve, rận, bọ chét cho mèo
– Nhỏ gáy có hiệu quả trong vòng 12 giờ đối với bọ chét và trong vòng 48h với ve.
– Sản phẩm có thể được sử dụng như 1 phần của quy trình kiểm soát viêm da dị ứng do bọ chét.
– Kiểm soát ve rận trên da lông và trong tai mèo
– Nhỏ gáy Advocate up to 4kg : 1  tuýp 0.4ml', 150000, 'https://petservicehcm.com/wp-content/uploads/2023/09/Thiet-ke-chua-co-ten-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Nhỏ gáy Advocate trị ve rận cho mèo trên 4kg – 1 tuýp', N'Công dụng: Tiêu diệt, điều trị ve, rận, bọ chét cho mèo
– Nhỏ gáy có hiệu quả trong vòng 12 giờ đối với bọ chét và trong vòng 48h với ve.
– Sản phẩm có thể được sử dụng như 1 phần của quy trình kiểm soát viêm da dị ứng do bọ chét.
– Kiểm soát ve rận trên da lông và trong tai mèo
– Nhỏ gáy Advocate over 4kg : 1 tuýp 0.8ml', 180000, 'https://petservicehcm.com/wp-content/uploads/2023/08/bat-an-inox-51-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Nhỏ gáy trị ve Advocate chó 10-25kg – 1 TUÝP', N'Nhỏ gáy trị ve Advocate chó 10-25kg
Advocate là sản phẩm dùng để phòng ngừa giun và ấu trùng của các loại giun tim, giun móc, giun kim, giun tròn; giúp giảm chứng viêm da dị ứng do bọ chét; đồng thời phòng ngừa bọ chét, rận, ghẻ, ve và demodex cho chó từ 7 tuần tuổi trở lên trong vòng 1 tháng
Giải pháp toàn diện, cải tiến diệt nội – ngoại ký sinh
Ưu điểm nổi bật:
– Điều trị hiệu quả bọ chét và rận
– Đặc trị ghẻ Demodex, ghẻ Sarcoptes và ghẻ tai Otodectes
– Phòng ngừa hiệu quả bệnh giun tim Dirofilaria immitis
– Hiệu quả điều trị cao đối với giun phổi Angiostrongylus vasorum
– Điều trị và kiểm soát mọi giai đoạn phát triển của ký sinh trùng đường tiêu hoá như giun đũa, giun móc, giun tóc
– Điều trị bệnh viêm da dị ứng do bọ chét (Flea Allergy Dermatitis)
Thành phần
Imidacloprid 10% và Moxidectin 2,5%, Benzyl alcohol, 0,1% butylhydroxy-toluene(E 321: chất chống oxy hóa)
Mô tả sản phẩm
– Dung tích: Advocate có size cho từng kích thước chó
– Phù hợp: Cho chó từ 7 tuần tuổi
– Đặc điểm chính: Điều trị phòng ngừa nội – ngoại ký sinh trùng
Hướng dẫn sử dụng:
+ B1: Lấy tuýp thuốc trong hộp ra, giữ tuýp thuốc ở vị trí thẳng đứng, vặn và kéo nắp tuýp thuốc ra. Sau đó, quay ngược nắp tuýp thuốc lại, vặn vào đầu tuýp thuốc để đâm thủng phần nhựa ép trên miệng tuýp thuốc
+ B2: Nhỏ ngoài da. Giữ chó thẳng đứng, vạch phần lông giữa hai xương bả vai cho đến khi nhìn thấy da. Đặt đầu tuýp thuốc trên da và bóp nhẹ tuýp thuốc nhiều lần cho đến khi thuốc được bơm ra hết. Không bơm thuốc quá nhiều tại mỗi điểm để tránh thuốc chảy tràn xuống hai bên. Với chó lớn có thể nhỏ tại 3 đến 4 điểm dọc theo sống lưng
– Khuyến cáo: 10 mg Imidacloprid/ kg thể trọng và 2,5 mg Moxidectin/ kg thể trọng. Tương đương với 0,1 ml Advocate/ kg thể trọng', 300000, 'https://petservicehcm.com/wp-content/uploads/2022/07/bat-an-inox-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Sữa Tắm Điều Trị Viêm Da, Ghẻ Nấm DAVIS 355ml', N'Sữa Tắm Điều Trị Viêm Da, Ghẻ Nấm DAVIS
Sữa tắm Ketohexidine kết hợp 2 thành phần hiệu lực cao Ketoconazole and Chlorhexidine Gluconate. Điều trị hiệu quả các vấn đề viêm da do vi khuẩn, nấm. Thành phần Colloidal Oatmeal trong sữa tắm giúp làm mềm da & giữ ẩm. Sữa tắm Ketohexidine lý tưởng hơn với mùi bạc hà thoang thoảng.

Chỉ định:
Sữa Tắm Điều Trị Viêm Da, Ghẻ Nấm DAVIS phù Hợp Cho chó cưng mèo iu bị ngứa, rối loạn da kết hợp nấm, vi khuẩn & viêm da tăng tiết chất & nấm.
Hướng Dẫn Sử Dụng:
+ Lắc kĩ trước khi sử dụng
+ Làm ướt đều toàn bộ lông
+ Xoa Sữa tắm lên đầu & tai cho sủi bọt. Tránh để vây vào mắt.
+ Tiếp tục cho Sữa tắm lên cổ, ngực phần giữa, phần sau thân & cuối cùng đến chân.
+ Để Sữa tắm thấm đều trong 5-10 phút.
+ Gội sạch lau khô & sấy nếu cần.
Lưu ý:
Có thể tắm cho thú cưng 2 – 3 lần 1 tuần trong vòng 4 tuần đầu, sau đó giảm còn 1 lần 1 tuần hoặc theo chỉ định của bác sĩ thú y.
– Nếu trường hợp da vẫn tiếp tục bị ngứa sau khi sử dụng, ngưng sử dụng & thông báo cho Bác Sĩ Thú Y
– Để nơi thoáng mát & tránh ánh sáng trực tiếp.
– Tránh xa tầm tay trẻ em.
– Chỉ dùng trong thú y', 390000, 'https://petservicehcm.com/wp-content/uploads/2022/06/bat-an-inox-12-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Thuốc điều trị viêm tai oridemyl', N'Điều trị viêm tai ngoài và viêm tai giữa gây ra bởi vi khuẩn, nấm, ký sinh trùng có nguồn gốc tương ứng do các chủng vi khuẩn nhạy cảm với Neo-mycine, nấm men và nấm nhạy cảm với Ný-tatine, Ve ở tai nhạy cảm với Reme-thrine ở chó và mèo
CHỐNG CHỈ ĐỊNH- Không sử dụng cho những loài vật có màng nhĩ đục- Không sử dụng cho chó mèo có trọng lượng ít hơn 1,5kg
PHẢN ỨNG PHỤ- Kích thích nhỏ có thể xảy ra khi điều trị- Nếu kích thích kéo dài hay nặng hơn nên dừng lại- Ở chó, đặc biệt những con chó lớn tuổi được quan sát tình trạng điếc tạm thời rất hiếm xảy ra- Ở mèo, triệu chứng thần kinh như rối loạn và run rất hiếm xảy ra – Nếu có triệu chứng rối loạn và run thì nên ngưng điều trị lại
LIỀU LƯỢNG VÀ CÁCH DÙNG- Sử dụng tuyến nhĩ trên chó và mèo- Cho vào lỗ tai lượng bằng kích thước của một hạt đậu nhỏ mỗi ngày trong 21 ngày.- Làm sạch tai ngoài. Nhỏ một lượng bằng kích thước của một hạt đậu nhỏ Oridermyl vào lỗ tai sau đó massage quanh gốc tai và làm sạch sản phẩm dư thừa trên nắp tai.', 190000, 'https://petservicehcm.com/wp-content/uploads/2023/02/bat-an-inox-18-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Thuốc điều trị viêm tai Vemedim', N'CHỐNG CHỈ ĐỊNH :
– Không dùng cho chó bị thủng màng nhĩ.
– Không dùng cho chó mẫn cảm với các thành phần của thuốc.
CÁCH DÙNG : Lắc kỹ trước khi sử dụng.
Làm sạch và khô ống tai ngoài. Nghiêng tai bị bệnh lên phía trên, kéo vành tai ra phía sau nhỏ 3-6 giọt vào tai (tùy kích cỡ chó), 1-2 lần 1 ngày, sử dụng liên tục 7-14 ngày tùy theo tình trạng bệnh.
Sau khi nhỏ thuốc, mát-xa gốc tai nhẹ nhàng trong một vài phút để giúp thuốc thấm sâu vào phần thấp hơn của ống tai ngoài.
Mỗi giọt thuốc chứa 150 mcg marbofloxacin, 500 mcg clotrimazole và 50 mcg dexamethasone acetate.
THẬN TRỌNG :
Trước khi sử dụng thuốc nhỏ tai cho chó, nên khám xem chó có bị thủng màng nhĩ không. Thuốc chống chỉ định đối với chó bị thủng màng nhĩ.
Tránh để thuốc tiếp xúc với mắt con vật bệnh. Nếu mắt dính phải thuốc, nên rửa mắt bằng nước sạch nhiều lần.', 55000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-26-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Thuốc nhỏ mắt Bio-Gentadrop', N'CÔNG DỤNG: Điều trị các trường hợp viêm mắt do nhiễm trùng gây triệu chứng mắt đỏ, chảy nhiều nước mắt, ghèn mắt, có mủ hoặc đục giác mạc trên chó mèo, gia súc, gia cầm.
LIỀU LƯỢNG & CÁCH DÙNG:

Với thú nhỏ: 1-2 giọt/mắt/lần. Ngày 4-5 lần.
Với thú lớn: 4-5 giọt/mắt/lần. Ngày 4-5 lần.
LƯU Ý: Nên rửa sạch ghèn, nước mắt bằng nước sôi để nguội hoặc bằng nước muối sinh lý trước khi nhỏ thuốc. Nên nhỏ thuốc cả 2 mắt cùng một lúc kể cả trường hợp chỉ có một bên mắt bị viêm. Không sử dụng thuốc trong trường hợp loét giác mạc, các trường hợp tăng nhãn áp. Thuốc sau khi mở chỉ nên sử dụng trong 2 tuần. Không nên sử dụng thuốc liên tục quá 10 ngày. Không dùng quá liều quy định.', 30000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Anh-San-pham-Pet-Services-23-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Thuốc trị rận tai cho chó mèo ALKIN Otoklen', N'Thuốc trị rận tai cho chó mèo ALKIN Otoklen
Công dụng: 

Điều trị ngứa tai, viêm tai, tai có mùi hôi, có mủ, có màng sáp đen
Hỗ trợ điều trị nhiễm khuẩn da cục bộ, nấm, ve.
Tiêu diệt và ức chế hiệu quả các nguyên thể bệnh như ve tai, nấm.
Đối với nhiễm trùng ve, viêm tai kế phát, viêm tai tổng hợp đều có hiệu quả điều trị tốt.
Điều trị hiệu quả các triệu chứng ngứa tai, tai có mùi hôi, tai có màng sáp màu nâu đen, chảy mủ.
Sử dụng chăm sóc hàng ngày phòng ngừa lây nhiễm ve gây ngứa tai, viêm tai.

Thành phần: thành phấn Moxidectin Clindamycin Hydrochloride Lidocaine Hydrochloride Salicylic acid Boracid acid. Công thức tăng hiệu quả, thành phần kích hoạt an toàn cho chó và mèo.
Thuốc trị rận tai cho chó mèo ALKIN Otoklen không xâm nhập vào hệ thống tuần hoàn máu, không gây tổn thương chức năng gan của động vật.
Xem thêm sản phẩm tại https://www.facebook.com/petshopquan7', 160000, 'https://petservicehcm.com/wp-content/uploads/2022/06/bat-an-inox-17-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Thuốc xổ giun, sán Zentab hiệu quả cho chó mèo – viên lẻ', N'LIỀU VÀ CÁCH DÙNG:
ZENTAB – Viên uống loại bỏ giun sán chó mèo
– Công dụng: hỗ trợ loại bỏ và kiểm soát giun sán cho chó mèo (sán lá gan, sán dây, giun dạ dày, giun đường ruột, phổi)
– Hướng dẫn: cho uống trực tiếp/nghiền với thức ăn
– Liều dùng: tẩy giun định kỳ 1 viên/5kg thể trọng (3-8kg)
+ Chó mèo con dưới 1 tuổi: 1 tháng 1 lần
+ Trên 1 tuổi: 3-4 tháng/lần
+ Điều trị từng loại giun theo liều dùng cho chó và mèo.
– Quy cách: 1 vỉ 10 viên // Gói 1 viên lẻ (tùy phân loại)
Gói viên lẻ đã có đính kèm HDSD chi tiết cho bé.', 25000, 'https://petservicehcm.com/wp-content/uploads/2021/10/Zentab-2-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai Nexgard Spectra trị ve, ghẻ, bọ chét, xổ giun, nội ngoại kí sinh 7.5kg-15kg', N'CÔNG DỤNG
• Diệt bọ chét – Hiệu quả kéo dài 5 tuần
• Hỗ trợ điều trị ghẻ Demodex, Sarcoptes
• Kiểm soát giun đũa, giun móc, giun tóc, duy trì trọng lượng bình thường của cơ thể.
• Nexgard Spectra bắt đầu diệt bọ chét trong vòng 2h sau khi sử dụng. Diệt 100% bọ chét trong 6h sau khi sử dụng.
• Ngăn chặn bọ chét cái đẻ trứng.
• Diệt ve trong vòng 12h sau khi sử dụng.
• Hỗ trợ điều trị Demodex, Sarcoptes.

HIỆU QUẢ AN TOÀN ĐÃ ĐƯỢC CHỨNG MINH
• Nghiên cứu thử nghiệm đã được thực hiện trên 700 con chó với các giống khác nhau.
• Không có bất kỳ phản ứng bất lợi cho chó con từ 8 tuần tuổi với các liều 1 lần, gấp 3 lần, gấp 5 lần.
• An toàn trên cả giống chó Collie với liều gấp 5 lần (5x).
– Nexgard Spectra đầy đủ các kích thước cho chó: 2 – 3.5kg ; 3.5 – 7.5kg ; 7.5 – 15kg ; 15 – 30kg và 30 – 60kg
• Nexgard đã được chứng minh hiệu quả chống lại giun đường tiêu hóa từ 92,5% – 99,2% cả nhiễm tự nhiên và nhiễm thực nghiệm.
• Phòng ngừa bệnh giun tim: hiệu quả 100% với ấu trùng giun tim khi dung hàng tháng
.
HƯỚNG DẪN SỬ DỤNG:
Cách 1: Với trường hợp chó bướng bỉnh không chịu uống bạn nên cậy miệng nhẹ nhàng và đặt viên nhai vào trong với 1 chút nước và giữ chặt miệng chúng trong vài giây để thuốc có thể trôi xuống dạ dày. Vuốt nhẹ cổ đến khi chó nuốt xong viên thuốc.
Chú ý: Theo dõi cún nuốt xong. Nếu không cún gian xảo sẽ nhè ra khi bạn bỏ tay.
Sử dụng biện pháp mạnh: Ghì mõm chó và mở hàm của chúng, đưa đầu chó hướng lên trên và đặt viên nhai vào lưỡi, giữ chặt mõm của cún và chờ đợi 20 – 30s cho tới khi viên nhai đã trôi xuống dạ dày.

Cách 2: Bạn có thể trộn viên nhai với cơm hoặc hạt, đồ ăn mà cún yêu thích nhất, việc ăn kèm thức ăn này sẽ khiến cún không còn nhận ra mùi.

LƯU Ý: Viên nhai sử dụng an toàn trên cún từ 8 tuần tuổi trở lên. Khi điều trị chó nhỏ hơn 8 tuần tuổi và / hoặc chó có trọng lượng nhỏ hơn 2 kg/ hoặc chó trong giai đoạn mang thai và cho con bú nên có bác sỹ thú y tư vấn. Chỉ lấy sản phẩm ra khỏi vĩ ngay khi dùng. Đặt vĩ còn viên nhai vào hộp carton để bảo quản. Rửa sạch tay sau khi sử dụng sản phẩm.

BẢO QUẢN:
– Tránh xa tầm tay trẻ em. Không dùng thuốc khi hết hạn sử dụng.
– Bảo quản ở nhiệt độ dưới 30oC. SẢN XUẤT TẠI: Merial, Pháp.', 350000, 'https://petservicehcm.com/wp-content/uploads/2023/07/Ban-sao-cua-khung-Shopee-4-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai Nexgard Spectra trị ve,ghẻ,bọ chét, xổ giun, nội ngoại kí sinh- 3.5kg-7.5kg', N'CÔNG DỤNG
• Diệt bọ chét – Hiệu quả kéo dài 5 tuần
• Hỗ trợ điều trị ghẻ Demodex, Sarcoptes
• Kiểm soát giun đũa, giun móc, giun tóc, duy trì trọng lượng bình thường của cơ thể.
• Nexgard Spectra bắt đầu diệt bọ chét trong vòng 2h sau khi sử dụng. Diệt 100% bọ chét trong 6h sau khi sử dụng.
• Ngăn chặn bọ chét cái đẻ trứng.
• Diệt ve trong vòng 12h sau khi sử dụng.
• Hỗ trợ điều trị Demodex, Sarcoptes.

HIỆU QUẢ AN TOÀN ĐÃ ĐƯỢC CHỨNG MINH
• Nghiên cứu thử nghiệm đã được thực hiện trên 700 con chó với các giống khác nhau.
• Không có bất kỳ phản ứng bất lợi cho chó con từ 8 tuần tuổi với các liều 1 lần, gấp 3 lần, gấp 5 lần.
• An toàn trên cả giống chó Collie với liều gấp 5 lần (5x).
– Nexgard Spectra đầy đủ các kích thước cho chó: 2 – 3.5kg ; 3.5 – 7.5kg ; 7.5 – 15kg ; 15 – 30kg và 30 – 60kg
• Nexgard đã được chứng minh hiệu quả chống lại giun đường tiêu hóa từ 92,5% – 99,2% cả nhiễm tự nhiên và nhiễm thực nghiệm.
• Phòng ngừa bệnh giun tim: hiệu quả 100% với ấu trùng giun tim khi dung hàng tháng
.
HƯỚNG DẪN SỬ DỤNG:
Cách 1: Với trường hợp chó bướng bỉnh không chịu uống bạn nên cậy miệng nhẹ nhàng và đặt viên nhai vào trong với 1 chút nước và giữ chặt miệng chúng trong vài giây để thuốc có thể trôi xuống dạ dày. Vuốt nhẹ cổ đến khi chó nuốt xong viên thuốc.
Chú ý: Theo dõi cún nuốt xong. Nếu không cún gian xảo sẽ nhè ra khi bạn bỏ tay.
Sử dụng biện pháp mạnh: Ghì mõm chó và mở hàm của chúng, đưa đầu chó hướng lên trên và đặt viên nhai vào lưỡi, giữ chặt mõm của cún và chờ đợi 20 – 30s cho tới khi viên nhai đã trôi xuống dạ dày.

Cách 2: Bạn có thể trộn viên nhai với cơm hoặc hạt, đồ ăn mà cún yêu thích nhất, việc ăn kèm thức ăn này sẽ khiến cún không còn nhận ra mùi.

LƯU Ý: Viên nhai sử dụng an toàn trên cún từ 8 tuần tuổi trở lên. Khi điều trị chó nhỏ hơn 8 tuần tuổi và / hoặc chó có trọng lượng nhỏ hơn 2 kg/ hoặc chó trong giai đoạn mang thai và cho con bú nên có bác sỹ thú y tư vấn. Chỉ lấy sản phẩm ra khỏi vĩ ngay khi dùng. Đặt vĩ còn viên nhai vào hộp carton để bảo quản. Rửa sạch tay sau khi sử dụng sản phẩm.

BẢO QUẢN:
– Tránh xa tầm tay trẻ em. Không dùng thuốc khi hết hạn sử dụng.
– Bảo quản ở nhiệt độ dưới 30oC. SẢN XUẤT TẠI: Merial, Pháp.', 310000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Ban-sao-cua-khung-Shopee-3-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai trị ve cho chó Bravecto 10kg-20kg', N'Viên nhai trị ve cho chó Bravecto
Mô tả sản phẩm

CÔNG DỤNG
– Ngăn ngừa sự phá hoại của bọ chét, kiểm soát sự xâm nhập của chúng trong 12 tuần
– Loại bỏ sự xâm nhập của bọ chét trong nhà bạn chỉ với một liều Bravecto
PHÂN LOẠI
+ Vàng dành cho chó 2kg – 4,5kg
+ Cam dành cho chó từ 4,5kg-10kg
+ Xanh lá dành cho chó từ 10kg – 20kg
+ Trắng dành cho chó từ 20kg-40kg
+ Hồng dành cho chó từ 40kg – 65kg

 Ưu điểm nổi bật:
– Loại bỏ bọ chét và ve trong tối đa 12 tuần, có thể bắt đầu bất kì lúc nào trong năm và có thể tiếp tục quanh năm
– Tiêu diệt 100% bọ chét trong 8 giờ – và tiếp tục trong 12 tuần
– An toàn cho chó nặng ít nhất 2kg và cho chó con từ 6 tháng trở lên
– Được FDA chấp thuận và bác sĩ thú y khuyên dùng
Thương hiệu: Bravecto
Nơi sản xuất: Hoa Kỳ', 595000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-3-1-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai trị ve cho chó Bravecto 20kg-40kg', N'Viên nhai trị ve cho chó Bravecto
Mô tả sản phẩm

CÔNG DỤNG
– Ngăn ngừa sự phá hoại của bọ chét, kiểm soát sự xâm nhập của chúng trong 12 tuần
– Loại bỏ sự xâm nhập của bọ chét trong nhà bạn chỉ với một liều Bravecto
PHÂN LOẠI
+ Vàng dành cho chó 2kg – 4,5kg
+ Cam dành cho chó từ 4,5kg-10kg
+ Xanh lá dành cho chó từ 10kg – 20kg
+ Trắng dành cho chó từ 20kg-40kg
+ Hồng dành cho chó từ 40kg – 65kg

 Ưu điểm nổi bật:
– Loại bỏ bọ chét và ve trong tối đa 12 tuần, có thể bắt đầu bất kì lúc nào trong năm và có thể tiếp tục quanh năm
– Tiêu diệt 100% bọ chét trong 8 giờ – và tiếp tục trong 12 tuần
– An toàn cho chó nặng ít nhất 2kg và cho chó con từ 6 tháng trở lên
– Được FDA chấp thuận và bác sĩ thú y khuyên dùng
Thương hiệu: Bravecto
Nơi sản xuất: Hoa Kỳ', 810000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-3-2-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai trị ve cho chó Bravecto 2kg-4.5kg', N'Viên nhai trị ve cho chó Bravecto
Mô tả sản phẩm

CÔNG DỤNG
– Ngăn ngừa sự phá hoại của bọ chét, kiểm soát sự xâm nhập của chúng trong 12 tuần
– Loại bỏ sự xâm nhập của bọ chét trong nhà bạn chỉ với một liều Bravecto
PHÂN LOẠI
+ Vàng dành cho chó 2kg – 4,5kg
+ Cam dành cho chó từ 4,5kg-10kg
+ Xanh lá dành cho chó từ 10kg – 20kg
+ Trắng dành cho chó từ 20kg-40kg
+ Hồng dành cho chó từ 40kg – 65kg

 Ưu điểm nổi bật:
– Loại bỏ bọ chét và ve trong tối đa 12 tuần, có thể bắt đầu bất kì lúc nào trong năm và có thể tiếp tục quanh năm
– Tiêu diệt 100% bọ chét trong 8 giờ – và tiếp tục trong 12 tuần
– An toàn cho chó nặng ít nhất 2kg và cho chó con từ 6 tháng trở lên
– Được FDA chấp thuận và bác sĩ thú y khuyên dùng
Thương hiệu: Bravecto
Nơi sản xuất: Hoa Kỳ', 450000, 'https://petservicehcm.com/wp-content/uploads/2022/07/bat-an-inox-3-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai trị ve cho chó Bravecto 4.5kg-10kg', N'Viên nhai trị ve cho chó Bravecto
Mô tả sản phẩm

CÔNG DỤNG
– Ngăn ngừa sự phá hoại của bọ chét, kiểm soát sự xâm nhập của chúng trong 12 tuần
– Loại bỏ sự xâm nhập của bọ chét trong nhà bạn chỉ với một liều Bravecto
PHÂN LOẠI
+ Vàng dành cho chó 2kg – 4,5kg
+ Cam dành cho chó từ 4,5kg-10kg
+ Xanh lá dành cho chó từ 10kg – 20kg
+ Trắng dành cho chó từ 20kg-40kg
+ Hồng dành cho chó từ 40kg – 65kg

 Ưu điểm nổi bật:
– Loại bỏ bọ chét và ve trong tối đa 12 tuần, có thể bắt đầu bất kì lúc nào trong năm và có thể tiếp tục quanh năm
– Tiêu diệt 100% bọ chét trong 8 giờ – và tiếp tục trong 12 tuần
– An toàn cho chó nặng ít nhất 2kg và cho chó con từ 6 tháng trở lên
– Được FDA chấp thuận và bác sĩ thú y khuyên dùng
Thương hiệu: Bravecto
Nơi sản xuất: Hoa Kỳ', 510000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-3-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (4, N'Viên nhai trị ve cho chó Bravecto 40kg-56kg', N'Viên nhai trị ve cho chó Bravecto
Mô tả sản phẩm

CÔNG DỤNG
– Ngăn ngừa sự phá hoại của bọ chét, kiểm soát sự xâm nhập của chúng trong 12 tuần
– Loại bỏ sự xâm nhập của bọ chét trong nhà bạn chỉ với một liều Bravecto
Viên nhai Bravecto ngăn ngừa sự phá hoại của bọ chét, kiểm soát sự xâm nhập của chúng.
Hiệu quả kéo dài trong suốt 12 tuần.
An toàn trên cả chó mang thai và cho con bú.
Giúp lông da bóng mượt, không còn mùi hôi.
Viên nhai Bravecto chứa hoạt chất Fluralaner, giúp điều trị và kiếm soát các bệnh do ghẻ, ve, bọ chét, rận… trong suốt 12 tuần:
Bệnh ghẻ do Demodex, Sarcoptes…
Bệnh viêm da dị ứng do bọ chét
PHÂN LOẠI
+ Vàng dành cho chó 2kg – 4,5kg
+ Cam dành cho chó từ 4,5kg-10kg
+ Xanh lá dành cho chó từ 10kg – 20kg
+ Trắng dành cho chó từ 20kg-40kg
+ Hồng dành cho chó từ 40kg – 65kg

 Ưu điểm nổi bật:
– Loại bỏ bọ chét và ve trong tối đa 12 tuần, có thể bắt đầu bất kì lúc nào trong năm và có thể tiếp tục quanh năm
– Tiêu diệt 100% bọ chét trong 8 giờ – và tiếp tục trong 12 tuần
– An toàn cho chó nặng ít nhất 2kg và cho chó con từ 6 tháng trở lên
– Được FDA chấp thuận và bác sĩ thú y khuyên dùng
Thương hiệu: Bravecto
Nơi sản xuất: Hoa Kỳ', 890000, 'https://petservicehcm.com/wp-content/uploads/2023/07/bat-an-inox-3-2-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (5, N'Men tiêu hóa Bio gói 5g', N'MEN HỖ TRỢ ĐƯỜNG TIÊU HÓA CHO CHÓ, MÈO
Cung cấp vitamin và vi khuẩn có lợi, tăng cường tiêu hóa,
phòng ngừa viêm ruột, tiêu chảy, phù đầu trên gia súc, gia cầm, thú cưng.
LIỀU LƯỢNG VÀ CÁCH DÙNG: Dùng liên tục
Thú nhỏ và gia cầm: 1,5 g / lít nước hoặc 3 g / kg thức ăn hoặc 1 g / 7 – 10
kg thể trọng.
ĐẶC ĐIỂM: Biotic bổ sung các vi khuẩn có ích và các vitamin cần thiết,
giúp ức chế các vi khuẩn có hại trong đường ruột, làm giảm tiêu chảy ở thú
cưng.
– Tăng tiêu hóa và hấp thu thức ăn, giúp tăng trọng nhanh.
– Giảm khí độc và mùi hôi trong chuồng nuôi.
CHỐNG CHỈ ĐỊNH: Không dùng cho thú mẫn cảm với thành phần của sản
phẩm.', 8000, 'https://petservicehcm.com/wp-content/uploads/2023/10/Pawo-chan-chan-2-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (5, N'Siro trị chảy nước mắt chó mèo LGF EYES', N'Siro trị chảy nước mắt chó mèo LGF EYES
Chảy nước mắt khiến bộ lông trắng tinh của các boss bị ố vàng ở khóe mắt , trong hơi kém xinh 1 ít, nếu đó là vấn đề đau đầu của các Sen thì đảm bảo đây sẽ là giải pháp cực kì hiệu quả
Mô tả sản phẩm 

Phiên bản mới hơn lợi hại hơn của Pro-white thần thánh
Dung tích: 20ml
Thuốc dạng siro ngọt
Thành phần gồm: vitamin A, C, E, Beta-carotene.

Công dụng:

Điều trị và phòng chống hiện tượng chảy nước mắt và ố lông vùng mắt ở chó mèo,
Làm mượt và chăm sóc lông vùng mắt
Cung cấp dưỡng chất, tốt cho thị giác
Tăng sức đề kháng, điều hoà sự bài tiết tuyến lệ.

Cách dùng: Cho uống trực tiếp

Đối với vật nuôi bị ố nhiều: Ngày uống 2 lần, mỗi lần 1/2 ống.
Vật nuôi bị ố ít: Ngày uống 2 lần, mỗi lần 1/4 ống
Dùng liên tục trong 7-10 ngày. Khuyên dùng từ 2 lọ trở lên để đạt hiệu quả tốt nhất.
1 ống = 1cc
Trong thời gian điều trị:
1. kiêng ăn mặn, ăn tanh, uống nhiều nước, hoa quả
2. Nhỏ thuốc nhỏ mắt natriclorit 0.9 3 lần/ngày
3. Vệ sinh mắt thường xuyên, cắt ngắn lông xung quanh mắt, không để lông chọc vào mắt, lấy bông và nước muối lau sạch gèn mắt mỗi ngày.
Lưu Ý: Thuốc mắt dạng uống (không được nhỏ trực tiếp vào mắt)', 140000, 'https://petservicehcm.com/wp-content/uploads/2022/07/Anh-San-pham-Pet-Services-9-800x800.png', 20);
INSERT INTO Products (CategoryID, Name, Description, Price, ImageURL, Stock) VALUES (5, N'Viên uống dưỡng lông Oderm – Vỉ 10 viên', N'1. CÔNG DỤNG SẢN PHẨM
DR. PETZ ODERM 
– Viên uống bổ trợ với thành phần chưa Collagen thuỷ phân, giúp lông và da luôn khỏe mạnh, phục hồi sau thời kỳ rụng, điều trị tổn thương ngoài da. 
– Dr.PETZ ODERM cung cấp vitamin, khoáng hữu cơ và acid amin giúp da, lông phát triển chắc khỏe, mềm mượt, phục hồi da lông sau thời kì rụng, thay lông hoặc điều trị viêm da, ghẻ, vẩy nến… trên chó mèo.
2. THÀNH PHẦN
Vitamin E, Zinc amino acid chelate, Copper amino acid chelate, Biotin, L-Cysteine, Methionine, Vitamin B2, Vitamin B3, Vitamin B6, Collagen thủy phân.', 99000, 'https://petservicehcm.com/wp-content/uploads/2024/03/Khung-background-san-pham-shopee-mau-2024-03-06T162752.042.png', 20);
