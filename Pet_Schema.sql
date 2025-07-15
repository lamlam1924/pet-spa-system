Create database PetDataShop
use PetDataShop

--CREATE TABLE Roles (
--    RoleID INT PRIMARY KEY IDENTITY(1,1),
--    RoleName NVARCHAR(50) NOT NULL UNIQUE,
--    Description NVARCHAR(MAX),
--    CreatedAt DATETIME2 DEFAULT GETDATE()
--);
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

--CREATE TABLE Users (
--    UserID INT PRIMARY KEY IDENTITY(1,1),
--    Username NVARCHAR(50) NOT NULL UNIQUE,
--    Email NVARCHAR(100) NOT NULL UNIQUE,
--    PasswordHash NVARCHAR(255) NOT NULL,
--    FullName NVARCHAR(100),
--    Phone NVARCHAR(20),
--    Address NVARCHAR(MAX),
--    RoleID INT NOT NULL, 
--    ProfilePictureUrl NVARCHAR(255),
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    UpdatedAt DATETIME2,
--    CONSTRAINT CHK_Users_UpdatedAt CHECK (UpdatedAt IS NULL OR UpdatedAt >= CreatedAt),
--    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE NO ACTION
--);
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
--CREATE TABLE ProductCategories (
--    CategoryID INT PRIMARY KEY IDENTITY(1,1),
--    Name NVARCHAR(50) NOT NULL,
--    Cate_parent INT, -- Tham chiếu đến danh mục cha
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    FOREIGN KEY (Cate_parent) REFERENCES ProductCategories(CategoryID) ON DELETE NO ACTION,
--    CONSTRAINT UQ_ProductCategories_Name_CateParent UNIQUE (Name, Cate_parent)
--);
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
-- Table Products (depends on ProductCategories)
--CREATE TABLE Products (
--    ProductID INT PRIMARY KEY IDENTITY(1,1),
--    CategoryID INT NOT NULL,
--    Name NVARCHAR(100) NOT NULL,
--    Description NVARCHAR(MAX),
--    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
--    ImageURL NVARCHAR(500),
--    Stock INT NOT NULL CHECK (Stock >= 0),
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    FOREIGN KEY (CategoryID) REFERENCES ProductCategories(CategoryID) ON DELETE NO ACTION
--);
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
--CREATE TABLE Species (
--    SpeciesID INT PRIMARY KEY IDENTITY(1,1),
--    SpeciesName NVARCHAR(50) NOT NULL UNIQUE
--);
CREATE TABLE Species (
    SpeciesID INT PRIMARY KEY IDENTITY(1,1),
    SpeciesName NVARCHAR(50) NOT NULL UNIQUE,
    IsActive BIT DEFAULT 1
);
-- Table Pets (depends on Users)
--CREATE TABLE Pets (
--    PetID INT PRIMARY KEY IDENTITY(1,1),
--    UserID INT NOT NULL,
--    SpeciesID INT,  -- Khóa ngoại đến bảng Species
--    Name NVARCHAR(50) NOT NULL,
--    Breed NVARCHAR(50),
--    Age INT,
--    Gender NVARCHAR(10) CHECK (Gender IN ('Male', 'Female', 'Other')),
--    HealthCondition NVARCHAR(MAX),
--    SpecialNotes NVARCHAR(MAX),
--    LastSpaVisit DATETIME2,
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
--    -- Khóa ngoại
--    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
--    FOREIGN KEY (SpeciesID) REFERENCES Species(SpeciesID) ON DELETE SET NULL
--);
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
USE PetDataShop
GO

CREATE TABLE PetImages (
    PetImageId INT PRIMARY KEY IDENTITY(1,1),
    PetId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    DisplayOrder INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (PetId) REFERENCES Pets(PetID)
)
GO
GO
--CREATE TABLE Ser_cate (
--    CategoryID INT PRIMARY KEY IDENTITY(1,1),
--    Name NVARCHAR(50) NOT NULL UNIQUE,
--    Description NVARCHAR(MAX),
--    Cate_parent INT, -- Supports hierarchical structure
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    FOREIGN KEY (Cate_parent) REFERENCES Ser_cate(CategoryID) ON DELETE NO ACTION
--);
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
--CREATE TABLE Services (
--    ServiceID INT PRIMARY KEY IDENTITY(1,1),
--    Name NVARCHAR(100) NOT NULL,
--    Description NVARCHAR(MAX),
--    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
--    DurationMinutes INT CHECK (DurationMinutes > 0),
--    CategoryID INT NOT NULL, -- Changed from Category NVARCHAR(50)
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    FOREIGN KEY (CategoryID) REFERENCES Ser_cate(CategoryID) ON DELETE NO ACTION
--);
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
-- Table Promotions (independent)
--CREATE TABLE Promotions (
--    PromotionID INT PRIMARY KEY IDENTITY(1,1),
--    Code NVARCHAR(20) NOT NULL UNIQUE,
--    Description NVARCHAR(MAX),
--    DiscountType NVARCHAR(20) CHECK (DiscountType IN ('Percentage', 'Fixed')) NOT NULL,
--    DiscountValue DECIMAL(10, 2) NOT NULL CHECK (DiscountValue >= 0),
--    StartDate DATE NOT NULL,
--    EndDate DATE NOT NULL,
--    MinOrderValue DECIMAL(10, 2),
--    ApplicableTo NVARCHAR(20) CHECK (ApplicableTo IN ('Product', 'Service', 'Both')) NOT NULL,
--    MaxUsage INT,
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    CONSTRAINT CHK_Promotions_Dates CHECK (EndDate >= StartDate)
--);
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
-- Table Promotion_Products (depends on Promotions, Products)
--CREATE TABLE Promotion_Products (
--    PromotionProductID INT PRIMARY KEY IDENTITY(1,1),
--    PromotionID INT NOT NULL,
--    ProductID INT NOT NULL,
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID) ON DELETE CASCADE,
--    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
--    CONSTRAINT UQ_PromotionProduct UNIQUE (PromotionID, ProductID)
--);
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
-- Table Promotion_Services (depends on Promotions, Services)
--CREATE TABLE Promotion_Services (
--    PromotionServiceID INT PRIMARY KEY IDENTITY(1,1),
--    PromotionID INT NOT NULL,
--    ServiceID INT NOT NULL,
--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID) ON DELETE CASCADE,
--    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID) ON DELETE CASCADE,
--    CONSTRAINT UQ_PromotionService UNIQUE (PromotionID, ServiceID)
--);
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

