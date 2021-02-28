using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoTrashLeftBehind
{
    public static class SpriteSheetConfig
    {
        public enum ConnectionType
        {
            None,
            Connected,
            ConnectedHorizontal,
            ConnectedVertical,
            Animation,
            ConnectedHorizontalAnimation,
            ConnectedVerticalAnimation
        }

        public static SpriteSheet ReadFile(string filePath)
        {
            //SpriteSheet sheetToReturn;
            SpriteSheet sheetToReturn = null;

            int lineNum = 1;

            using (StreamReader reader = File.OpenText(filePath))
            {
                string line;
                if ((line = reader.ReadLine()) != null)
                {
                    string sheetFileName;
                    int pixelWidth, pixelHeight, offset, gutter;
                    string[] split = line.Split(',');
                    if(split.Length != 5)
                    {
                        throw new FormatException("Header line must contain a file name and four integers");
                    }
                    sheetFileName = GetSheetFileName(line, lineNum);
                    pixelWidth = GetNum(split[1], lineNum);
                    pixelHeight = GetNum(split[2], lineNum);
                    offset = GetNum(split[3], lineNum);
                    gutter = GetNum(split[4], lineNum);

                    //sheetToReturn = new SpriteSheet(sheetFileName, pixelWidth, pixelHeight, offset, gutter);

                    lineNum++;
                }
                else
                {
                    throw new FormatException("File must start with header line. Line: 0, Pos: 0");
                }

                while ((line = reader.ReadLine()) != null)
                {
                    line = CleanString(line);
                    if (line == null)
                    {
                        lineNum++;
                        continue;
                    }
                    else
                    {
                        //REMOVE THIS LINE
                        sheetToReturn.Columns = 3;
                        // ^ 
                        Sprite sprite = ReadSpriteLine(sheetToReturn, line, lineNum);
                    }
                }
            }




            return null;
        }

        private static int GetNum(string str, int lineNum)
        {
            int num;
            try
            {
                int.TryParse(str, out num);
            }
            catch
            {
                throw new FormatException("Not a number! Line: " + lineNum);
            }
            return num;
        }

        private static string CleanString(string line)
        {
            if(line == null)
            {
                return null;
            }
            string toReturn = "";
            for(int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if(!char.IsWhiteSpace(c))
                {
                    toReturn += c;
                }
            }
            if(toReturn == "")
            {
                return null;
            }

            return toReturn.ToLower();
        }

        private static Sprite ReadSpriteLine(SpriteSheet sheet, string line, int lineNum)
        {
            string name = GetSpriteName(line, lineNum);
            string rest = line.Substring(name.Length + 2);
            int colonIndex = rest.IndexOf(":");
            if(colonIndex <= 0)
            {
                throw new FormatException("Invalid CountX and CountY on Line: " + lineNum);
            }
            int x, y;
            SpriteEffects flip;
            int len = GetCountXCountYFlip(rest.Substring(0), lineNum, out x, out y, out flip);
            rest = rest.Substring(len + 1);
            List<List<int>> temp = GetPositions(rest, lineNum, x, y, sheet.Columns);




            return null;
        }

        private static List<List<int>> GetPositions(string input, int lineNum, int imagesX, int imagesY, int spriteSheetWidth)
        {
            Queue<int> nums = new Queue<int>();
            Queue<ConnectionType> connections = new Queue<ConnectionType>();

            string line = input;
            int pos = 0;
            ConnectionType connection;
            bool again = true;
            int connectionLength;
            while (again)
            {
                if(next(line, out pos, out connection, out connectionLength))
                {
                    int num;
                    string toParse = line.Substring(0, pos);
                    int.TryParse(toParse, out num);
                    nums.Enqueue(num);
                    connections.Enqueue(connection);

                    line = line.Substring(pos + connectionLength);
                }
                else
                {
                    int num;
                    int.TryParse(line, out num);
                    nums.Enqueue(num);
                    again = false;
                }
            }
            //Queues are ready now
            List<List<int>> frames = new List<List<int>>();
            frames.Add(new List<int>());

            int firstThisFrame = -1;
            int lastAdded = 0;
            int frame = 0;
            while (nums.Count > 0)
            {
                ConnectionType currentConnection = ConnectionType.Connected;
                if (connections.Count != 0)
                {
                    currentConnection = connections.Dequeue();
                }
                if (currentConnection == ConnectionType.Connected)
                {
                    int num = nums.Dequeue();
                    if (firstThisFrame == -1)
                    {
                        firstThisFrame = num;
                    }
                    lastAdded = num;
                    frames[frame].Add(num);
                }
                else if (currentConnection == ConnectionType.ConnectedHorizontal)
                {
                    int lastToAdd = nums.Dequeue();
                    for (int i = lastAdded; i <= lastToAdd; i++)
                    {
                        frames[frame].Add(i);
                        lastAdded = i;
                    }
                }
                else if (currentConnection == ConnectionType.ConnectedVertical)
                {
                    int lastToAdd = nums.Dequeue();
                    for (int i = lastAdded; i <= lastToAdd; i += spriteSheetWidth)
                    {
                        frames[frame].Add(i);
                        lastAdded = i;
                    }
                }
                else if (currentConnection == ConnectionType.Animation)
                {
                    int num = nums.Dequeue();
                    frames[frame].Add(num);
                    firstThisFrame = num;
                    lastAdded = num;
                    frame++;
                    frames.Add(new List<int>());
                }
                else if (currentConnection == ConnectionType.ConnectedHorizontalAnimation)
                {
                    int originalFrame = frame;
                    int originalFirst = firstThisFrame;
                    int originalLast = lastAdded;
                    int num = nums.Dequeue();
                    for (int j = 0; j < (num - originalFirst); j++)
                    {
                        frame++;
                        frames.Add(new List<int>());

                        for (int k = 0; k < frames[originalFrame].Count; k++)
                        {
                            int toAdd = (frames[originalFrame][k] - originalFirst) + (originalFirst + j);
                            if (k == 0)
                            {
                                firstThisFrame = toAdd;
                            }
                            lastAdded = toAdd;
                            frames[frame].Add(toAdd);
                        }
                    }
                }
                else if (currentConnection == ConnectionType.ConnectedVerticalAnimation)
                {
                    int originalFrame = frame;
                    int originalFirst = firstThisFrame;
                    int originalLast = lastAdded;
                    int num = nums.Dequeue();
                    for (int j = 0; j < (num / originalFirst); j++)
                    {
                        frame++;
                        frames.Add(new List<int>());

                        for (int k = 0; k < frames[originalFrame].Count; k++)
                        {
                            int toAdd = (frames[originalFrame][k] - originalFirst) + (originalFirst + j * spriteSheetWidth);
                            if (k == 0)
                            {
                                firstThisFrame = toAdd;
                            }
                            lastAdded = toAdd;
                            frames[frame].Add(toAdd);
                        }
                    }
                }
            }
            return frames;

            //Returns true if it finds another of these chars ,;-|
            bool next(string a, out int loc, out ConnectionType type, out int connectionLen)
            {
                type = ConnectionType.None;
                connectionLen = 1;
                int i;
                for(i = 0; i < a.Length; i++)
                {
                    switch (a[i])
                    {
                        case ',':
                            {
                                type = ConnectionType.Connected;
                                break;
                            }
                        case ';':
                            {
                                type = ConnectionType.Animation;
                                break;
                            }
                        case '-':
                            {
                                type = ConnectionType.ConnectedHorizontal;
                                if(i + 1 < a.Length)
                                {
                                    if(a[i + 1] == ';')
                                    {
                                        type = ConnectionType.ConnectedHorizontalAnimation;
                                        connectionLen = 2;
                                        i++;
                                    }
                                }
                                break;
                            }
                        case '|':
                            {
                                type = ConnectionType.ConnectedVertical;
                                if (i + 1 < a.Length)
                                {
                                    if (a[i + 1] == ';')
                                    {
                                        type = ConnectionType.ConnectedVerticalAnimation;
                                        connectionLen = 2;
                                        i++;
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    if(type != ConnectionType.None)
                    {
                        break;
                    }
                }
                if(type == ConnectionType.None)
                {
                    loc = 0;
                    return false;
                }
                else
                {
                    loc = i;
                    return true;
                }

            }
        }

        private static int GetCountXCountYFlip(string line, int lineNum, out int countX, out int countY, out SpriteEffects flip)
        {
            countX = -1;
            countY = -1;
            flip = SpriteEffects.None;
            string error = "CountX and CountY format error on line: ";

            int colon = line.IndexOf(':');
            if (colon <= 0)
            {
                throw new FormatException("Invalid format, no colon found on line: " + lineNum);
            }
            string first = line.Substring(0, colon);
            if (first[0] != '(' || first[first.Length - 1] != ')')
            {
                throw new FormatException(error + lineNum);
            }
            first = first.Substring(1, first.Length - 2);
            string[] split = first.Split(',');
            if(split.Length != 2 && split.Length != 3)
            {
                throw new FormatException(error + lineNum);
            }
            try
            {
                if (split.Length == 2)
                {
                    int.TryParse(split[0], out countX);
                    int.TryParse(split[1], out countY);
                }
                else if(split.Length == 3)
                {
                    int.TryParse(split[0], out countX);
                    int.TryParse(split[1], out countY);
                    if(split[2].Length != 1)
                    {
                        char flipChar = split[2][0];
                        if(flipChar == 'h')
                        {
                            flip = SpriteEffects.FlipHorizontally;
                        }
                        else if(flipChar == 'v')
                        {
                            flip = SpriteEffects.FlipVertically;
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }
                    else
                    {
                        throw new Exception("");
                    }
                }
            }
            catch
            {
                throw new FormatException(error + lineNum);
            }
            return first.Length + 2;
        }

        private static string GetSpriteName(string line, int lineNum)
        {
            if (line.IndexOf('"') != 0)
            {
                throw new FormatException("Invalid sprite name format!" + lineNum + ", Pos: 1");
            }
            int length = 0;
            length = line.Substring(1).IndexOf('"');
            if (length == -1 || length == 0)
            {
                throw new FormatException("Invalid sprite name format!" + lineNum + ", Pos: 2");
            }
            return line.Substring(1, length);
        }

        private static string GetSheetFileName(string line, int lineNum)
        {
            if(line.IndexOf('"') != 0)
            {
                throw new FormatException("Sprite sheet file name is incorrect! Line: " + lineNum + ", Pos: 1");
            }
            int length = 0;
            length = line.Substring(1).IndexOf('"');
            if(length == -1 || length == 0)
            {
                throw new FormatException("Sprite sheet file name is incorrect! Line: " + lineNum + ", Pos: 2");
            }
            return line.Substring(1, length);
        }
    }
}
