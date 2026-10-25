using System;
using System.CodeDom.Compiler;
using System.Text;


namespace Rottytooth.Esolang.LightPattern
{
    public static class ExeGenerator
    {
        public static bool CompileExecutable(string source, string exeName)
        {
            CodeDomProvider provider = null;
            provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters options = new CompilerParameters {
                GenerateExecutable = true,
                OutputAssembly = exeName,
                GenerateInMemory = false,
                TreatWarningsAsErrors = false
            };
            CompilerResults results = provider.CompileAssemblyFromSource(options, new string[] { source });
            if (results.Errors.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    builder.AppendFormat("  {0}", error.ToString());
                }
                throw new ParserError("Error in code generation stage. " + builder.ToString());
            }
            if (results.Errors.Count > 0)
            {
                return false;
            }
            return true;
        }
    }
}

