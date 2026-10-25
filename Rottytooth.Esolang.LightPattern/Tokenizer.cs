using System;
using System.Collections.Generic;
using System.Text;
using Rottytooth.Esolang.LightPattern.Tokens;


namespace Rottytooth.Esolang.LightPattern
{
    public static class Tokenizer
    {
        private static Dictionary<Symbol, string> Commands = new Dictionary<Symbol, string>();
        private static Dictionary<Symbol, string> DoublePhotoExpressions;
        private static Dictionary<Symbol, string> SinglePhotoExpressions;
        private static Dictionary<Symbol, LpType> Types;

        static Tokenizer()
        {
            //FIXME: all this should be moved to a config file

            Commands.Add(new Symbol(2, 0, 0), "Let");
            Commands.Add(new Symbol(2, 0, 1), "Declare");
            Commands.Add(new Symbol(2, 0, 2), "BeginWhile");
            Commands.Add(new Symbol(2, 1, 0), "EndWhile");
            Commands.Add(new Symbol(2, 1, 1), "BeginIf");
            Commands.Add(new Symbol(2, 1, 2), "Else");
            Commands.Add(new Symbol(2, 2, 0), "EndIf");
            Commands.Add(new Symbol(2, 2, 1), "PrintScreen");
            Commands.Add(new Symbol(2, 2, 2), "Reset");

            // new commands
            Commands.Add(new Symbol(0, 0, 1), "Fork");
            Commands.Add(new Symbol(0, 1, 0), "DeclareFunction");
            Commands.Add(new Symbol(0, 1, 1), "EndFunction");
            Commands.Add(new Symbol(0, 2, 1), "CallFunction");

            Commands.Add(new Symbol(0, 1, 2), "NOP");


            Types = new Dictionary<Symbol, LpType>();
            Types.Add(new Symbol(0, 0, 1), LpType.Int);
            Types.Add(new Symbol(0, 1, 0), LpType.Char);
            Types.Add(new Symbol(0, 1, 1), LpType.Float);
            Types.Add(new Symbol(0, 1, 2), LpType.String);

            SinglePhotoExpressions = new Dictionary<Symbol, string>();
            SinglePhotoExpressions.Add(new Symbol(1, 0, 0), "Variable");
            SinglePhotoExpressions.Add(new Symbol(1, 0, 1), "+int");
            SinglePhotoExpressions.Add(new Symbol(1, 0, 2), "-int");
            SinglePhotoExpressions.Add(new Symbol(1, 1, 0), "char");
            SinglePhotoExpressions.Add(new Symbol(1, 1, 1), "+double");
            SinglePhotoExpressions.Add(new Symbol(1, 1, 2), "-double");
            SinglePhotoExpressions.Add(new Symbol(1, 2, 0), "BeginString");
            SinglePhotoExpressions.Add(new Symbol(0, 0, 0), "EndString");
            SinglePhotoExpressions.Add(new Symbol(0, 2, 0), "DoubleIndicator");

            DoublePhotoExpressions = new Dictionary<Symbol, string>();
            DoublePhotoExpressions.Add(new Symbol(0, 0, 0), "==");
            DoublePhotoExpressions.Add(new Symbol(0, 0, 1), ">");
            DoublePhotoExpressions.Add(new Symbol(0, 0, 2), "<");
            DoublePhotoExpressions.Add(new Symbol(0, 1, 0), "NOT");
            DoublePhotoExpressions.Add(new Symbol(0, 1, 1), "AND");
            DoublePhotoExpressions.Add(new Symbol(0, 1, 2), "OR");
            DoublePhotoExpressions.Add(new Symbol(0, 2, 0), "(");
            DoublePhotoExpressions.Add(new Symbol(0, 2, 1), ")");
            DoublePhotoExpressions.Add(new Symbol(0, 2, 2), "+");
            DoublePhotoExpressions.Add(new Symbol(1, 0, 0), "-");
            DoublePhotoExpressions.Add(new Symbol(1, 0, 1), "*");
            DoublePhotoExpressions.Add(new Symbol(1, 0, 2), "/");
            DoublePhotoExpressions.Add(new Symbol(1, 1, 0), "%");

            // new expression
            DoublePhotoExpressions.Add(new Symbol(1, 1, 1), "RAND");
        }

