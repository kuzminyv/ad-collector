using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class RuntimeCompiler
    {
        public CompilerResults Compile(string code)
        {
            Microsoft.CSharp.CSharpCodeProvider provider =
               new CSharpCodeProvider();
            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = true;
            
            compilerparams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerparams.ReferencedAssemblies.Add("System.dll");
            compilerparams.ReferencedAssemblies.Add("System.Xml.dll");
            compilerparams.ReferencedAssemblies.Add("Core.dll");
            compilerparams.ReferencedAssemblies.Add("System.Core.dll");

            return provider.CompileAssemblyFromSource(compilerparams, code);
        }

        private Assembly BuildAssembly(string code)
        {
            CompilerResults results = Compile(code);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n",
                           error.Line, error.Column, error.ErrorText);
                }
                throw new Exception(errors.ToString());
            }
            else
            {
                return results.CompiledAssembly;
            }
        }

        public T CreateInstance<T>(Assembly assembly)
        {
            Type type = typeof(T);
            Type concreteType = assembly.GetTypes().Where(t => type.IsAssignableFrom(t)).Single();
            return (T)assembly.CreateInstance(concreteType.FullName);
        }

        public T CreateInstance<T>(string code)
        {
            return CreateInstance<T>(BuildAssembly(code));
        }
    }
}