CREATE TABLE Status_Appointment (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

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

-- Appointment - Pets relation
CREATE TABLE AppointmentPets (
    AppointmentPetID INT PRIMARY KEY IDENTITY(1,1),
    AppointmentID INT NOT NULL,
    PetID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID),
    FOREIGN KEY (PetID) REFERENCES Pets(PetID),
    CONSTRAINT UQ_AppointmentPet UNIQUE (AppointmentID, PetID)
);

-- Appointment - Services relation
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
CREATE TABLE StatusOrder (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL CHECK (TotalAmount >= 0),
    StatusID INT NOT NULL DEFAULT 1, -- Changed from Status NVARCHAR(20), default to Pending
    OrderDate DATETIME2 DEFAULT GETDATE(),
    ShippingAddress NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (StatusID) REFERENCES StatusOrder(StatusID) ON DELETE NO ACTION
);
GO
-- Table OrderItems (depends on Orders, Products)
CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE NO ACTION
);
GO
-- Table Cart (depends on Users, Products)
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
CREATE TABLE PaymentMethods (
    PaymentMethodID INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO
CREATE TABLE PaymentStatuses (
    PaymentStatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(20) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO
CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    UserID INT NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL CHECK (Amount >= 0),
    PaymentMethodID INT NOT NULL,
    PaymentStatusID INT NOT NULL DEFAULT 1, -- Default to Pending
    TransactionID NVARCHAR(100),
    PaymentDate DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE NO ACTION,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods(PaymentMethodID) ON DELETE NO ACTION,
    FOREIGN KEY (PaymentStatusID) REFERENCES PaymentStatuses(PaymentStatusID) ON DELETE NO ACTION
);
GO

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
-- Table Blogs (depends on Users)
CREATE TABLE Blogs (
    BlogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL, -- Employee who created the blog
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ContentFormat NVARCHAR(20) CHECK (ContentFormat IN ('HTML', 'Markdown')) DEFAULT 'Markdown',
    Category NVARCHAR(50),
    Status NVARCHAR(20) CHECK (Status IN ('Draft', 'PendingApproval', 'Approved', 'Rejected', 'Published', 'Archived')) DEFAULT 'Draft',
    PublishedAt DATETIME2,
    ApprovedBy INT, -- Admin who approved/rejected
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID) ON DELETE NO ACTION,
    CONSTRAINT CHK_Blogs_Published CHECK (Status <> 'Published' OR PublishedAt IS NOT NULL)
);
GO

-- Table Blog_Images (depends on Blogs)
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
CREATE TABLE Blog_Comments (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    BlogID INT NOT NULL,
    UserID INT NOT NULL,
    ParentCommentID INT NULL, 
    Content NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected')) DEFAULT 'Pending',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ,
    FOREIGN KEY (ParentCommentID) REFERENCES Blog_Comments(CommentID)
);
CREATE TABLE Blog_Likes (
    LikeID INT PRIMARY KEY IDENTITY(1,1),
    BlogID INT NOT NULL,
    UserID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ,
    UNIQUE(BlogID, UserID) -- Mỗi user chỉ like 1 lần
);

CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL, -- nếu bạn muốn gắn thông báo cho từng người dùng
    Title NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsRead BIT NOT NULL DEFAULT 0,

    FOREIGN KEY (UserId) REFERENCES Users(UserId) -- giả sử bạn đã có bảng Users
);
 ALTER TABLE OrderItems
ADD UnitPrice DECIMAL(18,2);

CREATE TABLE Blog_Comments (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    BlogID INT NOT NULL,
    UserID INT NOT NULL,
    ParentCommentID INT NULL, 
    Content NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected')) DEFAULT 'Pending',
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2,
    FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ,
    FOREIGN KEY (ParentCommentID) REFERENCES Blog_Comments(CommentID)
);
CREATE TABLE Blog_Likes (
    LikeID INT PRIMARY KEY IDENTITY(1,1),
    BlogID INT NOT NULL,
    UserID INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (BlogID) REFERENCES Blogs(BlogID) ,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ,
    UNIQUE(BlogID, UserID) -- Mỗi user chỉ like 1 lần
);


