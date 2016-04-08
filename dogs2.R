#install.packages("xgboost")
library(caret)
library(lubridate)
#library(gbm)
library(xgboost)


setwd("d:/R/pets/")
dfraw    <- read.csv("./train.csv")
dfrawsub <- read.csv("./test.csv")

table(dfDogTrain$OutcomeType, dfDogTrain$Weekday)

dfDogRaw   =dfraw[dfraw$AnimalType   =="Dog",]
dfCatRaw   =dfraw[dfraw$AnimalType   =="Cat",]
dfDogRawSub=dfrawsub[dfrawsub$AnimalType=="Dog",]
dfCatRawSub=dfrawsub[dfrawsub$AnimalType=="Cat",]

# Make columns match so rbind is possible
dfDogRawSub=dfDogRawSub[-c(1)]
dfDogRawSub$OutcomeType = as.factor("NULL")
dfDogRawSub$OutcomeSubtype = as.factor("NULL")
dfDogRawSub$AnimalID = as.factor("NULL")
allDog = rbind(dfDogRaw, dfDogRawSub)

dfCatRawSub=dfCatRawSub[-c(1)]
dfCatRawSub$OutcomeType = as.factor("NULL")
dfCatRawSub$OutcomeSubtype = as.factor("NULL")
dfCatRawSub$AnimalID = as.factor("NULL")

allCat = rbind(dfCatRaw, dfCatRawSub)

# Assemble popularity stats
popularDogBreeds <- names(summary(allDog$Breed,maxsum=8L))
popularCatBreeds <- names(summary(allCat$Breed,maxsum=6L))
dogNameSummary   <- summary(allDog$Name, maxsum=Inf)
catNameSummary   <- summary(allCat$Name, maxsum=Inf)
dogBreedsSummary <- summary(allDog$Breed,maxsum=Inf)
catBreedsSummary <- summary(allCat$Breed,maxsum=Inf)
dogColorSummary <- summary(allDog$Color,maxsum=Inf)
catColorSummary <- summary(allCat$Color,maxsum=Inf)

cleanGeneral <- function(x){
  # This is irrelevant
  x$OutcomeSubtype <- NULL
  
  #Datetime stuff
  
  x$MinuteOfDay <- (hour(x$DateTime) * 60) + minute(x$DateTime) 
  x$Weekday     <- wday(x$DateTime)
  x$IsWeekend = FALSE
  x[x$Weekday == 1 | x$Weekday == 7,]$IsWeekend = TRUE
  x$Month       <- month(x$DateTime)
  x$DateTime    <- as.numeric(as.POSIXct(x$DateTime))
  
  AgeYears <- as.numeric(gsub(" years?","",x$AgeuponOutcome))
  AgeMonths <- as.numeric(gsub(" months?","",x$AgeuponOutcome))
  AgeWeeks <- as.numeric(gsub(" weeks?","",x$AgeuponOutcome))
  AgeDays <- as.numeric(gsub(" days?","",x$AgeuponOutcome))
  AgeYears[is.na(AgeYears)] = 0
  AgeMonths[is.na(AgeMonths)] = 0
  AgeWeeks[is.na(AgeWeeks)] = 0
  AgeDays[is.na(AgeDays)] = 0
  x$AgeuponOutcome = AgeDays + (AgeWeeks * 7) + (AgeMonths * 30) + (AgeYears * 365)
  
  x$NameLen   = nchar(as.character(x$Name))
  x$NameKnown = TRUE
  x[x$NameLen == 0,"NameKnown"] = FALSE
  
  for(i in c("Spayed Female","Intact Male","Intact Female","Neutered Male","Unknown")) x[[paste0("sex.",i)]] <- grepl(i,x$SexuponOutcome)
  x$SexuponOutcome <- NULL
  
  x$ColorMix = FALSE
  x[grepl("/",x$Color) == TRUE,]$ColorMix  = TRUE
  
  x$BreedMix = FALSE
  x[grepl("Mix",x$Breed) == TRUE,]$BreedMix  = TRUE
  
  x
}


