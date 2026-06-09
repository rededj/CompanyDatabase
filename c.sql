USE master;
GO
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'cource')
    CREATE DATABASE cource COLLATE Cyrillic_General_CI_AS;
GO
USE cource;
GO

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Cost MONEY NOT NULL CHECK (Cost >= 1),
    OperationsRequired TINYINT NOT NULL DEFAULT 0
);

CREATE TABLE Workshops (
    WorkshopID INT IDENTITY(1,1) PRIMARY KEY,
    WorkshopName NVARCHAR(100) NOT NULL UNIQUE,
    Adress NVARCHAR(255) NOT NULL
);

CREATE TABLE Blueprint (
    BlueprintNumber VARCHAR(50) PRIMARY KEY,
    TechnicalRequirements NVARCHAR(MAX) NOT NULL
);

CREATE TABLE Operations (
    OperationID INT IDENTITY(1,1) PRIMARY KEY,
    Description NVARCHAR(500) NOT NULL,
    AverageDuration TIME NOT NULL,
    BlueprintNumber VARCHAR(50) NOT NULL
);

CREATE TABLE Materials (
    MaterialID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    NumberOf INT NOT NULL CHECK (NumberOf > -1)
);

CREATE TABLE ProductsOperations (
    ProductID INT NOT NULL,
    OperationID INT NOT NULL,
    CONSTRAINT PK_ProductsOperations PRIMARY KEY (ProductID, OperationID)
);

CREATE TABLE OperationMaterialsUsage (
    OperationID INT NOT NULL,
    MaterialID INT NOT NULL,
    RequiredQuantity SMALLINT NOT NULL CHECK (RequiredQuantity > 0),
    CONSTRAINT PK_OperationMaterialsUsage PRIMARY KEY (OperationID, MaterialID)
);

CREATE TABLE ToolTypes (
    ToolTypeID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    InStock INT NOT NULL DEFAULT 0 CHECK (InStock >= 0),
    Allocated INT NULL DEFAULT 0
);

CREATE TABLE WorkOrder (
    WorkOrderID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    RegistrationDate DATE NOT NULL,
    DueDate DATE NOT NULL,
    RequiredQuantity INT NOT NULL CHECK (RequiredQuantity > 0),
    Completed BIT NOT NULL DEFAULT 0
);

CREATE TABLE Tools (
    SerialNumber NVARCHAR(50) PRIMARY KEY,
    ToolTypeID INT NOT NULL,
    ArrivalDate DATE NOT NULL,
    CurrentWorkOrderID INT NULL
);

CREATE TABLE OperationToolsUsage (
    OperationID INT NOT NULL,
    ToolTypeID INT NOT NULL,
    QuantityInUse SMALLINT NOT NULL CHECK (QuantityInUse > 0),
    CONSTRAINT PK_OperationToolsUsage PRIMARY KEY (OperationID, ToolTypeID)
);

CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Login NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    RoleID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    WorkshopName NVARCHAR(100) NULL
);

CREATE TABLE MaterialIssuance (
    IssuanceID INT IDENTITY(1,1) PRIMARY KEY,
    WorkOrderID INT NOT NULL,
    OperationID INT NOT NULL,
    UserID INT NOT NULL,
    MaterialID INT NOT NULL,
    ActualQuantity INT NOT NULL CHECK (ActualQuantity > 0),
    IssueDateTime DATETIME NOT NULL
);

CREATE TABLE ToolIssuance (
    IssuanceID INT IDENTITY(1,1) PRIMARY KEY,
    WorkOrderID INT NOT NULL,
    OperationID INT NOT NULL,
    SerialNumber NVARCHAR(50) NOT NULL,
    WorkshopID INT NOT NULL,
    UserID INT NOT NULL,
    IssueDateTime DATE NOT NULL,
    ReturnDateTime DATE NOT NULL,
    ActualReturnDate DATE NULL
);
GO

/*ALTER TABLE Operations ADD CONSTRAINT FK_Operations_Workshops
    FOREIGN KEY (WorkshopID) REFERENCES Workshops(WorkshopID);
*/
ALTER TABLE Operations ADD CONSTRAINT FK_Operations_Blueprint
    FOREIGN KEY (BlueprintNumber) REFERENCES Blueprint(BlueprintNumber);
GO

ALTER TABLE ProductsOperations ADD CONSTRAINT FK_ProductsOperations_Products
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID);
ALTER TABLE ProductsOperations ADD CONSTRAINT FK_ProductsOperations_Operations
    FOREIGN KEY (OperationID) REFERENCES Operations(OperationID);
GO

ALTER TABLE OperationMaterialsUsage ADD CONSTRAINT FK_OperationMaterialsUsage_Operations
    FOREIGN KEY (OperationID) REFERENCES Operations(OperationID);
ALTER TABLE OperationMaterialsUsage ADD CONSTRAINT FK_OperationMaterialsUsage_Materials
    FOREIGN KEY (MaterialID) REFERENCES Materials(MaterialID);
GO

ALTER TABLE WorkOrder ADD CONSTRAINT FK_WorkOrder_Products
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID);
GO

ALTER TABLE Tools ADD CONSTRAINT FK_Tools_ToolTypes
    FOREIGN KEY (ToolTypeID) REFERENCES ToolTypes(ToolTypeID);
ALTER TABLE Tools ADD CONSTRAINT FK_Tools_WorkOrder
    FOREIGN KEY (CurrentWorkOrderID) REFERENCES WorkOrder(WorkOrderID);
GO

ALTER TABLE OperationToolsUsage ADD CONSTRAINT FK_OperationToolsUsage_Operations
    FOREIGN KEY (OperationID) REFERENCES Operations(OperationID);
