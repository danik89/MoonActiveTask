Hi MoonActive!
this is my service, that helps you decide if a vehicle is allowed to enter the parking lot.

all you need to do is:
1. run this script to create a table at your sql service:

CREATE TABLE [dbo].[EntranceDecisions] (
    [PlateNumber]      CHAR (10) NOT NULL,
    [TimeStamp]        DATETIME  NOT NULL,
    [ProhibitedReason] INT       NULL,
    [Decision]         INT       NOT NULL,
    PRIMARY KEY CLUSTERED ([PlateNumber] ASC, [TimeStamp] ASC)
);

2. edit the file called 'PermissionServiceRunner.exe.config',
1.1 add your own image path at appSettings/picPath
(the path should have an extension of jpeg/jpg/png)
1.2 add an api key for the API at appSettings/apiKey
1.3 connectionString at appSettings/connectionString
1.4 if you didnt used my script for inserting the table, you should change the query at appSettings/insertQuery

3. please notice - the Log file is written to the same folder as the exe file

4. the exe file uses the image path from the config file (PermissionServiceRunner.exe.config) and checks for permission, all log messages are in the log file.

5. you can also use the service other way - it just needs to recieve a path of an image with the right format and it will return 0/1 as expected.


Thanks
Daniel Levinson



