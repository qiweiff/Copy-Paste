using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace 复制粘贴
{
    public class DB
    {
        public const string 复制粘贴图格数据 = "复制粘贴图格数据";
        public const string 复制粘贴建筑信息 = "复制粘贴建筑信息";
        /// <summary>
        /// 生成数据表
        /// </summary>
        public static void Reload()
        {
            创建表();
        }
        public static void 创建表()
        {
            {
                SqlTable table = new(复制粘贴图格数据, new SqlColumn[]
                {
                new SqlColumn("ID", MySqlDbType.Int32),
                new SqlColumn("X", MySqlDbType.Int32),
                new SqlColumn("Y", MySqlDbType.Int32),
                new SqlColumn("bTileHeader", MySqlDbType.Int32),
                new SqlColumn("bTileHeader2", MySqlDbType.Int32),
                new SqlColumn("bTileHeader3", MySqlDbType.Int32),
                new SqlColumn("frameX", MySqlDbType.Int32),
                new SqlColumn("frameY", MySqlDbType.Int32),
                new SqlColumn("liquid", MySqlDbType.Int32),
                new SqlColumn("sTileHeader", MySqlDbType.Int32),
                new SqlColumn("type", MySqlDbType.Int32),
                new SqlColumn("wall", MySqlDbType.Int32),
                });
                IDbConnection db = TShock.DB;
                IQueryBuilder provider;
                if (TShock.DB.GetSqlType() != SqlType.Sqlite)
                {
                    IQueryBuilder queryBuilder = new MysqlQueryCreator();
                    provider = queryBuilder;
                }
                else
                {
                    IQueryBuilder queryBuilder = new SqliteQueryCreator();
                    provider = queryBuilder;
                }
                SqlTableCreator sqlTableCreator = new(db, provider);
                sqlTableCreator.EnsureTableStructure(table);
            }
            {
                SqlTable table = new(复制粘贴建筑信息, new SqlColumn[]
                {
                new SqlColumn("ID", MySqlDbType.Int32)
                {
                    Primary = true
                },
                new SqlColumn("备注", MySqlDbType.String),
                });
                IDbConnection db = TShock.DB;
                IQueryBuilder provider;
                if (TShock.DB.GetSqlType() != SqlType.Sqlite)
                {
                    IQueryBuilder queryBuilder = new MysqlQueryCreator();
                    provider = queryBuilder;
                }
                else
                {
                    IQueryBuilder queryBuilder = new SqliteQueryCreator();
                    provider = queryBuilder;
                }
                SqlTableCreator sqlTableCreator = new(db, provider);
                sqlTableCreator.EnsureTableStructure(table);
            }

        }
        public static int 获取最大ID()
        {
            int i = 0;
            while (true)
            {
                i++;
                using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + 复制粘贴建筑信息 + " WHERE `ID` = @0", i))
                {
                    if (表.Read())
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return i;
        }
        public static void 添加图格信息(int ID, 复制粘贴.图格数据 g)
        {
            TShock.DB.Query("INSERT INTO " + 复制粘贴图格数据 + " (`ID`,`X`,`Y`,`bTileHeader`,`bTileHeader2`,`bTileHeader3`,`frameX`,`frameY`,`liquid`,`sTileHeader`,`type`,`wall`) VALUES " + "(@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11);", ID, g.X, g.Y, g.bTileHeader, g.bTileHeader2, g.bTileHeader3, g.frameX, g.frameY, g.liquid, g.sTileHeader, g.type, g.wall);
        }
        public static void 添加建筑信息(int ID, string text)
        {
            TShock.DB.Query("INSERT INTO " + 复制粘贴建筑信息 + " (`ID`,`备注`) VALUES " + "(@0,@1);", ID, text);
        }
        public static void 删除图格信息(int ID)
        {
            TShock.DB.Query("delete from " + 复制粘贴图格数据 + " where `ID`=@0", ID);
        }
        public static void 删除建筑信息(int ID)
        {
            TShock.DB.Query("delete from " + 复制粘贴建筑信息 + " where `ID`=@0", ID);
        }
        public static List<复制粘贴.图格数据> 获取图格信息(int ID)
        {
            List<复制粘贴.图格数据> list = new() { };
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + 复制粘贴图格数据 + " WHERE `ID` = @0", ID))
            {
                while (表.Read())
                {
                    int X = 表.Get<int>("X");
                    int Y = 表.Get<int>("Y");
                    byte bTileHeader = 表.Get<byte>("bTileHeader");
                    byte bTileHeader2 = 表.Get<byte>("bTileHeader2");
                    byte bTileHeader3 = 表.Get<byte>("bTileHeader3");
                    short frameX = 表.Get<short>("frameX");
                    short frameY = 表.Get<short>("frameY");
                    byte liquid = 表.Get<byte>("liquid");
                    ushort sTileHeader = 表.Get<ushort>("sTileHeader");
                    ushort type = 表.Get<ushort>("type");
                    ushort wall = 表.Get<ushort>("wall");
                    list.Add(new() { X = X, Y = Y, bTileHeader = bTileHeader, bTileHeader2 = bTileHeader2, bTileHeader3 = bTileHeader3, frameX = frameX, frameY = frameY, liquid = liquid, sTileHeader = sTileHeader, type = type, wall = wall });
                }
                return list;
            }
        }
        public static void 粘贴_获取图格信息(int ID, int X1, int Y1)
        {
            try
            {
                using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + 复制粘贴图格数据 + " WHERE `ID` = @0", ID))
                {
                    while (表.Read())
                    {
                        int X = 表.Get<int>("X");
                        int Y = 表.Get<int>("Y");
                        byte bTileHeader = byte.Parse(表.Get<int>("bTileHeader").ToString());
                        byte bTileHeader2 = byte.Parse(表.Get<int>("bTileHeader2").ToString());
                        byte bTileHeader3 = byte.Parse(表.Get<int>("bTileHeader3").ToString());
                        short frameX = short.Parse(表.Get<int>("frameX").ToString());
                        short frameY = short.Parse(表.Get<int>("frameY").ToString());
                        byte liquid = byte.Parse(表.Get<int>("liquid").ToString());
                        ushort sTileHeader = ushort.Parse(表.Get<int>("sTileHeader").ToString());
                        ushort type = ushort.Parse(表.Get<int>("type").ToString());
                        ushort wall = ushort.Parse(表.Get<int>("wall").ToString());
                        复制粘贴.图格数据 t = new() { X = X, Y = Y, bTileHeader = bTileHeader, bTileHeader2 = bTileHeader2, bTileHeader3 = bTileHeader3, frameX = frameX, frameY = frameY, liquid = liquid, sTileHeader = sTileHeader, type = type, wall = wall };
                        复制粘贴.Paste(t, X1, Y1);
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("获取图格信息异常" + ex.ToString());
            }
        }
        public static string? 获取建筑备注(int ID)
        {
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + 复制粘贴建筑信息 + " WHERE `ID` = @0", ID))
            {
                while (表.Read())
                {
                    return 表.Get<string>("备注");
                }
                return null;
            }
        }
        public static List<复制粘贴.建筑数据> 获取所有建筑备注()
        {
            List<复制粘贴.建筑数据> list = new() { };
            using (var 表 = TShock.DB.QueryReader("SELECT * FROM " + 复制粘贴建筑信息))
            {
                while (表.Read())
                {
                    list.Add(new() { 编号 = 表.Get<int>("ID"), 备注 = 表.Get<string>("备注") });
                }
                return list;
            }
        }
    }
}
