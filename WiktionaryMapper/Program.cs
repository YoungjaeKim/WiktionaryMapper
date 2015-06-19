using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memoling.Tools.WiktionaryMapper.Data;
using Memoling.Tools.WiktionaryParser.Data;
using Memoling.Tools.WiktionaryParser.Input;
using Memoling.Tools.WiktionaryParser.Output;

namespace Memoling.Tools.WiktionaryMapper
{
    class Program
    {
        static StreamCollection streamCollection;
        static StreamReader source;
        static OutputProcessor outputProcessor;
        static Stopwatch stopwatch;
        static int processedEntry;
        static StreamUris uris;

        static void Init()
        {
            streamCollection = new StreamCollection();
            outputProcessor = new OutputProcessor();
            stopwatch = new Stopwatch();
        }

        static void Main(string[] args)
        {
            Init();
            try
            {
                ParseArgs(args);
            }
            catch (Exception exception)
            {
                Environment.Exit(1);
            }

            IDataProcessor data = CreateDataProcessor();
            InputProcessor inputProcessor = new InputProcessor(data);
            
            PrintImportStarted();

            try
            {
                using (streamCollection)
                {
                    int i = 0;
                    foreach (var result in inputProcessor.Process(source))
                    {
                        // If failed to Parse
                        if (result == null)
                        {
                            continue;
                        }

                        //if (i++ > 100) break;             // For testing
                
                        outputProcessor.Next(result);
                        PrintProgress();
                    }
                }

                PrintOptimizationStarted();

                Optimize(uris.Synonims);
                //Optimize(uris.Antonyms);

                PrintFinished();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }

        static void ParseArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new ArgumentNullException("args");
            }
            if (args.Length > 1)
            {
                throw new ArgumentException("argument error");
            }

            var fileInfo = new FileInfo(args[0]);
            if (!fileInfo.Exists || fileInfo.DirectoryName == null)
            {
                throw new FileNotFoundException("File Not Found: " + fileInfo.FullName);		        
            }

            string suffix = DateTime.Now.ToString("s").Replace(":","-");
            uris.Source.Uri = fileInfo.FullName;
            uris.Translations.Uri = Path.Combine(fileInfo.DirectoryName, String.Format("translations-{0}.sql", suffix));
            uris.Definitions.Uri = Path.Combine(fileInfo.DirectoryName, String.Format("definitions-{0}.sql", suffix));
            uris.Synonims.Uri = Path.Combine(fileInfo.DirectoryName, String.Format("synonims-{0}.sql", suffix));
            uris.Temp.Uri = Path.Combine(fileInfo.DirectoryName, String.Format("temporary-{0}.sql", suffix));

            uris.Translations.Format = OutputFormat.Sql;
            uris.Definitions.Format = OutputFormat.Sql;
            uris.Synonims.Format = OutputFormat.Sql;
            uris.Temp.Format = OutputFormat.Sql;

            source = new StreamReader(uris.Source.Uri);
            streamCollection.Add(source);

            AddOutputStream(uris.Translations, DataProcessorConfigBuilder.TranslationSection);
            AddOutputStream(uris.Definitions, DataProcessorConfigBuilder.PartsOfSpeechSections);
            AddOutputStream(uris.Synonims, DataProcessorConfigBuilder.SynonymSection);
        }

        static void AddOutputStream(StreamUris.StreamDetail detail, string header) 
        {
            AddOutputStream(detail, new string[] { header });
        }

        static void AddOutputStream(StreamUris.StreamDetail detail, string[] headers)
        {
            var stream = new StreamWriter(detail.Uri);
            outputProcessor.Outputs.Add(new OutputResult(headers, stream, detail.Format));
            streamCollection.Add(stream);
        }

        static IDataProcessor CreateDataProcessor()
        {
            var config = DataProcessorConfigBuilder.Build();
            return new DataProcessor(config);
        }

        static void Optimize(StreamUris.StreamDetail detail)
        {
            using (StreamReader sr = new StreamReader(detail.Uri))
            using (StreamWriter sw = new StreamWriter(uris.Temp.Uri))
            {
                BinaryExpressionOutputResult.RemoveDuplicates(sr, sw, detail.Format);
            }

            File.Delete(detail.Uri);
            File.Move(uris.Temp.Uri, detail.Uri);
        }

        #region Printing & UI

        static void PrintImportStarted()
        {
            Console.WriteLine("Importing");
            stopwatch.Start();
            processedEntry = 0;
        }

        static void PrintProgress()
        {
            processedEntry++;
            if (processedEntry % 100 == 0)
            {
                Console.Write("\r{0} ", processedEntry);
            }
        }

        static void PrintOptimizationStarted()
        {
            Console.WriteLine(stopwatch.Elapsed.ToString());
            Console.WriteLine();
            Console.WriteLine("Optimizing");
            stopwatch.Reset();
            stopwatch.Start();
        }

        static void PrintFinished()
        {
            Console.WriteLine();
            Console.WriteLine(stopwatch.Elapsed.ToString());
            Console.WriteLine("Done");
            Console.Read();
        }

        static void PrintException(Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.ToString());
            Console.Read();
        }

        #endregion

        struct StreamUris
        {
            public struct StreamDetail
            {
                public string Uri;
                public OutputFormat Format;
            }

            public StreamDetail Source;
            public StreamDetail Definitions;
            public StreamDetail Translations;
            public StreamDetail Synonims;
            public StreamDetail Antonyms;
            public StreamDetail Temp;
        }

    }
}
