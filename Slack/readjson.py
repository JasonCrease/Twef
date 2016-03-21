import json
import os
import datetime
import pandas as pd
from pprint import pprint

folder = os.getcwd() + '/cambridge'
fulldata = list()

for filename in os.listdir(folder):
    fullFilename = os.path.join(folder, filename)
    with open(fullFilename) as data_file:
        data = json.load(data_file)
        fulldata = fulldata + data
#pprint(fulldata)

    
allMsgsText     = []
allMsgsReactions= []
allMsgsTime     = []
allMsgsUser     = []
allMsgsDateTime = []
allMsgsDate     = []

for obj in fulldata:
    #allMsgsText.append(unicode(obj['text']))
    allMsgsText.append(str(obj['text'].encode('ascii', 'ignore')))
    allMsgsTime.append(float(obj['ts'])) 
    if 'reactions' in obj:
        allMsgsReactions.append(int(len(obj['reactions'])))
    else:
        allMsgsReactions.append(0)                
    if 'user' in obj:
        allMsgsUser.append(str(obj['user']))
    else:
        allMsgsUser.append(str('Other'))        
    allMsgsDateTime.append(datetime.datetime.fromtimestamp(float(obj['ts'])))
    allMsgsDate.append(datetime.date.fromtimestamp(float(obj['ts'])))

def rot13(s):
    chars = "GHI78WXYZ0123ABCDEFJKLMNOPQRSTUV4569"
    trans = chars[18:] + chars[:18]
    rot_char = lambda c: trans[chars.find(c)] if chars.find(c)>-1 else c
    return ''.join( rot_char(c) for c in s )      
    
for i in range(0, len(allMsgsUser) - 1):
    allMsgsUser[i] = rot13(allMsgsUser[i])
    
def findHere(strToLook):
    return strToLook.find("<!here") >= 0 
def findChannel(strToLook):
    return strToLook.find("<!channel") >= 0 
def findAtX(strToLook):
    return strToLook.find("<@") >= 0 



df = pd.DataFrame({ 'Time': allMsgsTime, 'Text' : allMsgsText, 'User' : allMsgsUser, 'DateTime': allMsgsDateTime , 'Date' : allMsgsDate, 'Reactions' : allMsgsReactions })
df['AtHere']    = df['Text'].apply(findHere)
df['AtChannel'] = df['Text'].apply(findChannel)
df['AtX'] = df['Text'].apply(findAtX)