Date Time
#################

datetime(year, mon, day)
*****************

Returns datetime from year, month, and day.

datetime(year, mon, day, hour, min, sec)
*****************

Returns datetime from year, month, day, hour, minute, and second.

dayOfMonth(*t)
*****************

Returns day component of datetime, expressed as 1..31.

dayOfWeek(*t)
*****************

Returns day of week of datetime, expressed as 0 (Sunday)..6 (Saturday).

dayOfYear(*t)
*****************

Returns day of year of datetime, expressed as 1..366.

fromDays(*x)
*****************

Converts from days to epoch time. ::

    fromDays(123.45) //--> 10666080

fromHours(*x)
*****************

Converts from hours to epoch time. ::

    fromHours(123.45) //--> 444420

fromMinutes(*x)
*****************

Converts from minutes to epoch time. ::

    fromMinutes(123.45) //--> 7407

fromSeconds(*x)
*****************

Converts from seconds to epoch time. ::

    fromSeconds(123.45) //--> 123.45

hourOf(*t)
*****************

Returns hour component of datetime, expressed as 0..23.

minuteOf(*t)
*****************

Returns minute component of datetime, expressed as 0..59.

monthOf(*t)
*****************

Returns month component of datetime, expressed as 1..12.

now()
*****************

Returns current datetime.

secondOf(*t)
*****************

Returns second component of datetime, expressed as 0..60.

today()
*****************

Returns datetime of today's 00:00:00.

toDays(*x)
*****************

Converts from epoch time to days. ::

    toDays(#+123.12:34:56.789#) //--> 123.524268391

toHours(*x)
*****************

Converts from epoch time to hours. ::

    toHours(#+123.12:34:56.789#) //--> 2964.582441389

toMinutes(*x)
*****************

Converts from epoch time to minutes. ::

    toMinutes(#+123.12:34:56.789#) //--> 177874.946483333

toSeconds(*x)
*****************

Converts from epoch time to seconds. ::

    toSeconds(#+123.12:34:56.789#) //--> 10672496.789

yearOf(*t)
*****************

Returns year component of datetime.


