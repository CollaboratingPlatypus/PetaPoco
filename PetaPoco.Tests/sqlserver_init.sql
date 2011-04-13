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
	
	PRIMARY KEY (id)
);

