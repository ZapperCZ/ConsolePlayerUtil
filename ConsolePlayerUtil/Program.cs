using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Media;
using System.Text;

namespace ConsolePlayerUtil
{
    class Program
    {
        enum FileExtensions
        {
            jpg,
            png
        }
        static FileExtensions selectedExtension = new FileExtensions();
        static string selectedDirectory;
        static FileInfo[] selectedFiles;
        static SoundPlayer audio = new SoundPlayer();
        static List<Bitmap> SelectedBitmaps = new List<Bitmap>();
        static List<string> PlayableDirectories;
        static void Main(string[] args)
        {
            HandleInput();
            Console.Write("Loading files");
            selectedFiles = new DirectoryInfo(selectedDirectory).GetFiles("*." + selectedExtension.ToString());
            int writeInterval = selectedFiles.Length / 10;
            for (int i = 0; i < selectedFiles.Length; i++)
            {
                Bitmap newBitmap = new Bitmap(selectedFiles[i].FullName);
                SelectedBitmaps.Add(newBitmap);
                if (i % writeInterval == 0)
                    Console.Write(".");
            }
            ImageFormat selectedFormat = ImageFormat.Png;
            switch (selectedExtension)
            {
                case FileExtensions.png:
                    selectedFormat = ImageFormat.Png;
                    break;
                case FileExtensions.jpg:
                    selectedFormat = ImageFormat.Jpeg;
                    break;
            }
            Console.Clear();
            Console.Write("Commencing playback");
            Thread.Sleep(500);
            if (audio.SoundLocation != "")
            {
                PlayBitmaps(SelectedBitmaps, 25, 25, audio);
            }
            else
                PlayBitmaps(SelectedBitmaps, 25, 25);
            Console.ReadLine();
        }
        static bool HandleInput()  //Returns true if all input is valid, otherwise returns false
        {
            audio.SoundLocation = "";
            selectedDirectory = "";
            PlayableDirectories = new List<string>();
            Console.Write("Welcome to Console Player Util\nYou can use arrows for menu orientation and Enter to confirm.\nPress Enter to start");
            Console.ReadLine();
            selectedExtension = (FileExtensions)ArrowChoice("Select file type",new string[] {".jpg",".png"});
            Console.Write(selectedExtension + " selected");
            Thread.Sleep(800);
            while (true)
            {
                PlayableDirectories = new List<string>();
                Console.Clear();
                if (ArrowChoice("Select file location", new string[] { "Current directory", "Custom directory" }) == 0)
                {
                    string currDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    DirectoryInfo[] di = new DirectoryInfo(currDir).GetDirectories("*.*", SearchOption.AllDirectories);
                    foreach (DirectoryInfo d in di)
                    {
                        FileInfo[] currDirFiles = d.GetFiles("*" + selectedExtension.ToString());
                        if (currDirFiles.Length > 0)
                        {
                            PlayableDirectories.Add(d.FullName.Replace(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ""));
                        }
                    }
                    if (PlayableDirectories.Count == 0)
                    {
                        Console.WriteLine("No folder containing the selected file type found.");
                        Thread.Sleep(1000);
                        continue;
                    }
                    selectedDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + PlayableDirectories[ArrowChoice("Select playback folder", PlayableDirectories.ToArray())];
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.Write("Input desired folder path: ");
                    string path = Console.ReadLine();
                    string upperDir;
                    if (Directory.Exists(path))
                    {
                        upperDir = path.Split('/')[path.Split('/').Length-1] + "/";
                        DirectoryInfo[] di = new DirectoryInfo(path).GetDirectories("*.*", SearchOption.AllDirectories);
                        FileInfo[] currDirFiles = new DirectoryInfo(path).GetFiles("*" + selectedExtension.ToString());
                        if(currDirFiles.Length > 0)
                        {
                            PlayableDirectories.Add(new DirectoryInfo(path).FullName.Replace(path, ""));
                        }
                        foreach (DirectoryInfo d in di)
                        {
                            currDirFiles = d.GetFiles("*" + selectedExtension.ToString());
                            if (currDirFiles.Length > 0)
                            {
                                PlayableDirectories.Add(d.FullName.Replace(path,""));
                            }
                        }
                        if (PlayableDirectories.Count == 0)
                        {
                            Console.WriteLine("No folder containing the selected file type found.");
                            Thread.Sleep(1000);
                            continue;
                        }
                        selectedDirectory = path + PlayableDirectories[ArrowChoice("Select playback folder", PlayableDirectories.ToArray())];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Input path was not found.");
                        Thread.Sleep(1000);
                        continue;
                    }
                }
            }
            bool useAudio;
            string audioPath = "";
            while (true)
            {
                Console.Clear();
                useAudio = ChoiceYesNo("Use audio file?");
                if (useAudio)
                {
                    Console.Clear();
                    Console.Write("Input desired audio file path (.wav): ");
                    audioPath = Console.ReadLine();
                    if (File.Exists(audioPath))
                    {
                        if(Path.GetExtension(audioPath) != ".wav")
                        {
                            Console.WriteLine("Input file is not a .wav file.");
                            Thread.Sleep(1000);
                            continue;
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Input file was not found.");
                        Thread.Sleep(1000);
                        continue;
                    }
                }
                break;
            }
            Console.Clear();
            audio.SoundLocation = audioPath;
            return true;
        }
        static void PlayBitmaps(List<Bitmap> inputBitmaps, int originalFPS, int targetFPS, SoundPlayer videoAudio = null)
        {
            Console.Clear();
            int skippedFrames = originalFPS / targetFPS;
            Bitmap currFrame;
            StringBuilder frameBuilder = new StringBuilder();
            int frameTime = 1000 / targetFPS;
            float frameTimeRemainder = (1000f / (float)targetFPS) - frameTime;
            int leapFrameIndex = 0;
            int leapMs;
            for (int i = 0; i < 1000; i++)
            {
                if (Math.Round(frameTimeRemainder * i, 2) >= 1)
                {
                    leapFrameIndex = i;
                    break;
                }
            }
            DateTime nextFrameTime = DateTime.Now.AddMilliseconds(1000 / targetFPS);
            if(videoAudio != null)
                videoAudio.Play();
            for (int i = 0; i < inputBitmaps.Count; i++)
            {
                leapMs = 0;
                if (skippedFrames > 0 && i % skippedFrames != 0)
                {
                    continue;
                }
                if (leapFrameIndex > 0 && i % leapFrameIndex == 0)
                {
                    leapMs = 1;
                }
                currFrame = inputBitmaps[i];
                for (int x = 0; x < inputBitmaps[0].Height * 2; x++)
                {
                    frameBuilder.AppendLine("");
                    //FastConsole.WriteLine("");
                }
                char c;
                Color currPixel;
                for (int y = 0; y < currFrame.Height; y++)
                {
                    for (int x = 0; x < currFrame.Width; x++)
                    {
                        currPixel = inputBitmaps[i].GetPixel(x, y);
                        if (currPixel.A == 0)
                            c = ' ';
                        else
                            c = CharacterGrayScale(currPixel.R);
                        frameBuilder.Append(c + " ");
                        //FastConsole.Write(c.ToString() + " ");
                    }
                    frameBuilder.Append('\n');
                    //FastConsole.WriteLine("");
                }
                FastConsole.Write(frameBuilder.ToString());
                DateTime currTime;
                frameBuilder.Clear();
;               FastConsole.Flush();
                while (true)
                {
                    currTime = DateTime.Now;
                    if ((nextFrameTime - currTime).Milliseconds <= 0)
                    {
                        nextFrameTime = currTime.AddMilliseconds((1000 / targetFPS) + 1 + leapMs);  //No clue why the additional ms is needed, but it just works with it and I'm leaving it as is
                        break;
                    }
                }
            }
        }
        static int ArrowChoice(string output, string[] items)
        {
            Console.CursorVisible = false;
            int selection = 0;
            int maxItemLength = 0;
            string item;
            string padding;
            char selectedChar = ' ';
            bool userSelected = false;
            foreach(string s in items)
            {
                maxItemLength = s.Length > maxItemLength ? s.Length : maxItemLength;
            }
            padding = "{0,-" + (maxItemLength + 4) + "}" + "{1}";
            while (!userSelected)
            {
                Console.Clear();
                Console.Write(output);
                for (int i = 0; i < items.Length; i++)
                {
                    item = items[i];
                    selectedChar = selection == i ? '■' : ' ';
                    Console.Write(padding,"\n" + item, selectedChar);
                }
                ConsoleKey inputKey = Console.ReadKey().Key;
                switch (inputKey)
                {
                    case ConsoleKey.UpArrow:
                        selection--;
                        break;
                    case ConsoleKey.DownArrow:
                        selection++;
                        break;
                    case ConsoleKey.Enter:
                        userSelected = true;
                        break;
                }
                LimitValue(ref selection, 0, items.Length - 1);
            }
            Console.CursorVisible = true;
            Console.Clear();
            return selection;
        }
        static void LimitValue(ref int value, int lowerLimit, int upperLimit)
        {
            value = (value < lowerLimit) ? lowerLimit : value;
            value = (value > upperLimit) ? upperLimit : value;
        }
        static int ChoiceMultiple(string question, char[] selectableCharacters)
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
        static bool ChoiceYesNo(string question)
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
        static char CharacterGrayScale(int grayScale)
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
