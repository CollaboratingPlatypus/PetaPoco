
DROP TABLE IF EXISTS petapoco;

CREATE TABLE petapoco (
	id				bigserial NOT NULL,
	title			varchar(127) NOT NULL,
	draft			boolean NOT NULL,
	date_created	timestamp NOT NULL,
	date_edited		timestamp NULL,
	content			text NOT NULL,
	state			int NOT NULL,
	"col w space"   int,
	nullreal		real NULL,

	PRIMARY KEY (id)
);

DROP TABLE IF EXISTS petapoco2;

CREATE TABLE petapoco2 (
	email		varchar(127) NOT NULL,
	name		varchar(127) NOT NULL,
	PRIMARY KEY (email)
);
