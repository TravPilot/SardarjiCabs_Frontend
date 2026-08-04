/***********************************************************************************************************
*Procedure Name     : GetDriver_RecordFailedLogin   
*Authore            : Baijayanta(Rohit)
*Version            : 1.0              
*Organisation       : Stoutweb.com     
*Date               : 30-10-2020       
*Purpose            : This Procedure is used to Get Driver Failed Login Record
------------------------------------------------------------------------------------------------------------
Modified By                | Modified On              | Purpose
                           |                          |        
***********************************************************************************************************/
CREATE OR ALTER PROCEDURE GetDriver_RecordFailedLogin
    @DriverId INT,
    @FailedAttempts INT,
    @LockedUntil DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Drivers
    SET FailedLoginAttempts = @FailedAttempts,
        LockedUntil = @LockedUntil,
        UpdatedOn = dbo.GetLocalIST(),
		UpdatedBy = @DriverId
    WHERE Id = @DriverId;
END