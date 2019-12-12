using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T = 経路探査アルゴリズム.TileBlock;
using TileType = 経路探査アルゴリズム.TileBlock.TileType;
using AnalyzeAttributes = 経路探査アルゴリズム.TileBlock.AnalyzeAttributes;
using AnalyzeDatas = 経路探査アルゴリズム.TileBlock.AnalyzeData;
using A = 経路探査アルゴリズム.TileBlock.AnalyzeAttributes;

namespace 経路探査アルゴリズム
{
    class World
    {
        public enum ArrowVector
        {
            NULL,

            UP,
            DOWN,
            RIGHT,
            LEFT,

            UP_RIGHT,
            UP_LEFT,

            DOWN_RIGHT,
            DOWN_LEFT,
        }

        public static Graphics  graphics { get; set; }
        public static Bitmap    bitmap { get; set; }

        public static Font font     = new Font("Arial", 6);
        public static Font TileFont = new Font("Arial", 50);

        public static readonly int MAX_COORD_X = 25;
        public static readonly int MAX_COORD_Y = 25;

        private static Form1 f              = new Form1();
        private static LoggerForm logger    = new LoggerForm();

        private static Pen   colorWorkable  = Pens.Black;
        private static Brush colorWall      = Brushes.Gray;
        private static Brush colorStart     = Brushes.Blue;
        private static Brush colorGoal      = Brushes.Red;
        private static Brush colorNull      = Brushes.White;
        private static Brush colorAnalyzed  = Brushes.DarkKhaki;

        private static int tilePadding  = 2;
        private static int tileSizeX    = 0;
        private static int tileSizeY    = 0;

        private static TileBlock[,] tileBlocks { get; set; }

        public World() { }

        public static void CreateMap()
        {
            tileBlocks = new TileBlock[MAX_COORD_X, MAX_COORD_Y];
            
            tileSizeX = (bitmap.Width) / MAX_COORD_X;
            tileSizeY = (bitmap.Height) / MAX_COORD_Y;

            for (int x = 0; x < MAX_COORD_X; x++)
            {
                for (int y = 0; y < MAX_COORD_Y; y++)
                {
                    AddTile(new Vector2(x, y), TileType.Walkable);
                }
            }

            LoggerForm.WriteSuccess("Map created.");
        }

        public static void ResetMap()
        {
            GetGraphics().Clear(Color.White);
            CreateMap();
        }

        public static void UpdateMap(TileBlock[,] tiles = null)
        {
            if(tiles!=null)ResetMap();
            for (int x = 0; x < MAX_COORD_X; x++)
            {
                for (int y = 0; y < MAX_COORD_Y; y++)
                {
                    var tileType = (tiles == null) ? GetTileBlock(new Vector2(x, y)).GetTileType() : tiles[x, y].GetTileType();
                    UpdateTile(new Vector2(x, y), tileType);
                }
            }
        }

        public static bool AddTile(Vector2 coord, TileType type)
        {
            if (!VaildCoord(coord)) return false;

            var block = GetTileBlock(new Vector2(coord.x, coord.y));
            if (block != null)
            {
                if (block.GetTileType() != type)
                {
                    return DrawRectWithTileType(type, GetRectangle(coord));
                }
            }
            else
            {
                tileBlocks[coord.x, coord.y] = new TileBlock(coord, type);
                return DrawRectWithTileType(type, GetRectangle(coord));
            }

            return false;
        }

        public static bool UpdateTile(Vector2 coord, TileType type)
        {
            if (!VaildCoord(coord)) return false;

            var block = GetTileBlock(new Vector2(coord.x, coord.y));
            if (block == null) return false;

            if (block.GetTileType() != type)
            {
                block.SetTileType(type);
                var rect = GetRectangle(coord);
                DrawRectWithTileType(TileType.NullTile, rect);
                LoggerForm.WriteInfo(string.Format("Tile Updated | X: {0}, Y: {1}", coord.x, coord.y));
                return DrawRectWithTileType(type, rect);
            }
            else
            {
                return false;
            }
        }

