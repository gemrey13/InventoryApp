CREATE TABLE user_account (
    ID INT PRIMARY KEY IDENTITY,
    lastName nchar(20),
    firstName nchar(20),
    email nchar(50),
    account_type INT,
	username nchar(20),
	[password] nchar(20)
);

CREATE TABLE item (
    ID INT PRIMARY KEY IDENTITY,
    name nchar(10),
    brand nchar(10),
    description nvarchar(100),
    dateAdded DATETIME,
    cost INT,
    [status] nvarchar(64),
    userID INT NULL,
    [highlight] nvarchar(64),
    FOREIGN KEY (userID) REFERENCES [user_account](ID)
);

CREATE TABLE request (
    ID INT PRIMARY KEY IDENTITY,
    employeeID INT,
    adminID INT,
    itemID INT, -- New column to reference the item
    [status] nchar(10),
    description nchar(50),
    creationDate DATETIME,
    FOREIGN KEY (employeeID) REFERENCES [user_account](ID),
    FOREIGN KEY (adminID) REFERENCES [user_account](ID),
    FOREIGN KEY (itemID) REFERENCES item(ID) -- Reference to the item table
);

CREATE TABLE account_history (
    ID INT PRIMARY KEY IDENTITY,
    userID INT,
    action nvarchar(100),
    actionDate DATETIME,
    FOREIGN KEY (userID) REFERENCES user_account(ID)
);



INSERT INTO user_account (lastName, firstName, email, account_type, username, [password])
VALUES ('Doe', 'John', 'john.doe@example.com', 1, 'admin', '123');
INSERT INTO user_account (lastName, firstName, email, account_type, username, [password])
VALUES ('Smith', 'Jane', 'jane.smith@example.com', 0, 'client', '123');
