CREATE DATABASE PetDataShop
GO
USE PetDataShop
GO

-- Table Roles
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- Table Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Phone NVARCHAR(20),
    Address NVARCHAR(MAX),
    RoleID INT NOT NULL,
    ProfilePictureUrl NVARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT CHK_Users_UpdatedAt CHECK (UpdatedAt IS NULL OR UpdatedAt >= CreatedAt),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

-- Table ProductCategories
CREATE TABLE ProductCategories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Cate_parent INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (Cate_parent) REFERENCES ProductCategories(CategoryID),
    CONSTRAINT UQ_ProductCategories_Name_CateParent UNIQUE (Name, Cate_parent)
);
GO

-- Table Products
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    CategoryID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    ImageURL NVARCHAR(500),
    Stock INT NOT NULL CHECK (Stock >= 0),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (CategoryID) REFERENCES ProductCategories(CategoryID)
);
GO

-- Table Species
CREATE TABLE Species (
    SpeciesID INT PRIMARY KEY IDENTITY(1,1),
    SpeciesName NVARCHAR(50) NOT NULL UNIQUE,
    IsActive BIT DEFAULT 1
);
GO

-- Table Pets
CREATE TABLE Pets (
    PetID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    SpeciesID INT,
    Name NVARCHAR(50) NOT NULL,
    Breed NVARCHAR(50),
    Age INT,
    Gender NVARCHAR(10) CHECK (Gender IN ('Male', 'Female', 'Other')),
    HealthCondition NVARCHAR(MAX),
    SpecialNotes NVARCHAR(MAX),
    LastSpaVisit DATETIME2,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (SpeciesID) REFERENCES Species(SpeciesID)
);
GO

-- Table Ser_cate
CREATE TABLE Ser_cate (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    Cate_parent INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (Cate_parent) REFERENCES Ser_cate(CategoryID)
);
GO

-- Table Services
CREATE TABLE Services (
    ServiceID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    DurationMinutes INT CHECK (DurationMinutes > 0),
    CategoryID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (CategoryID) REFERENCES Ser_cate(CategoryID)
);
GO

-- Table Promotions
CREATE TABLE Promotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    DiscountType NVARCHAR(20) CHECK (DiscountType IN ('Percentage', 'Fixed')) NOT NULL,
    DiscountValue DECIMAL(10, 2) NOT NULL CHECK (DiscountValue >= 0),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    MinOrderValue DECIMAL(10, 2),
    ApplicableTo NVARCHAR(20) CHECK (ApplicableTo IN ('Product', 'Service', 'Both')) NOT NULL,
    MaxUsage INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT CHK_Promotions_Dates CHECK (EndDate >= StartDate)
);
GO

-- Table Promotion_Products
CREATE TABLE Promotion_Products (
    PromotionProductID INT PRIMARY KEY IDENTITY(1,1),
    PromotionID INT NOT NULL,
    ProductID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UQ_PromotionProduct UNIQUE (PromotionID, ProductID)
);
GO

-- Table Promotion_Services
CREATE TABLE Promotion_Services (
    PromotionServiceID INT PRIMARY KEY IDENTITY(1,1),
    PromotionID INT NOT NULL,
    ServiceID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID),
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID),
    CONSTRAINT UQ_PromotionService UNIQUE (PromotionID, ServiceID)
);
GO

-- Table Status_Appointment
CREATE TABLE Status_Appointment (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- Table Appointments
CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    EmployeeID INT,
    AppointmentDate DATETIME2 NOT NULL CHECK (AppointmentDate >= GETDATE()),
    StatusID INT NOT NULL DEFAULT 1,
    Notes NVARCHAR(MAX),
    PromotionID INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (EmployeeID) REFERENCES Users(UserID),
    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID),
    FOREIGN KEY (StatusID) REFERENCES Status_Appointment(StatusID)
);
GO

-- Table AppointmentPets
CREATE TABLE AppointmentPets (
    AppointmentPetID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT NOT NULL,
    PetID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID),
    FOREIGN KEY (PetID) REFERENCES Pets(PetID),
    CONSTRAINT UQ_AppointmentPet UNIQUE (AppointmentID, PetID)
);
GO

-- Table AppointmentServices
CREATE TABLE AppointmentServices (
    AppointmentServiceID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT NOT NULL,
    ServiceID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID),
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID),
    CONSTRAINT UQ_AppointmentService UNIQUE (AppointmentID, ServiceID)
);
GO

-- Table StatusOrder
CREATE TABLE StatusOrder (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- Table Orders
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL CHECK (TotalAmount >= 0),
    StatusID INT NOT NULL DEFAULT 1,
    OrderDate DATETIME2 DEFAULT GETDATE(),
    ShippingAddress NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (StatusID) REFERENCES StatusOrder(StatusID) ON DELETE NO ACTION
);
GO

-- Table OrderItems
CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE NO ACTION
);
GO

-- Table Cart
CREATE TABLE Cart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    AddedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    CONSTRAINT UQ_Cart_User_Product UNIQUE (UserID, ProductID)
);
GO

