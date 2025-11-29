CREATE DATABASE SportStore;
GO
USE SportStore;
GO 

CREATE TABLE Branch(
	BranchID INT IDENTITY(1,1) PRIMARY KEY,
	BranchCode NVARCHAR(30) UNIQUE,
	FullName NVARCHAR(255),
	Adress NVARCHAR(255),
	Phonenumber CHAR(10)
);
CREATE TABLE Category(
	CategoryID INT IDENTITY(1,1) PRIMARY KEY,
	CategoryCode VARCHAR(30),
	FullName NVARCHAR(255),
	Description NVARCHAR(255)
);
GO
CREATE TABLE Supplier(
	SupplierID INT IDENTITY(1,1) PRIMARY KEY,
	SupplierCode VARCHAR(30) UNIQUE NOT NULL,
	SupplierName NVARCHAR(255) NOT NULL,
	ContactPerson NVARCHAR(100),
	PhoneNumber VARCHAR(15),
	Email VARCHAR(100),
	Address NVARCHAR(255),
	IsActive BIT DEFAULT 1
);
GO
CREATE TABLE Product(
	ProductID INT IDENTITY(1,1) PRIMARY KEY,
	ProductCode VARCHAR(30) UNIQUE NOT NULL,
	FullName NVARCHAR(255) NOT NULL,
	Description NVARCHAR(500),
	Brand NVARCHAR(50),
	CategoryID INT NOT NULL,
	SupplierID INT, -- Nhà cung cấp cho sản phẩm
	Img VARCHAR(30),
	CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
	CONSTRAINT FK_Product_Supplier FOREIGN KEY (SupplierID) REFERENCES Supplier(SupplierID)
);
GO
CREATE TABLE ProductDetail(
	ProductDetailID INT IDENTITY(1,1) PRIMARY KEY,
	ProductID INT NOT NULL,
	SKU VARCHAR(50) UNIQUE,
	Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
	Size TINYINT, 
	Color NVARCHAR(30),
	Quantity INT DEFAULT 0 CHECK (Quantity >= 0),
	CONSTRAINT FK_ProductDetail_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
GO
DROP TABLE IF EXISTS Employee;
GO

CREATE TABLE Employee(
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode VARCHAR(30),
    FullName NVARCHAR(50),
    Sex NVARCHAR(10) CHECK (Sex IN (N'Nam', N'Nữ', N'Khác')),
    BirthDate DATE,
    PhoneNumber CHAR(10),
    Email VARCHAR(100),
    Address NVARCHAR(255),
    BranchID INT,
    Position NVARCHAR(100),
    HireDate DATE,
    IsActive BIT DEFAULT 1
	CONSTRAINT FK_Employee_Branch FOREIGN KEY (BranchID) REFERENCES Branch(BranchID),
);
GO
CREATE TABLE Customer(
	CustomerID INT IDENTITY(1,1) PRIMARY KEY,
	CustomerCode VARCHAR(30),
	FullName  NVARCHAR(50),
	Email VARCHAR(100),
	PhoneNumber VARCHAR(10),
	Address NVARCHAR(255)
);
GO
CREATE TABLE Invoice(
	InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
	InvoiceCode VARCHAR(30),
	InvoiceDate DATE,
	InvoiceStatus INT,
	CustomerID INT,
	EmployeeID INT,
	CONSTRAINT FK_Invoice_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
	CONSTRAINT FK_Invoice_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);
GO
CREATE TABLE InvoiceDetail(
	InvoiceDetailID INT IDENTITY(1,1) PRIMARY KEY,
	InvoiceID INT,
	ProductID INT, 
	Quantity INT, 
	UnitPrice DECIMAL,
	CONSTRAINT FK_InvoiceDetail_Invoice FOREIGN KEY (InvoiceID) REFERENCES Invoice(InvoiceID),
	CONSTRAINT FK_InvoiceDetail_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
GO
CREATE TABLE Account(
	AccountID INT IDENTITY(1,1) PRIMARY KEY,
	PhoneNumber VARCHAR(10),
	Password VARCHAR(255),
	Role NVARCHAR(50),
	Status INT,
	CustomerID INT, 
	EmployeeID INT,
	CONSTRAINT FK_Account_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
	CONSTRAINT FK_Account_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);
GO
INSERT INTO Branch (BranchCode, FullName, Adress, Phonenumber) VALUES
('BR001', N'Chi nhánh Quận 1', N'123 Nguyễn Huệ, Quận 1, TP.HCM', '0281234567'),
('BR002', N'Chi nhánh Quận 3', N'456 Lê Văn Sỹ, Quận 3, TP.HCM', '0287654321'),
('BR003', N'Chi nhánh Hà Nội', N'789 Hoàng Quốc Việt, Cầu Giấy, Hà Nội', '0241234567'),
('BR004', N'Chi nhánh Đà Nẵng', N'321 Trần Phú, Hải Châu, Đà Nẵng', '0236123456'),
('BR005', N'Chi nhánh Cần Thơ', N'555 Mậu Thân, Ninh Kiều, Cần Thơ', '0292123456');

-- 2. CATEGORY (Danh mục)
INSERT INTO Category (CategoryCode, FullName, Description) VALUES
('CAT001', N'Giày thể thao', N'Giày chạy bộ, bóng đá, tennis, bóng rổ và các môn thể thao khác'),
('CAT002', N'Quần áo thể thao', N'Áo thun, quần short, áo khoác và đồ tập gym'),
('CAT003', N'Phụ kiện thể thao', N'Găng tay, băng đô, tất thể thao, túi đựng giày'),
('CAT004', N'Dụng cụ tập luyện', N'Tạ tay, tạ chân, thảm yoga, dây kháng lực, bóng tập'),
('CAT005', N'Túi và balo thể thao', N'Balo gym, túi xách thể thao, túi đựng giày'),
('CAT006', N'Đồ bơi', N'Quần áo bơi, kính bơi, mũ bơi, phao tập bơi');

INSERT INTO Supplier 
(SupplierCode, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive) VALUES
('SUP001', N'Nike Vietnam Co., Ltd', N'Ngô Minh Tuấn', '0281111111', 'contact@nike.vn', N'Tầng 10, Tòa nhà Vietcombank, Quận 1, TP.HCM', 1),
('SUP002', N'Adidas Vietnam Trading', N'Phạm Thị Kim Anh', '0282222222', 'info@adidas.vn', N'Số 1 Nguyễn Huệ, Quận 1, TP.HCM', 1),
('SUP003', N'Puma Vietnam Ltd', N'Vũ Hoàng Nam', '0283333333', 'sales@puma.vn', N'254 Điện Biên Phủ, Quận 3, TP.HCM', 1),
('SUP004', N'Under Armour Vietnam', N'Nguyễn Thị Thu Hà', '0284444444', 'contact@underarmour.vn', N'456 Lê Lợi, Quận 1, TP.HCM', 1),
('SUP005', N'Reebok Vietnam', N'Lê Quang Huy', '0285555555', 'info@reebok.vn', N'789 Võ Văn Tần, Quận 3, TP.HCM', 1),
('SUP006', N'Công ty TNHH Thiết bị TD Việt Nam', N'Đặng Thị Minh Trang', '0286666666', 'sales@fitness.vn', N'123 Nguyễn Trãi, Quận 5, TP.HCM', 1);

INSERT INTO Product 
(ProductCode, FullName, Description, Brand, CategoryID, SupplierID, Img) VALUES
('P001', N'Giày chạy bộ Nike Air Zoom Pegasus', N'Giày chạy bộ với công nghệ đệm khí Zoom Air, phù hợp chạy đường dài', 'Nike', 1, 1, 'P001.jpg'),
('P002', N'Giày bóng đá Adidas Predator Edge', N'Giày đá bóng chuyên nghiệp với đinh AG, kiểm soát bóng tốt', 'Adidas', 1, 2, 'P002.jpg'),
('P003', N'Giày tennis Nike Court Vapor', N'Giày tennis chuyên dụng, độ bám sân tốt', 'Nike', 1, 1, 'P003.jpg'),
('P004', N'Giày bóng rổ Adidas Harden Vol.7', N'Giày bóng rổ cao cổ, hỗ trợ cổ chân tốt', 'Adidas', 1, 2, 'P004.jpg'),
('P005', N'Áo thun thể thao Nike Dri-FIT', N'Áo thun thoáng khí, thấm hút mồ hôi cực tốt', 'Nike', 2, 1, 'P005.jpg'),
('P006', N'Áo thun Under Armour HeatGear', N'Áo compression, ôm body, thoát nhiệt nhanh', 'Under Armour', 2, 4, 'P006.jpg'),
('P007', N'Quần short Adidas Own The Run', N'Quần short chạy bộ, có túi zip an toàn', 'Adidas', 2, 2, 'P007.jpg'),
('P008', N'Quần legging Nike Pro', N'Quần legging tập gym, co giãn 4 chiều', 'Nike', 2, 1, 'P008.jpg'),
('P009', N'Áo khoác gió Nike Windrunner', N'Áo khoác 2 lớp, chống nước nhẹ', 'Nike', 2, 1, 'P009.jpg'),
('P010', N'Găng tay tập gym Harbinger Pro', N'Găng tay có đệm, bảo vệ bàn tay khi tập tạ', 'Harbinger', 3, 6, 'P010.jpg'),
('P011', N'Băng đô thể thao Nike Swoosh', N'Băng đô thấm mồ hôi, co giãn tốt', 'Nike', 3, 1, 'P011.jpg'),
('P012', N'Tất thể thao Adidas Cushioned', N'Tất có đệm, hỗ trợ vòm bàn chân', 'Adidas', 3, 2, 'P012.jpg'),
('P013', N'Thảm yoga Adidas Premium', N'Thảm yoga dày 6mm, chống trượt 2 mặt', 'Adidas', 4, 2, 'P013.jpg'),
('P014', N'Tạ tay cao su Reebok 5kg', N'Cặp tạ tay bọc cao su, chống trượt', 'Reebok', 4, 5, 'P014.jpg'),
('P015', N'Dây kháng lực Reebok 5 mức độ', N'Bộ 5 dây kháng lực với độ căng khác nhau', 'Reebok', 4, 5, 'P015.jpg'),
('P016', N'Bóng tập Yoga 65cm', N'Bóng tập yoga/pilates, chịu lực 300kg', 'Reebok', 4, 5, 'P016.jpg'),
('P017', N'Balo thể thao Puma Challenger', N'Balo đựng đồ gym, nhiều ngăn tiện dụng', 'Puma', 5, 3, 'P017.jpg'),
('P018', N'Túi xách thể thao Adidas Duffel', N'Túi xách size lớn, chống nước', 'Adidas', 5, 2, 'P018.jpg'),
('P019', N'Bình nước thể thao Nike 1000ml', N'Bình nước không BPA, giữ nhiệt 12h', 'Nike', 3, 1, 'P019.jpg'),
('P020', N'Quần bơi Speedo Endurance', N'Quần bơi chuyên nghiệp, kháng chlorine', 'Speedo', 6, 6, 'P020.jpg');

INSERT INTO ProductDetail (ProductID, SKU, Price, Size, Color, Quantity) VALUES
-- Giày Nike Air Zoom (P001)
(1, 'P001-42-BLACK', 2500000, 42, N'Đen', 15),
(1, 'P001-43-BLACK', 2500000, 43, N'Đen', 20),
(1, 'P001-42-WHITE', 2500000, 42, N'Trắng', 12),
(1, 'P001-44-WHITE', 2500000, 44, N'Trắng', 10),
-- Giày Adidas Predator (P002)
(2, 'P002-42-RED', 3200000, 42, N'Đỏ', 8),
(2, 'P002-43-RED', 3200000, 43, N'Đỏ', 10),
(2, 'P002-42-BLUE', 3200000, 42, N'Xanh dương', 6),
-- Giày tennis Nike (P003)
(3, 'P003-42-WHITE', 2800000, 42, N'Trắng', 12),
(3, 'P003-43-WHITE', 2800000, 43, N'Trắng', 15),
-- Giày bóng rổ Adidas (P004)
(4, 'P004-43-BLACK', 3500000, 43, N'Đen', 5),
(4, 'P004-44-BLACK', 3500000, 44, N'Đen', 8),
-- Áo Nike Dri-FIT (P005)
(5, 'P005-M-BLUE', 450000, 38, N'Xanh dương', 50),
(5, 'P005-L-BLUE', 450000, 40, N'Xanh dương', 60),
(5, 'P005-M-BLACK', 450000, 38, N'Đen', 45),
(5, 'P005-XL-BLACK', 450000, 42, N'Đen', 40),
-- Áo Under Armour (P006)
(6, 'P006-M-RED', 550000, 38, N'Đỏ', 30),
(6, 'P006-L-RED', 550000, 40, N'Đỏ', 35),
(6, 'P006-M-BLACK', 550000, 38, N'Đen', 40),
-- Quần short Adidas (P007)
(7, 'P007-M-BLACK', 380000, 38, N'Đen', 50),
(7, 'P007-L-BLACK', 380000, 40, N'Đen', 55),
(7, 'P007-L-GREY', 380000, 40, N'Xám', 45),
-- Quần legging Nike (P008)
(8, 'P008-M-BLACK', 650000, 38, N'Đen', 40),
(8, 'P008-L-BLACK', 650000, 40, N'Đen', 45),
-- Áo khoác Nike (P009)
(9, 'P009-L-BLACK', 1200000, 40, N'Đen', 20),
(9, 'P009-XL-BLACK', 1200000, 42, N'Đen', 15),
(9, 'P009-L-BLUE', 1200000, 40, N'Xanh dương', 18),
-- Găng tay gym (P010)
(10, 'P010-M-BLACK', 250000, 38, N'Đen', 60),
(10, 'P010-L-BLACK', 250000, 40, N'Đen', 55),
-- Băng đô Nike (P011)
(11, 'P011-OS-BLACK', 120000, 0, N'Đen', 100),
(11, 'P011-OS-WHITE', 120000, 0, N'Trắng', 80),
-- Tất thể thao (P012)
(12, 'P012-L-WHITE', 80000, 40, N'Trắng', 150),
(12, 'P012-L-BLACK', 80000, 40, N'Đen', 120),
-- Thảm yoga (P013)
(13, 'P013-OS-PURPLE', 650000, 0, N'Tím', 25),
(13, 'P013-OS-GREEN', 650000, 0, N'Xanh lá', 30),
-- Tạ tay 5kg (P014)
(14, 'P014-5KG-BLACK', 850000, 0, N'Đen', 40),
-- Dây kháng lực (P015)
(15, 'P015-SET-MULTI', 450000, 0, N'Nhiều màu', 35),
-- Bóng tập (P016)
(16, 'P016-65CM-BLUE', 380000, 0, N'Xanh dương', 20),
(16, 'P016-65CM-PINK', 380000, 0, N'Hồng', 15),
-- Balo Puma (P017)
(17, 'P017-OS-BLACK', 890000, 0, N'Đen', 30),
(17, 'P017-OS-BLUE', 890000, 0, N'Xanh dương', 25),
-- Túi Adidas (P018)
(18, 'P018-OS-BLACK', 750000, 0, N'Đen', 20),
-- Bình nước Nike (P019)
(19, 'P019-1L-BLACK', 280000, 0, N'Đen', 50),
(19, 'P019-1L-BLUE', 280000, 0, N'Xanh dương', 45),
-- Quần bơi Speedo (P020)
(20, 'P020-M-BLACK', 580000, 38, N'Đen', 30),
(20, 'P020-L-BLACK', 580000, 40, N'Đen', 25);

INSERT INTO Employee (EmployeeCode, FullName, Sex, BirthDate, PhoneNumber, Email, Address, BranchID, Position, HireDate, IsActive) VALUES
('EMP001', N'Nguyễn Văn An', N'Nam', '1990-05-15', '0901234567', 'an.nguyen@sportstore.vn', N'123 Lê Lợi, Quận 1, TP.HCM', 1, N'Quản lý', '2020-01-15', 1),
('EMP002', N'Trần Thị Bình', N'Nữ', '1992-08-20', '0902345678', 'binh.tran@sportstore.vn', N'456 Hai Bà Trưng, Quận 3, TP.HCM', 2, N'Nhân viên bán hàng', '2021-03-10', 1),
('EMP003', N'Lê Văn Cường', N'Nam', '1988-12-05', '0903456789', 'cuong.le@sportstore.vn', N'789 Cầu Giấy, Hà Nội', 3, N'Quản lý', '2019-06-01', 1),
('EMP004', N'Phạm Thị Dung', N'Nữ', '1995-03-25', '0904567890', 'dung.pham@sportstore.vn', N'321 Trần Phú, Đà Nẵng', 4, N'Nhân viên bán hàng', '2022-02-14', 1),
('EMP005', N'Hoàng Văn Em', N'Nam', '1991-07-10', '0905678901', 'em.hoang@sportstore.vn', N'555 Mậu Thân, Cần Thơ', 5, N'Nhân viên bán hàng', '2021-09-20', 1),
('EMP006', N'Vũ Thị Phương', N'Nữ', '1993-11-30', '0906789012', 'phuong.vu@sportstore.vn', N'111 Điện Biên Phủ, Quận 1, TP.HCM', 1, N'Nhân viên bán hàng', '2021-11-05', 1),
('EMP007', N'Đặng Văn Giang', N'Nam', '1989-04-18', '0907890123', 'giang.dang@sportstore.vn', N'222 Lý Thường Kiệt, Quận 10, TP.HCM', 1, N'Thủ kho', '2020-05-20', 1),
('EMP008', N'Bùi Thị Hoa', N'Nữ', '1994-09-12', '0908901234', 'hoa.bui@sportstore.vn', N'333 Nguyễn Văn Cừ, Quận 5, TP.HCM', 2, N'Quản lý', '2020-08-15', 1),
('EMP009', N'Ngô Văn Ích', N'Nam', '1987-06-22', '0909012345', 'ich.ngo@sportstore.vn', N'444 Láng Hạ, Đống Đa, Hà Nội', 3, N'Nhân viên bán hàng', '2021-01-10', 1),
('EMP010', N'Admin System', N'Khác', '1985-01-01', '0900000000', 'admin@sportstore.vn', N'Trụ sở chính', 1, N'Admin', '2019-01-01', 1);


INSERT INTO Customer (CustomerCode, FullName, Email, PhoneNumber, Address) VALUES
('CUS001', N'Nguyễn Minh Hải', 'hai.nguyen@email.com', '0911234567', N'15 Võ Văn Tần, Quận 3, TP.HCM'),
('CUS002', N'Trần Thu Hương', 'huong.tran@email.com', '0912345678', N'25 Nguyễn Thị Minh Khai, Quận 1, TP.HCM'),
('CUS003', N'Lê Quang Khải', 'khai.le@email.com', '0913456789', N'35 Lạc Long Quân, Tây Hồ, Hà Nội'),
('CUS004', N'Phạm Thị Lan', 'lan.pham@email.com', '0914567890', N'45 Đinh Tiên Hoàng, Hải Châu, Đà Nẵng'),
('CUS005', N'Hoàng Văn Minh', 'minh.hoang@email.com', '0915678901', N'55 Trần Hưng Đạo, Ninh Kiều, Cần Thơ'),
('CUS006', N'Vũ Thị Nga', 'nga.vu@email.com', '0916789012', N'65 Pasteur, Quận 1, TP.HCM'),
('CUS007', N'Đặng Văn Phong', 'phong.dang@email.com', '0917890123', N'75 Lê Duẩn, Quận 1, TP.HCM'),
('CUS008', N'Bùi Thị Quỳnh', 'quynh.bui@email.com', '0918901234', N'85 Nguyễn Trãi, Thanh Xuân, Hà Nội');

INSERT INTO Invoice (InvoiceCode, InvoiceDate, InvoiceStatus, CustomerID, EmployeeID) VALUES
('INV001', '2024-11-01', 1, 1, 1),
('INV002', '2024-11-02', 1, 2, 2),
('INV003', '2024-11-03', 1, 3, 3),
('INV004', '2024-11-05', 1, 4, 4),
('INV005', '2024-11-07', 1, 5, 5),
('INV006', '2024-11-10', 1, 6, 1),
('INV007', '2024-11-12', 1, 7, 2),
('INV008', '2024-11-15', 1, 8, 3),
('INV009', '2024-11-16', 1, 1, 6),
('INV010', '2024-11-18', 1, 3, 7);

INSERT INTO InvoiceDetail (InvoiceID, ProductID, Quantity, UnitPrice) VALUES
(1, 1, 1, 2500000),
(1, 3, 2, 450000),
(2, 2, 1, 3200000),
(2, 4, 1, 380000),
(3, 6, 1, 650000),
(3, 5, 1, 250000),
(4, 9, 1, 2800000),
(5, 7, 2, 850000),
(5, 8, 1, 890000),
(6, 10, 1, 1200000),
(6, 3, 1, 450000),
(7, 1, 1, 2500000),
(8, 4, 2, 380000),
(8, 5, 1, 250000),
(9, 2, 1, 3200000),
(10, 6, 1, 650000);

INSERT INTO Account (AccountID, PhoneNumber, Password, Role, Status, CustomerID, EmployeeID)
VALUES
(1, '0911234567', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', N'Khách hàng', 1, 1, NULL),
(2, '0912345678', '$2a$10$VcCcB8zq8EqwzB/nC3XqCeDvGfJ8J8W8W5V7N0ZqP3OJHjBqzGiC6', N'Khách hàng', 1, 2, NULL),
(3, '0913456789', '$2a$10$8K1p/a0dL2LvzA8qS5XpuOGKEkqCRZ7Y1J8qm5lQZK9LdYzn3XvOu', N'Khách hàng', 1, 3, NULL),
(4, '0914567890', '$2a$10$rZ6gxG9lQqJ8YhK7L2nXMuP4N6fH8T5zW9qV3sX1mY7cR2oK5jE8a', N'Khách hàng', 1, 4, NULL),
(5, '0901234567', '$2a$10$kN8T9pQ2mL5vR1yW3sH4jE7cX6oZ9nA2lK8fB5dV7qM1rP3gY4hT6', N'Nhân viên', 1, NULL, 1),
(6, '0902345678', '$2a$10$aB7cD9eF2gH4iJ6kL8mN0oP1qR3sT5uV7wX9yZ1aBcDeF2gH4iJ6k', N'Nhân viên', 1, NULL, 2),
(7, '0903456789', 'AQAAAAIAAYagAAAAEOihFQYYPIclmNHNuqtuMy4jdoC61xlk5Ee2Fs5Zy1kyHrGjKSUu/hnjebpnfgbNRQ==', N'Admin', 1, 1, 3),
(10, '0961439151', 'AQAAAAIAAYagAAAAEBax9BaeSk0F+2KqtUlTgxLfE+ndJSeJBVHbLk6RUCmT6GgG1vfvj13KyPROvU1sAw==', N'Khách hàng', 1, 12, NULL);
