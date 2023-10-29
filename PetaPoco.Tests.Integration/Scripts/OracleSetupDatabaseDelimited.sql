-- Drop PETAPOCO_ORDINARY user COMPLETELY (if it exists)
DECLARE
    found number := 0;
BEGIN
    SELECT COUNT(*) INTO found
    FROM all_users
    WHERE username = 'PETAPOCO_DELIMITED';

    IF found <> 0 THEN
        BEGIN
            EXECUTE IMMEDIATE 'DROP USER petapoco_delimited CASCADE';
        END;
    END IF;
END;
/

-- Create fresh user
CREATE USER petapoco_delimited IDENTIFIED BY petapoco;

-- Ensure that the data tablespace is the default for the user. This tablespace will be used when creating tables for example
ALTER USER petapoco_delimited DEFAULT TABLESPACE data_ts;
-- Give user quota e.g. to perform inserts
ALTER USER petapoco_delimited QUOTA UNLIMITED ON data_ts;

-- Grant the application developer role
GRANT app_dev_role TO petapoco_delimited;
/
