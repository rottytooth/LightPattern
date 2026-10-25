using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Rottytooth.Esolang.LightPattern;


namespace Rottytooth.Esolang.LightPattern.Lp
{

    public class PhotoPatternGenerator
    {
        private ColorType _colorIndex;
        private int _wheelTurn;
        private int _fStopIndex;
        private int _shutterIndex;

        public class CameraSetting
        {
            public int Code;
            public double Value;
            public string DisplayValue; // friendly value to print
        };

        public class LpLine // one line of LP code
        {
            public string Comment;
            public int ShutterChange;
            public int ApertureChange;
            public int ColorChange;
        };

        private List<CameraSetting> Apertures;
        private List<CameraSetting> ShutterSpeeds;

        public int StartingAperture { get; set; }
        public int StartingShutterSpeed { get; set; }


        public PhotoPatternGenerator(string settingsFile = "", string directory = "")
        {
            if (settingsFile == "")
            {
                settingsFile = ConfigurationManager.AppSettings["SettingsFile"];
            }
            if (directory == "")
            {
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            }

            // load apertures and shutterspeeds from appropriate settings file
            XDocument doc = XDocument.Load(directory + settingsFile);

            Setup(doc);
        }
        public PhotoPatternGenerator(XDocument doc)
        {
            Setup(doc);
        }

        public void Setup(XDocument doc)
        {
            var data = from item in doc.Descendants("FStop")
                       select new CameraSetting
                       {
                           Code = Convert.ToInt32(item.Attribute("code").Value),
                           Value = Convert.ToDouble(item.Attribute("aperture").Value),
                           DisplayValue = item.Attribute("aperture").Value
                       };

            Apertures = data.ToList();

            data = from item in doc.Descendants("ShutterSpeed")
                       select new CameraSetting
                       {
                           Code = Convert.ToInt32(item.Attribute("code").Value),

                           Value = (Convert.ToBoolean(item.Attribute("inverse").Value) ? 
                                1.0 / Convert.ToDouble(item.Attribute("seconds").Value) :
                                Convert.ToDouble(item.Attribute("seconds").Value)),

                           DisplayValue = (Convert.ToBoolean(item.Attribute("inverse").Value) ?
                                "1/" + item.Attribute("seconds").Value :
                                item.Attribute("seconds").Value)
                       };

            ShutterSpeeds = data.ToList();

            this.StartingShutterSpeed = (int)Math.Floor((double)(((double)ShutterSpeeds.Count) / 2.0));
            this.StartingAperture = (int)Math.Floor((double)(((double)Apertures.Count) / 2.0));

            //foreach (CameraSetting aperture in Apertures)
            //{
            //    Console.Write("code = " + aperture.Code);
            //    Console.WriteLine(" aperture = " + aperture.Value);
            //}

            //foreach (CameraSetting shutterSpeed in ShutterSpeeds)
            //{
            //    Console.Write("code = " + shutterSpeed.Code);
            //    Console.WriteLine(" seconds = " + shutterSpeed.Value);
            //}

        }

