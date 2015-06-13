CREATE PROCEDURE [dbo].[SEL_USERS_BY_ID]
	@id int = 0
AS
	SELECT * from [User] where id = @id

RETURN 0
