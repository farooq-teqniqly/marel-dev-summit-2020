using Microsoft.Data.Analysis;
using Microsoft.Spark.Sql;
using Newtonsoft.Json;
using System;
using System.IO;
using static Microsoft.Spark.Sql.Functions;

namespace StockPriceApp
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine($"USAGE: stockpriceapp [symbol] [year] [input data folder] [output folder]");
                return -1;
            }

            // Parse program args.
            var symbol = args[0];
            var year = int.Parse(args[1]);
            var dataFolder = args[2];
            var inputFilePath = Path.Join(dataFolder, $"{symbol}.us.txt");
            var outputFilePath = Path.Join(args[3], $"{symbol}-{year}-summary.txt");

            // Create Spark session.
            var spark = SparkSession.Builder().AppName("stock-price-app").GetOrCreate();

            Console.WriteLine($"Reading file {inputFilePath}...");

            // Read the input data file.
            var df = spark.Read()
                .Option("header", true)
                .Option("inferSchema", true)
                .Csv(inputFilePath);
            
            // Drop columns we don't want. Rename columns we keep.
            df = df.Drop("Date,Close")
            .WithColumnRenamed("Date", "ts")
            .WithColumnRenamed("Close", "close");

            // Get prices only for the year we're interested in.
            df = df.Filter(df["ts"] >= Lit($"{year}-01-01"))
                .Filter(df["ts"] <= Lit($"{year}-12-31"));

            // Get the min and max prices from the dataset.
            var minDf = df.Select(Min(df["close"]));
            var maxDf = df.Select(Max(df["close"]));

            // Get the actual min and max values for writing to the output file.
            // This action executes on the driver node.
            var highestPrice = maxDf.First()[0];
            var lowestPrice = minDf.First()[0];

            var summary = new { symbol, year, highestPrice, lowestPrice };

            // Write summary to output file.
            File.WriteAllText(outputFilePath, JsonConvert.SerializeObject(summary));

            Console.WriteLine($"Summary written to {outputFilePath}");

            // Stop the Spark session.
            spark.Stop();

            return 0;
        }
    }
}
