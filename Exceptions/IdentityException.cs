﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.Exceptions
{
    public class IdentityException : Exception
    {
        public IdentityException(string message) : base(message)
        {

        }
    }
}
