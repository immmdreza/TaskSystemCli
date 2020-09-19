from persiantools.jdatetime import JalaliDateTime, JalaliDate
import pytz


today = JalaliDateTime.now(pytz.timezone("Asia/Tehran"));

print(today.strftime("%c"))


import re

p = re.compile(r'^(/)?date$|^(/)?time$|^تاریخ$', re.IGNORECASE)

m = re.search(p,'/تاریخ')

print(m)
