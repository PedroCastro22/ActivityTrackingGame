from datetime import datetime
import sys

def get_log(hours, message):
    date = "(" + datetime.today().strftime('%d-%m-%Y') + ") "
    hours_msg = "[" + str(hours) + " hours] "
    return date + hours_msg + message

def write_in_file(hours, message):
    with open("timelog.md", "a") as timelog_file:
        timelog_file.write("\n" + get_log(hours=hours, message=message))

write_in_file(sys.argv[1], str(sys.argv[2]))