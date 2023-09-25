-- Need to drop functions first, because Postgres stores the table relations
DROP FUNCTION IF EXISTS SelectPeople();
DROP FUNCTION IF EXISTS SelectPeopleWithParam(age integer);
DROP FUNCTION IF EXISTS CountPeople();
DROP FUNCTION IF EXISTS CountPeopleWithParam(age integer);
DROP FUNCTION IF EXISTS UpdatePeople();
DROP FUNCTION IF EXISTS UpdatePeopleWithParam(age integer);

DROP TABLE IF EXISTS "OrderLines";
DROP TABLE IF EXISTS "Orders";
DROP TABLE IF EXISTS "People";
DROP TABLE IF EXISTS "SpecificOrderLines";
DROP TABLE IF EXISTS "SpecificOrders";
DROP TABLE IF EXISTS "SpecificPeople";
DROP TABLE IF EXISTS "TransactionLogs";
DROP TABLE IF EXISTS "Note";

CREATE TABLE "People" (
	"Id" UUID NOT NULL PRIMARY KEY,
	"FullName" VARCHAR(255),
	"Age" BIGINT NOT NULL,
	"Height" INT NOT NULL,
	"Dob" TIMESTAMP NULL
);

CREATE TABLE "Orders" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"PersonId" UUID NOT NULL,
	"PoNumber" VARCHAR(15) NOT NULL,
	"OrderStatus" INT NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL,
	"CreatedBy" VARCHAR(255) NOT NULL,
	CONSTRAINT "FK_Orders_People" FOREIGN KEY ("PersonId") REFERENCES "People"("Id")
);

CREATE TABLE "OrderLines" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"OrderId" INT NOT NULL,
	"Qty" SMALLINT NOT NULL,
	"Status" SMALLINT NOT NULL,
	"SellPrice" NUMERIC(10, 4) NOT NULL,
	CONSTRAINT "FK_OrderLines_Orders" FOREIGN KEY ("OrderId") REFERENCES "Orders"("Id")
);

CREATE TABLE "SpecificPeople" (
	"Id" UUID NOT NULL PRIMARY KEY,
	"FullName" VARCHAR(255),
	"Age" BIGINT NOT NULL,
	"Height" INT NOT NULL,
	"Dob" TIMESTAMP NULL
);

CREATE TABLE "SpecificOrders" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"PersonId" UUID NOT NULL,
	"PoNumber" VARCHAR(15) NOT NULL,
	"OrderStatus" INT NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL,
	"CreatedBy" VARCHAR(255) NOT NULL,
	CONSTRAINT "FK_SpecificOrders_SpecificPeople" FOREIGN KEY ("PersonId") REFERENCES "SpecificPeople"("Id")
);

CREATE TABLE "SpecificOrderLines" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"OrderId" INT NOT NULL,
	"Qty" SMALLINT NOT NULL,
	"Status" SMALLINT NOT NULL,
	"SellPrice" NUMERIC(10, 4) NOT NULL,
	CONSTRAINT "FK_SpecificOrderLines_SpecificOrders" FOREIGN KEY ("OrderId") REFERENCES "SpecificOrders"("Id")
);

CREATE TABLE "TransactionLogs" (
	"Description" VARCHAR(5000) NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL
);

CREATE TABLE "Note" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"Text" VARCHAR(5000) NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL
);

-- MSAccess Specific Tables;

DROP TABLE IF EXISTS "BugInvestigation_7K2TX4VR";

CREATE TABLE "BugInvestigation_7K2TX4VR" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"Json1" JSON NOT NULL,
	"Json2" JSONB NOT NULL
);

-- Investigation Tables

DROP TABLE IF EXISTS "BugInvestigation_10R9LZYK";

CREATE TABLE "BugInvestigation_10R9LZYK" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"TestColumn1" BYTEA
);

DROP TABLE IF EXISTS "BugInvestigation_3F489XV0";

CREATE TABLE "BugInvestigation_3F489XV0" (
	"Id" SERIAL PRIMARY KEY NOT NULL,
	"TC1" INT NOT NULL,
	"TC2" INT NOT NULL,
	"TC3" INT NOT NULL,
	"TC4" INT NOT NULL
);

DROP TABLE IF EXISTS BugInvestigation_64O6LT8U;

CREATE TABLE BugInvestigation_64O6LT8U (
	"ColumnA" VARCHAR(20),
	"Column2" VARCHAR(20)
);

DROP TABLE IF EXISTS BugInvestigation_5TN5C4U4;

CREATE TABLE BugInvestigation_5TN5C4U4 (
	"ColumnA" VARCHAR(20),
	"Column2" VARCHAR(20)
);

-- Stored Procedures

CREATE FUNCTION SelectPeople()
RETURNS SETOF "People"
AS $$ BEGIN RETURN QUERY SELECT * FROM "People";  END $$
LANGUAGE plpgsql;

CREATE FUNCTION SelectPeopleWithParam(age integer)
RETURNS SETOF "People"
AS $$ BEGIN RETURN QUERY SELECT * FROM "People" WHERE "Age" > age;  END $$
LANGUAGE plpgsql;

CREATE FUNCTION CountPeople()
RETURNS integer
AS $$ BEGIN RETURN (SELECT COUNT(*) FROM "People");  END $$
LANGUAGE plpgsql;

CREATE FUNCTION CountPeopleWithParam(age integer)
RETURNS integer
AS $$ BEGIN RETURN (SELECT COUNT(*) FROM "People" WHERE "Age" > age);  END $$
LANGUAGE plpgsql;

CREATE FUNCTION UpdatePeople()
RETURNS VOID
AS $$ BEGIN UPDATE "People" SET "FullName" = 'Updated';  END $$
LANGUAGE plpgsql;

CREATE  FUNCTION UpdatePeopleWithParam(age integer)
RETURNS VOID
AS $$ BEGIN UPDATE "People" SET "FullName" = 'Updated' WHERE "Age" > age;  END $$
LANGUAGE plpgsql;