cleanDog <- function(x){
  x$BreedWeirdness <- dogBreedsSummary[match(x$Breed,names(dogBreedsSummary))]
  x$ColorWeirdness  <- dogColorSummary[match(x$Color, names(dogColorSummary))]
  x$NameWeirdness   <- dogNameSummary[match(x$Name, names(dogNameSummary))]
  
  x$AnimalID <- NULL
  for(i in c("Black","White","Brown","Tan","Tricolor","Brindle","Blue","Red")) x[[paste0("col.",i)]] <- grepl(i,x$Color)
  x$Color <- NULL
  for(i in popularDogBreeds) x[[paste0("breed.",make.names(i))]] <- x$Breed == i
  x$Breed <- NULL
  
  #Impute medians
  medianNameLen = median(x[x$NameLen > 0,]$NameLen)
  maxWeirdness = max(x$NameWeirdness)
  x[x$NameWeirdness >= maxWeirdness,]$NameWeirdness = medianNameLen 
  x[x$NameLen == 0,]$NameLen = medianNameLen
  
  medianAge = median(x$AgeuponOutcome)
  x[x$AgeuponOutcome == 0,]$AgeuponOutcome = medianAge
  
  x$Name <- NULL
  
  # The reciprical makes more sense
  x$NameWeirdness = 1 / x$NameWeirdness
  
  x
}

sort(table(dfCatRaw$Color))

cleanCat <- function(x){
  x$BreedWeirdness <- catBreedsSummary[match(x$Breed,names(catBreedsSummary))]
  x$ColorWeirdness <- catColorSummary[match(x$Color, names(catColorSummary))]
  x$NameWeirdness  <- catNameSummary[match(x$Name, names(catNameSummary))]
  
  x$AnimalID <- NULL
  for(i in c("Black","White","Tabby","Brown","Orange","Tortie","Blue","Calico")) x[[paste0("col.",i)]] <- grepl(i,x$Color)
  x$Color <- NULL
  for(i in popularCatBreeds) x[[paste0("breed.",make.names(i))]] <- x$Breed == i
  x$Breed <- NULL
  
  #Impute medians
  medianNameLen = median(x[x$NameLen > 0,]$NameLen)
  maxWeirdness = max(x$NameWeirdness)
  x[x$NameWeirdness >= maxWeirdness,]$NameWeirdness = medianNameLen 
  x[x$NameLen == 0,]$NameLen = medianNameLen
  
  medianAge = median(x$AgeuponOutcome)
  x[x$AgeuponOutcome == 0,]$AgeuponOutcome = medianAge
  
  x$Name <- NULL
  
  # The reciprical makes more sense
  x$NameWeirdness = 1 / x$NameWeirdness
  
  x
}

#Run cleaning

dfDog <- cleanGeneral(dfDogRaw)
dfCat <- cleanGeneral(dfCatRaw)
dfDog <- cleanDog(dfDog)
dfCat <- cleanCat(dfCat)

subDog=cleanGeneral(dfDogRawSub)
subCat=cleanGeneral(dfCatRawSub)
subDog=cleanDog(subDog)
subCat=cleanCat(subCat)


#Build training rows

trainingRows <- createDataPartition(dfDog$OutcomeType, p = 0.9999, list = FALSE)
dfDogTrain  = dfDog[trainingRows,]
dfDogTest   = dfDog[-trainingRows,]
trainingRows <- createDataPartition(dfCat$OutcomeType, p = 0.9999, list = FALSE)
dfCatTrain  = dfCat[trainingRows,]
dfCatTest   = dfCat[-trainingRows,]

for(i in names(dfDogTrain))
  if(is.logical(dfDogTrain[[i]])) 
  {
    dfDogTrain[[i]] <- as.numeric(dfDogTrain[[i]]); 
    dfDogTest[[i]]  <- as.numeric(dfDogTest[[i]])
  }
for(i in names(dfCatTrain))
  if(is.logical(dfCatTrain[[i]])) 
  {
    dfCatTrain[[i]] <- as.numeric(dfCatTrain[[i]]); 
    dfCatTest[[i]] <- as.numeric(dfCatTest[[i]])
  }

set.seed(20160407L)

options(na.action="na.fail")

dfDogMat = model.matrix(OutcomeType ~ ., dfDogTrain)
mode(dfDogMat) = "numeric"
yDog   = as.matrix(as.integer(dfDogTrain$OutcomeType))
yDog = yDog -1

