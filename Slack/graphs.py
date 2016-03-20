import readjson
import matplotlib as plt

df = readjson.df

# Correlation
df.corr()

# Most-common messages
df.groupby('Text').Text.count().sort_values(ascending=False).head(30)

# Changing over time
fig = plt.pyplot.figure()
ax = fig.add_subplot(111)
ax.hist(df['Time'], bins = 8, range = (df['Time'].min(),df['Time'].max()))
plt.pyplot.title('Time distribution')
plt.pyplot.xlabel('Time')
plt.pyplot.ylabel('Count of Time')
plt.pyplot.show()

# People's reactions
tempReactions = df.groupby('User').Reactions.sum().sort_values(ascending=False).head(50)
fig = plt.pyplot.figure(figsize = (10,4))
ax1 = fig.add_subplot(111)
ax1.set_xlabel('User')
ax1.set_ylabel('Count of Reactions')
ax1.set_title("Reactions by User")
tempReactions.plot(kind='bar')

# Total messages
tempMsgCount = df.groupby('User').count().Text.sort_values(ascending=False).head(50)
fig = plt.pyplot.figure(figsize = (10,4))
ax1 = fig.add_subplot(111)
ax1.set_xlabel('User')
ax1.set_ylabel('Count of Messages')
ax1.set_title("Messages by User")
tempMsgCount.plot(kind='bar')

# Day of week
tempMsgCount = df.DateTime.apply(lambda x: x.dayofweek)
fig = plt.pyplot.figure()
ax = fig.add_subplot(111)
ax.hist(tempMsgCount, bins = 7)
plt.pyplot.title('Day distribution')
plt.pyplot.xlabel('Day')
plt.pyplot.ylabel('Count of Messages')
plt.pyplot.show()

# Month
tempMsgCount = df.DateTime.apply(lambda x: x.month)
fig = plt.pyplot.figure()
ax = fig.add_subplot(111)
ax.hist(tempMsgCount, bins = 12)
plt.pyplot.title('Month distribution')
plt.pyplot.xlabel('Month')
plt.pyplot.ylabel('Count of Messages')
plt.pyplot.show()

# Day of week
tempMsgCount = df.groupby('Date').count().Text.sort_values(ascending=False).head(20)
fig = plt.pyplot.figure(figsize = (10,4))
ax1 = fig.add_subplot(111)
ax1.set_xlabel('Date')
ax1.set_ylabel('Count of Messages')
ax1.set_title("Messages by Date")
tempMsgCount.plot(kind='bar')

# Messages per day
tempMsgCount = df.groupby('Date').count().Text.sort_values(ascending=False)
fig = plt.pyplot.figure(figsize = (10,4))
ax1 = fig.add_subplot(111)
ax1.set_xlabel('Date')
ax1.set_ylabel('Count of Messages')
ax1.set_title("Messages by Date")
tempMsgCount.plot(kind='line')