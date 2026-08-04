/***********************************************************************************************************
*Procedure Name     : GetDriver_ByMobile   
*Authore            : Baijayanta(Rohit)
*Version            : 1.0              
*Organisation       : Stoutweb.com     
*Date               : 30-10-2020       
*Purpose            : This Procedure is used to Create Cars in traviyo backend                  
------------------------------------------------------------------------------------------------------------
Modified By                | Modified On              | Purpose
                           |                          |        
***********************************************************************************************************/
CREATE PROCEDURE GetDriver_ByMobile
    @MobileNumber NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id
		,MobileNumber
		,PasswordHash
		,Email
		,FullName
		,IsActive
		,IsVerified
		,ApprovalStatus
		,FailedLoginAttempts
		,LockedUntil
		,LastLoginAt
		,LastLoginIp
    FROM Drivers
    WHERE MobileNumber = @MobileNumber;
END