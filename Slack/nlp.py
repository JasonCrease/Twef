import readjson
import nltk
import nltk.classify.util
from nltk.classify import NaiveBayesClassifier

def word_feats(words):
    return dict([(word, True) for word in words])
 
df = readjson.df
 
noReactionTexts   = df[df.Reactions <= 0].Text
someReactionTexts = df[df.Reactions >  0].Text
 
negfeats = [(word_feats(nltk.word_tokenize(f)), 'n') for f in noReactionTexts]
posfeats = [(word_feats(nltk.word_tokenize(f)), 'y') for f in someReactionTexts]
 
negcutoff = int(len(negfeats)*0.8)
poscutoff = int(len(posfeats)*0.8)

trainfeats = negfeats[:negcutoff] + posfeats[:poscutoff]
testfeats = negfeats[negcutoff:] + posfeats[poscutoff:]
print 'train on %d instances, test on %d instances' % (len(trainfeats), len(testfeats))
 
classifier = NaiveBayesClassifier.train(trainfeats)
print 'accuracy:', nltk.classify.util.accuracy(classifier, testfeats)
classifier.show_most_informative_features(n=30)