
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

	PRIMARY KEY (id)
);
