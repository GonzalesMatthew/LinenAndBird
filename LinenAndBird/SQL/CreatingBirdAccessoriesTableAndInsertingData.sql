create table dbo.BirdAccessories (
	Id uniqueidentifier not null primary key default(newsequentialid()),
	[Name] varchar(50) not null,
	BirdId uniqueidentifier not null,
	CONSTRAINT FK_BirdAccessories_Birds FOREIGN KEY (BirdId)
		REFERENCES Birds (Id)
)


-- refresh table
-- right click table and Edit Top 200 
-- -- lets you manually enter data easily