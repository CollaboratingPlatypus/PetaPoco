ALTER SESSION SET container=FREEPDB1;

-- Drop DATA_TS tablespace COMPLETELY (if it exists) and create a fresh one
DECLARE
    found number := 0;
BEGIN
    SELECT COUNT(*) INTO found
    FROM dba_data_files
    WHERE tablespace_name = 'DATA_TS';

    IF found <> 0 THEN
        BEGIN
			--Drop tablespace COMPLETELY;
            EXECUTE IMMEDIATE 'DROP TABLESPACE data_ts ' ||
                'INCLUDING CONTENTS AND DATAFILES ' ||
                'CASCADE CONSTRAINTS';
	
			--Wait for the DROP operation to complete
			LOOP
				SELECT COUNT(*) INTO found
				FROM dba_tablespaces
				WHERE tablespace_name = 'DATA_TS';
				
				EXIT WHEN found = 0;
				
				--Sleep 1s before checking again
				DBMS_LOCK.SLEEP(1);
			END LOOP;
        END;
    END IF;
	
	--Create a fresh tablespace
	EXECUTE IMMEDIATE 'CREATE TABLESPACE DATA_TS ' ||
		'DATAFILE ''/opt/oracle/oradata/FREE/FREEPDB1/data01.dbf''' ||
		'SIZE 200M AUTOEXTEND ON NEXT 10M MAXSIZE UNLIMITED';
END;
/

-- Convenience procedure to drop certain database objects without having to deal with exceptions
CREATE OR REPLACE PROCEDURE DROP_IF_EXISTS (object_type IN VARCHAR2, object_name IN VARCHAR2) IS
   command VARCHAR2(500);
BEGIN
    -- Avoid sql injection by limiting object_type values and escaping the object_name
    IF UPPER(object_type) NOT IN ('ROLE', 'TABLE', 'VIEW', 'SEQUENCE', 'INDEX', 'TRIGGER', 'PROCEDURE', 'FUNCTION', 'TYPE') THEN
        RAISE_APPLICATION_ERROR(-20001, 'Invalid object type: ' || object_type);
    END IF;

    command := 'DROP ' || object_type || ' ' || SYS.DBMS_ASSERT.QUALIFIED_SQL_NAME(object_name);
   
    EXECUTE IMMEDIATE command;
EXCEPTION
    WHEN OTHERS THEN
        --            (  role, table, sequence, index, trigger, procedure/function/type)
        IF SQLCODE IN ( -1919,  -942,    -2289, -1418,   -4080,                   -4043) THEN
            NULL;
        ELSE
            RAISE;
        END IF;
END;
/

CALL DROP_IF_EXISTS('ROLE', 'app_dev_role');
/

-- Create an application developer role
CREATE ROLE app_dev_role;

-- Grant privileges to the application developer role
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE, CREATE PROCEDURE, CREATE TRIGGER, CREATE VIEW TO app_dev_role;
GRANT EXECUTE ON dbms_lock TO app_dev_role;
GRANT EXECUTE ON DROP_IF_EXISTS TO app_dev_role;