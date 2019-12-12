using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 経路探査アルゴリズム
{
    class TileBlock
    {
        public enum TileType : int
        {
            Walkable = 1,
            Wall = 2,
            StartTile = 3,
            GoalTile = 4,
            NullTile = 5,
            AnalyzedTile = 6
        }

        public enum AnalyzeAttributes : int
        {
            INull = 0x00000000,
            IOpenedTile = 0x00000030,
            IClosedTile = 0x00000040,
        }

        public struct AnalyzeData
        {
            public int コスト;
            public int 推定コスト;
            public int スコア;
        }

        private struct TileStruct
        {
            public Vector2 coordinate;
            public TileType tileType;
            public AnalyzeAttributes attributes;
            public AnalyzeData analyzeData;
            public bool isAnalyzed;
        }

        private TileStruct tile;

        public TileBlock(Vector2 coord, TileType type)
        {
            tile = new TileStruct();
            tile.coordinate = coord;
            tile.tileType = type;
            tile.attributes = AnalyzeAttributes.INull;
            tile.isAnalyzed = false;
        }

        public Vector2 GetCoordinate()
        {
            return this.tile.coordinate;
        }

        public Vector2 GetTopCoord()
        {
            return new Vector2(tile.coordinate.x, tile.coordinate.y - 1);
        }

        public Vector2 GetBottomCoord()
        {
            return new Vector2(tile.coordinate.x, tile.coordinate.y + 1);
        }

        public Vector2 GetRightCoord()
        {
            return new Vector2(tile.coordinate.x + 1, tile.coordinate.y);
        }

        public Vector2 GetLeftCoord()
        {
            return new Vector2(tile.coordinate.x - 1, tile.coordinate.y);
        }

        public TileType GetTileType()
        {
            return tile.tileType;
        }

        public void SetTileType(TileType type)
        {
            tile.tileType = type;
        }

        //typeがTileTypeの範囲を超えた場合にInvalidCastExceptionが発生することを想定する必要がある。
        public void SetTileType(int type)
        {
            try
            {
                tile.tileType = (TileType)type;
            }
            catch(InvalidCastException ex)
            {
                LoggerForm.WriteError("例外エラー: " + ex.Message);
            }
        }

        public AnalyzeAttributes GetAttributes()
        {
            return tile.attributes;
        }

        public void SetAttribute(AnalyzeAttributes attributes)
        {
            tile.attributes = attributes;
        }

        public AnalyzeData GetAnalyzeData()
        {
            return tile.analyzeData;
        }

        public void SetAnalyzeData(int cost, int heuristic_cost)
        {
            tile.analyzeData.コスト = cost;
            tile.analyzeData.推定コスト = heuristic_cost;
            tile.analyzeData.スコア = heuristic_cost + cost;
        }

        public void UpdateTileType(TileType type)
        {
            World.UpdateTile(tile.coordinate, type);
        }

        public void UpdateTileType()
        {
            World.UpdateTile(tile.coordinate, tile.tileType);
        }

        public int GetScore()
        {
            //return tile.analyzeData.スコア;
            return GetAnalyzeData().スコア;
        }

        public bool Open()
        {
            if(GetAttributes() != AnalyzeAttributes.IClosedTile)
            {
                SetAttribute(AnalyzeAttributes.IOpenedTile);
                return true;
            }
            return false;
        }

        public void Open(bool force)
        {
            if (force)
            {
                SetAttribute(AnalyzeAttributes.IOpenedTile);
            }
        }

        public void Close()
        {
            if (GetAttributes() == AnalyzeAttributes.IOpenedTile)
            {
                SetAttribute(AnalyzeAttributes.IClosedTile);
            }
        }

        public void SetAnalyzed(bool enable)
        {
            tile.isAnalyzed = enable;
        }

        public bool GetAnalyzed()
        {
            return tile.isAnalyzed;
        }
    }
}
