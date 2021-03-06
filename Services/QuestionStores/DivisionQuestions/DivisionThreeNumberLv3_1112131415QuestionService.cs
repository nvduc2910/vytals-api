﻿using System;
using System.Collections.Generic;
using Vytals.Models;

namespace Vytals.Services.QuestionStores
{
    public class DivisionThreeNumberLv3_1112131415QuestionService : IQuestionStores
    {
        List<object> QuestionAndAwsers = new List<object>();

        public DivisionThreeNumberLv3_1112131415QuestionService()
        {
           
        }

        public List<object> GennerateAnwersQuestion()
        {
            RandomQuestion();
            return QuestionAndAwsers;
        }

        public bool Level(int level)
        {
            return 11 == level || 12 == level || 13 == level || 14 == level || 15 == level;
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
                var firstNumber = rd.Next(3, 6);
                var secondNumber = rd.Next(3, 6);
                var thirdNumber = rd.Next(3, 6);

                var bigestFirtsNumber = firstNumber * secondNumber * thirdNumber;

                var positionCorrectAnwer = rd.Next(1, 4);

                if (positionCorrectAnwer == 1) // that mean true answer will be in first result
                {
                    List<int> answers = new List<int>();
                    while (answers.Count < 2)
                    {
                        int rnd = rd.Next(3, 6);
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
                        int rnd = rd.Next(3, 6);
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
                        int rnd = rd.Next(3, 6);
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