        private List<LpLine> LoadProgram(string input, bool isString = false)
        {
            this.Reset();

            List<LpLine> program = new List<LpLine>();
            //                FileInfo info = new FileInfo(input);

            TextReader reader;

            if (!isString)
                reader = File.OpenText(input);
            else
                reader = new StringReader(input);

            try
            {
                string str;
                int num = 0;

                while ((str = reader.ReadLine()) != null)
                {
                    num++;
                    if (str != "")
                    {
                        // retrieve new set of codes
                        string[] strArray = Regex.Split(str, "//"); // remove comment
                        string numberCodes = strArray[0];

                        LpLine currentLine = new LpLine();

                        currentLine.Comment = (strArray.Length > 1 ? strArray[1] : "");
                        currentLine.ShutterChange = (int)char.GetNumericValue(numberCodes[0]);
                        currentLine.ApertureChange = (int)char.GetNumericValue(numberCodes[1]);
                        currentLine.ColorChange = (int)char.GetNumericValue(numberCodes[2]);

                        program.Add(currentLine);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
            return program;
        }

        private void OutputFiles(List<LpLine> program, TextWriter lpWriter)
        {
            // FIXME: yes, this is terrible -- cppWriter and xmlWriter were thrown in as afterthoughts -- clean up or dump them
            StringWriter garbageWriter = new StringWriter();
            OutputFiles(program, lpWriter, garbageWriter, garbageWriter); // if we only need the exposures, collect cpp and xml data and dump it
        }

        private void OutputFiles(List<LpLine> program, TextWriter lpWriter, TextWriter cppWriter, TextWriter xmlWriter = null)
        {
            // write initial state
            // for .lp write one line
            this.WriteLpLine(lpWriter);
            this.WriteCppLine(cppWriter, 0, null);
            if (xmlWriter != null)
            {
                this.WriteXmlLine(xmlWriter, 0, "initial state");
            }

            for (int i = 0; i < program.Count; i++)
            {
                LpLine line = program[i];

                this._shutterIndex = this.SetNewIndex(line.ShutterChange, this._shutterIndex, ShutterSpeeds);
                this._fStopIndex = this.SetNewIndex(line.ApertureChange, this._fStopIndex, Apertures);

                this.SetNewColorIndex(line.ColorChange);
                try
                {
                    this.WriteLpLine(lpWriter, line.Comment); 
                    this.WriteCppLine(cppWriter, i + 1, line.Comment);

                    if (xmlWriter != null)
                    {
                        this.WriteXmlLine(xmlWriter, i + 1, line.ShutterChange.ToString() + line.ApertureChange.ToString() + line.ColorChange.ToString(), line.Comment);
                    }

                    //FIXME: should be testing for this situation and adding 222s as needed
                    if (((line.ShutterChange == 2) && (line.ApertureChange == 2)) && (line.ColorChange == 2))
                    {
                        this.Reset();
                        this.WriteLpLine(lpWriter);
                        this.WriteCppLine(cppWriter, i + 1);

                        if (xmlWriter != null)
                        {
                            this.WriteXmlLine(xmlWriter, i + 1, line.ShutterChange.ToString() + line.ApertureChange.ToString() + line.ColorChange.ToString());
                        }
                    }
                }
                catch (Exception exception)
                {
                    string errorMessage = string.Concat(new object[] { "Error caught in line ", i, ": ", exception.Message });

                    if (exception is IndexOutOfRangeException)
                    {
                        throw new IndexOutOfRangeException(errorMessage);
                    }
                    else
                    {
                        throw new ApplicationException(errorMessage);
                    }
                }
            }
        }

        public string GenerateToString(string input)
        {
            StringWriter retString = new StringWriter();
            List<LpLine> program = LoadProgram(input, true);
            try
            {
                OutputFiles(program, retString);
            }
            catch(IndexOutOfRangeException ex)
            {
                return "Your program went out of range: " + ex.Message +
                    "<br/>Try inserting a reset (222) somewhere before that line";
            }
            return retString.ToString();
        }


        /// <summary>
        /// Generates a pseudo-code file that lists the photos that must be taken to generate a program / data
        /// </summary>
        /// <param name="input">path to file to input or string</param>
        /// <param name="hasFile">whether input is a file or a string</param>
        public void Generate(string input, bool hasFile = false)
        {
            FileInfo info = null;
            StreamWriter lpWriter; // to output the list of exposures
            StreamWriter cppWriter; // to output the cpp file (for arduino)
            StreamWriter xmlWriter; // this is for documentation / creating video

            if (!hasFile)
            {
                info = new FileInfo(input);
                string name = info.Name.Split(new char[] { '.' })[0];

                lpWriter = new StreamWriter(info.DirectoryName + @"\" + name + ".lp");
                cppWriter = new StreamWriter(info.DirectoryName + @"\" + name + "_EosProgram.cpp");
                xmlWriter = new StreamWriter(info.DirectoryName + @"\" + name + "_XmlTemplate.xml");
            }
            else
            {
                lpWriter = new StreamWriter("Output.lp");
                cppWriter = new StreamWriter("EosProgram.cpp");
                xmlWriter = new StreamWriter("XmlTemplate.xml");
            }


            try
            {
                List<LpLine> program = LoadProgram(input, hasFile);

                xmlWriter.WriteLine("<XmlTemplate>");

                // load top of .cpp file to write out
                using (StreamReader cppTemplate = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ArduinoTemplate.txt"))
                {
                    string templateLine = "";

                    while ((templateLine = cppTemplate.ReadLine()) != null)
                    {
                        if (templateLine.Contains("%%length%%"))
                        {
                            templateLine = templateLine.Replace("%%length%%", (program.Count + 1).ToString());
                            cppWriter.WriteLine(templateLine);
                        }
                        else if (templateLine.Contains("%%program%%"))
                        {
                            OutputFiles(program, lpWriter, cppWriter, xmlWriter);
                        }
                        else
                        {
                            cppWriter.WriteLine(templateLine);
                        }
                    }
                }
            }
            finally
            {
                xmlWriter.WriteLine("</XmlTemplate>");
                lpWriter.Dispose();
                cppWriter.Dispose();
                xmlWriter.Dispose();
            }
        }

        private void Reset()
        {
            this._shutterIndex = this.StartingShutterSpeed;
            this._fStopIndex = this.StartingAperture;
            this._colorIndex = ColorType.Red;
            this._wheelTurn = 0;
        }

        private void SetNewColorIndex(int colorChange)
        {
            // colorChange = change of color (in terms of the language). It is modular, so loops back to the beginning:
            // 0 = turn back one
            // 1 = stay same
            // 2 = turn forward one

            // _wheelTurn = change of color (in terms of the wheel itself). It is *not* modular, we need to tell the wheel to loop back:
            // -2 = turn back two (advance one past the last color) 
            // -1 = turn back one
            //  0 = no change
            //  1 = turn forward one
            //  2 = turn forward two (turn back one from the first color)

            if (colorChange == 2)
            {
                if (this._colorIndex < ColorType.Blue)
                {
                    this._colorIndex += 1;
                    this._wheelTurn = 1;
                }
                else
                {
                    this._colorIndex = ColorType.Red;
                    this._wheelTurn = -2;
                }
            }
            else if (colorChange == 0)
            {
                if (this._colorIndex > ColorType.Red)
                {
                    this._colorIndex -= 1;
                    this._wheelTurn = -1;
                }
                else
                {
                    this._colorIndex = ColorType.Blue;
                    this._wheelTurn = 2;
                }
            }
            else
                this._wheelTurn = 0;
        }

        private int SetNewIndex(int change, int currValue, List<CameraSetting> list)
        {
            if (change == 2) // upward change
            {
                // FIXME: favors slower shutter, more open aperture with this commented out

                if (currValue > ((int)Math.Floor((double)((((double)list.Count) / 4.0) * 3.0)))) // we're close to the edge, return a big jump
                {
                    return (int)Math.Floor((double)(((double)list.Count) / 2.0));
                }
                return (currValue - 1);
            }
            if (change == 0) // downward change
            {
                //if (currValue < ((int)Math.Floor((double)(((double)list.Count) / 4.0))))
                //{
                //    return (int)Math.Floor((double)(((double)list.Count) / 2.0));
                //}
                return (currValue + 1);
            }

            //FIXME: if it's right at the edge, reset with 222

            return currValue;
        }

        private void CheckOutOfRange()
        {
            if ((this._shutterIndex >= ShutterSpeeds.Count) || (this._shutterIndex < 0))
            {
                throw new IndexOutOfRangeException("Shutter is out of range, current value of shutter index: " + this._shutterIndex);
            }
            if ((this._fStopIndex >= Apertures.Count) || (this._fStopIndex < 0))
            {
                throw new IndexOutOfRangeException("FStop is out of range, current value of fstop index: " + this._fStopIndex);
            }
        }

        private void WriteCppLine(TextWriter writer, int programLineNumber, string comment = null)
        {
            CheckOutOfRange();
            StringBuilder str = new StringBuilder();

            str.Append("\tprogram[" + programLineNumber.ToString() + "] = EosPatternStep(");
            str.Append(ShutterSpeeds[this._shutterIndex].Code.ToString() + ", ");
            str.Append(ShutterSpeeds[this._shutterIndex].Value.ToString("F3") + ", ");
            str.Append(Apertures[this._fStopIndex].Code.ToString() + ", ");
            str.Append(Apertures[this._fStopIndex].Value.ToString() + ", ");
            str.Append(this._wheelTurn.ToString() + ", ");
            str.Append("'" + this._colorIndex.ToString().ToLower().Substring(0,1) + "'");
            str.Append(");");
            
                //ShutterSpeeds[this._shutterIndex].DisplayValue + ", " + Apertures[this._fStopIndex].DisplayValue + ", " + this._colorIndex.ToString();

            if (comment != null && comment.Length > 0)
            {
                str.Append(" //" + comment);
            }
            writer.WriteLine(str.ToString());
        }

        private void WriteLpLine(TextWriter writer, string comment = null)
        {
            CheckOutOfRange();
            string str = ShutterSpeeds[this._shutterIndex].DisplayValue + " sec, " + Apertures[this._fStopIndex].DisplayValue + "f, " + this._colorIndex.ToString();

            if (comment != null && comment.Length > 0)
            {
                str = str + " //" + comment;
            }
            writer.WriteLine(str);
        }

        private void WriteXmlLine(TextWriter writer, int programLineNumber, string codes, string comment = null)
        {
            CheckOutOfRange();
            StringBuilder str = new StringBuilder();

            str.AppendLine("\t<Entry Number='" + (programLineNumber).ToString() + "'>");
            str.AppendLine("\t\t<Shutter>" + ShutterSpeeds[this._shutterIndex].Value.ToString("F2") + "</Shutter>");
            str.AppendLine("\t\t<Aperture>f/" + Apertures[this._fStopIndex].Value.ToString("F1") + "</Aperture>");
            str.AppendLine("\t\t<Color>" + this._colorIndex.ToString() + "</Color>");
            str.AppendLine("\t\t<Code>" + codes + "</Code>");
            str.AppendLine("\t\t<Comment>" + comment + "</Comment>");
            str.AppendLine("\t</Entry>");

            writer.Write(str.ToString());
        }

    }
}