        public static Vector2 GetTileCoordByTileType(TileType type)
        {
            for (int x = 0; x < MAX_COORD_X; x++)
            {
                for (int y = 0; y < MAX_COORD_Y; y++)
                {
                    if (GetTileBlock(new Vector2(x, y)).GetTileType() == type)
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            LoggerForm.WriteError("GetTileCoordByTileType() Tile not found!");
            return new Vector2(0, 0);
        }

        public static TileBlock GetTileBlock(Vector2 coord)
        {
            if (!IsTileBlockExists(coord)) return null;
            return tileBlocks[coord.x, coord.y];
        }

        public static TileBlock GetTileBlockByTileType(TileType type)
        {
            for (int x = 0; x < MAX_COORD_X; x++)
            {
                for (int y = 0; y < MAX_COORD_Y; y++)
                {
                    var tile = GetTileBlock(new Vector2(x, y));
                    if (tile.GetTileType() == type)
                    {
                        return tile;
                    }
                }
            }
            LoggerForm.WriteError("GetTileBlockByTileType() Tile not found!");
            return null;
        }

        private static bool DrawRectWithTileType(TileType type, Rectangle rect)
        {
            switch (type)
            {
                case TileType.Walkable:
                    GetGraphics().DrawRectangle(colorWorkable, rect);
                    break;
                case TileType.Wall:
                    GetGraphics().FillRectangle(colorWall, rect);
                    break;
                case TileType.StartTile:
                    GetGraphics().FillRectangle(colorStart, rect);
                    break;
                case TileType.GoalTile:
                    GetGraphics().FillRectangle(colorGoal, rect);
                    break;
                case TileType.NullTile:
                    GetGraphics().FillRectangle(colorNull, rect);
                    break;
                case TileType.AnalyzedTile:
                    GetGraphics().FillRectangle(colorAnalyzed, rect);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static bool VaildCoord(Vector2 coord)
        {
            if (coord.x > MAX_COORD_X || coord.y > MAX_COORD_Y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsTileBlockExists(Vector2 coord)
        {
            if (IsOutOfIndex(coord)) return false;

            if (tileBlocks[coord.x, coord.y] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Rectangle GetRectangle(Vector2 coord)
        {
            return new Rectangle(GetRednerTileLocation(coord), GetTileSize());
        }

        public static Size GetTileSize()
        {
            return new Size(tileSizeX, tileSizeY);
        }

        public static Vector2 GetWhiteSpace()
        {
            return new Vector2((bitmap.Width - (GetTileSize().Width * MAX_COORD_X)) / 2, (bitmap.Height - (GetTileSize().Height * MAX_COORD_Y)) / 2);
        }

        public static Point GetRednerTileLocation(Vector2 coord)
        {
            Vector2 drawCoord = new Vector2(tileSizeX * coord.x, tileSizeY * coord.y);
            return new Point(drawCoord.x + GetWhiteSpace().x, drawCoord.y + GetWhiteSpace().y);
        }

        public static Point GetRenderTileLocationCenter(Vector2 coord)
        {
            Vector2 drawCoord = new Vector2(tileSizeX * coord.x, tileSizeY * coord.y);
            return new Point(drawCoord.x + GetWhiteSpace().x / 2, drawCoord.y + GetWhiteSpace().y / 2);
        }

        public static void DrawText(string text, Brush brush, Vector2 coord)
        {
            GetGraphics().DrawString(text, font, brush, new Point(coord.x, coord.y));
        }

        public static void DrawLine(Vector2 start, Vector2 end)
        {
            GetGraphics().DrawLine(Pens.Green, start.x, start.y, end.x, end.y);
        }

        public static void DrawLineCenterTileToTile(Vector2 start, Vector2 end)
        {
            Vector2 誤差 = new Vector2(1, 1);
            DrawLine(PointToVector2(GetRenderTileLocationCenter(start + 誤差)),
                    PointToVector2(GetRenderTileLocationCenter(end + 誤差)));
        }

        public static Vector2 GetTileCoordByMouseCoord(Vector2 coord)
        {
            for(int x = 0; x < MAX_COORD_X; x++)
            {
                for(int y = 0; y < MAX_COORD_Y; y++)
                {
                    var tileLocation = GetRednerTileLocation(new Vector2(x, y));
                    var tileSize = GetTileSize();

                    var xX = tileLocation.X;
                    var Xx = tileLocation.X + tileSize.Width;

                    var yY = tileLocation.Y;
                    var Yy = tileLocation.Y + tileSize.Height;

                    if(xX <= coord.x && Xx >= coord.x)
                    {
                        if(yY <= coord.y && Yy >= coord.y)
                        {
                            return new Vector2(x, y);
                        }
                    }
                }
            }
            return new Vector2(0, 0);
        }

        public static AnalyzeAttributes GetTileAttributeByMouseCoord(Vector2 coord)
        {
            return GetTileBlock(coord).GetAttributes();
        }

        private static Graphics GetGraphics()
        {
            return graphics;
        }

        public static Bitmap GetBitmap()
        {
            lock(bitmap)
            {
                return bitmap;
            }
        }

        public static void CreateContext()
        {
            graphics = Graphics.FromImage(GetBitmap());
            if(graphics != null)
            {
                LoggerForm.WriteSuccess("Context created.");
            }
            else
            {
                LoggerForm.WriteError("CreateContext() failed.");
            }
        }

        public static void DestroyContext()
        {
            lock (graphics)
            {
                graphics = null;
            }
        }

        public static void NewFrame()
        {
            GetGraphics().Clear(Color.White);
        }

        public static void EndFrame()
        {
            f.pictureBox1.Refresh();
        }

        public static TileBlock[,] ExportCurrentMapData()
        {
            return tileBlocks;
        }

        public static void ImportMapData(TileBlock[,] datas)
        {
            try
            {
                UpdateMap(datas);
            }
            catch(IndexOutOfRangeException ex)
            {
                LoggerForm.WriteError("例外エラー: " + ex.Message);
            }
        }

        public static void SetTileAttributesToAll()
        {
            for (int x = 0; x < MAX_COORD_X; x++)
            {
                for (int y = 0; y < MAX_COORD_Y; y++)
                {
                    var tile = GetTileBlock(new Vector2(x, y));
                    var tileType = tile.GetTileType();
                    if(tile.GetAttributes() == AnalyzeAttributes.INull)
                    {
                        if (tileType == TileType.Wall || tileType == TileType.NullTile
                            || tileType == TileType.StartTile || tileType == TileType.GoalTile)
                        {
                            tile.SetAttribute(AnalyzeAttributes.IClosedTile);
                        }
                        else
                        {
                            tile.SetAttribute(AnalyzeAttributes.INull);
                        }
                    }
                }
            }
        }

        public static ArrowVector CalculateVector(ArrowVector vectorX, ArrowVector vectorY)
        {
            if(vectorY == ArrowVector.UP && vectorX == ArrowVector.RIGHT)
            {
                return ArrowVector.UP_RIGHT;
            }
            else if (vectorY == ArrowVector.UP && vectorX == ArrowVector.LEFT)
            {
                return ArrowVector.UP_LEFT;
            }

            else if (vectorY == ArrowVector.DOWN && vectorX == ArrowVector.RIGHT)
            {
                return ArrowVector.UP_RIGHT;
            }
            else if (vectorY == ArrowVector.DOWN && vectorX == ArrowVector.LEFT)
            {
                return ArrowVector.UP_LEFT;
            }
            else if (vectorX == vectorY && vectorX == ArrowVector.UP)
            {
                return ArrowVector.UP;
            }
            else if (vectorX == vectorY && vectorX == ArrowVector.DOWN)
            {
                return ArrowVector.DOWN;
            }
            else if (vectorX == vectorY && vectorX == ArrowVector.RIGHT)
            {
                return ArrowVector.RIGHT;
            }
            else if (vectorX == vectorY && vectorX == ArrowVector.LEFT)
            {
                return ArrowVector.LEFT;
            }

            return ArrowVector.NULL;
        }

        public static ArrowVector CalculateVector(Vector2 start, Vector2 goal)
        {
            if(start.x == goal.x)
            {
                if(start.y > goal.y)
                {
                    return ArrowVector.UP;
                }
                else
                {
                    return ArrowVector.DOWN;
                }
            }

            if(start.y == goal.y)
            {
                if(start.x > goal.x)
                {
                    return ArrowVector.LEFT;
                }
                else
                {
                    return ArrowVector.RIGHT;
                }
            }

            if (start.x > goal.x && start.y > goal.y) return ArrowVector.UP_LEFT;
            if (start.x < goal.x && start.y > goal.y) return ArrowVector.UP_RIGHT;

            if (start.x < goal.x && start.y < goal.y) return ArrowVector.DOWN_RIGHT;
            if (start.x > goal.x && start.y < goal.y) return ArrowVector.DOWN_LEFT;

            return ArrowVector.NULL;
        }

        public static int CalculateHeuristic(Vector2 currentPos, Vector2 goal)
        {
            var xX = Math.Abs(goal.x - currentPos.x);
            var Yy = Math.Abs(goal.y - currentPos.y);
            return xX + Yy;
        }

        public static bool IsOutOfIndex(Vector2 coord)
        {
            if (coord.x < 0 || coord.x >= MAX_COORD_X)
            {
                return true;
            }
            if (coord.y < 0 || coord.y >= MAX_COORD_Y)
            {
                return true;
            }
            return false;
        }

        public static bool IsNegativeAttribute(TileBlock tile)
        {
            var attr = tile.GetAttributes();
            switch(attr)
            {
                case A.IClosedTile:
                    return true;
                case A.INull:
                    return true;
            }
            return false;
        }

        public static Vector2 PointToVector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        private static TileBlock GetBestTile(TileBlock origin, int cost)
        {
            if (origin.GetTileType() == TileType.GoalTile || origin.GetAnalyzed())
            {
                return origin;
            }

            var goalCoord   = GetTileBlockByTileType(TileType.GoalTile).GetCoordinate();
            var originCoord = origin.GetCoordinate();

            var up      = GetTileBlock(new Vector2(originCoord.x, originCoord.y - 1));
            var bottom  = GetTileBlock(new Vector2(originCoord.x, originCoord.y + 1));
            var right   = GetTileBlock(new Vector2(originCoord.x + 1, originCoord.y));
            var left    = GetTileBlock(new Vector2(originCoord.x - 1, originCoord.y));

            if (up      != null && up.      GetTileType() != TileType.Walkable && up.GetTileType()      != TileType.GoalTile) up      = null;
            if (bottom  != null && bottom.  GetTileType() != TileType.Walkable && bottom.GetTileType()  != TileType.GoalTile) bottom  = null;
            if (right   != null && right.   GetTileType() != TileType.Walkable && right.GetTileType()   != TileType.GoalTile) right   = null;
            if (left    != null && left.    GetTileType() != TileType.Walkable && left.GetTileType()    != TileType.GoalTile) left    = null;

            if (up      != null && up.      GetAnalyzed())  up     = null;
            if (bottom  != null && bottom.  GetAnalyzed())  bottom = null;
            if (right   != null && right.   GetAnalyzed())  right  = null;
            if (left    != null && left.    GetAnalyzed())  left   = null;


            if (up      != null && up.      GetTileType() == TileType.GoalTile) DrawLineCenterTileToTile(origin.GetCoordinate(), up.    GetCoordinate());
            if (bottom  != null && bottom.  GetTileType() == TileType.GoalTile) DrawLineCenterTileToTile(origin.GetCoordinate(), bottom.GetCoordinate());
            if (right   != null && right.   GetTileType() == TileType.GoalTile) DrawLineCenterTileToTile(origin.GetCoordinate(), right. GetCoordinate());
            if (left    != null && left.    GetTileType() == TileType.GoalTile) DrawLineCenterTileToTile(origin.GetCoordinate(), left.  GetCoordinate());

            var up_hcost        = 0;
            var bottom_hcost    = 0;
            var right_hcost     = 0;
            var left_hcost      = 0;

            if (up      != null)    up_hcost        = CalculateHeuristic(up.GetCoordinate()     , goalCoord);
            if (bottom  != null)    bottom_hcost    = CalculateHeuristic(bottom.GetCoordinate() , goalCoord);
            if (right   != null)    right_hcost     = CalculateHeuristic(right.GetCoordinate()  , goalCoord);
            if (left    != null)    left_hcost      = CalculateHeuristic(left.GetCoordinate()   , goalCoord);

            if (up      != null)    up.     SetAnalyzeData(     cost,   up_hcost       );
            if (bottom  != null)    bottom. SetAnalyzeData(     cost,   bottom_hcost   );
            if (right   != null)    right.  SetAnalyzeData(     cost,   right_hcost    );
            if (left    != null)    left.   SetAnalyzeData(     cost,   left_hcost     );

            var up_score        = 0;
            var bottom_score    = 0;
            var right_score     = 0;
            var left_score      = 0;

            if (up      != null)    up_score        = up.       GetScore();
            if (bottom  != null)    bottom_score    = bottom.   GetScore();
            if (right   != null)    right_score     = right.    GetScore();
            if (left    != null)    left_score      = left.     GetScore();

            var scores  = new int[4];
            scores[0]   = up_score;
            scores[1]   = bottom_score;
            scores[2]   = right_score;
            scores[3]   = left_score;

            var hcosts  = new int[4];
            hcosts[0]   = up_hcost;
            hcosts[1]   = bottom_hcost;
            hcosts[2]   = right_hcost;
            hcosts[3]   = left_hcost;

            var tiles   = new TileBlock[4];
            tiles[0]    = up;
            tiles[1]    = bottom;
            tiles[2]    = right;
            tiles[3]    = left;

            var min_score   = int.MaxValue;
            var min_cost    = int.MaxValue;
            var min_hcost   = int.MaxValue;
            var min_tile    = origin;

            for(int m = 0; m < 4; m++)
            {
                if (scores[m] == 0)         continue;
                if (scores[m] > min_score)  continue;
                if (scores[m] == min_score && cost >= min_cost) continue;
                if (scores[m] == min_score && cost == min_cost) LoggerForm.WriteError("both same");

                min_score   = scores[m];
                min_cost    = cost;
                min_tile    = tiles[m];
                min_hcost   = hcosts[m];
            }

            if (origin != null)
            {
                origin.Close();
            }

            if (min_tile.GetTileType() == TileType.Walkable)
            {
                origin.SetAnalyzed(true);
                //UpdateTile(origin.GetCoordinate(), TileType.AnalyzedTile);
                //DrawText(min_cost.ToString(), Brushes.Black, PointToVector2(GetRednerTileLocation(min_tile.GetCoordinate())));
                DrawLineCenterTileToTile(origin.GetCoordinate(), min_tile.GetCoordinate());

                bool showScore = f.checkBox1.Checked;

                if(up != null && showScore)
                {
                    DrawText((up_hcost + cost).ToString(), Brushes.Black, PointToVector2(GetRednerTileLocation(up.GetCoordinate())));
                    LoggerForm.WriteWarn("UP HCOST: " + up_hcost.ToString());
                    LoggerForm.WriteWarn("UP  COST: " + cost.ToString());
                    LoggerForm.WriteWarn("UP SCORE: " + up_score.ToString());
                    LoggerForm.WriteInfo("-------------");
                }
                if(bottom != null && showScore)
                {
                    DrawText((bottom_hcost + cost).ToString(), Brushes.Black, PointToVector2(GetRednerTileLocation(bottom.GetCoordinate())));
                    LoggerForm.WriteWarn("BOTTOM HCOST: " + bottom_hcost.ToString());
                    LoggerForm.WriteWarn("BOTTOM  COST: " + cost.ToString());
                    LoggerForm.WriteWarn("BOTTOM SCORE: " + bottom_score.ToString());
                    LoggerForm.WriteInfo("-------------");
                }
                if(right != null && showScore)
                {
                    DrawText((right_hcost + cost).ToString(), Brushes.Black, PointToVector2(GetRednerTileLocation(right.GetCoordinate())));
                    LoggerForm.WriteWarn("RIGHT HCOST: " + right_hcost.ToString());
                    LoggerForm.WriteWarn("RIGHT  COST: " + cost.ToString());
                    LoggerForm.WriteWarn("RIGHT SCORE: " + right_score.ToString());
                    LoggerForm.WriteInfo("-------------");
                }
                if(left != null && showScore)
                {
                    DrawText((left_hcost + cost).ToString(), Brushes.Black, PointToVector2(GetRednerTileLocation(left.GetCoordinate())));
                    LoggerForm.WriteWarn("LEFT HCOST: " + left_hcost.ToString());
                    LoggerForm.WriteWarn("LEFT  COST: " + cost.ToString());
                    LoggerForm.WriteWarn("LEFT SCORE: " + left_score.ToString());
                    LoggerForm.WriteInfo("-------------");
                }
            }

            return min_tile;
        }

        public static async void AnalyzeMap()
        {
            LoggerForm.WriteSuccess("探索開始");

            //各タイル座標に属性を付与
            SetTileAttributesToAll();

            var startTile   = GetTileBlockByTileType(TileType.StartTile);
            var goalTile    = GetTileBlockByTileType(TileType.GoalTile);

            if(startTile == null || goalTile == null)
            {
                if(startTile == null)
                {
                    LoggerForm.WriteError("Start tile not found.");
                }
                else if(goalTile == null)
                {
                    LoggerForm.WriteError("Goal tile not found.");
                }
                return;
            }

            var startTileCoord = startTile.GetCoordinate();
            var goalTileCoord = goalTile.GetCoordinate();

            var k = 0;
            TileBlock bestTile = startTile;
            while (k < 999)
            {
                k++;
                if (bestTile == null || !IsTileBlockExists(bestTile.GetCoordinate()))
                {
                    LoggerForm.WriteError("Tile not found.");
                    break;
                }
                else if (bestTile.GetTileType() == TileType.GoalTile)
                {
                    LoggerForm.WriteSuccess("Goal found.");
                    break;
                }
                else
                {
                    bestTile = GetBestTile(bestTile, k);
                }
                await Task.Delay(10);
            }
        }
    }
}
