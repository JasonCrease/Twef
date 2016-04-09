import readjson
import nltk
import numpy as np
import pandas as pd
import re
from bs4 import BeautifulSoup

df = readjson.df[['Text','Reactions']]

from nltk.corpus import stopwords # Import the stop word list
stops = set(stopwords.words("english"))     

def review_to_words( raw_review ):
    review_text = BeautifulSoup(raw_review).get_text() 
    letters_only = re.sub("[^a-zA-Z]", " ", review_text) 
    words = letters_only.lower().split()    
    meaningful_words = [w for w in words if not w in stops]   
    return( " ".join( meaningful_words ))   

cleanTxts = []

for i in xrange( 0, len(df)  ):
    cleanTxts.append( review_to_words( df["Text"][i] ) )

print "Creating the bag of words...\n"
from sklearn.feature_extraction.text import CountVectorizer

# Initialize the "CountVectorizer" object, which is scikit-learn's
# bag of words tool.  
vectorizer = CountVectorizer(analyzer = "word",   \
                             tokenizer = None,    \
                             preprocessor = None, \
                             stop_words = None,   \
                             max_features = 5000) 

# fit_transform() does two functions: First, it fits the model
# and learns the vocabulary; second, it transforms our training data
# into feature vectors. The input to fit_transform should be a list of 
# strings.
data_features = vectorizer.fit_transform(cleanTxts)

# Numpy arrays are easy to work with, so convert the result to an 
# array
data_features = data_features.toarray()

print "Training the random forest..."
from sklearn.ensemble import RandomForestClassifier

# Initialize a Random Forest classifier with 100 trees
forest = RandomForestClassifier(n_estimators = 100) 

# Fit the forest to the training set, using the bag of words as 
# features and the sentiment labels as the response variable
#
# This may take a few minutes to run
forest = forest.fit(data_features[:5000], df[:5000].Reactions )

preds = pd.DataFrame( { 'pred' : forest.predict(data_features[5000:]), 
                        'actual' :  df[5000:].Reactions } )

forest.score(data_features[5000:], df[5000:].Reactions)