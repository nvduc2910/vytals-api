using System;
using System.Collections.Generic;
using Vytals.Models;

namespace Vytals.Services.QuestionStores
{
    public class SubtractionThreeNumberLv4_1617181920QuestionService : IQuestionStores
    {
        List<object> QuestionAndAwsers = new List<object>();

        public SubtractionThreeNumberLv4_1617181920QuestionService()
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
            return "Subtraction".Equals(mathType);
        }

        public void RandomQuestion()
        {
            Random rd = new Random();
            for (int i = 0; i < 100; i++)
            {

                var firstNumber = rd.Next(7, 15);
                var secondNumber = rd.Next(7, 15);
                var thirdNumer = rd.Next(7, 15);

                var bigNumber = firstNumber + secondNumber + thirdNumer;


                var positionCorrectAnwer = rd.Next(1, 4);

                if (positionCorrectAnwer == 1)
                {

                    List<int> answers = new List<int>();

                    while (answers.Count < 2)
                    {
                        int rnd = rd.Next(1, firstNumber + 4);
                        if (rnd != firstNumber && !answers.Contains(rnd)) answers.Add(rnd);
                    }
                    var firstAnwer = answers[0];
                    var secondAnwer = answers[1];


                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = bigNumber,
                        SecondNumber = secondNumber,
                        ThreeNumber = thirdNumer,

                        Result1 = firstNumber,
                        Result2 = firstAnwer,
                        Result3 = secondAnwer,

                        ResultTrue = thirdNumer,
                    });
                }
                else if (positionCorrectAnwer == 2)
                {

                    List<int> answers = new List<int>();

                    while (answers.Count < 2)
                    {
                        int rnd = rd.Next(1, secondNumber + 4);
                        if (rnd != secondNumber && !answers.Contains(rnd)) answers.Add(rnd);
                    }
                    var firstAnwer = answers[0];
                    var thirdAnswer = answers[1];



                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = bigNumber,
                        SecondNumber = firstNumber,
                        ThreeNumber = thirdNumer,

                        Result1 = firstAnwer,
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
                        int rnd = rd.Next(1, thirdNumer + 4);
                        if (rnd != thirdNumer && !answers.Contains(rnd)) answers.Add(rnd);
                    }
                    var firstAnwer = answers[0];
                    var secondAnswer = answers[1];


                    QuestionAndAwsers.Add(new AdditionThreeNumber()
                    {
                        FristNumber = bigNumber,
                        SecondNumber = firstNumber,
                        ThreeNumber = secondNumber,

                        Result1 = firstAnwer,
                        Result2 = secondAnswer,
                        Result3 = thirdNumer,


                        ResultTrue = thirdNumer,
                    });
                }
            }
        }
    }
}