ALTER TABLE OperationToolsUsage ADD CONSTRAINT FK_OperationToolsUsage_ToolTypes
    FOREIGN KEY (ToolTypeID) REFERENCES ToolTypes(ToolTypeID);
GO

ALTER TABLE Users ADD CONSTRAINT FK_Users_Role
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID);
GO

ALTER TABLE MaterialIssuance ADD CONSTRAINT FK_MaterialIssuance_WorkOrder
    FOREIGN KEY (WorkOrderID) REFERENCES WorkOrder(WorkOrderID);
ALTER TABLE MaterialIssuance ADD CONSTRAINT FK_MaterialIssuance_Operations
    FOREIGN KEY (OperationID) REFERENCES Operations(OperationID);
ALTER TABLE MaterialIssuance ADD CONSTRAINT FK_MaterialIssuance_Users
    FOREIGN KEY (UserID) REFERENCES Users(UserID);
ALTER TABLE MaterialIssuance ADD CONSTRAINT FK_MaterialIssuance_Materials
    FOREIGN KEY (MaterialID) REFERENCES Materials(MaterialID);
GO

ALTER TABLE ToolIssuance ADD CONSTRAINT FK_ToolIssuance_WorkOrder
    FOREIGN KEY (WorkOrderID) REFERENCES WorkOrder(WorkOrderID);
ALTER TABLE ToolIssuance ADD CONSTRAINT FK_ToolIssuance_Operations
    FOREIGN KEY (OperationID) REFERENCES Operations(OperationID);
ALTER TABLE ToolIssuance ADD CONSTRAINT FK_ToolIssuance_Tools
    FOREIGN KEY (SerialNumber) REFERENCES Tools(SerialNumber);
ALTER TABLE ToolIssuance ADD CONSTRAINT FK_ToolIssuance_Workshops
    FOREIGN KEY (WorkshopID) REFERENCES Workshops(WorkshopID);
ALTER TABLE ToolIssuance ADD CONSTRAINT FK_ToolIssuance_Users
    FOREIGN KEY (UserID) REFERENCES Users(UserID);
GO

CREATE TRIGGER trg_ProductsOperations_Insert
ON ProductsOperations
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p
    SET OperationsRequired = p.OperationsRequired + 1
    FROM Products p
    INNER JOIN inserted i ON p.ProductID = i.ProductID;
END;
GO

CREATE TRIGGER trg_ProductsOperations_Delete
ON ProductsOperations
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p
    SET OperationsRequired = p.OperationsRequired - 1
    FROM Products p
    INNER JOIN deleted d ON p.ProductID = d.ProductID;
END;
GO

CREATE TRIGGER trg_MaterialIssuance_Insert
ON MaterialIssuance
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Materials m ON i.MaterialID = m.MaterialID
        WHERE i.ActualQuantity > m.NumberOf
    )
    BEGIN
        RAISERROR('Недостаточно материала на складе', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    UPDATE m
    SET NumberOf = m.NumberOf - i.ActualQuantity
    FROM Materials m
    INNER JOIN inserted i ON m.MaterialID = i.MaterialID;
END;
GO

CREATE TRIGGER trg_Tools_Insert
ON Tools
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tt
    SET InStock = tt.InStock + 1
    FROM ToolTypes tt
    INNER JOIN inserted i ON tt.ToolTypeID = i.ToolTypeID;
END;
GO

CREATE TRIGGER trg_Tools_Delete
ON Tools
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE tt
    SET InStock = tt.InStock - 1
    FROM ToolTypes tt
    INNER JOIN deleted d ON tt.ToolTypeID = d.ToolTypeID;
END;
GO

CREATE TRIGGER trg_ToolIssuance_Insert
ON ToolIssuance
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        WHERE EXISTS (
            SELECT 1 FROM Tools t
            WHERE t.SerialNumber = i.SerialNumber AND t.CurrentWorkOrderID IS NOT NULL
        )
    )
    BEGIN
        RAISERROR('Инструмент уже выдан', 16, 1);
        ROLLBACK;
        RETURN;
    END

    UPDATE tt
    SET InStock = tt.InStock - 1,
        Allocated = ISNULL(tt.Allocated, 0) + 1
    FROM ToolTypes tt
    INNER JOIN inserted i ON tt.ToolTypeID = (SELECT ToolTypeID FROM Tools WHERE SerialNumber = i.SerialNumber);

    UPDATE t
    SET CurrentWorkOrderID = i.WorkOrderID
    FROM Tools t
    INNER JOIN inserted i ON t.SerialNumber = i.SerialNumber;
END;
GO

CREATE TRIGGER trg_ToolIssuance_Update
ON ToolIssuance
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(ActualReturnDate)
    BEGIN
        UPDATE tt
        SET InStock = tt.InStock + 1,
            Allocated = tt.Allocated - 1
        FROM ToolTypes tt
        INNER JOIN inserted i ON tt.ToolTypeID = (SELECT ToolTypeID FROM Tools WHERE SerialNumber = i.SerialNumber)
        WHERE i.ActualReturnDate IS NOT NULL 
          AND (SELECT ActualReturnDate FROM deleted d WHERE d.IssuanceID = i.IssuanceID) IS NULL;

        UPDATE t
        SET CurrentWorkOrderID = NULL
        FROM Tools t
        INNER JOIN inserted i ON t.SerialNumber = i.SerialNumber
        WHERE i.ActualReturnDate IS NOT NULL 
          AND (SELECT ActualReturnDate FROM deleted d WHERE d.IssuanceID = i.IssuanceID) IS NULL;
    END
END;
GO