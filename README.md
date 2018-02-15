# S7PLCVar

library : 
https://github.com/netdata/libnodave

Default credentials for administration area:
    Login: admin        Password: admin



compile :
mcs S7PLCVar.cs -r:libnodave.net.dll -out:./S7PLCVar

Usage:S7PLCVar.exe [ipaddress] [rack] [slot] [address] [value]

Examples: 

Read bit value from I7.3
S7PLCVar 172.24.40.191 0 3 I7.3


Read bit value from DB10.DBX14.5 
S7PLCVar 172.24.40.191 0 3 DB10.DBX14.5


Write DWord value 1234 to DB10.DBD14 
S7PLCVar 172.24.40.191 0 3 DB10.DBD14 1234


Write Output bit True to Q3.5 
S7PLCVar 172.24.40.191 0 3 Q3.5 True
