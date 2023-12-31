CREATE TABLE [CartItems] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [ProductId] int,
  [Quantity] int,
  [SizeQuantityId] int,
  [IsDeleted] bool,
  [IsOrdered] bool,
  [CartId] int,
  [created_at] datetime
)
GO

CREATE TABLE [Carts] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [SupplierId] int,
  [UserId] int,
  [IsDeleted] bool,
  [DateCreated] datetime,
  [created_at] datetime
)
GO

CREATE TABLE [Departments] (
  [DepartmentId] int PRIMARY KEY IDENTITY(1, 1),
  [Department_Name] varchar(max)
)
GO

CREATE TABLE [Notifications] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [UserId] int,
  [OrderId] int,
  [Message] varchar(max),
  [DateCreated] datetime,
  [DateUpdated] datetime,
  [IsRead] bool,
  [UserRole] int,
  [created_at] datetime
)
GO

CREATE TABLE [OrderItems] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [OrderId] int,
  [ProductId] int,
  [Quantity] int,
  [SizeQuantityId] int,
  [created_at] datetime
)
GO

CREATE TABLE [Orders] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [UserId] int,
  [CartId] int,
  [OrderNumber] varchar(max),
  [ReferenceId] varchar(max),
  [ProofOfPayment] varchar(max),
  [Total] real,
  [DateCreated] datetime,
  [DateUpdated] datetime,
  [EstimateDate] datetime,
  [PaymentType] int,
  [Status] int,
  [IsDeleted] bool,
  [created_at] datetime
)
GO

CREATE TABLE [ProductDepartments] (
  [ProductId] int,
  [DepartmentId] int
)
GO

CREATE TABLE [Products] (
  [ProductId] int PRIMARY KEY IDENTITY(1, 1),
  [SupplierId] int,
  [ProductTypeId] int,
  [RatingId] int,
  [ProductName] varchar(max),
  [Description] varchar(max),
  [Category] varchar(max),
  [Image] varchar(max),
  [FrontViewImage] varchar(max),
  [SideViewImage] varchar(max),
  [BackViewImage] varchar(max),
  [SizeGuide] varchar(max),
  [Price] real,
  [IsActive] bool,
  [created_at] datetime
)
GO

CREATE TABLE [ProductTypes] (
  [ProductTypeId] int PRIMARY KEY IDENTITY(1, 1),
  [Product_Type] varchar(max)
)
GO

CREATE TABLE [Ratings] (
  [RatingId] int PRIMARY KEY IDENTITY(1, 1),
  [UserId] int,
  [ProductId] int,
  [SupplierId] int,
  [Value] int,
  [DateCreated] datetime,
  [Role] int,
  [created_at] datetime
)
GO

CREATE TABLE [SizeQuantities] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [ProductId] int,
  [Size] varchar(max),
  [Quantity] int,
  [created_at] datetime
)
GO

CREATE TABLE [Users] (
  [Id] int PRIMARY KEY,
  [DepartmentId] int,
  [RatingId] int,
  [FirstName] varchar(max),
  [LastName] varchar(max),
  [Password] varchar(max),
  [Email] varchar(max),
  [PhoneNumber] varchar(max),
  [Gender] varchar(max),
  [ShopName] varchar(max),
  [Address] varchar(max),
  [Image] varchar(max),
  [StudyLoad] varchar(max),
  [BIR] varchar(max),
  [CityPermit] varchar(max),
  [SchoolPermit] varchar(max),
  [IsActive] bool,
  [Role] int,
  [IsValidate] bool,
  [ConfirmationCode] varchar(max),
  [EmailConfirmationToken] varchar(max),
  [IsEmailConfirmed] bool,
  [PasswordResetToken] varchar(max),
  [ResetTokenExpires] datetime,
  [DateCreated] datetime,
  [DateUpdated] datetime
)
GO

CREATE TABLE [follows] (
  [following_user_id] int,
  [followed_user_id] int,
  [created_at] timestamp
)
GO

CREATE TABLE [users] (
  [id] int PRIMARY KEY,
  [username] nvarchar(255),
  [role] nvarchar(255),
  [created_at] timestamp
)
GO

CREATE TABLE [posts] (
  [id] int PRIMARY KEY,
  [title] nvarchar(255),
  [body] text,
  [user_id] int,
  [status] nvarchar(255),
  [created_at] timestamp
)
GO

EXEC sp_addextendedproperty
@name = N'Column_Description',
@value = 'Content of the post',
@level0type = N'Schema', @level0name = 'dbo',
@level1type = N'Table',  @level1name = 'posts',
@level2type = N'Column', @level2name = 'body';
GO

ALTER TABLE [CartItems] ADD FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id])
GO

ALTER TABLE [CartItems] ADD FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
GO

ALTER TABLE [CartItems] ADD FOREIGN KEY ([SizeQuantityId]) REFERENCES [SizeQuantities] ([Id])
GO

ALTER TABLE [Carts] ADD FOREIGN KEY ([SupplierId]) REFERENCES [Users] ([Id])
GO

ALTER TABLE [Notifications] ADD FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id])
GO

ALTER TABLE [OrderItems] ADD FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id])
GO

ALTER TABLE [OrderItems] ADD FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
GO

ALTER TABLE [OrderItems] ADD FOREIGN KEY ([SizeQuantityId]) REFERENCES [SizeQuantities] ([Id])
GO

ALTER TABLE [Orders] ADD FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id])
GO

ALTER TABLE [Orders] ADD FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
GO

ALTER TABLE [ProductDepartments] ADD FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
GO

ALTER TABLE [ProductDepartments] ADD FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId])
GO

ALTER TABLE [Products] ADD FOREIGN KEY ([ProductTypeId]) REFERENCES [ProductTypes] ([ProductTypeId])
GO

ALTER TABLE [Products] ADD FOREIGN KEY ([RatingId]) REFERENCES [Ratings] ([RatingId])
GO

ALTER TABLE [Products] ADD FOREIGN KEY ([SupplierId]) REFERENCES [Users] ([Id])
GO

ALTER TABLE [Ratings] ADD FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
GO

ALTER TABLE [Ratings] ADD FOREIGN KEY ([SupplierId]) REFERENCES [Users] ([Id])
GO

ALTER TABLE [Ratings] ADD FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
GO

ALTER TABLE [SizeQuantities] ADD FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
GO

ALTER TABLE [Users] ADD FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId])
GO

ALTER TABLE [Users] ADD FOREIGN KEY ([RatingId]) REFERENCES [Ratings] ([RatingId])
GO

ALTER TABLE [posts] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [follows] ADD FOREIGN KEY ([following_user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [follows] ADD FOREIGN KEY ([followed_user_id]) REFERENCES [users] ([id])
GO
