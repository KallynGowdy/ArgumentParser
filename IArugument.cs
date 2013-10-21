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

namespace KallynGowdy
{
    /// <summary>
    /// Defines an interface for an argument that is identified by it's name.
    /// </summary>
    /// <typeparam name="T">The type of value that the argument accepts as input.</typeparam>
    public interface INamedArgument<T> : INamedArgument
    {
        /// <summary>
        /// Gets the value returned if the argument is specified with no value.
        /// (Provided if the argument is supplied with no value)
        /// </summary>
        new T PassedValue
        {
            get;
        }

        /// <summary>
        /// The default value for the argument when there is no given value or when no value is accepted.
        /// </summary>
        new T DefaultValue
        {
            get;
        }

        /// <summary>
        /// Sets the action to be performed when the value has been matched through GetValue(string).
        /// It is a Predicate that, provided the passed value, determines if it is a valid argument. Called when a match to this argument is made.
        /// </summary>
        new Predicate<T> OnMatch
        {
            set;
        }

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns default.
        /// If the argument does not accept a value, returns default if it was passed, otherwise null.
        /// </summary>
        /// <param name="argumentStr">A string containing the argument from the command line.</param>
        /// <returns></returns>
        new T GetValue(string argumentStr);
    }

    /// <summary>
    /// Defines an interface for an argument that is identified by it's position.
    /// </summary>
    /// <typeparam name="T">The type of value that the argument accepts as input.</typeparam>
    public interface IPositionalArgument<T> : IPositionalArgument
    {
        /// <summary>
        /// The default value for the argument when there is no given value or when no value is accepted.
        /// </summary>
        new T DefaultValue
        {
            get;
        }

        /// <summary>
        /// Sets the action to be performed when the value has been matched through GetValue(string).
        /// It is a Predicate that, provided the passed value, determines if it is a valid argument. Called when a match to this argument is made.
        /// </summary>
        new Predicate<T> OnMatch
        {
            set;
        }

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns default.
        /// If the argument does not accept a value, returns default if it was passed, otherwise null.
        /// </summary>
        /// <param name="argumentStr">A string containing the argument from the command line.</param>
        /// <returns></returns>
        new T GetValue(string argumentStr);
    }

    /// <summary>
    /// Defines an interface for an argument.
    /// </summary>
    public interface IArgument
    {
        /// <summary>
        /// The name of the argument used for error description
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// The name of the value to pass. Used with the HelpText.
        /// </summary>
        string ValueName
        {
            get;
        }

        /// <summary>
        /// The text used to describe the operation that the argument specifies.
        /// </summary>
        string HelpText
        {
            get;
        }

        /// <summary>
        /// Is this Argument required?
        /// </summary>
        bool Required
        {
            get;
        }

        /// <summary>
        /// Gets the default value for the argument that is used if the argument is not provided.
        /// </summary>
        object DefaultValue
        {
            get;
        }

        /// <summary>
        /// Sets the action to be performed when the value has been matched through GetValue(string).
        /// It is a Predicate that, provided the passed value, determines if it is a valid argument. Called when a match to this argument is made.
        /// </summary>
        Predicate<object> OnMatch
        {
            set;
        }

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns default.
        /// If the argument does not accept a value, returns default if it was passed, otherwise null.
        /// </summary>
        /// <param name="argumentStr">A string containing the argument from the command line.</param>
        /// <returns></returns>
        object GetValue(string argumentStr);

        /// <summary>
        /// Gets the full HelpString for this argument.
        /// </summary>
        /// <returns></returns>
        string GetHelpString();

        /// <summary>
        /// Gets the full HelpString for this argument.
        /// </summary>
        /// <param name="lengthToColumn">The length in characters from the start of the line to the help text.</param>
        /// <returns></returns>
        string GetHelpString(int lengthToColumn);
    }

    public interface IPositionalArgument : IArgument
    {
        int Position
        {
            get;
        }
    }

    /// <summary>
    /// Defines an interface for a named argument.
    /// </summary>
    public interface INamedArgument : IArgument
    {
        /// <summary>
        /// Gets all of the definitions that can be used to identifiy this argument.
        /// </summary>
        string[] Definitions
        {
            get;
        }

        /// <summary>
        /// Gets whether this argument can be provided a value.
        /// </summary>
        bool HasValue
        {
            get;
        }

        /// <summary>
        /// Gets the value returned if the argument is specified with no value.
        /// (Provided if the argument is supplied with no value)
        /// </summary>
        object PassedValue
        {
            get;
        }
    }

    /// <summary>
    /// Defines an interface for an argument.
    /// </summary>
    /// <typeparam name="T">The type of value that the argument accepts as input.</typeparam>
    public interface IArgument<T> : IArgument
    {
        /// <summary>
        /// The default value for the argument when there is no given value or when no value is accepted.
        /// </summary>
        new T DefaultValue
        {
            get;
        }

        /// <summary>
        /// Sets the Predicate that, provided the passed value, determines if it is a valid argument. Called when a match to this argument is made.
        /// </summary>
        new Predicate<T> OnMatch
        {
            set;
        }

        /// <summary>
        /// Gets the value passed for the argument if a value was passed, otherwise returns default.
        /// If the argument does not accept a value, returns default if it was passed, otherwise null.
        /// </summary>
        /// <param name="argumentStr">A string containing the argument from the command line.</param>
        /// <returns></returns>
        new T GetValue(string argumentStr);
    }
}
