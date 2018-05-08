using System;
using System.Collections.Generic;

namespace Vytals.Services.QuestionStores
{
    public interface IQuestionStores
    {
        bool Level(int level);
        bool MathType(string mathType);
        List<object> GennerateAnwersQuestion();

    }
}
