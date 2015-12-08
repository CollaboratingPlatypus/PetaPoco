IF OBJECT_ID('dbo.petapoco','U') IS NOT NULL
	DROP TABLE dbo.petapoco;

CREATE TABLE petapoco (

	id				bigint IDENTITY(1,1) NOT NULL,
	title			varchar(127) NOT NULL,
	draft			bit NOT NULL,
	date_created	datetime NOT NULL,
	date_edited		datetime NULL,
	content			VARCHAR(MAX) NOT NULL,
	state			int NOT NULL,
	[col w space]	int,
	nullreal		real NULL,
	
	PRIMARY KEY (id)
);

IF OBJECT_ID('dbo.petapoco2','U') IS NOT NULL
	DROP TABLE dbo.petapoco2;

CREATE TABLE petapoco2 (
	email		varchar(127) NOT NULL,
	name		varchar(127) NOT NULL,
	PRIMARY KEY (email)
);
