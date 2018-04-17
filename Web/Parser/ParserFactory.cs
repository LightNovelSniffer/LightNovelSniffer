using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using LightNovelSniffer.Exception;
using LightNovelSniffer.Resources;

namespace LightNovelSniffer.Web.Parser
{
    public class ParserFactory
    {
        private static List<IParser> _registeredParsers;

        public ParserFactory()
        {
            if (_registeredParsers == null)
            {
                _registeredParsers = new List<IParser>();
                RegisterAllParserFromAssembly(GetType().Assembly.Location);//loading of all parsers existing in this project
            }
        }

        public ReadOnlyCollection<IParser> AvailableParsers
        {
            get { return _registeredParsers.AsReadOnly(); }
        }

        public void RegisterParser(IParser parser)
        {
            if (!_registeredParsers.ContainsType(parser.GetType()))
                _registeredParsers.Add(parser);
        }

        /// <summary>
        /// Register the parser in this factory
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly containing parsers</param>
        /// <param name="parserClass">Parser (with namespace) to register</param>
        public void RegisterParserFromAssemblyDllAndClass(string assemblyPath, string parserClass)
        {
            if (!File.Exists(assemblyPath))
                throw new DynamicParserException(string.Format(LightNovelSniffer_Strings.FindAssemblyExceptionMessage, assemblyPath));

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            if (assembly == null)
                throw new DynamicParserException(string.Format(LightNovelSniffer_Strings.LoadAssemblyExceptionMessage, assemblyPath));

            object instance = assembly.CreateInstance(parserClass);
            IParser parser = instance as IParser;
            if (parser == null)
                throw new DynamicParserException(string.Format(LightNovelSniffer_Strings.InstantiateAssemblyExceptionMessage, parserClass));
            RegisterParser(parser);    
        }

        /// <summary>
        /// Register every class implementing IParser in the specified assembly
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly containing parsers</param>
        public void RegisterAllParserFromAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
                throw new DynamicParserException(string.Format(LightNovelSniffer_Strings.FindAssemblyExceptionMessage, assemblyPath));

            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            if (assembly == null)
                throw new DynamicParserException(string.Format(LightNovelSniffer_Strings.LoadAssemblyExceptionMessage, assemblyPath));

            
            IEnumerable<Type> types = assembly.GetExportedTypes().Where(x => x.IsClass && x.IsPublic && x.GetInterfaces().Contains(typeof(IParser)));

            foreach (Type type in types)
            {
                object instance = Activator.CreateInstance(type);
                IParser parser = instance as IParser;
                if (parser == null)
                    throw new DynamicParserException(string.Format(LightNovelSniffer_Strings.InstantiateAssemblyExceptionMessage, type));
                RegisterParser(parser);
            }
        }

        public IParser GetParser(string baseUrl)
        {
            return AvailableParsers.FirstOrDefault(parser => parser.CanParse(baseUrl));
        }
    }
}