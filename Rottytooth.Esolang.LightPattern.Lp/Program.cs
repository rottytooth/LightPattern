using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

using Rottytooth.Esolang.LightPattern;


namespace Rottytooth.Esolang.LightPattern.Lp
{
    public class Program
    {
        public const int COMPILATION_FAIL = 2;
        private static string inputPath;
        public const int INVALID_PARAMETER = 1;

        /// <summary>
        /// Reads list of exposure changes from .lp file (in shutterspeed, aperture, color format)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<ImageData> LoadListOfExposures(string filePath)
        {
            List<ImageData> list = new List<ImageData>();
            using (StreamReader reader = File.OpenText(filePath))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    if (str == "")
                    {
                        continue;
                    }
                    string[] strArray2 = Regex.Split(str, "//")[0].Split(new char[] { ',' });
                    ImageData item = new ImageData();
                    if (strArray2[0].Contains<char>('/'))
                    {
                        string[] strArray3 = strArray2[0].Split(new char[] { '/' });
                        double num = Convert.ToDouble(strArray3[0]);
                        double num2 = Convert.ToDouble(strArray3[1]);
                        item.ExposureTime = num / num2;
                    }
                    else
                    {
                        item.ExposureTime = Convert.ToDouble(strArray2[0]);
                    }
                    item.FStop = Convert.ToDouble(strArray2[1]);
                    string str2 = strArray2[2].Trim().ToLower();

                    if (str2 == null)
                        throw new ApplicationException("color not populated");

                    item.Color = (ColorType)Enum.Parse(typeof(ColorType), str2, true);
                    list.Add(item);

                }
            }
            return list;
        }

        private static bool IsSecondArgumentRequred(string arg)
        {
            return false;
        }

        private static Dictionary<string, string> LoadParameters(string[] args)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string str = "";
            bool flag = false;

            foreach (string strInner in args)
            {
                string param = strInner;

                if (flag)
                {
                    dictionary[str] = param;
                    flag = false;
                }
                else if (!param.StartsWith("/"))
                {
                    string pathToTest = param;
                    if (param.EndsWith("\"") && param.StartsWith("\""))
                    {
                        param = param.Substring(1, param.Length - 2);
                    }

                    //if (inputPath == null)
                        inputPath = param;
                    //else
                    //    outputPath = param;
                }
                else
                {
                    dictionary.Add(param, "");
                    str = param;
                    flag = IsSecondArgumentRequred(param);
                }
            }
            return dictionary;
        }

        public static int Main(string[] args)
        {
            ParserError error;
            Dictionary<string, string> parameterList = LoadParameters(args);
            if (inputPath == null)
            {
                Console.WriteLine();
                Console.WriteLine("No source file indicated");
                PrintHelp();
                return 1;
            }
            List<ImageData> program = null;
            Scanner scanner = null;
            ICodeConverter codeConverter = null;

            if (parameterList.ContainsKey("/j")) // we are going to javascript with this, rather than c#
            {
                codeConverter = new JavascriptConverter();
            }
            else
            {
                codeConverter = new CSharpConverter();
            }

            if (parameterList.ContainsKey("/h") || parameterList.ContainsKey("/?"))
            {
                PrintHelp();
                return 0;
            }
            if (parameterList.ContainsKey("/g")) // create both instruction list (.lp) and c++ class for arduino (.cpp)
            {
                if (!File.Exists(inputPath))
                {
                    throw new ApplicationException("Could not open program file");
                }
                PhotoPatternGenerator gen = new PhotoPatternGenerator();
                gen.StartingAperture = 4;
                gen.StartingShutterSpeed = 3;
                gen.Generate(inputPath);
                return 0;
            }
            if (parameterList.ContainsKey("/t")) // convert binary or string input into ternary data, then generate instruction list and c++ class (as /g)
            {
                if (!File.Exists(inputPath))
                {
                    throw new ApplicationException("Could not open program file");
                }
                string base3data;

                //if (parameterList.ContainsKey("/s"))
                //    base3data = Ternarizer.FromString(inputPath);
                //else
                    base3data = Ternarizer.FromBinary(inputPath);

                PhotoPatternGenerator gen = new PhotoPatternGenerator();
                gen.StartingAperture = 4;
                gen.Generate(base3data, true);
                return 0;

            }

            if (parameterList.ContainsKey("/n")) // compile from list of exposures rather than actual photos
            {
                if (!File.Exists(inputPath))
                {
                    throw new ApplicationException("Could not open program file");
                }
                program = LoadListOfExposures(args[0]);
            }
            else // default behavior, compile
            {
                ExposurePattern.Image_PreLoad += new ExposurePattern._Image_PreLoad(Program.OnImagePreLoad);
                ExposurePattern.Image_Loaded += new ExposurePattern._Image_Loaded(Program.OnImageLoaded);


                if (!Directory.Exists(inputPath))
                {
                    throw new ApplicationException("Could not find directory");
                }
                ExposurePattern pattern = new ExposurePattern(inputPath);
                try
                {
                    pattern.Load();
                    program = pattern.ImageList;
                }
                catch (ParserError error1)
                {
                    error = error1;
                    Console.WriteLine("\n! Could not compile!\nParser Error: " + error.Message);
                    return 2;
                }
            }
            try
            {
                FileInfo info = new FileInfo(args[0]);
                scanner = new Scanner(program);
                if (parameterList.ContainsKey("/v"))
                {
                    Console.WriteLine("\nReading images...");
                    Console.WriteLine();
                    foreach (Symbol symbol in scanner.DeltaList)
                    {
                        Console.Write("Shutter: " + symbol.Shutter.ToString() + " ");
                        Console.Write(", FStop: " + symbol.FStop.ToString() + " ");
                        Console.Write(", Color: " + symbol.Color.ToString() + "\n");

                        if (parameterList.ContainsKey("/s")) // with s, print it slower (for screen capture)
                            Thread.Sleep(100);
                    }
                }
                string str = codeConverter.Convert(Tokenizer.Tokenize(scanner.DeltaList), info.Name.Split(new char[] { '.' })[0], (parameterList.ContainsKey("/s")));

                // c# export in verbose but not screen capture
                if (parameterList.ContainsKey("/v") && !parameterList.ContainsKey("/s"))
                {
                    Console.WriteLine();
                    Console.WriteLine("program in C#:");
                    Console.Write(str);
                }

                if (parameterList.ContainsKey("/c") || parameterList.ContainsKey("/j")) // output code
                {
                    string extension;

                    if (parameterList.ContainsKey("/j"))
                    {
                        extension = ".js";
                    }
                    else
                    {
                        extension = ".cs";
                    }
                    using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\" + info.Name.Split(new char[] { '.' })[0] + extension))
                    {
                        writer.Write(str);
                        writer.Flush();
                        writer.Close();
                    }
                }
                else
                {
                    ExeGenerator.CompileExecutable(str, Directory.GetCurrentDirectory() + @"\" + info.Name.Split(new char[] { '.' })[0] + ".exe");
                }
            }
            catch (ParserError error2)
            {
                error = error2;
                Console.WriteLine("\n! Could not compile!\nParser Error: " + error.Message);
                return 2;
            }
            return 0;
        }

        public static void OnImageLoaded(string message)
        {
            Console.WriteLine(message);
        }

        public static void OnImagePreLoad(string path)
        {
            Console.WriteLine("Loading image: " + path);
        }

        private static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("LightPattern Compiler, version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            Console.WriteLine("\t/c\tGenerate C# file rather than executable");
            Console.WriteLine();
            Console.WriteLine("\t/n\tGenerate from test file (list of exposures) rather than actual photos -- see Example.lp for format");
            Console.WriteLine();
            Console.WriteLine("\t/g\tTranslate list of deltas into suggested exposures -- see HelloWorld.lpc for format");
            Console.WriteLine();
            Console.WriteLine("\t/t\tTernarize data: based on binary file, generate list of exposures and .cpp file for Arduino to generate");
            Console.WriteLine();
            Console.WriteLine("\t/v\tVerbose: print out deltas as interpreted by compiler, to aid in debugging (useful when reading from .lp, is on by default when reading from images)");
            Console.WriteLine();
            Console.WriteLine("\t/h /?\tPrint this message");
            Console.WriteLine();
            Console.WriteLine();
        }
        // "undocumented" extras:
        //Console.WriteLine("\t/t /s\tTernarize string: based on provided string, generate list of exposures and .cpp file for Arduino to generate");
        //Console.WriteLine();
        //Console.WriteLine("\t/j\tGenerate javascript file rather than executable");
        //Console.WriteLine();
    }
}

