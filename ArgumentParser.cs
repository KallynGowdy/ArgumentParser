// Copyright 2013 Kallyn Gowdy
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KallynGowdy.ArgumentParser
{
    /// <summary>
    /// Defines a parser for command line arguments.
    /// </summary>
    public class ArgumentParser
    {
        /// <summary>
        /// Gets or sets the arguments that can be provided.
        /// </summary>
        public IArgument[] Arguments
        {
            get;
            set;
        }

        private bool namedArgsFirst = true;

        /// <summary>
        /// Gets or sets whether the named arguments should be provided first.
        /// </summary>
        public bool NamedArgsFirst
        {
            get
            {
                return namedArgsFirst;
            }
            set
            {
                namedArgsFirst = value;
            }
        }

        private int maxLineLength = 80;

        /// <summary>
        /// Gets or sets the maximum line length allowed for help strings before they are wrapped.
        /// </summary>
        public int MaxLineLength
        {
            get { return maxLineLength = 80; }
            set { maxLineLength = value; }
        }

        /// <summary>
        /// Gets the name of the program.
        /// </summary>
        public static readonly string ProgramName = Process.GetCurrentProcess().ProcessName;

        /// <summary>
        /// The help argument. When this is defined by the user, the help screen will be shown.
        /// </summary>
        public static readonly NamedArgument<bool> HelpArgument = new NamedArgument<bool>
            {
                Required = false,
                HasValue = false,
                ValueName = string.Empty,
                DefaultValue = false,
                PassedValue = true,
                HelpText = "Shows this help screen.",
                Definitions = new string[]{
                    "-h",
                    "--help",}
            };

        /// <summary>
        /// Matches values to arguments in the array. Returns null and prints help info if a required arg was not passed.
        /// Also returns null and prints help info if "-h" or "--help" was specified.
        /// </summary>
        /// <param name="commandLineArgs">The arguements that was entered into the command line.</param>
        /// <returns></returns>
        public Dictionary<IArgument, object> GetValues(string[] commandLineArgs)
        {
            return GetValues(string.Join(" ", commandLineArgs));
        }

        /// <summary>
        /// Matches values to arguments in the array. Returns null and prints help info if a required arg was not passed.
        /// Also returns null and prints help info if "-h" or "--help" was specified.
        /// </summary>
        /// <param name="commandLineStr">The string that was entered into the command line.</param>
        /// <returns></returns>
        public Dictionary<IArgument, object> GetValues(string commandLineStr)
        {
            if (HelpArgument.GetValue(commandLineStr) == HelpArgument.PassedValue)
            {
                //Show help and exit
                ShowHelpScreen();
                return null;
            }

            Dictionary<IArgument, object> matches = new Dictionary<IArgument, object>();

            //sort the named arguments first
            Arguments = Arguments.OrderBy(a => a is INamedArgument).ToArray();

            //find all of the matches in the string
            foreach (IArgument arg in Arguments)
            {
                object value = null;
                if (arg is INamedArgument)
                {
                    value = arg.GetValue(commandLineStr);
                }
                else if(arg is IPositionalArgument)
                {
                    string argStr = removeNamedArgs(commandLineStr);
                    value = arg.GetValue(argStr);
                }
                //add the value to the list of matches if valid
                if (value != null && !value.Equals(arg.DefaultValue))
                {
                    matches.Add(arg, value);
                }
                    //otherwise show help and return null
                else if (arg.Required)
                {
                    ShowHelpScreen(arg.Name + " is required to be passed.");
                    return null;
                }
            }
            return matches;
        }

        private string removeNamedArgs(string line)
        {
            line = StringUtil.FormatSingleSpace(line);
            foreach (INamedArgument arg in Arguments.Where(a => a is INamedArgument))
            {
                string pattern = @"(";
                for (int i = 0; i < arg.Definitions.Length; i++)
                {
                    pattern += arg.Definitions[i];
                    if ((i + 1) < arg.Definitions.Length)
                    {
                        pattern += "|";
                    }
                }
                if (arg.HasValue)
                {
                    pattern += @")\s[\w]+";
                }
                else
                {
                    pattern += @")";
                }

                Regex reg = new Regex(pattern);
                Match m = reg.Match(line);
                if (m.Success)
                {
                    line = line.Replace(m.Value, string.Empty);
                }
            }
            return line;
        }

        /// <summary>
        /// Shows the help screen with the given problem description of what was wrong.
        /// </summary>
        /// <param name="problemDescription">A description of what the user did wrong.</param>
        public void ShowHelpScreen(string problemDescription)
        {
            Console.WriteLine("{0}", problemDescription);
            Console.WriteLine();
            ShowHelpScreen();
        }

        /// <summary>
        /// Shows the help screen for the Arguments array.
        /// </summary>
        public void ShowHelpScreen()
        {
            StringBuilder helpText = new StringBuilder();

            //Add usage header
            helpText.Append("Usage: ");

            //Add usage arguments
            if (NamedArgsFirst)
            {
                //required named arguments
                foreach (INamedArgument nArg in Arguments.Where(a => a is INamedArgument && a.Required))
                {
                    helpText.AppendFormat("{0} ", nArg.Definitions[0]);
                }

                //aditional named args

                helpText.AppendFormat("[Options] ");
            }

            //positional args
            foreach (IPositionalArgument pArg in Arguments.Where(a => a is IPositionalArgument).OrderBy(a => !a.Required))
            {
                //surround positional argument with '<' '>' if it isn't required.
                if (!pArg.Required)
                {
                    helpText.AppendFormat("<{0}> ", pArg.ValueName);
                }
                else
                {
                    helpText.AppendFormat("{0} ", pArg.ValueName);
                }
            }
            if (!NamedArgsFirst)
            {
                helpText.Append(" [Options]");

                //required named arguments
                foreach (INamedArgument nArg in Arguments.Where(a => a is INamedArgument && a.Required))
                {
                    helpText.AppendFormat(" {0}", nArg.Definitions[0]);
                }
            }
            
            //Add whitespace and options header
            helpText.AppendLine("\n\nOptions:");


            //Add options
            foreach (IArgument arg in Arguments.Where(a => a is INamedArgument))
            {
                string txt = arg.GetHelpString();
                helpText.AppendFormat("\t{0}\n", txt);
            }
            IEnumerable<IArgument> positionalArgs = Arguments.Where(a => a is IPositionalArgument);
            
            if (positionalArgs.Count() > 0)
            {
                helpText.AppendLine("\n\nPositional Options:\n");
                foreach (IArgument arg in positionalArgs)
                {
                    helpText.AppendFormat("\t{0}\n", arg.GetHelpString());
                }
            }

            helpText.AppendLine();

            Console.Write(helpText.ToString());
        }
    }
}
