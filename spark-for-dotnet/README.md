# .NET For Apache Spark

## Introduction
This is a sample Spark app written with C# and .NET Core 3.1. The application reads stock prices from a file, does some transformations, filtering, and aggregation. Finally the application writes the processed data to a file.

## Development Environment Setup
Follow the steps [here](https://docs.microsoft.com/en-us/dotnet/spark/tutorials/get-started?tabs=windows) to setup your developer environment. 

The article covers installation of the following prerequisites:

1. .NET Core 3.1
2. Java SDK 8.1
3. Apache Spark. The sample app uses Spark 2.4.4 with Hadoop 2.7. The tar-gzip file for this particular version is [here](https://archive.apache.org/dist/spark/spark-2.4.4/spark-2.4.4-bin-hadoop2.7.tgz).

## Download Datasets
The stock price dataset was obtained from [Kaggle](https://www.kaggle.com/borismarjanovic/price-volume-data-for-all-us-stocks-etfs).

Unzip the dataset. Note the path to the ```Data\Stocks``` folder. You will need to specify this path as a parameter to the application.

## Running the sample
1. Open **SparkForDotNet.sln**.
2. Build the solution.
3. Open a Terminal window and browse to the folder containing the **StockPriceApp** project.
4. Navigate to ```bin\debug\netcoreapp3.1```.
5. Run the command below to execute the application in Spark. Make sure the third parameter to ```StockPriceApp.dll``` accurately reflects the path to the ```Data\Stocks``` folder on your machine. The fourth parameter specifies the path where the output file will be written to. Make sure the path exists on your machine or change it to an existing path.

```
spark-submit --class org.apache.spark.deploy.dotnet.DotnetRunner --master local microsoft-spark-2-4_2.11-1.0.0.jar dotnet StockPriceApp.dll msft 2017 "C:\Users\User\Downloads\Data\Stocks" "C:\temp"
```

## View Processed Data
When the app terminates, check for a file named ```msft-2017-summary.txt``` in the output directory. Open the file to view the processed data.