using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vytals.Exceptions
{
    public class CrudFailedException : Exception
    {
        public CrudFailedException(string message) : base(message)
        {

        }
    }
}
