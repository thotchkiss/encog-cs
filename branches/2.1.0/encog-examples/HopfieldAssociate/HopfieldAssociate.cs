﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Pattern;

namespace HopfieldAssociate
{
    /**
 * Simple class to recognize some patterns with a Hopfield Neural Network.
 * This is very loosely based on a an example by Karsten Kutza, 
 * written in C on 1996-01-30.
 * http://www.neural-networks-at-your-fingertips.com/hopfield.html
 * 
 * I translated it to Java and adapted it to use Encog for neural
 * network processing.  I mainly kept the patterns from the 
 * original example.
 *
 */
    public class HopfieldAssociate
    {

        public const int HEIGHT = 10;
        public const int WIDTH = 10;

        /**
         * The neural network will learn these patterns.
         */
        public String[][] PATTERN = { 
        new String[WIDTH] { 
		"O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O"  },

      new String[WIDTH] { 
        "OO  OO  OO",
        "OO  OO  OO",
        "  OO  OO  ",
        "  OO  OO  ",
        "OO  OO  OO",
        "OO  OO  OO",
        "  OO  OO  ",
        "  OO  OO  ",
        "OO  OO  OO",
        "OO  OO  OO"  },

      new String[WIDTH]  { 
        "OOOOO     ",
        "OOOOO     ",
        "OOOOO     ",
        "OOOOO     ",
        "OOOOO     ",
        "     OOOOO",
        "     OOOOO",
        "     OOOOO",
        "     OOOOO",
        "     OOOOO"  },

      new String[WIDTH] { 
        "O  O  O  O",
        " O  O  O  ",
        "  O  O  O ",
        "O  O  O  O",
        " O  O  O  ",
        "  O  O  O ",
        "O  O  O  O",
        " O  O  O  ",
        "  O  O  O ",
        "O  O  O  O"  },

      new String[WIDTH]  { 
        "OOOOOOOOOO",
        "O        O",
        "O OOOOOO O",
        "O O    O O",
        "O O OO O O",
        "O O OO O O",
        "O O    O O",
        "O OOOOOO O",
        "O        O",
        "OOOOOOOOOO"  } };

        /**
         * The neural network will be tested on these patterns, to see
         * which of the last set they are the closest to.
         */
        public String[][] PATTERN2 = { 
        new String[WIDTH] { 
		"          ",
        "          ",
        "          ",
        "          ",
        "          ",
        " O O O O O",
        "O O O O O ",
        " O O O O O",
        "O O O O O ",
        " O O O O O"  },

        new String[WIDTH] { 
        "OOO O    O",
        " O  OOO OO",
        "  O O OO O",
        " OOO   O  ",
        "OO  O  OOO",
        " O OOO   O",
        "O OO  O  O",
        "   O OOO  ",
        "OO OOO  O ",
        " O  O  OOO"  },

        new String[WIDTH] { 
        "OOOOO     ",
        "O   O OOO ",
        "O   O OOO ",
        "O   O OOO ",
        "OOOOO     ",
        "     OOOOO",
        " OOO O   O",
        " OOO O   O",
        " OOO O   O",
        "     OOOOO"  },

        new String[WIDTH] { 
        "O  OOOO  O",
        "OO  OOOO  ",
        "OOO  OOOO ",
        "OOOO  OOOO",
        " OOOO  OOO",
        "  OOOO  OO",
        "O  OOOO  O",
        "OO  OOOO  ",
        "OOO  OOOO ",
        "OOOO  OOOO"  },

        new String[WIDTH] { 
        "OOOOOOOOOO",
        "O        O",
        "O        O",
        "O        O",
        "O   OO   O",
        "O   OO   O",
        "O        O",
        "O        O",
        "O        O",
        "OOOOOOOOOO"  } };

        public BiPolarNeuralData ConvertPattern(String[][] data, int index)
        {
            int resultIndex = 0;
            BiPolarNeuralData result = new BiPolarNeuralData(WIDTH * HEIGHT);
            for (int row = 0; row < HEIGHT; row++)
            {
                for (int col = 0; col < WIDTH; col++)
                {
                    char ch = data[index][row][col];
                    result.SetBoolean(resultIndex++, ch == 'O');
                }
            }
            return result;
        }

        public void Display(BiPolarNeuralData pattern1, BiPolarNeuralData pattern2)
        {
            int index1 = 0;
            int index2 = 0;

            for (int row = 0; row < HEIGHT; row++)
            {
                StringBuilder line = new StringBuilder();

                for (int col = 0; col < WIDTH; col++)
                {
                    if (pattern1.GetBoolean(index1++))
                        line.Append('O');
                    else
                        line.Append(' ');
                }

                line.Append("   ->   ");

                for (int col = 0; col < WIDTH; col++)
                {
                    if (pattern2.GetBoolean(index2++))
                        line.Append('O');
                    else
                        line.Append(' ');
                }

                Console.WriteLine(line.ToString());
            }
        }


        public void Evaluate(BasicNetwork hopfield, String[][] pattern)
        {
            HopfieldLogic hopfieldLogic = (HopfieldLogic)hopfield.Logic;
            for (int i = 0; i < pattern.Length; i++)
            {
                BiPolarNeuralData pattern1 = ConvertPattern(pattern, i);
                hopfieldLogic.CurrentState = pattern1;
                int cycles = hopfieldLogic.RunUntilStable(100);
                BiPolarNeuralData pattern2 = (BiPolarNeuralData)hopfieldLogic.CurrentState;
                Console.WriteLine("Cycles until stable(max 100): " + cycles + ", result=");
                Display(pattern1, pattern2);
                Console.WriteLine("----------------------");
            }
        }

        public void Run()
        {
            HopfieldPattern pattern = new HopfieldPattern();
            pattern.InputNeurons = WIDTH * HEIGHT;
            BasicNetwork hopfield = pattern.Generate();
            HopfieldLogic hopfieldLogic = (HopfieldLogic)hopfield.Logic;

            for (int i = 0; i < PATTERN.Length; i++)
            {
                hopfieldLogic.AddPattern(ConvertPattern(PATTERN, i));
            }

            Evaluate(hopfield, PATTERN);
            Evaluate(hopfield, PATTERN2);
        }

        static void Main(string[] args)
        {
            HopfieldAssociate program = new HopfieldAssociate();
            program.Run();
        }
    }
}