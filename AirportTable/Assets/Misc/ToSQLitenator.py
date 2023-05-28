from zipfile import ZipFile
import sqlite3
import json
import os

name = "storage.db"
if os.path.exists(name): os.remove(name)
con = sqlite3.connect(name)
cur = con.cursor()

res = cur.execute("""CREATE TABLE content (
  num   INTEGER   PRIMARY KEY AUTOINCREMENT   UNIQUE   NOT NULL,
  day   INTEGER   NOT NULL,
  data   STRING   NOT NULL
);""")
res = cur.execute("""CREATE TABLE images (
  id   STRING   PRIMARY KEY   UNIQUE   NOT NULL,
  data   STRING   NOT NULL
);""")

def escape(s): return '"' + s.replace('"', '""') + '"'

names = (
  "yeah_timetable_вчера.zip",
  "yeah_timetable_сегодня.zip",
  "yeah_timetable_завтра.zip",
  "yeah_timetable_day4.zip",
  "yeah_timetable_day5.zip",
  "yeah_timetable_day6.zip",
)

storage, renamer = {}, {}
for day in range(len(names)):
  with ZipFile(names[day], "r") as zip:
    data = json.loads(zip.read("yeah.json"))
    images = json.loads(zip.read("images.json"))

  for name, img in images.items():
    try: renamer[name] = storage[img]
    except KeyError:
      storage[img] = name
      renamer[name] = name
  for line in data: # 1367 Кб превратилось в 636 кб за счёт убирания повторов картинок между днями ;'-}
    line[0] = renamer[line[0]]
    line[7] = renamer[line[7]]
  
  yeah = []
  for line in data:
    yeah.append("(%s, %s)" % (day, escape(json.dumps(line, ensure_ascii=False))))
  s = "INSERT INTO content (day, data) VALUES %s;" % ", ".join(yeah)
  print("num of newy rows:", cur.execute(s).rowcount)

yeah = ["(%s, %s)" % (escape(name), escape(img)) for img, name in storage.items()]
s = "INSERT INTO images (id, data) VALUES %s;" % ", ".join(yeah)
print("num of images:", cur.execute(s).rowcount)

con.commit() # пришлось бошку поломать, а на деле этой строчки не хватало ;'-} типо сохранения
con.close()
