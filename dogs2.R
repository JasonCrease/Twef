#install.packages("caret")
#install.packages("lubridate")
#install.packages("xgboost")
#install.packages("h2o")
#install.packages("MASS")
#install.packages("MLmetrics")
library(MASS)
library(caret)
library(xgboost)
library(h2o)
library(lubridate)
<<<<<<< HEAD
library(MLmetrics)
=======
library(stringr)
>>>>>>> a48a2b6d0549e8c5be55af46a8cff8dc0f4392f9

#table(dfDog$OutcomeType)
#hist(dfDog[dfDog$OutcomeType=='Euthanasia',]$MinuteOfDay,breaks=20)
#hist(dfDog[dfDog$OutcomeType=='Died',]$MinuteOfDay,breaks=20)
#hist(dfDog[dfDog$OutcomeType=='Adoption',]$MinuteOfDay,breaks=20)
#hist(dfDog[dfDog$OutcomeType=='Transfer',]$MinuteOfDay,breaks=20)
#hist(dfDog[dfDog$OutcomeType=='Return_to_owner',]$MinuteOfDay,breaks=20)

setwd("f:/Github/twef/pets/")
breedData = read.csv("./PetInfoGrabber/breedListout.csv")
dfraw    <- read.csv("./train.csv")
dfrawsub <- read.csv("./test.csv")

dfDogRaw   =dfraw[dfraw$AnimalType   =="Dog",]
dfCatRaw   =dfraw[dfraw$AnimalType   =="Cat",]
dfDogRawSub=dfrawsub[dfrawsub$AnimalType=="Dog",]
dfCatRawSub=dfrawsub[dfrawsub$AnimalType=="Cat",]

# Add breedData data. We have to add a row number column and sort by it to preserve order. Sigh.
dfDogRaw$RowNums = as.numeric(row.names(dfDogRaw))
dfDogRaw = merge(dfDogRaw,breedData,by="Breed",all.y=FALSE,all.x=TRUE)
dfDogRaw = dfDogRaw[order(dfDogRaw$RowNums),] 
dfDogRaw$RowNums <- NULL
dfDogRawSub$RowNums = as.numeric(row.names(dfDogRawSub))
dfDogRawSub = merge(dfDogRawSub,breedData,by="Breed",all.y=FALSE,all.x=TRUE)
dfDogRawSub = dfDogRawSub[order(dfDogRawSub$RowNums),] 
dfDogRawSub$RowNums <- NULL

# Make columns match so rbind is possible
dfDogRawSub=subset(dfDogRawSub, select = -c(ID))
dfDogRawSub$OutcomeType = as.factor("NULL")
dfDogRawSub$OutcomeSubtype = as.factor("NULL")
dfDogRawSub$AnimalID = as.factor("NULL")
allDog = rbind(dfDogRaw, dfDogRawSub)
allDog$Breed = factor(allDog$Breed)   # drop unused levels
#breedTable = sort(table(allDog$Breed),decreasing = TRUE)
#write.csv(file = "dogBreedList.csv", x=names(breedTable),col.names = FALSE,row.names=FALSE)

dfCatRawSub=subset(dfCatRawSub, select = -c(ID))
dfCatRawSub$OutcomeType = as.factor("NULL")
dfCatRawSub$OutcomeSubtype = as.factor("NULL")
dfCatRawSub$AnimalID = as.factor("NULL")
allCat = rbind(dfCatRaw, dfCatRawSub)
allCat$Breed = factor(allCat$Breed)   # drop unused levels
#breedTable = sort(table(allCat$Breed),decreasing = TRUE)
#write.csv(file = "catBreedList.csv", x=names(breedTable),col.names = FALSE,row.names=FALSE)

# Assemble popularity stats
popularDogBreeds <- names(summary(allDog$Breed,maxsum=3L))
popularCatBreeds <- names(summary(allCat$Breed,maxsum=8))
dogNameSummary   <- summary(allDog$Name, maxsum=Inf)
catNameSummary   <- summary(allCat$Name, maxsum=Inf)
dogBreedsSummary <- summary(allDog$Breed,maxsum=Inf)
catBreedsSummary <- summary(allCat$Breed,maxsum=Inf)
dogColorSummary <- summary(allDog$Color,maxsum=Inf)
catColorSummary <- summary(allCat$Color,maxsum=Inf)

# 886 478
# 885 478

interestingBreeds=c(
  "Pit Bull Mix",
  "Yorkshire Terrier Mix",
  "Dachshund Mix"
)

