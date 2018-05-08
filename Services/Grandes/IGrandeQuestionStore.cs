using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Vytals.Services.Grandes
{
    public interface IGrandeQuestionStore
    {
        bool Grande(int grande);
        bool MathType(string mathType);
        List<object> GennerateAnwersQuestion();

    }
}
