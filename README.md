# lab01
C# lab01, by LuYuan, 2019-3-14, lasts 5 hours, 164 lines

## Introduction
1. This application is used to create qrcode for numerous cellphone numbers (11 digits)
2. This app supported command line of Windows System to execute only 
3. This app can only read txt file with encoding UTF8 

## Usage
1. input "-F"/"-f"(captial insensitive) and filename(.txt)
2. input "-SQL" to select database
3. input "-help"(captial sensitive) to get help from this readme.md
4. each line will print a qrcode in the console
5. output qrcode is .png format and named by the number of rows and the first four number of the data

## Change Log
#### first version(a941dcb), released in 14:54, March 15th, 2019
1. the default txt file is test.txt
2. the default data is 17822007566, 17822007567, 17822007568
3. screencut (by vs complier) ![screen ](screencut/0001782.png) ![screen ](screencut/0011782.png) ![screen ](screencut/0021782.png) 

#### second version(8146729), released in 9:20, March 27th, 2019
1. add sql data reader, which can read data from sql server and print qrcode
2. if you get connected with your sql server, console will print "connection success" in green foreground
3. if you choose the fixed table, console will print "operation success" in green foreground
4. the default column of table is "number", please change the name of your table's column to "number" and set it the only column in your table
5. ![screen ](screencut/sql.png)

## xmind
![screen ](screencut/xmind.png)
