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
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace KallynGowdy.ArgumentParser
{
    /// <summary>
    /// Defines a positional argument that is a string.
    /// </summary>
    public class StringedPositionalArgument : PositionalArgument<string>
    {
        public StringedPositionalArgument() : base() { }
    }

    /// <summary>
    /// Defines a class that implements NamedArgument(string)
    /// </summary>
    public class StringedNamedArgument : NamedArgument<string>
    {
        /// <summary>
        /// Creates a new Argument object with the given definition.
        /// </summary>
        /// <param name="definition"></param>
        public StringedNamedArgument(string definition) : base(definition) { }

        /// <summary>
        /// Creates a new Argument object with the given help text and definitions.
        /// </summary>
        /// <param name="helpText">The text for this argument used to display help information when the user passes invalid arguments.</param>
        /// <param name="definitions">The different possible ways that the user could specify this argument in the command-line.</param>
        public StringedNamedArgument(string helpText, params string[] definitions) : base(helpText, definitions) { }

        public StringedNamedArgument() : base() { }
    }

    /// <summary>
    /// Defines an argument that is identified by it's position.
    /// </summary>
    /// <typeparam name="T">The type of value that the argument accepts as input.</typeparam>
    public class PositionalArgument<T> : IPositionalArgument<T>
    {
        /// <summary>
        /// The zero-based index of the position that the argument is in.
        /// </summary>
        public int Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the value.
        /// </summary>
        public string ValueName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bit of text that is provided as an explanantion of the argument's purpose.
        /// </summary>
        public string HelpText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this argument is required (Must be provided).
        /// </summary>
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default value for this argument if it is not provided.
        /// </summary>
        public T DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Predicate that, provided the passed value, determines if it is a valid argument.
        /// </summary>
        public Predicate<T> OnMatch
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the Predicate that, provided the passed value, determines if it is a valid argument.
        /// </summary>
        Predicate<object> IArgument.OnMatch
        {
            set
            {
                OnMatch = new Predicate<T>(value as Predicate<T>);
            }
        }

        /// <summary>
        /// Gets the Name of the argument.
        /// </summary>
        public string Name
        {
            get { return ValueName; }
        }

        TypeConverter converter;

        /// <summary>
        /// Creates a new Positional argument that is used to accept arguments(parameters) from the user based on it's location relative to other arguments.
        /// </summary>
        public PositionalArgument()
        {
            converter = TypeDescriptor.GetConverter(typeof(T));
            if (!converter.CanConvertFrom(typeof(string)))
            {
                string TName = typeof(T).Name;
                throw new InvalidTypeException(TName + " is an invalid type.", new TypeConverterException("Could not convert to " + TName + " from System.String"));
            }
            DefaultValue = default(T);
            Required = false;
            HelpText = string.Empty;
            ValueName = string.Empty;
            Position = 0;
        }

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns defaultValue.
        /// </summary>
        /// <param name="argumentStr">A string that contains only the positional arguments from the command line</param>
        /// <returns></returns>
        public T GetValue(string argumentStr)
        {
            //format input string
            string[] splits = argumentStr.Split(' ').Where(a => !string.IsNullOrEmpty(a)).ToArray();

            //access the value by position
            if (Position < splits.Length)
            {
                try
                {
                    //get the last element, which by position has the given value
                    T value = (T)converter.ConvertFrom(splits[Position]);

                    if (OnMatch != null)
                    {
                        //if invalid input
                        if (!OnMatch.Invoke(value))
                        {
                            //return not passed
                            return DefaultValue;
                        }
                    }
                    return value;
                }
                catch (NotSupportedException)
                {
                    return DefaultValue;
                }
            }
            else
            {
                return DefaultValue;
            }
        }

        /// <summary>
        /// Gets the full help string for this argument.
        /// </summary>
        /// <param name="lengthToColumn">The length in characters from the start of the line to the help text.</param>
        /// <returns></returns>
        public string GetHelpString(int lengthToColumn)
        {
            StringBuilder helpString = new StringBuilder();

            if (string.IsNullOrEmpty(ValueName))
            {
                helpString.AppendFormat("Position: {0}", (Position + 1));
            }
            else
            {
                helpString.AppendFormat("Position: {0} [{1}]", (Position + 1), ValueName);
            }

            while (helpString.Length < lengthToColumn)
            {
                helpString.Append(' ');
            }

            helpString.AppendFormat(" {0}", HelpText);
            return helpString.ToString();
        }

        /// <summary>
        /// Gets the full help string for this argument with a default length to the column as 30 spaces.
        /// </summary>
        /// <returns></returns>
        public string GetHelpString()
        {
            return GetHelpString(30);
        }

        /// <summary>
        /// Gets the default value if the agument is not provided.
        /// </summary>
        object IArgument.DefaultValue
        {
            get { return DefaultValue; }
        }

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns defaultValue.
        /// </summary>
        /// <param name="argumentStr">A string that contains only the positional arguments from the command line</param>
        /// <returns></returns>
        object IArgument.GetValue(string argumentStr)
        {
            return GetValue(argumentStr);
        }
    }

    /// <summary>
    /// Defines an argument that is identified by it's name.
    /// </summary>
    /// <typeparam name="T">The type of value that the argument accepts as input.(bool if the argument is "true" or "false", etc.)</typeparam>
    public class NamedArgument<T> : INamedArgument<T>
    {
        /// <summary>
        /// The text used to describe the operation that the argument specifies.
        /// </summary>
        public string HelpText
        {
            get;
            set;
        }

        /// <summary>
        /// The different possible definitions for passing this argument in the command line. Should include
        /// basic nuances like the dash or forward-slash if those are required.
        /// </summary>
        public string[] Definitions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this Argument required.
        /// </summary>
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if the argument accepts a value as a parameter.
        /// </summary>
        public bool HasValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the value to pass. Used with the HelpText.
        /// </summary>
        public string ValueName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default value for the argument when there is no given value or when no value is accepted.
        /// (Only provided if the argument is not provided at all)
        /// </summary>
        public T DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the Predicate that, provided the passed value, determines if it is a valid argument. Called when a match to this argument is made.
        /// </summary>
        public Predicate<T> OnMatch
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the Predicate that, provided the passed value, determines if it is a valid argument. Called when a match to this argument is made.
        /// </summary>
        Predicate<object> IArgument.OnMatch
        {
            set
            {
                if (value is Predicate<T>)
                {
                    OnMatch = new Predicate<T>(value as Predicate<T>);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value returned if the argument is specified with no value.
        /// (Provided if the argument is supplied with no value)
        /// </summary>
        public T PassedValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value returned if the argument is specified with no value.
        /// (Provided if the argument is supplied with no value)
        /// </summary>
        object INamedArgument.PassedValue
        {
            get { return PassedValue; }
        }

        TypeConverter converter;

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns defaultValue.
        /// If the argument does not accept a value, returns presentValue if it was passed, otherwise defaultValue.
        /// </summary>
        /// <param name="argumentStr">A string containing the argument from the command line.</param>
        /// <returns></returns>
        public T GetValue(string argumentStr)
        {
            StringBuilder pattern = new StringBuilder();

            //the full pattern should be "\b(def1|def2|def3|...)\s[\w]+\b" or "\b(def1|def2|...)\b"
            pattern.Append(@"(");
            for (int i = 0; i < Definitions.Length; i++)
            {
                pattern.Append(Definitions[i]);
                if ((i + 1) < Definitions.Length)
                {
                    pattern.Append("|");
                }
            }

            //if the argument accepts a value
            if (HasValue)
            {
                pattern.Append(@")\s[\w]+");
            }
            else
            {
                pattern.Append(@")");
            }

            //format the input string(remove all double spaces, ect.)
            string[] splits = argumentStr.Split(' ');
            //remove all empty strings
            splits = splits.Where(a => !string.IsNullOrEmpty(a)).ToArray();

            //clear
            argumentStr = string.Empty;

            //concat with spaces
            foreach (string s in splits)
            {
                argumentStr += " " + s;
            }

            //remove first whitespace char
            argumentStr = argumentStr.Trim();

            Regex reg = new Regex(pattern.ToString());

            Match m = reg.Match(argumentStr);

            if (m.Success)
            {
                if (HasValue)
                {
                    //get and return the value given
                    splits = m.Value.Split(' ');

                    //get the last element, which by position has the given value
                    T value = (T)converter.ConvertFrom(splits.Last());
                    if (OnMatch != null)
                    {
                        //perform the action
                        OnMatch.Invoke(value);
                    }
                    return value;
                }
                else
                {
                    OnMatch.Invoke(PassedValue);
                    //return that the argument was passed
                    return PassedValue;
                }
            }
            else //if no pattern match
            {
                return DefaultValue;
            }

        }

        private void setConverter()
        {
            converter = TypeDescriptor.GetConverter(typeof(T));
            if (!converter.CanConvertFrom(typeof(string)))
            {
                string TName = typeof(T).Name;
                throw new InvalidTypeException(TName + " is an invalid type. It must be able to convert from System.String", new TypeConverterException("Cannot convert to " + TName + " from System.String"));
            }
        }

        /// <summary>
        /// Creates a new Argument object with the given definition.
        /// </summary>
        /// <param name="definition"></param>
        public NamedArgument(string definition)
        {
            setConverter();
            Definitions = new string[] { definition };
            HelpText = string.Empty;
            Required = false;
            DefaultValue = default(T);
            PassedValue = default(T);
            ValueName = string.Empty;
            HasValue = false;
        }

        /// <summary>
        /// Creates a new Argument object with the given help text and definitions.
        /// </summary>
        /// <param name="helpText">The text for this argument used to display help information when the user passes invalid arguments.</param>
        /// <param name="definitions">The different possible ways that the user could specify this argument in the command-line.</param>
        public NamedArgument(string helpText, params string[] definitions)
        {
            setConverter();
            Definitions = definitions;
            HelpText = helpText;
            Required = false;
            DefaultValue = default(T);
            PassedValue = default(T);
            ValueName = string.Empty;
            HasValue = false;
        }

        public NamedArgument()
        {
            setConverter();
            //Set defaults
            Required = false;
            DefaultValue = default(T);
            PassedValue = default(T);
            ValueName = string.Empty;
            HelpText = string.Empty;
            Definitions = new string[] { };
            HasValue = false;
        }

        /// <summary>
        /// Gets the full HelpString for this argument.
        /// </summary>
        /// <param name="lengthToColumn">The length in characters from the start of the line to the help text.</param>
        /// <returns></returns>
        public string GetHelpString(int lengthToColumn)
        {
            StringBuilder helpString = new StringBuilder();

            if (string.IsNullOrEmpty(ValueName))
            {
                helpString.AppendFormat("{0}", Definitions[0]);
            }
            else
            {
                helpString.AppendFormat("{0} [{1}]", Definitions[0], ValueName);
            }



            while (helpString.Length < lengthToColumn)
            {
                helpString.Append(' ');
            }

            helpString.AppendFormat(" {0}", HelpText);
            helpString.AppendLine();
            for (int i = 1; i < Definitions.Length; i++)
            {
                if (string.IsNullOrEmpty(ValueName))
                {
                    helpString.AppendFormat("\t{0}", Definitions[i]);
                    helpString.AppendLine();
                }
                else
                {
                    helpString.AppendFormat("\t{0} [{1}]", Definitions[i], ValueName);
                    helpString.AppendLine();
                }
                helpString.AppendLine();
            }

            return helpString.ToString();
        }

        public string GetHelpString()
        {
            return GetHelpString(30);
        }


        object IArgument.DefaultValue
        {
            get { return DefaultValue; }
        }

        object IArgument.GetValue(string argumentStr)
        {
            return GetValue(argumentStr);
        }

        public string Name
        {
            get { return Definitions[0]; }
        }
    }
}
