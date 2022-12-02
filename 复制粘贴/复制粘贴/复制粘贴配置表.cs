using Newtonsoft.Json;
using TShockAPI;

namespace 复制粘贴
{
    public class 复制粘贴配置表
    {
        public static void GetConfig()
        {
            try
            {
                if (!File.Exists(path))
                {
                    FileTools.CreateIfNot(Path.Combine(TShock.SavePath, "复制粘贴配置表.json"), JsonConvert.SerializeObject(复制粘贴.配置, Formatting.Indented));
                    复制粘贴.配置 = JsonConvert.DeserializeObject<复制粘贴配置表>(File.ReadAllText(Path.Combine(TShock.SavePath, "复制粘贴配置表.json")));
                    复制粘贴.配置.远程服务器设置.Add(new() { IP = "127.0.0.1", REST端口 = 7878, 秘钥 = "" });
                    File.WriteAllText(path, JsonConvert.SerializeObject(复制粘贴.配置, Formatting.Indented));
                }
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[复制粘贴]配置文件读取错误！！！");
            }
        }

        public bool 启用远程复制 = false;
        public List<远程> 远程服务器设置 = new() { };
        public List<数据> 保存的建筑 = new() { };
        public static string path = "tshock/复制粘贴配置表.json";

        public class 远程
        {
            public int 编号 = 0;
            public string IP = "127.0.0.1";
            public int REST端口 = 7878;
            public string 秘钥 = "";
        }
        public class 数据
        {
            public int 编号 = 0;
            public string 备注 = "无备注";
            public List<图格数据> grid = new() { };
        }
        public class 图格数据
        {
            public int X;
            public int Y;
            public byte bTileHeader;
            public byte bTileHeader2;
            public byte bTileHeader3;
            //public int collisionType;
            public short frameX;
            public short frameY;
            public byte liquid;
            public ushort sTileHeader;
            public ushort type;
            public ushort wall;
            /*
            public bool active;
            public bool actuator;
            public int blockType;
            public bool bottomSlope;
            public bool checkingLiquid;
            public object? Clone;
            public byte color;
            public byte frameNumber;
            public bool fullbrightBlock;
            public bool fullbrightWall;
            */
        }
    }
}