-- Table PaymentMethods
CREATE TABLE PaymentMethods (
    PaymentMethodID INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- Table PaymentStatuses
CREATE TABLE PaymentStatuses (
    PaymentStatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

-- Table Payments
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    UserID INT NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL CHECK (Amount >= 0),
    PaymentMethodID INT NOT NULL,
    PaymentStatusID INT NOT NULL DEFAULT 1,
    TransactionID NVARCHAR(100),
    PaymentDate DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE NO ACTION,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods(PaymentMethodID) ON DELETE NO ACTION,
    FOREIGN KEY (PaymentStatusID) REFERENCES PaymentStatuses(PaymentStatusID) ON DELETE NO ACTION
);
GO

-- Table Reviews
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ProductID INT,
    ServiceID INT,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected')) DEFAULT 'Pending',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID) ON DELETE CASCADE,
    CONSTRAINT CHK_Reviews_Target CHECK (
        (ProductID IS NOT NULL AND ServiceID IS NULL) OR 
        (ProductID IS NULL AND ServiceID IS NOT NULL)
    )
);
GO

CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL, -- nếu bạn muốn gắn thông báo cho từng người dùng
    Title NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (UserId) REFERENCES Users(UserId) -- giả sử bạn đã có bảng Users
);

-- Table Blogs
CREATE TABLE Blogs (
    BlogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ContentFormat NVARCHAR(20) CHECK (ContentFormat IN ('HTML', 'Markdown')) DEFAULT 'Markdown',
    Category NVARCHAR(50),
    Status NVARCHAR(20) CHECK (Status IN ('Draft', 'PendingApproval', 'Approved', 'Rejected', 'Published', 'Archived')) DEFAULT 'Draft',
    PublishedAt DATETIME2,
    ApprovedBy INT,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID) ON DELETE NO ACTION,
    CONSTRAINT CHK_Blogs_Published CHECK (Status <> 'Published' OR PublishedAt IS NOT NULL)
);
GO

-- Table Blog_Images
CREATE TABLE Blog_Images (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    BlogID INT NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL,
    Caption NVARCHAR(MAX),
    DisplayOrder INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ON DELETE CASCADE
);
GO



ALTER TABLE Users
ALTER COLUMN PasswordHash NVARCHAR(255) NULL;
GO

ALTER TABLE Reviews
ADD ParentReviewId INT NULL;

ALTER TABLE Reviews
ADD CONSTRAINT FK_Reviews_ParentReview
    FOREIGN KEY (ParentReviewId) REFERENCES Reviews(ReviewID);

	DELETE FROM Reviews;

	-- Script to update database schema for blog system
-- Run this script in SQL Server Management Studio

USE PetDataShop;
GO

-- Check if Blog_Comments table exists, if not create it
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Blog_Comments' AND xtype='U')
BEGIN
    CREATE TABLE Blog_Comments (
        CommentID INT PRIMARY KEY IDENTITY(1,1),
        BlogID INT NOT NULL,
        UserID INT NOT NULL,
        ParentCommentID INT NULL, -- For reply comments (nested comments)
        Content NVARCHAR(MAX) NOT NULL,
        Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected')) DEFAULT 'Pending',
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2,
        FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ON DELETE CASCADE,
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
        FOREIGN KEY (ParentCommentID) REFERENCES Blog_Comments(CommentID) ON DELETE NO ACTION
    );
    PRINT 'Blog_Comments table created successfully.';
END
ELSE
BEGIN
    PRINT 'Blog_Comments table already exists.';
END
GO

-- Check if Blog_Likes table exists, if not create it
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Blog_Likes' AND xtype='U')
BEGIN
    CREATE TABLE Blog_Likes (
        LikeID INT PRIMARY KEY IDENTITY(1,1),
        BlogID INT NOT NULL,
        UserID INT NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ON DELETE CASCADE,
        FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
        UNIQUE(BlogID, UserID) -- Each user can only like a blog once
    );
    PRINT 'Blog_Likes table created successfully.';
END
ELSE
BEGIN
    PRINT 'Blog_Likes table already exists.';
END
GO

-- Insert some sample categories if Blogs table is empty
IF NOT EXISTS (SELECT * FROM Blogs)
BEGIN
    PRINT 'No blogs found. You can now create blogs through the web interface.';
END
ELSE
BEGIN
    PRINT 'Blogs table contains data.';
END
GO

PRINT 'Database schema update completed successfully!';
PRINT 'Please restart your application to apply the changes.';

ALTER TABLE Services
ADD ImageUrl NVARCHAR(255);



SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers';
SELECT * FROM AspNetRoles;
SELECT * FROM AspNetRoles WHERE Name = 'Admin';
SELECT * FROM AspNetUserRoles ;
SELECT Id, Email FROM AspNetUsers;

SELECT Id, Name FROM AspNetRoles;

INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES ('d3e36069-461b-453b-a73d-8adbff5e54e1', '4f570f43-2518-4de8-96cd-9c400b32cfdf');

SELECT u.Email, r.Name
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = '82vodanh@gmail.com'


CREATE TABLE AppointmentServiceImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentServiceID INT NOT NULL,
    ImgUrl NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ServiceImages_AppointmentService 
        FOREIGN KEY (AppointmentServiceID) REFERENCES AppointmentServices(AppointmentServiceID)
        ON DELETE CASCADE
);

INSERT INTO AppointmentServiceStatus (StatusName)
VALUES 
('Pending'),        -- 1
('In Progress'),    -- 2
('Completed'),      -- 3
('Cancelled');      -- 4
CREATE TABLE AppointmentServiceStatus (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(100) NOT NULL
);

-- Thêm cột Status (FK tới AppointmentServiceStatus)
ALTER TABLE AppointmentServices
ADD Status INT;

-- Thêm ràng buộc khóa ngoại Status → AppointmentServiceStatus
ALTER TABLE AppointmentServices
ADD CONSTRAINT FK_AppointmentServices_Status
FOREIGN KEY (Status) REFERENCES AppointmentServiceStatus(StatusID);