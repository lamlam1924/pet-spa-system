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
