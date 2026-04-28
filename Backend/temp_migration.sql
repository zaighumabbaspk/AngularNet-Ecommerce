-- Simple SQL to make UserId nullable since guest columns already exist
ALTER TABLE Orders ALTER COLUMN UserId NVARCHAR(450) NULL;