        public static string GetCommands(List<Symbol> program, ref int step, List<Token> tokenizedProgram)
        {
            while (step < program.Count)
            {
                CommandToken token = null;
                if (step > program.Count)
                {
                    throw new ParserError("Unexpected end of program at step " + ((int) step).ToString());
                }
                //if (!SinglePhotoExpressions.ContainsKey(program[step]))
                //{
                //    throw new ParserError(String.Format("Invalid Command: {0} at step {1}", program[step].ToString(), step));
                //}
                string name = Commands[program[step]];
                if (!Commands.ContainsKey(program[step]))
                {
                    throw new ParserError("Syntax error: Command expected in step " + ((int) step).ToString());
                }
                switch (name)
                {
                    case "EndIf":
                    case "EndWhile":
                    case "Else":
                    {
                        string str2 = name;
                        step++;
                        return str2;
                    }
                    case "Let":
                    {
                        token = new CommandToken(name);
                        step++;
                        token.Variable = new VariableToken(name);
                        token.Variable.VariableName = GetVariableName(program, ref step, tokenizedProgram);
                        GetExpression(program, ref step, tokenizedProgram, token.Expressions);
                        tokenizedProgram.Add(token);
                        continue;
                    }
                    case "Declare":
                    {
                        token = new CommandToken(name);
                        step++;
                        token.Variable = new VariableToken(name);
                        token.Variable.VariableName = GetVariableName(program, ref step, tokenizedProgram);
                        token.Variable.Type = GetType(program, ref step, tokenizedProgram);
                        tokenizedProgram.Add(token);
                        continue;
                    }
                    case "BeginWhile":
                        token = new CommandToken(name);
                        step++;
                        GetExpression(program, ref step, tokenizedProgram, token.Expressions);
                        token.Children = new List<Token>();
                        if (GetCommands(program, ref step, token.Children) != "EndWhile")
                        {
                            throw new ParserError("While Loop ended incorrectly at step " + ((int) step).ToString());
                        }
                        break;

                    case "BeginIf":
                    {
                        token = new IfToken(name);
                        step++;
                        GetExpression(program, ref step, tokenizedProgram, token.Expressions);
                        string str4 = GetCommands(program, ref step, token.Children);
                        if (str4 == "Else")
                        {
                            str4 = GetCommands(program, ref step, ((IfToken)token).Else);
                        }
                        if (str4 != "EndIf")
                        {
                            throw new ParserError("If Statement ended incorrectly at step " + ((int) step).ToString());
                        }
                        tokenizedProgram.Add(token);
                        continue;
                    }
                    case "PrintScreen":
                    {
                        token = new CommandToken(name);
                        step++;
                        GetExpression(program, ref step, tokenizedProgram, token.Expressions);
                        tokenizedProgram.Add(token);
                        continue;
                    }
                    case "NOP":
                    {
                        token = new CommandToken(name);
                        step++;
                        tokenizedProgram.Add(token);
                        continue;
                    }
                    case "Reset":
                    {
                        step ++; // FIXME: for .lp files, it only needs to skip 1 step. But from images, does it skip another image?
                        continue;
                    }
                    default:
                        throw new ParserError("Syntax error: Unknown command in step " + ((int) step).ToString());
                }
                //tokenizedProgram.Add(token);
            }
            return null;
        }

