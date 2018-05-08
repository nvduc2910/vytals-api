using System;
using System.Collections.Generic;
using Vytals.Models;

namespace Vytals.Services.QuestionStores
{
    public class DivisionThreeNumberLv4_1617181920QuestionService : IQuestionStores
    {
        List<object> QuestionAndAwsers = new List<object>();

        public DivisionThreeNumberLv4_1617181920QuestionService()
        {
           
        }

        public List<object> GennerateAnwersQuestion()
        {
            RandomQuestion();
            return QuestionAndAwsers;
        }

        public bool Level(int level)
        {
            return 16 == level || 17 == level || 18 == level || 19 == level || 20 == level;
        }

        public bool MathType(string mathType)
        {
            return "Division".Equals(mathType);
        }

        public void RandomQuestion()
        {
            Random rd = new Random();

            for (int i = 0; i < 50; i++)
            {
                var firstNumber = rd.Next(4, 7);
                var secondNumber = rd.Next(4, 7);
                var thirdNumber = rd.Next(4, 7);

                var bigestFirtsNumber = firstNumber * secondNumber * thirdNumber;

                var positionCorrectAnwer = rd.Next(1, 4);

                if (positionCorrectAnwer == 1) // that mean true answer will be in first result
                {
                    List<int> answers = new List<int>();
                    while (answers.Count < 2)
                    {
                        int rnd = rd.Next(4, 7);
                        if (rnd != firstNumber && !answers.Contains(rnd)) answers.Add(rnd);
                    }
                    var secondAnswer = answers[0];
                    var thirdAnswer = answers[1];

                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = bigestFirtsNumber,
                        SecondNumber = secondNumber,
                        ThreeNumber = thirdNumber,

                        Result1 = firstNumber,
                        Result2 = secondAnswer,
                        Result3 = thirdAnswer,

                        ResultTrue = firstNumber,
                    });
                }
                else if (positionCorrectAnwer == 2)
                {
                    List<int> answers = new List<int>();
                    while (answers.Count < 2)
                    {
                        int rnd = rd.Next(4, 7);
                        if (rnd != secondNumber && !answers.Contains(rnd)) answers.Add(rnd);
                    }
                    var firstAnswer = answers[0];
                    var thirdAnswer = answers[1];


                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = bigestFirtsNumber,
                        SecondNumber = firstNumber,
                        ThreeNumber = thirdNumber,

                        Result1 = firstAnswer,
                        Result2 = secondNumber,
                        Result3 = thirdAnswer,

                        ResultTrue = secondNumber,
                    });
                }
                else if (positionCorrectAnwer == 3)
                {
                    List<int> answers = new List<int>();
                    while (answers.Count < 2)
                    {
                        int rnd = rd.Next(4, 7);
                        if (rnd != thirdNumber && !answers.Contains(rnd)) answers.Add(rnd);
                    }
                    var firstAnswer = answers[0];
                    var secondAnser = answers[1];


                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = bigestFirtsNumber,
                        SecondNumber = firstNumber,
                        ThreeNumber = secondNumber,

                        Result1 = firstAnswer,
                        Result2 = secondAnser,
                        Result3 = thirdNumber,


                        ResultTrue = thirdNumber,
                    });
                }
            }
        }
    }
}
