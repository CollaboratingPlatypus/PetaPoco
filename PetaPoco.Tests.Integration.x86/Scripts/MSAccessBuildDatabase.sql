DROP TABLE [OrderLines];
DROP TABLE [Orders];
DROP TABLE [People];
DROP TABLE [SpecificOrderLines];
DROP TABLE [SpecificOrders];
DROP TABLE [SpecificPeople];
DROP TABLE [TransactionLogs];
DROP TABLE [Note];

CREATE TABLE [People] (
	[Id] MEMO NOT NULL PRIMARY KEY,
	[FullName] MEMO,
	[Age] Long NOT NULL,
	[Height] INTEGER NOT NULL,
	[Dob] Datetime NOT NULL
);

CREATE TABLE [Orders] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[PersonId] MEMO CONSTRAINT FK_O_PersonId REFERENCES [People](Id),
	[PoNumber] MEMO NOT NULL,
	[OrderStatus] INTEGER NOT NULL,
	[CreatedOn] Datetime NOT NULL,
	[CreatedBy] MEMO NOT NULL
);

CREATE TABLE [OrderLines] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[OrderId] INTEGER NOT NULL CONSTRAINT FK_OL_OrderId REFERENCES [Orders](Id),
	[Qty] INTEGER NOT NULL,
	[SellPrice] NUMERIC(10, 4) NOT NULL
);

CREATE TABLE [SpecificPeople] (
	[Id] MEMO NOT NULL PRIMARY KEY,
	[FullName] MEMO,
	[Age] Long NOT NULL,
	[Height] INTEGER NOT NULL,
	[Dob] Datetime NOT NULL
);

CREATE TABLE [SpecificOrders] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[PersonId] MEMO CONSTRAINT FK_SO_PersonId REFERENCES [SpecificPeople](Id),
	[PoNumber] MEMO NOT NULL,
	[OrderStatus] INTEGER NOT NULL,
	[CreatedOn] Datetime NOT NULL,
	[CreatedBy] MEMO NOT NULL
);

CREATE TABLE [SpecificOrderLines] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[OrderId] INTEGER NOT NULL CONSTRAINT FK_SO_OrderId REFERENCES [SpecificOrders](Id),
	[Qty] INTEGER NOT NULL,
	[SellPrice] NUMERIC(10, 4) NOT NULL
);

CREATE TABLE [TransactionLogs] (
	[Description] MEMO,
	[CreatedOn] Datetime NOT NULL
);

CREATE TABLE [Note] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[Text] MEMO NOT NULL,
	[CreatedOn] Datetime NOT NULL
);

-- Investigation Tables;

DROP TABLE [BugInvestigation_10R9LZYK];

CREATE TABLE [BugInvestigation_10R9LZYK] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[TestColumn1] BINARY
);