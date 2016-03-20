import json
import os
import datetime
import numpy as np
import pandas as pd
from pprint import pprint
import matplotlib as plt

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

def findHere(strToLook):
    return strToLook.find("<!here") >= 0 
def findChannel(strToLook):
    return strToLook.find("<!channel") >= 0 
def findAtX(strToLook):
    return strToLook.find("<@") >= 0 

#pprint(allMsgs)
df = pd.DataFrame({ 'Time': allMsgsTime, 'Text' : allMsgsText, 'User' : allMsgsUser, 'DateTime': allMsgsDateTime , 'Reactions' : allMsgsReactions })
df['AtHere']    = df['Text'].apply(findHere)
df['AtChannel'] = df['Text'].apply(findChannel)
df['AtX'] = df['Text'].apply(findAtX)

# Changing over time
fig = plt.pyplot.figure()
ax = fig.add_subplot(111)
ax.hist(df['Time'], bins = 5, range = (df['Time'].min(),df['Time'].max()))
plt.pyplot.title('Time distribution')
plt.pyplot.xlabel('Time')
plt.pyplot.ylabel('Count of Time')
plt.pyplot.show()

# People's reactions
tempReactions = df.groupby('User').Reactions.sum().sort_values(ascending=False).head(50)
fig = plt.pyplot.figure(figsize = (20,4))
ax1 = fig.add_subplot(121)
ax1.set_xlabel('User')
ax1.set_ylabel('Count of Reactions')
ax1.set_title("Reactions by User")
tempReactions.plot(kind='bar')

# Total messages
tempMsgCount = df.groupby('User').count().Text.sort_values(ascending=False).head(50)
fig = plt.pyplot.figure(figsize = (20,4))
ax1 = fig.add_subplot(121)
ax1.set_xlabel('User')
ax1.set_ylabel('Count of Messages')
ax1.set_title("Messages by User")
tempMsgCount.plot(kind='bar')

df.corr