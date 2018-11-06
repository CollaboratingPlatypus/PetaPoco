-- Drop stored procedures first
SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$procedures WHERE rdb$procedure_name = 'SELECTPEOPLE'))	then execute statement 'DROP PROCEDURE "SELECTPEOPLE";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$procedures WHERE rdb$procedure_name = 'SELECTPEOPLEWITHPARAM')) then execute statement 'DROP PROCEDURE "SELECTPEOPLEWITHPARAM";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$procedures WHERE rdb$procedure_name = 'COUNTPEOPLE'))	then execute statement 'DROP PROCEDURE "COUNTPEOPLE";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$procedures WHERE rdb$procedure_name = 'COUNTPEOPLEWITHPARAM'))	then execute statement 'DROP PROCEDURE "COUNTPEOPLEWITHPARAM";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$procedures WHERE rdb$procedure_name = 'UPDATEPEOPLE'))	then execute statement 'DROP PROCEDURE "UPDATEPEOPLE";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$procedures WHERE rdb$procedure_name = 'UPDATEPEOPLEWITHPARAM'))	then execute statement 'DROP PROCEDURE "UPDATEPEOPLEWITHPARAM";';
END!!
SET TERM ; !!






SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'OrderLines'))	then execute statement 'DROP TABLE "OrderLines";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'Orders'))	then execute statement 'DROP TABLE "Orders";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'People'))	then execute statement 'DROP TABLE "People";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'SpecificOrderLines'))	then execute statement 'DROP TABLE "SpecificOrderLines";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'SpecificOrders'))	then execute statement 'DROP TABLE "SpecificOrders";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'SpecificPeople'))	then execute statement 'DROP TABLE "SpecificPeople";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'TransactionLogs')) then execute statement 'DROP TABLE "TransactionLogs";';
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'Note')) then execute statement 'DROP TABLE "Note";';
END!!
SET TERM ; !!

CREATE TABLE "People" (
	"Id" CHAR(36) NOT NULL PRIMARY KEY,
	"FullName" VARCHAR(255),
	"Age" BIGINT NOT NULL,
	"Height" INT NOT NULL,
	"Dob" TIMESTAMP
);

CREATE TABLE "Orders" (
	"Id" INT PRIMARY KEY NOT NULL,
	"PersonId" CHAR(36) NOT NULL,
	"PoNumber" VARCHAR(15) NOT NULL,
	"OrderStatus" INT NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL,
	"CreatedBy" VARCHAR(255) NOT NULL,
	CONSTRAINT "FK_Orders_People" FOREIGN KEY ("PersonId") REFERENCES "People" ("Id")
);

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$generators WHERE rdb$generator_name = 'GEN_ORDERS_ID')) then execute statement 'DROP SEQUENCE GEN_ORDERS_ID;';
END!!
SET TERM ; !!

CREATE SEQUENCE GEN_ORDERS_ID;
ALTER SEQUENCE GEN_ORDERS_ID RESTART WITH 0;

SET TERM !! ;
CREATE TRIGGER trigger_Orders_id FOR "Orders"
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
	IF (NEW."Id" is NULL) THEN NEW."Id" = GEN_ID(GEN_ORDERS_ID, 1);
END!!
SET TERM ; !!

CREATE TABLE "OrderLines" (
	"Id" INT PRIMARY KEY NOT NULL,
	"OrderId" INT NOT NULL,
	"Qty" SMALLINT NOT NULL,
	"Status" SMALLINT NOT NULL,
	"SellPrice" NUMERIC(10, 4) NOT NULL,
	CONSTRAINT "FK_OrderLines_Orders" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id")
);

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$generators WHERE rdb$generator_name = 'GEN_ORDERLINES_ID')) then execute statement 'DROP SEQUENCE GEN_ORDERLINES_ID;';
END!!
SET TERM ; !!

CREATE SEQUENCE GEN_ORDERLINES_ID;
ALTER SEQUENCE GEN_ORDERLINES_ID RESTART WITH 0;

SET TERM !! ;
CREATE TRIGGER trigger_OrderLines_id FOR "OrderLines"
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
	IF (NEW."Id" is NULL) THEN NEW."Id" = GEN_ID(GEN_ORDERLINES_ID, 1);
END!!
SET TERM ; !!

CREATE TABLE "SpecificPeople" (
	"Id" CHAR(36) NOT NULL PRIMARY KEY,
	"FullName" VARCHAR(255),
	"Age" BIGINT NOT NULL,
	"Height" INT NOT NULL,
	"Dob" TIMESTAMP
);

CREATE TABLE "SpecificOrders" (
	"Id" INT PRIMARY KEY NOT NULL,
	"PersonId" CHAR(36) NOT NULL,
	"PoNumber" VARCHAR(15) NOT NULL,
	"OrderStatus" INT NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL,
	"CreatedBy" VARCHAR(255) NOT NULL,
	CONSTRAINT "FK_SOrders_SPeople" FOREIGN KEY ("PersonId") REFERENCES "SpecificPeople" ("Id")
);

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$generators WHERE rdb$generator_name = 'GEN_SPECIFICPEOPLE_ID')) then execute statement 'DROP SEQUENCE GEN_SPECIFICPEOPLE_ID;';
END!!
SET TERM ; !!

CREATE SEQUENCE GEN_SPECIFICPEOPLE_ID;
ALTER SEQUENCE GEN_SPECIFICPEOPLE_ID RESTART WITH 0;

