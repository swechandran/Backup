CREATE TABLE RegisteredUsers (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    DateOfBirth DATE,
    Gender VARCHAR(10),
    PhoneNumber VARCHAR(20),
    EmailAddress VARCHAR(100),
    Address VARCHAR(200),
    Username VARCHAR(50),
    Password VARCHAR(100)
)
INSERT INTO RegisteredUsers (FirstName, LastName, DateOfBirth, Gender, PhoneNumber, EmailAddress, Address, Username, Password)
VALUES 
('Ravi', 'Chandran', '1990-05-15', 'Male', '123-456-7890', 'ravi@example.com', '123 Main St', 'Ravi', 'Ravi@123');


select * from RegisteredUsers;
S
CREATE PROCEDURE SPI_RegisteredUsers
    @UserID INT= NULL,@FirstName VARCHAR(50),@LastName VARCHAR(50),@DateOfBirth DATE,@Gender VARCHAR(10),@PhoneNumber VARCHAR(15),
    @EmailAddress VARCHAR(100),@Address VARCHAR(255),@Username VARCHAR(50),
    @Password VARCHAR(100)
AS
BEGIN
    INSERT INTO RegisteredUsers(FirstName,LastName,DateOfBirth,Gender,PhoneNumber,EmailAddress,Address,Username,Password)
    VALUES (@FirstName,@LastName,@DateOfBirth,@Gender,@PhoneNumber,@EmailAddress,@Address,@Username,@Password)
END;

CREATE PROCEDURE SPR_GetAllRegisteredUsers
AS
BEGIN
    SELECT 
        UserID,FirstName,LastName,DateOfBirth,Gender,PhoneNumber,EmailAddress,Address,Username,Password
    FROM RegisteredUsers;
END;

CREATE PROCEDURE SPR_GetRegisteredUsersbyId
@UserID int  
AS
BEGIN
    SELECT 
        UserID,FirstName,LastName,DateOfBirth,Gender,PhoneNumber,EmailAddress,Address,Username,Password
    FROM RegisteredUsers WHERE UserID=@UserID
END;

CREATE PROCEDURE SPU_EditRegisteredUsers
    @UserID INT,@FirstName VARCHAR(50),@LastName VARCHAR(50),@DateOfBirth DATE,@Gender VARCHAR(10),@PhoneNumber VARCHAR(15),
    @EmailAddress VARCHAR(100),@Address VARCHAR(255),@Username VARCHAR(50),
    @Password VARCHAR(100)
AS
BEGIN
    UPDATE RegisteredUsers
    SET 
        FirstName = @FirstName,LastName = @LastName,DateOfBirth = @DateOfBirth,Gender = @Gender,PhoneNumber = @PhoneNumber,
        EmailAddress = @EmailAddress,Address = @Address,Username = @Username,Password = @Password
    WHERE 
        UserID = @UserID ;
END;

CREATE PROCEDURE SPD_DeleteRegisteredUsers
    @UserID INT
AS
BEGIN
    DELETE FROM RegisteredUsers
    WHERE UserID = @UserID  
END;

CREATE TABLE ErrorLog (
    ErrorID INT IDENTITY(1,1) PRIMARY KEY,
    ErrorMessage NVARCHAR(500),
    ErrorDate DATETIME DEFAULT GETDATE()
);
CREATE PROCEDURE InsertErrorLog 
    @ErrorMessage NVARCHAR(500)
AS
BEGIN
    INSERT INTO ErrorLog (ErrorMessage)
    VALUES (@ErrorMessage);
END;
select * from ErrorLog;