cleanGeneral <- function(x){
  # This is irrelevant
  x$OutcomeSubtype <- NULL
  
  #Datetime stuff
  
  x$MinuteOfDay <- (lubridate::hour(x$DateTime) * 60) + minute(x$DateTime) 
  x$Weekday     <- wday(x$DateTime) - 1
  x[x$Weekday == 0,]$Weekday = 7  # Move Sunday to after Saturday
  x$IsWeekend = FALSE
  x[x$Weekday == 1 | x$Weekday == 7,]$IsWeekend = TRUE
  x$Month       <- lubridate::month(x$DateTime)
  x$DateTime    <- as.numeric(as.POSIXct(x$DateTime))
  x$ZeroMinute=FALSE
  x[x$MinuteOfDay == 0,]$ZeroMinute=TRUE
  
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
  x$Cutename = FALSE
  x$Cutename   = grepl("(ie|i|y)$",  x$Name)
  x$VowelRatio = str_count(x$Name, "a|e|i|o|u|y|A|E|I|O|U") / x$NameLen
  x[is.na(x$VowelRatio), "VowelRatio" ] = median(x$VowelRatio, na.rm = TRUE)  
  
  x$Male   = FALSE
  x$Intact = FALSE
  x$UnknownIntactness = FALSE
  x[x$SexuponOutcome == "Intact Male"   | x$SexuponOutcome == "Neutered Male",]$Male = TRUE
  x[x$SexuponOutcome == "Intact Female" | x$SexuponOutcome == "Intact Male",]$Intact = TRUE
  x[x$SexuponOutcome == "Unknown",]$UnknownIntactness = TRUE
  x$SexuponOutcome <- NULL
  
  x$ColorMix = FALSE
  x[grepl("/",x$Color) == TRUE,]$ColorMix  = TRUE
  
  x$BreedMix = FALSE
  x[grepl("Mix",x$Breed) == TRUE,]$BreedMix  = TRUE
  
  x
}

sort(table(dfDogRaw$Color))

cleanDog <- function(x){
  x$BreedWeirdness  <- dogBreedsSummary[match(x$Breed,names(dogBreedsSummary))]
  x$ColorWeirdness  <- dogColorSummary[match(x$Color, names(dogColorSummary))]
  x$NameWeirdness   <- dogNameSummary[match(x$Name, names(dogNameSummary))]

  x$AnimalID <- NULL
  for(i in c("Black","White","Tan","Brown","Blue","Tricolor","Brindle","Red")) x[[paste0("col.",i)]] <- grepl(i,x$Color)
  x$Color <- NULL
  for(i in interestingBreeds) x[[paste0("breed.",make.names(i))]] <- x$Breed == i
  x$Breed <- NULL
  
  #Impute medians
  medianNameLen = median(x[x$NameLen > 0,]$NameLen)
  maxWeirdness = max(x$NameWeirdness)
  x[x$NameWeirdness >= maxWeirdness,]$NameWeirdness = medianNameLen 
  x[x$NameLen == 0,]$NameLen = medianNameLen
  
  medianAge = median(x$AgeuponOutcome)
  x[x$AgeuponOutcome == 0,]$AgeuponOutcome = medianAge
  x$IsOld = FALSE
  dfDogTrain[dfDogTrain[,"AgeuponOutcome"] > dfDogTrain[,"Lifespan"] * 365 * 0.9,]$IsOld = TRUE
  
  x$Name <- NULL
  
  # The reciprical makes more sense
  x$NameWeirdness  = 1 / x$NameWeirdness
  x$ColorWeirdness = 1 / x$ColorWeirdness
  x$BreedWeirdness = 1 / x$BreedWeirdness
  
  # How old it is relatively. Might be useful for predicting death/euthanasia
  x$RelativeAge = x$AgeuponOutcome / x$Lifespan
  
  # Irrelevant columns
  x$AnimalType       <- NULL
  
  x
}


