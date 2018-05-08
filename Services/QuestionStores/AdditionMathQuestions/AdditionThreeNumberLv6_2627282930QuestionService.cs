using System;
using System.Collections.Generic;
using Vytals.Models;

namespace Vytals.Services.QuestionStores
{
    public class AdditionThreeNumberLv6_2627282930QuestionService : IQuestionStores
    {
        List<object> QuestionAndAwsers = new List<object>();

        public AdditionThreeNumberLv6_2627282930QuestionService()
        {
            
        }

        public List<object> GennerateAnwersQuestion()
        {
            RandomQuestion();

            return QuestionAndAwsers;
        }

        public bool Level(int level)
        {
            return 30 == level || 26 == level || 27 == level || 28 == level || 29 == level;
        }

        public bool MathType(string mathType)
        {
            return "Addition".Equals(mathType);
        }


        public void RandomQuestion()
        {
            Random rd = new Random();
            for (int i = 0; i < 100; i ++)
            {
                var firstNumber = rd.Next(15, 25);
                var secondNumber = rd.Next(15, 25);
                var thirdNumber = rd.Next(15, 25);

                var trueAnwer = firstNumber + secondNumber + thirdNumber;

                List<int> answers = new List<int>();
                while (answers.Count < 2)
                {
                    int rnd = rd.Next(45, 75);
                    if (rnd != trueAnwer && !answers.Contains(rnd)) answers.Add(rnd);
                }

                var firstAnwer = answers[0];
                var secondAnwer = answers[1];
                
                var positionCorrectAnwer = rd.Next(1, 4);

                if(positionCorrectAnwer == 1)
                {
                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = firstNumber,
                        SecondNumber = secondNumber,
                        ThreeNumber = thirdNumber,
                        Result1 = trueAnwer,
                        Result2 = secondAnwer,
                        Result3 = firstAnwer,
                        ResultTrue = trueAnwer,
                    });
                }
                else if(positionCorrectAnwer == 2)
                {
                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = firstNumber,
                        SecondNumber = secondNumber,
                        ThreeNumber = thirdNumber,
                        Result1 = firstAnwer,
                        Result2 = trueAnwer,
                        Result3 = secondAnwer,
                        ResultTrue = trueAnwer,
                    });
                }
                else if(positionCorrectAnwer == 3)
                {
                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = firstNumber,
                        SecondNumber = secondNumber,
                        ThreeNumber = thirdNumber,
                        Result1 = firstAnwer,
                        Result2 = secondAnwer,
                        Result3 = trueAnwer,
                        ResultTrue = trueAnwer,
                    });
                }
            }
        }
    }
}
