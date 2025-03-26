-- #) Check details
/*
-- a) Datatypes / Nullability
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Table_Player';

-- b) Check all contraints
SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
WHERE TABLE_NAME = 'Table_Player';
*/

/* -------------------- */

-- 1) Players Table
/* 
SELECT TOP (1000) [PlayerId]
      ,[Username]
      ,[Email]
      ,[Password]
      ,[Balance]
  FROM [MinesGame].[dbo].[Table_Player]
*/