SET TERM !! ;
CREATE TRIGGER trigger_gen_SPeople_id FOR "SpecificOrders"
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
	IF (NEW."Id" is NULL) THEN NEW."Id" = GEN_ID(GEN_SPECIFICPEOPLE_ID, 1);
END!!
SET TERM ; !!

CREATE TABLE "SpecificOrderLines" (
	"Id" INT PRIMARY KEY NOT NULL,
	"OrderId" INT NOT NULL,
	"Qty" SMALLINT NOT NULL,
	"Status" SMALLINT NOT NULL,
	"SellPrice" NUMERIC(10, 4) NOT NULL,
	CONSTRAINT "FK_SOLines_SOrders" FOREIGN KEY ("OrderId") REFERENCES "SpecificOrders"("Id")
);

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$generators WHERE rdb$generator_name = 'GEN_SORDERLINES_ID')) then execute statement 'DROP SEQUENCE GEN_SORDERLINES_ID;';
END!!
SET TERM ; !!

CREATE SEQUENCE GEN_SORDERLINES_ID;
ALTER SEQUENCE GEN_SORDERLINES_ID RESTART WITH 0;

SET TERM !! ;
CREATE TRIGGER trigger_gen_SPLines_id FOR "SpecificOrderLines"
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
	IF (NEW."Id" is NULL) THEN NEW."Id" = GEN_ID(GEN_SORDERLINES_ID, 1);
END!!
SET TERM ; !!

CREATE TABLE "TransactionLogs" (
	"Description" VARCHAR(5000) NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL
);

CREATE TABLE "Note" (
	"Id" INT PRIMARY KEY NOT NULL,
	"Text" VARCHAR(5000) NOT NULL,
	"CreatedOn" TIMESTAMP NOT NULL
);

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$generators WHERE rdb$generator_name = 'GEN_NOTE_ID')) then execute statement 'DROP SEQUENCE GEN_NOTE_ID;';
END!!
SET TERM ; !!

CREATE GENERATOR GEN_NOTE_ID;
SET GENERATOR GEN_NOTE_ID TO 0;

SET TERM !! ;
CREATE TRIGGER trigger_gen_Note_id FOR "Note"
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
	IF (NEW."Id" is NULL) THEN NEW."Id" = GEN_ID(GEN_NOTE_ID, 1);
END!!
SET TERM ; !!

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'BugInvestigation_10R9LZYK'))	then execute statement 'DROP TABLE "BugInvestigation_10R9LZYK";';
END!!
SET TERM ; !!

CREATE TABLE "BugInvestigation_10R9LZYK" (
	"Id" INT PRIMARY KEY NOT NULL,
	"TestColumn1" BLOB
);

SET TERM !! ;
EXECUTE BLOCK AS BEGIN
	if (exists(SELECT 1 FROM rdb$generators WHERE rdb$generator_name = 'GEN_BI_10R9LZYK_ID')) then execute statement 'DROP SEQUENCE GEN_BI_10R9LZYK_ID;';
END!!
SET TERM ; !!

CREATE SEQUENCE GEN_BI_10R9LZYK_ID;
ALTER SEQUENCE GEN_BI_10R9LZYK_ID RESTART WITH 0;

SET TERM !! ;
CREATE TRIGGER trigger_gen_BI_10R9LZYK_id FOR "BugInvestigation_10R9LZYK"
ACTIVE BEFORE INSERT POSITION 0
AS
BEGIN
	IF (NEW."Id" is NULL) THEN NEW."Id" = GEN_ID(GEN_BI_10R9LZYK_ID, 1);
END!!
SET TERM ; !!


-- Stored procedures
SET TERM !! ;
CREATE PROCEDURE SelectPeople
RETURNS (id varchar(100), fullname varchar(100), age integer)
AS
BEGIN
	FOR SELECT "Id", "FullName", "Age"
	FROM "People"
	INTO :id, :fullname, :age DO
	BEGIN
		SUSPEND;
	END
END!!
SET TERM ; !!

SET TERM !! ;
CREATE PROCEDURE SelectPeopleWithParam (age integer)
RETURNS (id varchar(100), fullname varchar(100))
AS
BEGIN
	FOR SELECT "Id", "FullName"
	FROM "People"
	WHERE "Age" > :age
	INTO :id, :fullname DO
	BEGIN
		SUSPEND;
	END
END!!
SET TERM ; !!

SET TERM !! ;
CREATE PROCEDURE CountPeople
RETURNS (numRecs integer)
AS
BEGIN
	SELECT COUNT(*) FROM "People" INTO :numRecs;
	SUSPEND;
END!!
SET TERM ; !!

SET TERM !! ;
CREATE PROCEDURE CountPeopleWithParam (age integer)
RETURNS (numRecs integer)
AS
BEGIN
	SELECT COUNT(*) FROM "People" WHERE "Age" > :age INTO :numRecs;
	SUSPEND;
END!!
SET TERM ; !!

SET TERM !! ;
CREATE PROCEDURE UpdatePeople
AS
BEGIN
	UPDATE "People" SET "FullName" = 'Updated';
END!!
SET TERM ; !!

SET TERM !! ;
CREATE PROCEDURE UpdatePeopleWithParam (age integer)
AS
BEGIN
	UPDATE "People" SET "FullName" = 'Updated' WHERE "Age" > :age;
END!!
SET TERM ; !!
