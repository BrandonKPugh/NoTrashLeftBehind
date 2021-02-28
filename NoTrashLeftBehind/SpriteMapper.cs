using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace NoTrashLeftBehind
{
    class SpriteMapper
    {
        public enum ConnectionType
        {
            Normal,
            Horizontal,
            Vertical,
            NextFrame,
            HorizontalFrames,
            VerticalFrames,
            Error
        }

        private enum ReadingState
        {
            ReadingNumber,
            ReadingConnection
        }

        public SpriteSheet ReadFile(string filePath, ContentManager content)
        {
            SpriteSheet sheetToReturn;

            int lineNum = 1;

            using (StreamReader reader = File.OpenText(filePath))
            {
                string line;
                if ((line = reader.ReadLine()) != null)
                {
                    string sheetFileName;
                    int pixelWidth, pixelHeight, offset, gutter;
                    string[] split = line.Split(',');
                    if (split.Length != 5)
                    {
                        throw new FormatException("Header line must contain a file name and four integers");
                    }
                    sheetFileName = GetSheetFileName(line, lineNum);
                    int.TryParse(split[1], out pixelWidth);
                    int.TryParse(split[2], out pixelHeight);
                    int.TryParse(split[3], out offset);
                    int.TryParse(split[4], out gutter);

                    //Texture2D sheetTexture = content.Load<Texture2D>(sheetFileName);
                    sheetToReturn = new SpriteSheet(sheetFileName, pixelWidth, pixelHeight, offset, gutter);

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
                        CreateSprite(sheetToReturn, line, lineNum);
                    }
                }
            }

            return sheetToReturn;
        }

        private void CreateSprite(SpriteSheet sheet, string line, int lineNum)
        {
            string name = GetSpriteName(line, lineNum);
            string rest = line.Substring(name.Length + 2);
            int colonIndex = rest.IndexOf(":");
            if (colonIndex <= 0)
            {
                throw new FormatException("Invalid CountX and CountY on Line: " + lineNum);
            }
            int x, y;
            SpriteEffects flip;
            int len = GetCountXCountYFlip(rest.Substring(0), lineNum, out x, out y, out flip);
            rest = rest.Substring(len + 1);
            ReadLine(rest, out List<int> numbers, out List<ConnectionType> connections);
            List<List<int>> frames = ProcessCommand(numbers, connections, sheet.Columns);

            Sprite sprite = new Sprite(sheet, sheet.ImagePixelWidth * x, sheet.ImagePixelHeight * y, x, y, frames, flip);
            sheet.AddSprite(name, sprite);
        }

        private int GetCountXCountYFlip(string line, int lineNum, out int countX, out int countY, out SpriteEffects flip)
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
            if (split.Length != 2 && split.Length != 3)
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
                else if (split.Length == 3)
                {
                    int.TryParse(split[0], out countX);
                    int.TryParse(split[1], out countY);
                    if (split[2].Length == 1)
                    {
                        char flipChar = split[2][0];
                        if (flipChar == 'h')
                        {
                            flip = SpriteEffects.FlipHorizontally;
                        }
                        else if (flipChar == 'v')
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

        private string GetSpriteName(string line, int lineNum)
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

        private string GetSheetFileName(string line, int lineNum)
        {
            if (line.IndexOf('"') != 0)
            {
                throw new FormatException("Sprite sheet file name is incorrect! Line: " + lineNum + ", Pos: 1");
            }
            int length = 0;
            length = line.Substring(1).IndexOf('"');
            if (length == -1 || length == 0)
            {
                throw new FormatException("Sprite sheet file name is incorrect! Line: " + lineNum + ", Pos: 2");
            }
            return line.Substring(1, length);
        }

        public void ReadLine(string line, out List<int> numbers, out List<ConnectionType> connections)
        {
            numbers = new List<int>();
            connections = new List<ConnectionType>();

            string currentSection = "";
            ReadingState state = ReadingState.ReadingNumber;
            for(int i = 0; i < line.Length; i++)
            {
                if (state == ReadingState.ReadingNumber)
                {
                    if (line[i] >= '0' && line[i] <= '9')
                    {
                        currentSection += line[i];
                    }
                    else if (IsConnectionChar(line[i]))
                    {
                        int.TryParse(currentSection, out int num);
                        numbers.Add(num);
                        currentSection = line[i].ToString();
                        state = ReadingState.ReadingConnection;
                    }
                    else
                    {
                        throw new FormatException("Error reading file!");
                    }
                }
                else if (state == ReadingState.ReadingConnection)
                {
                    if (IsConnectionChar(line[i]))
                    {
                        currentSection += line[i];
                    }
                    else if (line[i] >= '0' && line[i] <= '9')
                    {
                        ConnectionType connection = GetConnectionType(currentSection);
                        if(connection == ConnectionType.Error)
                        {
                            throw new FormatException("Error reading file!");
                        }
                        connections.Add(connection);
                        currentSection = line[i].ToString();
                        state = ReadingState.ReadingNumber;
                    }
                    else
                    {
                        throw new FormatException("Error reading file!");
                    }
                }
            }
            if(state == ReadingState.ReadingNumber)
            {
                int.TryParse(currentSection, out int num);
                numbers.Add(num);
            }
            else
            {
                throw new FormatException("Error reading file!");
            }
            return;
        }

        private string CleanString(string line)
        {
            if (line == null)
            {
                return null;
            }
            string toReturn = "";
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (!char.IsWhiteSpace(c))
                {
                    toReturn += c;
                }
            }
            if (toReturn == "")
            {
                return null;
            }

            return toReturn.ToLower();
        }

        private bool IsConnectionChar(char c)
        {
            return (c == ',' || c == '|' || c == '-' || c == ';');
        }

        private ConnectionType GetConnectionType(string s)
        {
            if (s == ",")
                return ConnectionType.Normal;
            if (s == ";")
                return ConnectionType.NextFrame;
            if (s == "-;")
                return ConnectionType.HorizontalFrames;
            if (s == "|;")
                return ConnectionType.VerticalFrames;
            if (s == "-")
                return ConnectionType.Horizontal;
            if (s == "|")
                return ConnectionType.Vertical;
            else
                return ConnectionType.Error;
        }

        public List<List<int>> ProcessCommand(List<int> numbers, List<ConnectionType> connections, int columns)
        {
            if(numbers == null || connections == null || numbers.Count == 0)
            {
                throw new FormatException("Error reading file!");
            }
            if(numbers.Count == 1)
            {
                List<List<int>> toReturn = new List<List<int>>();
                toReturn.Add(numbers);
                return toReturn;
            }

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] == ConnectionType.NextFrame)
                {

                    List<int> aNums = numbers.GetRange(0, i + 1);
                    List<ConnectionType> aConns = connections.GetRange(0, i);
                    List<int> bNums = numbers.GetRange(i + 1, numbers.Count - (i + 1));
                    List<ConnectionType> bConns = connections.GetRange(i + 1, connections.Count - (i + 1));

                    List<List<int>> toReturn = ProcessCommand(aNums, aConns, columns);
                    List<List<int>> toAdd = ProcessCommand(bNums, bConns, columns);
                    toReturn.AddRange(toAdd);
                    return toReturn;
                }
            }

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] == ConnectionType.HorizontalFrames)
                {
                    List<int> aNums = numbers.GetRange(0, i + 1);
                    List<ConnectionType> aConns = connections.GetRange(0, i);
                    List<int> bNums = numbers.GetRange(i + 1, numbers.Count - (i + 1));
                    List<ConnectionType> bConns = connections.GetRange(i + 1, connections.Count - (i + 1));

                    List<List<int>> first = ProcessCommand(aNums, aConns, columns);
                    List<List<int>> second = ProcessCommand(bNums, bConns, columns);
                    return HorizontalFrames(first[first.Count - 1], second[0][0]);
                }
                else if (connections[i] == ConnectionType.VerticalFrames)
                {
                    List<int> aNums = numbers.GetRange(0, i + 1);
                    List<ConnectionType> aConns = connections.GetRange(0, i);
                    List<int> bNums = numbers.GetRange(i + 1, numbers.Count - (i + 1));
                    List<ConnectionType> bConns = connections.GetRange(i + 1, connections.Count - (i + 1));

                    List<List<int>> first = ProcessCommand(aNums, aConns, columns);
                    List<List<int>> second = ProcessCommand(bNums, bConns, columns);
                    return VerticalFrames(first[first.Count - 1], second[0][0], columns);
                }
            }

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] == ConnectionType.Normal)
                {
                    List<int> aNums = numbers.GetRange(0, i + 1);
                    List<ConnectionType> aConns = connections.GetRange(0, i);
                    List<int> bNums = numbers.GetRange(i + 1, numbers.Count - (i + 1));
                    List<ConnectionType> bConns = connections.GetRange(i + 1, connections.Count - (i + 1));

                    List<List<int>> first = ProcessCommand(aNums, aConns, columns);
                    List<List<int>> second = ProcessCommand(bNums, bConns, columns);
                    foreach (int num in second[0])
                    {
                        first[first.Count - 1].Add(num);
                    }
                    for (int a = 1; a < second.Count; a++)
                    {
                        first.Add(second[a]);
                    }
                    return first;
                }
            }

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] == ConnectionType.Horizontal)
                {
                    List<int> aNums = numbers.GetRange(0, i + 1);
                    List<ConnectionType> aConns = connections.GetRange(0, i);
                    List<int> bNums = numbers.GetRange(i + 1, numbers.Count - (i + 1));
                    List<ConnectionType> bConns = connections.GetRange(i + 1, connections.Count - (i + 1));

                    List<List<int>> first = ProcessCommand(aNums, aConns, columns);
                    List<List<int>> second = ProcessCommand(bNums, bConns, columns);

                    List<int> lastInFirst = first[first.Count - 1];
                    List<int> firstInSecond = second[0];
                    if (lastInFirst.Count != 1 || firstInSecond.Count != 1)
                    {
                        throw new FormatException("Error");
                    }
                    int a = lastInFirst[0];
                    int b = firstInSecond[0];
                    first[first.Count - 1] = Horizontal(a, b);
                    for(int j = 1; j < second.Count; j++)
                    {
                        first.Add(second[j]);
                    }
                    return first;
                }
                else if (connections[i] == ConnectionType.Vertical)
                {
                    List<int> aNums = numbers.GetRange(0, i + 1);
                    List<ConnectionType> aConns = connections.GetRange(0, i);
                    List<int> bNums = numbers.GetRange(i + 1, numbers.Count - (i + 1));
                    List<ConnectionType> bConns = connections.GetRange(i + 1, connections.Count - (i + 1));

                    List<List<int>> first = ProcessCommand(aNums, aConns, columns);
                    List<List<int>> second = ProcessCommand(bNums, bConns, columns);

                    List<int> lastInFirst = first[first.Count - 1];
                    List<int> firstInSecond = second[0];
                    if (lastInFirst.Count != 1 || firstInSecond.Count != 1)
                    {
                        throw new FormatException("Error");
                    }
                    int a = lastInFirst[0];
                    int b = firstInSecond[0];
                    first[first.Count - 1] = Vertical(a, b, columns);
                    for (int j = 1; j < second.Count; j++)
                    {
                        first.Add(second[j]);
                    }
                    return first;
                }
            }

            throw new FormatException("Error");
        }

        public List<int> Horizontal(int a, int b)
        {
            List<int> list = new List<int>();

            for (int i = a; i <= b; i++)
            {
                list.Add(i);
            }

            return list;
        }

        public List<int> Vertical(int a, int b, int columns)
        {
            List<int> list = new List<int>();

            for (int i = a; i <= b; i += columns)
            {
                list.Add(i);
            }

            return list;
        }

        // Returns a as the first list in a set of lists
        public List<List<int>> HorizontalFrames(List<int> a, int b)
        {
            List<List<int>> toReturn = new List<List<int>>();

            int aFirst = a[0];
            List<int> original = Vertical(aFirst, b, WidthOfList(a));

            for (int i = 0; i < original.Count; i++)
            { 
                List<int> newList = new List<int>();

                for (int j = 0; j < a.Count; j++)
                {
                    int next = original[i] + (a[j] - aFirst);
                    newList.Add(next);
                }

                toReturn.Add(newList);
            }
            return toReturn;
        }

        // Returns a as the first list in a set of lists
        public List<List<int>> VerticalFrames(List<int> a, int b, int columns)
        {
            List<List<int>> toReturn = new List<List<int>>();

            int aFirst = a[0];
            List<int> original = Vertical(aFirst, b, columns);

            for (int i = 0; i < original.Count; i++)
            {
                List<int> newList = new List<int>();

                for (int j = 0; j < a.Count; j++)
                {
                    int next = original[i] + (a[j] - aFirst);
                    newList.Add(next);
                }

                toReturn.Add(newList);
            }
            return toReturn;
        }

        private int WidthOfList(List<int> a)
        {
            int width = 0;
            for(int i = 0; i < a.Count; i++)
            {
                if (a[i] == a[0] + i)
                    width++;
            }
            return width;
        }

    }
}