cleanCat <- function(x){
  x$BreedWeirdness <- catBreedsSummary[match(x$Breed,names(catBreedsSummary))]
  x$ColorWeirdness <- catColorSummary[match(x$Color, names(catColorSummary))]
  x$NameWeirdness  <- catNameSummary[match(x$Name, names(catNameSummary))]
  
  x$AnimalID <- NULL
  for(i in c("Black","White","Tortie","Tabby","Brown","Orange","Blue","Calico","Torbie","Cream")) x[[paste0("col.",i)]] <- grepl(i,x$Color)
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
  x$NameWeirdness  = 1 / x$NameWeirdness
  x$ColorWeirdness = 1 / x$ColorWeirdness
  x$BreedWeirdness = 1 / x$BreedWeirdness
  
  # Irrelevant columns
  x$AnimalType       <- NULL
  
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
trainingRows <- createDataPartition(dfDog$OutcomeType, p = 0.9, list = FALSE)
dfDogTrain  = dfDog[trainingRows,]
dfDogTest   = dfDog[-trainingRows,]
trainingRows <- createDataPartition(dfCat$OutcomeType, p = 0.9, list = FALSE)
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
yDog = as.matrix(as.integer(dfDogTrain$OutcomeType))
yDog = yDog - 1

dfCatMat = model.matrix(OutcomeType ~ ., dfCatTrain)
mode(dfCatMat) = "numeric"
yCat = as.matrix(as.integer(dfCatTrain$OutcomeType))
yCat = yCat - 1

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

nRounds = 900
nFold   = 4
param$subsample        = 0.7
param$colsample_bytree = 0.6

set.seed(20160415L)
bst.cv <- xgb.cv(param=param, data=dfDogMat, label=yDog, nfold=nFold, nrounds=nRounds, prediction=TRUE, verbose=TRUE, print.every.n = 20) 
minErrorDog = min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean])
minErrorDogIndex = which.min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean]) 
minErrorDog
minErrorDogIndex

param$subsample        = 0.8
param$colsample_bytree = 0.8
nRounds = 900
set.seed(920160415L)
bst.cv <- xgb.cv(param=param, data=dfCatMat, label=yCat, nfold=nFold, nrounds=nRounds, prediction=TRUE, verbose=TRUE, print.every.n = 20) 
minErrorCat = min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean])
minErrorCatIndex = which.min(bst.cv$dt[, bst.cv$dt$test.mlogloss.mean]) 
minErrorCat
minErrorCatIndex

errorEstOverall = ((minErrorDog * nrow(dfDogTrain)) +  (minErrorCat * nrow(dfCatTrain))) / (nrow(dfDogTrain) + nrow(dfCatTrain))
errorEstOverall

bstDog <- xgboost(param=param, data=dfDogMat, label=yDog, nrounds=minErrorDogIndex, verbose=1, print.every.n = 50)    
bstCat <- xgboost(param=param, data=dfCatMat, label=yCat, nrounds=minErrorCatIndex, verbose=1, print.every.n = 50)    


# Try out on test data
dfDogMatTest = model.matrix(OutcomeType ~ ., dfDogTest)
mode(dfDogMatTest) = "numeric"
yDogTest = as.matrix(as.integer(dfDogTest$OutcomeType))
yDogTest = yDogTest
dogPredsTest   <- predict(bstDog, dfDogMatTest)  
dogPredsTest = t(matrix(dogPredsTest, nrow=5))

yDogTestMat = matrix(nrow = nrow(yDogTest), ncol = 5, data = rep(0, 5* nrow(yDogTest)))
yDogTestMat[yDogTest==1,1] = 1
yDogTestMat[yDogTest==2,2] = 1
yDogTestMat[yDogTest==3,3] = 1
yDogTestMat[yDogTest==4,4] = 1
yDogTestMat[yDogTest==5,5] = 1
MultiLogLoss(yDogTestMat, dogPredsTest)


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
subDogDf$OutcomeType = 1
subDogDfMat    = model.matrix(OutcomeType ~ .  , subDogDf)
dogPreds   <- predict(bstDog, subDogDfMat)  
dogPredsMatrix = t(matrix(dogPreds, nrow=5))
dogPredsDf = data.frame(ID = dfrawsub[dfrawsub$AnimalType=="Dog",]$ID,  dogPredsMatrix)

subCatDf = as.data.frame(OutcomeType=rep(1, nrow(subCat)), subCat)
subCatDf$OutcomeType = 1
subCatDfMat    = model.matrix(OutcomeType ~ .  , subCatDf)
catPreds   <- predict(bstCat, subCatDfMat)  
catPredsMatrix = t(matrix(catPreds, nrow=5))
catPredsDf = data.frame(ID = dfrawsub[dfrawsub$AnimalType=="Cat",]$ID,  catPredsMatrix)

predPets = rbind(catPredsDf, dogPredsDf)
names(predPets) = c("ID", "Adoption", "Died", "Euthanasia", "Return_to_owner", "Transfer")
predPetsSorted = predPets[order(predPets$ID),]
#head(predPetsSorted,n=8)

