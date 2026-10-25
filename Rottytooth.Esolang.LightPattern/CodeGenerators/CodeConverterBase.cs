using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rottytooth.Esolang.LightPattern.Tokens;

namespace Rottytooth.Esolang.LightPattern
{
    public abstract class CodeConverterBase : ICodeConverter
    {
        protected string BeginProgram { get; set; }
        protected string EndProgram { get; set; }
        protected string PauseProgram { get; set; }

        protected string DeclareInt { get; set; }
        protected string DeclareFloat { get; set; }
        protected string DeclareChar { get; set; }
        protected string DeclareString { get; set; }

        protected string WriteToScreenBegin { get; set; }
        protected string WriteToScreenEnd { get; set; }

        public string Convert(List<Token> tokenList, string programName, bool includePause)
        {
            StringBuilder retProgram = new StringBuilder();
            GetCommandSet(tokenList, retProgram);
            retProgram.Insert(0, String.Format(this.BeginProgram, programName));

            if (includePause)
                retProgram.Append(this.PauseProgram); // FIXME: temp

            retProgram.Append(this.EndProgram);
            return retProgram.ToString();
        }

        public void GetCommandSet(List<Token> tokenList, StringBuilder retProgram)
        {
            foreach (Token token in tokenList)
            {
                if (!(token is CommandToken))
                {
                    throw new ApplicationException("Command Expected");
                }
                CommandToken token2 = (CommandToken)token;
                string tokenName = token2.TokenName;
                if (tokenName != null)
                {
                    if (!(tokenName == "Let"))
                    {
                        if (tokenName == "Declare")
                        {
                            switch (token2.Variable.Type)
                            {
                                case LpType.Int:
                                    retProgram.Append(this.DeclareInt + " ");
                                    break;

                                case LpType.Float:
                                    retProgram.Append(this.DeclareFloat + " ");
                                    break;

                                case LpType.Char:
                                    retProgram.Append(this.DeclareChar + " ");
                                    break;

                                case LpType.String:
                                    retProgram.Append(this.DeclareString + " ");
                                    break;
                            }
                            retProgram.Append(token2.Variable.VariableName + ";\n");
                            continue;
                        }
                        if (tokenName == "BeginWhile")
                        {
                            retProgram.Append("\nwhile (");
                            GetExpression(token2.Expressions, retProgram);
                            retProgram.Append(")\n{\n");
                            GetCommandSet(token2.Children, retProgram);
                            retProgram.Append("}\n");
                            continue;
                        }
                        if (tokenName == "BeginIf")
                        {
                            retProgram.Append("\nif (");
                            GetExpression(token2.Expressions, retProgram);
                            retProgram.Append(")\n{\n");
                            GetCommandSet(token2.Children, retProgram);
                            retProgram.Append("}\n");
                            if (((IfToken)token2).Else.Count > 0)
                            {
                                retProgram.Append("\nelse {\n");
                                GetCommandSet(((IfToken)token2).Else, retProgram);
                                retProgram.Append("\n}\n");
                            }
                            continue;
                        }
                        if (tokenName == "PrintScreen")
                        {
                            retProgram.Append(WriteToScreenBegin);
                            GetExpression(token2.Expressions, retProgram);
                            retProgram.Append(WriteToScreenEnd + "\n");
                        }
                    }
                    else
                    {
                        retProgram.Append(token2.Variable.VariableName + " = ");
                        GetExpression(token2.Expressions, retProgram);
                        retProgram.Append(";\n");
                    }
                }
                continue;
            }
        }

        public void GetExpression(List<ExpressionToken> tokenList, StringBuilder retProgram)
        {
            foreach (Token token in tokenList)
            {
                char ch;
                string str2;
                int num2;
                if (!(token is ExpressionToken))
                {
                    throw new ApplicationException("Expression Expected");
                }
                ExpressionToken token2 = (ExpressionToken)token;
                switch (token2.TokenName)
                {
                    case "BeginString":
                        retProgram.Append("\"");
                        str2 = token2.Value.ToString();
                        num2 = 0;
                        goto Label_0251;

                    case "char":
                        retProgram.Append("'");
                        if (!char.IsControl((char)token2.Value))
                        {
                            if (((char)token2.Value) == '\'')
                                retProgram.AppendFormat(@"\'", new object[0]);
                            else
                                retProgram.Append((char)token2.Value);
                        }
                        retProgram.AppendFormat(@"\u{0:X4}", System.Convert.ToInt32(token2.Value));
                        retProgram.Append("'");
                        continue;
                    case "+int":
                    case "-int":
                    case "+double":
                    case "-double":
                        {
                            retProgram.Append(token2.Value.ToString());
                            continue;
                        }
                    case "Variable":
                        {
                            retProgram.Append(((VariableToken)token2).VariableName);
                            continue;
                        }
                    case "==":
                    case ">":
                    case "<":
                    case "(":
                    case ")":
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "%":
                        {
                            retProgram.Append(" " + token2.TokenName + " ");
                            continue;
                        }
                    case "NOT":
                        {
                            retProgram.Append(" !");
                            continue;
                        }
                    case "AND":
                        {
                            retProgram.Append(" && ");
                            continue;
                        }
                    case "OR":
                        {
                            retProgram.Append(" || ");
                            continue;
                        }
                    default:
                        {
                            continue;
                        }
                }
            Label_01EC:
                ch = str2[num2];
                if (char.IsControl(ch))
                {
                    retProgram.AppendFormat(@"\u{0:X4}", ch);
                }
                else if (ch == '\\')
                {
                    retProgram.Append(@"\\");
                }
                else if (ch == '"')
                {
                    retProgram.AppendFormat("\\\"", new object[0]);
                }
                else
                {
                    retProgram.Append(ch);
                }
                num2++;
            Label_0251:
                if (num2 < str2.Length)
                {
                    goto Label_01EC;
                }
                retProgram.Append("\"");
                continue;
            }
        }
    }
}
