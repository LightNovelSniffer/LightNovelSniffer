using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LightNovelSniffer.Exception;

namespace LightNovelSniffer.Web.Parser
{
    public class ParserFactory
    {
        private static WuxiaworldParser wwParser;
        private static XiaowazParser xiaowasParser;
        private static GravityTaleParser gravityTaleParser;
        private List<IParser> _availableParsers;

        public ParserFactory()
        {
            _availableParsers = new List<IParser> { GetWuxiaworldParser(), GetXiaowazParser(), GetGravityTaleParser() };
        }

        public ICollection<IParser> AvailableParsers
        {
            get { return _availableParsers.AsReadOnly(); }
        }

        public void RegisterParser(IParser parser)
        {
            _availableParsers.Add(parser);
        }

        /// <summary>
        /// Register the parser in this factory
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly containing parsers</param>
        /// <param name="parserClass">Parser (with namespace) to register</param>
        public void RegisterParserFromAssemblyDllAndClass(string assemblyPath, string parserClass)
        {
            if (!File.Exists(assemblyPath))
                throw new DynamicParserException(string.Format("Cannot find assembly {0}", assemblyPath));

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            if (assembly == null)
                throw new DynamicParserException(string.Format("Cannot load assembly {0}", assemblyPath));

            object instance = assembly.CreateInstance(parserClass);
            IParser parser = instance as IParser;
            if (parser != null)
                RegisterParser(parser);
            else
                throw new DynamicParserException(string.Format("Cannot instantiate {0} as a IParser", parserClass));
        }

        /// <summary>
        /// Register every class implementing IParser in the specified assembly
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly containing parsers</param>
        public void RegisterAllParserFromAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                throw new DynamicParserException(string.Format("Cannot find assembly {0}", assemblyPath));

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            if (assembly == null)
                throw new DynamicParserException(string.Format("Cannot load assembly {0}", assemblyPath));

            
            IEnumerable<Type> types = assembly.GetExportedTypes().Where(x => x.IsClass && x.IsPublic && x.GetInterfaces().Contains(typeof(IParser)));

            foreach (Type type in types)
            {
                object instance = Activator.CreateInstance(type);
                IParser parser = instance as IParser;
                if (parser != null)
                    RegisterParser(parser);
                else
                    throw new DynamicParserException(string.Format("Cannot instantiate {0} as a IParser", type));
            }
        }

        public IParser GetParser(string baseUrl)
        {
            return AvailableParsers.FirstOrDefault(parser => parser.CanParse(baseUrl));
        }

        public static WuxiaworldParser GetWuxiaworldParser()
        {
            return wwParser ?? (wwParser = new WuxiaworldParser());
        }

        public static XiaowazParser GetXiaowazParser()
        {
            return xiaowasParser ?? (xiaowasParser = new XiaowazParser());
        }

        public static GravityTaleParser GetGravityTaleParser()
        {
            return gravityTaleParser ?? (gravityTaleParser = new GravityTaleParser());
        }
    }
}