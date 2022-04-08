using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePlayerUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            HandleInput();
            Console.ReadLine();
        }
        static bool HandleInput()  //Returns true if all input is valid, otherwise returns false
        {
            //Check and assign input arguments
            Console.WriteLine("Console Player Util");
            Console.WriteLine(choiceMultiple("Play files from [C]urrent folder | Play files from [D]irectory", new char[] { 'C', 'D' }));

            return true;
        }
        static int choiceMultiple(string question, char[] selectableCharacters)
        {
            int selection = -1;
            for(int i = 0; i < selectableCharacters.Length; i++)
            {
                selectableCharacters[i] = Convert.ToChar(selectableCharacters[i].ToString().ToLower());
            }
            while (true)
            {
                Console.Write(question + " > ");
                string answer = Console.ReadLine().ToLower();
                if (answer.Length != 1 || !selectableCharacters.Contains(answer[0]))
                {
                    Console.WriteLine("Error: Invalid input detected. Please enter valid character");
                    continue;
                }
                selection = Array.FindIndex(selectableCharacters, row => row == answer[0]);
                break;
            }
            return selection;
        }
        static bool choiceYesNo(string question)
        {
            bool answer;
            string choice;
            while (true)
            {
                Console.Write(question + " [Y/N] ");
                choice = Console.ReadLine().ToLower();
                if (choice == "y")
                {
                    answer = true;
                }
                else if (choice == "n")
                {
                    answer = false;
                }
                else
                {
                    Console.WriteLine("Error: Invalid input detected. Please enter valid character");
                    continue;
                }
                break;
            }
            return answer;
        }
        static char characterGrayScale(int grayScale)
        {
            string grayscaleMap = "@%#&+=-:.";
            //string grayscaleMap = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'.";
            //string grayscaleMap = "@#W9Ebe4ozu=>+r*;~,.";
            grayScale = Remap(grayScale, 0, 255, grayscaleMap.Length - 1, 0);   //flips the input from ascending to descending to fit the string
            return grayscaleMap[grayScale];
        }
        static int Remap(int value, int from1, int to1, int from2, int to2)
        {
            return Convert.ToInt32((double)(value - from1) / (double)(to1 - from1) * (double)(to2 - from2) + (double)from2);
        }
    }
}
