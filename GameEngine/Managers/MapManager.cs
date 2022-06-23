using System.IO;

namespace OpenGL_Game.Managers
{
    class MapManager
    {
        //loads in the a map from "maps"
        public char[,] loadMap(string mapName)
        {
            StreamReader sr = new StreamReader(mapName);
            string mapText = "";
            string[] TextSplit;
            string mapLine = sr.ReadLine();
            int row = (int)mapLine.Length;
            int col = 0;
            while (mapLine != null)
            {
                col += 1;
                mapText += mapLine + ',';
                mapLine = sr.ReadLine();
            }
            sr.Close();
            TextSplit = mapText.Split(',');
            char[,] map = new char[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    map[i, j] = TextSplit[i][j];
                }
            }
            return map;
        }
    }
}