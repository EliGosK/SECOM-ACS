Syntax:	<yourapp>.exe [/console | /c] [/install | /i | /install /start | /i /start] [/username=username] [/password=password] [/accounttype=networkservice] [/startup=manual]


/console or /c		Will run the application in Console mode.  If console is not provided the application will default to run as a Windows Service.

/install or /i		Will install the service.  This is ignored if /console or /c is provided.
/uninstall or /u	Will uninstall the service
/start			Will attempt to start the service after it is installed regardless of the startup.  This is ignored if /install is NOT provided.

/accounttype		Sets the account type that the service will run as.  Options are: LocalSystem (default), LocalService, NetworkService, and User.  (Not case sensitive)
/startup		Sets the startup mode for the sevice.  Options are Manual (default), Automatic, Disabled.  Does not start the service after installation.(Not case sensitive)
/username		Sets the user name that the service will run under.  Optional.  Only used if /install is provided and /accounttype is user.
/password		The password for the user the service will run as.  Optional.  Only used if /install is provided and /accounttype is user.

Note:	All arguments can be used with either / or – switch or their shortcuts.  Argument flags are not case sensitive.

Valid Examples:	
SECOMACSHostedProcess.exe /c
SECOMACSHostedProcess.exe /console
SECOMACSHostedProcess.exe /i /start /username=myuser /password=password /accounttype=localsystem /startup=automatic
SECOMACSHostedProcess.exe /install -start /username=myuser -password=password /accounttype=localsystem /startup=automatic
SECOMACSHostedProcess.exe /iNsTaLl -stArt /userName=myuser -paSSword=password /accOunTTyPe=localsystem /stArtUp=automatic

SECOMACSHostedProcess.exe /install -start /accounttype=localsystem /startup=automatic