        public static void GetExpression(List<Symbol> program, ref int step, List<Token> tokenizedProgram, List<ExpressionToken> expressions)
        {
            while (step < program.Count)
            {
                LiteralToken token;
                StringBuilder builder;
                if (Commands.ContainsKey(program[step]))
                {
                    break;
                }

                if (!SinglePhotoExpressions.ContainsKey(program[step]))
                {
                    throw new ParserError(String.Format("Invalid Expression: {0} at step {1}", program[step].ToString(), step));
                }

                string name = SinglePhotoExpressions[program[step]];
                int length = 0;
                switch (name)
                {
                    case "DoubleIndicator":
                    {
                        step++;
                        name = DoublePhotoExpressions[program[step]];
                        expressions.Add(new ExpressionToken(name));
                        step++;
                        continue;
                    }
                    case "BeginString":
                        token = new LiteralToken(name);
                        step++;
                        builder = new StringBuilder();
                        goto Label_016F;

                    case "Variable":
                    {
                        VariableToken item = new VariableToken(name);
                        step++;
                        item.VariableName = GetVariableName(program, ref step, tokenizedProgram);
                        expressions.Add(item);
                        continue;
                    }
                    case "+int":
                    case "-int":
                    {
                        LiteralToken token3 = new LiteralToken(name);
                        step++;
                        length = GetIntValue(program, ref step, tokenizedProgram);
                        token3.Value = GetIntValues(program, ref step, tokenizedProgram, length);
                        if (name == "-int")
                        {
                            token3.Value = ((int) token3.Value) * -1;
                        }
                        expressions.Add(token3);
                        continue;
                    }
                    case "char":
                    {
                        LiteralToken token4 = new LiteralToken(name);
                        step++;
                        length = GetIntValue(program, ref step, tokenizedProgram);
                        token4.Value = (char) GetIntValues(program, ref step, tokenizedProgram, length);
                        expressions.Add(token4);
                        continue;
                    }
                    case "+double":
                    case "-double":
                    {
                        LiteralToken token5 = new LiteralToken(name);
                        step++;
                        int num2 = GetIntValue(program, ref step, tokenizedProgram);
                        int num3 = GetIntValue(program, ref step, tokenizedProgram);
                        token5.Value = Convert.ToDouble(GetIntValues(program, ref step, tokenizedProgram, num2).ToString() + "." + GetIntValues(program, ref step, tokenizedProgram, num3).ToString());
                        if (name == "-double")
                        {
                            token5.Value = ((double) token5.Value) * -1.0;
                        }
                        expressions.Add(token5);
                        continue;
                    }
                    default:
                        throw new ParserError("Unknown expression encountered at step " + ((int) step).ToString());
                }
            Label_0144:
                length = GetIntValue(program, ref step, tokenizedProgram);
                if (length == 0)
                {
                    goto Label_0174; // FIXME: these labels are wack
                }
                builder.Append((char) GetIntValues(program, ref step, tokenizedProgram, length));
            Label_016F:
                //flag = true;
                goto Label_0144;
            Label_0174:
                token.Value = builder.ToString();
                expressions.Add(token);
            }
        }

        public static int GetIntValue(List<Symbol> program, ref int step, List<Token> tokenizedProgram)
        {
            if (step >= program.Count)
            {
                throw new ParserError("Unexpected end of program at step " + ((int) step).ToString());
            }
            int num = ((program[step].Shutter * 9) + (program[step].FStop * 3)) + program[step].Color;
            step++;
            return num;
        }

        public static int GetIntValues(List<Symbol> program, ref int step, List<Token> tokenizedProgram, int length)
        {
            int num = 0;
            for (int i = 0; i < length; i++)
            {
                num *= 0x1b;
                num += GetIntValue(program, ref step, tokenizedProgram);
            }
            return num;
        }

        public static LpType GetType(List<Symbol> program, ref int step, List<Token> tokenizedProgram)
        {
            LpType type;
            if (step > program.Count)
            {
                throw new ParserError("Unexpected end of program at step " + ((int) step).ToString());
            }
            try
            {
                type = Types[new Symbol(program[step].Shutter, program[step].FStop, program[step].Color)];
            }
            catch (KeyNotFoundException)
            {
                throw new ParserError("invalid type provided");
            }
            step++;
            return type;
        }

        public static string GetVariableName(List<Symbol> program, ref int step, List<Token> tokenizedProgram)
        {
            if (step > program.Count)
            {
                throw new ParserError("Unexpected end of program at step " + ((int) step).ToString());
            }
            string str = "_Var" + program[step].Shutter.ToString() + program[step].FStop.ToString() + program[step].Color.ToString();
            step++;
            return str;
        }

        public static List<Token> Tokenize(List<Symbol> program)
        {
            List<Token> tokenizedProgram = new List<Token>();
            int step = 0;
            GetCommands(program, ref step, tokenizedProgram);
            return tokenizedProgram;
        }
    }
}

