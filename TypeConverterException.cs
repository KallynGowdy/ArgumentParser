﻿// Copyright 2013 Kallyn Gowdy
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
    /// Defines an exception that signifies that a converter could not be found for a type.
    /// </summary>
    public class TypeConverterException : Exception
    {
        public TypeConverterException(string message) : base(message) { }
        public TypeConverterException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Defines an exception that signifies that a certian type was invalid for a task.
    /// </summary>
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message) : base(message) { }
        public InvalidTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
