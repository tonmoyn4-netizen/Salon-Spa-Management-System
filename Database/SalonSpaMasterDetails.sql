USE [master]
GO

IF DB_ID('SalonSpaMasterDetails') IS NOT NULL
DROP DATABASE SalonSpaMasterDetails
GO

CREATE DATABASE SalonSpaMasterDetails
GO

USE SalonSpaMasterDetails
GO

CREATE TABLE Service
(
    ServiceID INT NOT NULL PRIMARY KEY,
    ServiceName VARCHAR(50) NOT NULL,
    Cost MONEY NOT NULL
)
GO

CREATE TABLE Booking
(
    BookingID INT PRIMARY KEY NOT NULL,
    CustomerName VARCHAR(50) NOT NULL,
    ServiceID INT REFERENCES Service(ServiceID),
    CustomerImage VARBINARY(MAX),
    IsMember BIT NOT NULL,
    TotalAmount MONEY NOT NULL
)
GO

CREATE TABLE BookingDetails
(
    BookingDetailID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT REFERENCES Booking(BookingID),
    ItemName VARCHAR(50) NOT NULL,
    Quantity INT NOT NULL
)
GO

INSERT INTO Service VALUES
(1, 'Hair Cut', 500),
(2, 'Facial', 1200),
(3, 'Hair Spa', 1800)
GO

SELECT * FROM BookingDetails
GO