dfCatMat = model.matrix(OutcomeType ~ ., dfCatTrain)
head(dfCatTrain)
head(subCat)

mode(dfCatMat) = "numeric"
yCat   = as.matrix(as.integer(dfCatTrain$OutcomeType))
yCat = yCat -1

param <- list("objective" = "multi:softprob",   # multiclass classification 
              "eval_metric" = "mlogloss",       # evaluation metric 
              "nthread" = 4,               # number of threads to be used 
              "max_depth" = 8,             # maximum depth of tree 
              "eta" = 0.02,                # step size shrinkage 
              "gamma" = 0,                 # minimum loss reduction 
              "num_class" = 5,           # 5 different outcomes
              "subsample" = 0.8,         # part of data instances to grow tree 
              "early.stop.round" = 1,    # stop after 1 unimproving round (doesn't work!)
              "colsample_bytree" = 0.8   # subsample ratio of columns when constructing each tree 
              # "min_child_weight" = 12  # minimum sum of instance weight needed in a child 
)

nRounds = 800

bst.cv <- xgb.cv(param=param, data=dfDogMat, label=yDog, nfold=5, nrounds=nRounds, prediction=TRUE, verbose=TRUE, print.every.n = 20) 
minErrorDog = min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean])
minErrorDogIndex = which.min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean]) 
minErrorDog
minErrorDogIndex

bst.cv <- xgb.cv(param=param, data=dfCatMat, label=yCat, nfold=5, nrounds=nRounds, prediction=TRUE, verbose=TRUE, print.every.n = 20) 
minErrorCat = min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean])
minErrorCatIndex = which.min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean]) 
minErrorCat
minErrorCatIndex

errorEstOverall = ((minErrorDog * nrow(dfDogTrain)) +  (minErrorCat * nrow(dfCatTrain))) / (nrow(dfDogTrain) + nrow(dfCatTrain))
errorEstOverall

bstDog <- xgboost(param=param, data=dfDogMat, label=yDog, nrounds=minErrorDogIndex, verbose=1, print.every.n = 50)    
bstCat <- xgboost(param=param, data=dfCatMat, label=yCat, nrounds=minErrorCatIndex, verbose=1, print.every.n = 50)    


for(i in names(subDog))
  if(is.logical(subDog[[i]])) 
  {
    subDog[[i]] <- as.numeric(subDog[[i]]); 
  }
for(i in names(subCat))
  if(is.logical(subCat[[i]])) 
  {
    subCat[[i]] <- as.numeric(subCat[[i]]); 
  }

subDogDf = as.data.frame(OutcomeType=rep(1, nrow(subDog)), subDog)
subDogDfMat    = model.matrix(OutcomeType ~ .  , subDogDf)
dogPreds   <- predict(bstDog, subDogDfMat)  
dogPredsMatrix = t(matrix(dogPreds, nrow=5))
dogPredsDf = data.frame(ID = dfrawsub[dfrawsub$AnimalType=="Dog",]$ID,  dogPredsMatrix)

subCatDf = as.data.frame(OutcomeType=rep(1, nrow(subCat)), subCat)
subCatDfMat    = model.matrix(OutcomeType ~ .  , subCatDf)
catPreds   <- predict(bstCat, subCatDfMat)  
catPredsMatrix = t(matrix(catPreds, nrow=5))
catPredsDf = data.frame(ID = dfrawsub[dfrawsub$AnimalType=="Cat",]$ID,  catPredsMatrix)

predPets = rbind(catPredsDf, dogPredsDf)
names(predPets) = c("ID", "Adoption", "Died", "Euthanasia", "Return_to_owner", "Transfer")
predPetsSorted = predPets[order(predPets$ID),]
head(predPetsSorted,n=8)

write.csv(x=predPetsSorted, file = "submit1.csv", row.names = FALSE)



names <- dimnames(dfCatMat)[[2]]
importance_matrix <- xgb.importance(names, model = bstCat)
importance_matrix$Feature = factor(importance_matrix$Feature, levels = importance_matrix$Feature)
ggplot(importance_matrix, aes(x=Feature, y = Gain))  + coord_flip() + geom_bar(stat="identity") 