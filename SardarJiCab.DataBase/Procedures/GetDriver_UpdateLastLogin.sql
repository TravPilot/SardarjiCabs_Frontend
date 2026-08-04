/***********************************************************************************************************  
*Procedure Name     : GetDriver_UpdateLastLogin     
*Authore            : Baijayanta(Rohit)  
*Version            : 1.0                
*Organisation       : Stoutweb.com       
*Date               : 30-10-2020         
*Purpose            : This Procedure is used to Get Driver Last Login Update                    
------------------------------------------------------------------------------------------------------------  
Modified By                | Modified On              | Purpose  
                           |                          |          
***********************************************************************************************************/  
CREATE   PROCEDURE GetDriver_UpdateLastLogin  
    @DriverId INT,  
    @LastLoginIp NVARCHAR(45) = NULL  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    UPDATE Drivers  
    SET LastLoginAt = dbo.GetLocalIST(),  
        LastLoginIp = @LastLoginIp,  
        FailedLoginAttempts = 0,  
        LockedUntil = NULL,  
        UpdatedOn = dbo.GetLocalIST(),  
  UpdatedBy = @DriverId  
    WHERE Id = @DriverId;  
END