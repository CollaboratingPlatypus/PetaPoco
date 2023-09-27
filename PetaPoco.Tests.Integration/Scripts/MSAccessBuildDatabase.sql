DROP TABLE [OrderLines];
DROP TABLE [Orders];
DROP TABLE [People];
DROP TABLE [SpecificOrderLines];
DROP TABLE [SpecificOrders];
DROP TABLE [SpecificPeople];
DROP TABLE [TransactionLogs];
DROP TABLE [Note];

CREATE TABLE [People] (
	[Id] TEXT NOT NULL PRIMARY KEY,
	[FullName] MEMO,
	[Age] Long NOT NULL,
	[Height] INTEGER NOT NULL,
	[Dob] Datetime NULL
);

CREATE TABLE [Orders] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[PersonId] TEXT CONSTRAINT FK_O_PersonId REFERENCES [People](Id),
	[PoNumber] MEMO NOT NULL,
	[OrderStatus] INTEGER NOT NULL,
	[CreatedOn] Datetime NOT NULL,
	[CreatedBy] MEMO NOT NULL
);

CREATE TABLE [OrderLines] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[OrderId] INTEGER NOT NULL CONSTRAINT FK_OL_OrderId REFERENCES [Orders](Id),
	[Qty] INTEGER NOT NULL,
	[Status] INTEGER NOT NULL,
	[SellPrice] NUMERIC(10, 4) NOT NULL
);

CREATE TABLE [SpecificPeople] (
	[Id] TEXT NOT NULL PRIMARY KEY,
	[FullName] MEMO,
	[Age] Long NOT NULL,
	[Height] INTEGER NOT NULL,
	[Dob] Datetime NULL
);

CREATE TABLE [SpecificOrders] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[PersonId] TEXT CONSTRAINT FK_SO_PersonId REFERENCES [SpecificPeople](Id),
	[PoNumber] MEMO NOT NULL,
	[OrderStatus] INTEGER NOT NULL,
	[CreatedOn] Datetime NOT NULL,
	[CreatedBy] MEMO NOT NULL
);

CREATE TABLE [SpecificOrderLines] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[OrderId] INTEGER NOT NULL CONSTRAINT FK_SO_OrderId REFERENCES [SpecificOrders](Id),
	[Qty] INTEGER NOT NULL,
	[Status] INTEGER NOT NULL,
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

-- MSAccess Specific Tables;

DROP TABLE [JoinableOrderLines];
DROP TABLE [JoinableOrders];
DROP TABLE [JoinablePeople];

CREATE TABLE [JoinablePeople] (
	[Id] INTEGER NOT NULL PRIMARY KEY,
	[FullName] MEMO,
	[Age] Long NOT NULL,
	[Height] INTEGER NOT NULL,
	[Dob] Datetime NULL
);

CREATE TABLE [JoinableOrders] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[JoinablePersonId] INTEGER CONSTRAINT FK_O_JoinablePersonId REFERENCES [JoinablePeople](Id),
	[PoNumber] MEMO NOT NULL,
	[OrderStatus] INTEGER NOT NULL,
	[CreatedOn] Datetime NOT NULL,
	[CreatedBy] MEMO NOT NULL
);

CREATE TABLE [JoinableOrderLines] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[JoinableOrderId] INTEGER NOT NULL CONSTRAINT FK_OL_JoinableOrderId REFERENCES [JoinableOrders](Id),
	[Qty] INTEGER NOT NULL,
	[Status] INTEGER NOT NULL,
	[SellPrice] NUMERIC(10, 4) NOT NULL
);

-- Investigation Tables;

DROP TABLE [BugInvestigation_10R9LZYK];

CREATE TABLE [BugInvestigation_10R9LZYK] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[TestColumn1] BINARY
);

DROP TABLE [BugInvestigation_3F489XV0];

CREATE TABLE [BugInvestigation_3F489XV0] (
	[Id] AUTOINCREMENT PRIMARY KEY,
	[TC1] INTEGER NOT NULL,
	[TC2] INTEGER NOT NULL,
	[TC3] INTEGER NOT NULL,
	[TC4] INTEGER NOT NULL
);

DROP TABLE [BugInvestigation_64O6LT8U];

CREATE TABLE BugInvestigation_64O6LT8U (
	[ColumnA] TEXT(20),
	[Column2] TEXT(20)
);

DROP TABLE [BugInvestigation_5TN5C4U4];

CREATE TABLE BugInvestigation_5TN5C4U4 (
	[ColumnA] TEXT(20),
	[Column2] TEXT(20)
);
