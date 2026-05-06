USE TechVaultDb;
GO

-- Update admin password hash (just in case)
UPDATE Users 
SET PasswordHash = '$2a$11$BYQ95YTAtUd.YYmpBOZ2QO3brzbvBCUSTXMHZ0aK8zpT7IDs/eFyG' 
WHERE Username = 'admin';

-- Rename tech1 to tech and update password hash
UPDATE Users 
SET Username = 'tech', Email = 'tech@techvault.com', PasswordHash = '$2a$11$BYQ95YTAtUd.YYmpBOZ2QO3brzbvBCUSTXMHZ0aK8zpT7IDs/eFyG' 
WHERE Username = 'tech1' OR Username = 'tech';

-- Rename john / haze to hazem, update email, and update password hash
UPDATE Users 
SET Username = 'hazem', Email = 'hazem@techvault.com', PasswordHash = '$2a$11$BYQ95YTAtUd.YYmpBOZ2QO3brzbvBCUSTXMHZ0aK8zpT7IDs/eFyG' 
WHERE Username = 'john' OR Username = 'haze' OR Username = 'hazem';

-- Update Customer Profile name for hazem
UPDATE CustomerProfiles 
SET FullName = 'Hazem Fawzy' 
WHERE UserId IN (SELECT Id FROM Users WHERE Username = 'hazem');
GO
