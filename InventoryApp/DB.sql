CREATE TABLE user_account (
    ID INT PRIMARY KEY,
    lastName nchar(20),
    firstName nchar(20),
    email nchar(20),
    account_type INT,
	username nchar(20),
	[password] nchar(20)
);

CREATE TABLE item (
    ID INT PRIMARY KEY,
    name nchar(10),
    image IMAGE,
    brand nchar(10),
    description nvarchar(100),
    dateAdded DATETIME,
    cost INT,
    [status] nvarchar(10),
    userID INT,
    color nchar(10),
    FOREIGN KEY (userID) REFERENCES [user_account](ID)
);

CREATE TABLE request (
    ID INT PRIMARY KEY,
    employeeID INT,
    adminID INT,
    [status] nchar(10),
    description nchar(50),
    creationDate DATETIME,
    FOREIGN KEY (employeeID) REFERENCES [user_account](ID),
    FOREIGN KEY (adminID) REFERENCES [user_account](ID)
);
