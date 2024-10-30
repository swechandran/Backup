-- Table 1: Student 
CREATE  TABLE Student (
    ID INT PRIMARY KEY 
	CONSTRAINT FK_StudentDetails_Student FOREIGN KEY (ID) REFERENCES StudentDetails(StudentID)
);

select * from Student

-- Table 2: StudentDetails 
CREATE TABLE StudentDetails (
    StudentID INT Primary key,  
    Name NVARCHAR(50),
    Age INT,
    Email NVARCHAR(100),  
);
select * from StudentDetails

INSERT INTO Student (ID) VALUES (1), (2);
INSERT INTO Student (ID) VALUES(3);

INSERT INTO StudentDetails (StudentID, Name, Age, Email) 
VALUES 
(1, 'John', 22, 'john@example.com'),
(2, 'Jane', 23, 'jane@example.com');
INSERT INTO StudentDetails (StudentID, Name, Age, Email) 
VALUES 
(3, 'Mohan', 22, 'mohan@example.com');
INSERT INTO StudentDetails (StudentID, Name, Age, Email) 
VALUES 
(4, 'Sam', 22, 'sam@example.com');