write.csv(x=predPetsSorted, file = "submit1.csv", row.names = FALSE)

#plausible=read.csv("./plausible.csv", header=TRUE)
#head(plausible,n=8)

names <- dimnames(dfDogTrain)[[2]]
importance_matrix <- xgb.importance(names, model = bstDog)
importance_matrix
importance_matrix$Feature = factor(importance_matrix$Feature, levels = importance_matrix$Feature)
ggplot(importance_matrix, aes(x=Feature, y = Gain))  + coord_flip() + geom_bar(stat="identity")

stop()

# Startup h2o

localH2O = h2o.init(ip = "localhost", port = 54321, startH2O = TRUE, max_mem_size = '4g', nthreads = 2 )

# Use h2o on dogs

dat_h2o      = as.h2o(dfDogTrain)
testdat_h2o  = as.h2o(dfDogTest)
modelDogRF = h2o.randomForest(x = 1:56, y = 2, training_frame = dat_h2o, 
                            validation_frame = testdat_h2o,ntrees=1000)
h2o.logloss(modelDogRF)
modelDogGbm = h2o.gbm(x = 1:56, y = 2, training_frame = dat_h2o,ntrees=300, learn_rate=0.03, validation_frame=testdat_h2o )
summary(modelDogGbm)
h2o.logloss(modelDogGbm)

subDogForh2o = subDog
levels(subDogForh2o$OutcomeType) = c("Adoption", "Died", "Euthanasia", "Return_to_owner", "Transfer")
subDog_h2o  = as.h2o(subDogForh2o)

h2oDog_submit = h2o.predict(object = modelDog, newdata = subDog_h2o)
dfh2oDogSubmit = as.data.frame(h2oDog_submit)
dfh2oDogSubmit$ID = dfrawsub[dfrawsub$AnimalType=="Dog",]$ID




dfDogTrain$WasAdopted = 0
dfDogTrain[dfDogTrain$OutcomeType == 'Adoption',"WasAdopted"] = 1 
glm = glm(formula = WasAdopted ~ Weight + Height + Weekday + AgeuponOutcome 
          + dfDogTrain$GoodWithKids 
          + dfDogTrain$Trainability
          + dfDogTrain$Price
          + dfDogTrain$CatFriendly
          + dfDogTrain$DogFriendly
          + dfDogTrain$Shedding
          + dfDogTrain$Intelligence
          + dfDogTrain$Adaptability
          , data=dfDogTrain )

dfc = subset(dfDogTrain, select = c(WasAdopted,GoodWithKids,Price,CatFriendly,DogFriendly,
                                    Shedding,Intelligence,Adaptability,Trainability,Lifespan,Weight,Height ))
corrgram::corrgram(dfc)
corplot(dfDogTrain$Intelligence, dfDogTrain$DogFriendly)
ggplot(dfDogTrain, aes(Intelligence, Lifespan))+geom_point(color="firebrick")+geom_jitter()

stop()

# Use h2o on cats

trainingRows = createDataPartition(dfCat$OutcomeType, p = 0.9, list = FALSE)
dfCatTrain   = dfCat[trainingRows,]
dfCatTest    = dfCat[-trainingRows,]

dat_h2o      = as.h2o(dfCatTrain)
testdat_h2o  = as.h2o(dfCatTest)
modelCat = h2o.randomForest(x = 1:33, y = 2, training_frame = dat_h2o, 
                            validation_frame = testdat_h2o, ntrees=2000)
h2o.logloss(modelCat)
#summary(modelCat)

subCatForh2o = subCat
levels(subCatForh2o$OutcomeType) = c("Adoption", "Died", "Euthanasia", "Return_to_owner", "Transfer")
subCat_h2o  = as.h2o(subCatForh2o)
h2oCat_submit = h2o.predict(object = modelCat, newdata = subCat_h2o)
dfh2oCatSubmit = as.data.frame(h2oCat_submit)
dfh2oCatSubmit$ID = dfrawsub[dfrawsub$AnimalType=="Cat",]$ID


# Join h2o stuff together

predPetsh2o = rbind(dfh2oDogSubmit, dfh2oCatSubmit)
head(predPetsh2o)
predPetsSorted = predPetsh2o[order(predPetsh2o$ID),]
#head(predPetsSorted,n=8)

write.csv(x=predPetsSorted, file = "submit2.csv", row.names = FALSE)