--Task3-WindowService-automatic fetch Stu.Detail

CREATE TABLE Students (
  StudentID INT PRIMARY KEY IDENTITY(1,1),
  Name VARCHAR(50),
  Age INT
);

INSERT INTO Students (Name, Age) VALUES 
('Alice', 15),
('Bob', 20),
('Charlie', 17);

