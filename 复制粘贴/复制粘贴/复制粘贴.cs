using Newtonsoft.Json;
using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace 复制粘贴
{
    [ApiVersion(2, 1)]//api版本
    public class 复制粘贴 : TerrariaPlugin
    {
        /// 插件作者
        public override string Author => "奇威复反";
        /// 插件说明
        public override string Description => "在游戏内复制和粘贴图格";
        /// 插件名字
        public override string Name => "复制粘贴";
        /// 插件版本
        public override Version Version => new(1, 1, 0, 0);
        /// 插件处理
        public 复制粘贴(Main game) : base(game)
        {
        }
        //插件启动时，用于初始化各种狗子
        public static 复制粘贴配置表 配置 = new();
        /*public List<复制> 粘贴 = new();
        public class 复制
        {
            public string name = "";
            public bool zt = false;
            public int X = 0;
            public int Y = 0;
        }*/
        public static string path = "tshock/复制粘贴配置表.json";
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            复制粘贴配置表.GetConfig();
            Reload();
        }
        /// 插件关闭时
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);//销毁游戏初始化狗子
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)//游戏初始化的狗子
        {
            //第一个是权限，第二个是子程序，第三个是指令
            Commands.ChatCommands.Add(new Command("复制粘贴", 指令1, "复制", "fz") { });
            Commands.ChatCommands.Add(new Command("复制粘贴", 指令2, "粘贴", "zt") { });
            Commands.ChatCommands.Add(new Command("复制粘贴", 指令3, "远程粘贴", "zt") { });
            Commands.ChatCommands.Add(new Command("复制粘贴", 重载, "reload") { });
            TShock.RestApi.Register(new SecureRestCommand("/fzzt/list", REST1, "复制粘贴"));
        }

        private static void 指令1(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0)
                {
                    args.Player.SendInfoMessage($"正确指令：/复制 <1/2> --选择复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/复制 确定 <备注> --确定复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                    return;
                }
                switch (args.Parameters[0])
                {
                    case "1":
                        args.Player.AwaitingTempPoint = 1;
                        args.Player.SendInfoMessage("请选择复制区域的左上角");
                        break;
                    case "2":
                        args.Player.AwaitingTempPoint = 2;
                        args.Player.SendInfoMessage("请选择复制区域的右下角");
                        break;
                    case "确定":
                        if (args.Player.TempPoints[0].X == 0 || args.Player.TempPoints[1].X == 0)
                        {
                            args.Player.SendInfoMessage($"您还没有选择复制的区域！");
                        }
                        else
                        {
                            var q = Copy(args.Player.TempPoints[0].X, args.Player.TempPoints[0].Y, args.Player.TempPoints[1].X, args.Player.TempPoints[1].Y);
                            int L = 配置.保存的建筑.Count;
                            配置.保存的建筑.Add(new() { 编号 = L, grid = q });
                            if (args.Parameters.Count >= 2)
                            {
                                配置.保存的建筑[L].备注 = args.Parameters[1];
                            }
                            args.Player.SendInfoMessage($"复制成功，新建筑的编号为[c/FF3333:{L}]！");
                            File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
                        }
                        break;
                    default:
                        args.Player.SendInfoMessage($"正确指令：/复制 <1/2> --选择复制的区域");
                        args.Player.SendInfoMessage($"正确指令：/复制 确定 <备注> --确定复制的区域");
                        args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                        args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                        break;
                }
            }
            catch
            {
                args.Player.SendErrorMessage($"[复制粘贴]复制发送错误！");
            }
        }

        private static void 指令2(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0)
                {
                    args.Player.SendInfoMessage($"正确指令：/复制 <1/2> --选择复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/复制 确定 <备注> --确定复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                    return;
                }
                if (args.Parameters.Count == 1)
                {
                    if (args.Parameters[0] == "列表" || args.Parameters[0] == "list")
                    {
                        args.Player.SendInfoMessage($"建筑列表：\n");
                        foreach (var z in 配置.保存的建筑)
                        {
                            args.Player.SendInfoMessage($"[{z.编号}]{z.备注}\n");
                        }
                        return;
                    }
                    if (配置.保存的建筑.Exists(s => (Convert.ToString(s.编号)) == args.Parameters[0]))
                    {
                        var z = 配置.保存的建筑.Find(s => s.编号 == Convert.ToInt32(args.Parameters[0])).grid;
                        Paste(z, args.Player.TileX, args.Player.TileY);
                        Paste(z, args.Player.TileX, args.Player.TileY);//粘贴2遍防止家具等没放好
                        args.Player.SendInfoMessage($"[复制粘贴]粘贴成功！");
                    }
                    else
                    {
                        args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{args.Parameters[0]}]的建筑");
                    }
                    return;
                }
                if (args.Parameters.Count >= 3)
                {
                    if (配置.保存的建筑.Exists(s => (Convert.ToString(s.编号)) == args.Parameters[0]))
                    {
                        var z = 配置.保存的建筑.Find(s => s.编号 == Convert.ToInt32(args.Parameters[0])).grid;
                        Paste(z, Convert.ToInt32(args.Parameters[1]), Convert.ToInt32(args.Parameters[2]));
                        Paste(z, args.Player.TileX, args.Player.TileY);//粘贴2遍防止家具等没放好
                        args.Player.SendInfoMessage($"[复制粘贴]粘贴成功！");
                    }
                    else
                    {
                        args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{args.Parameters[0]}]的建筑");
                    }
                    return;
                }
            }
            catch
            {
                args.Player.SendErrorMessage($"[复制粘贴]粘贴发送错误！");

            }
        }

        private static async void 指令3(CommandArgs args)
        {
            try
            {
                if (配置.启用远程复制)
                {
                    if (args.Parameters.Count == 0)
                    {
                        args.Player.SendInfoMessage($"正确指令：/远程粘贴 <服务器编号> <建筑编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                        args.Player.SendInfoMessage($"正确指令：/远程粘贴 <服务器编号> <建筑编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                        return;
                    }
                    if (配置.远程服务器设置.Exists(s => Convert.ToString(s.编号) == args.Parameters[0]))
                    {
                        var s = 配置.远程服务器设置.Find(s => Convert.ToString(s.编号) == args.Parameters[0]);
                        await HQ(args, s);
                    }
                    else
                    {
                        args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{args.Parameters[0]}]的远程服务器");
                    }
                }
                else
                {
                    args.Player.SendErrorMessage($"[复制粘贴]该功能未启用！");
                }
            }
            catch
            {
                args.Player.SendErrorMessage($"[复制粘贴]远程粘贴发送错误！");
            }
        }
        private object REST1(RestRequestArgs args)//瞎写的
        {
            RestObject restObject = new RestObject();
            List<复制粘贴配置表.数据> F = new() { };
            restObject.Status = "200";
            foreach (var z in 配置.保存的建筑)
            {
                F.Add(z);
            }
            restObject.Add("result", F);
            return restObject;
        }
        public class 远程配置格式
        {
            public string status = "200";
            public List<复制粘贴配置表.数据> result = new() { };
        }
        static readonly HttpClient client = new HttpClient();

        private static async Task HQ(CommandArgs args, 复制粘贴配置表.远程 yc)//瞎写的
        {
            string responseBody = "";

            try
            {
                responseBody = await client.GetStringAsync("http://" + yc.IP + ":" + yc.REST端口 + "/fzzt/list?token=" + yc.秘钥);
                var 远程配置 = JsonConvert.DeserializeObject<远程配置格式>(responseBody);
                if (远程配置 == null)
                {
                    args.Player.SendErrorMessage($"[复制粘贴]远程粘贴发生错误！");
                    return;
                }
                if (args.Parameters[1] == "列表" || args.Parameters[1] == "list")
                {
                    args.Player.SendInfoMessage($"建筑列表：\n");
                    foreach (var z1 in 远程配置.result)
                    {
                        args.Player.SendInfoMessage($"[{z1.编号}]{z1.备注}\n");
                    }
                    return;
                }
                if (args.Parameters.Count == 2)
                {
                    if (远程配置.result.Exists(s => (Convert.ToString(s.编号)) == args.Parameters[1]))
                    {
                        var z = 远程配置.result.Find(s => s.编号 == Convert.ToInt32(args.Parameters[1])).grid;
                        Paste(z, args.Player.TileX, args.Player.TileY);
                        Paste(z, args.Player.TileX, args.Player.TileY);//粘贴2遍防止家具等没放好
                        args.Player.SendInfoMessage($"[复制粘贴]远程粘贴成功！");
                    }
                    else
                    {
                        args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{args.Parameters[1]}]的建筑");
                    }
                    return;
                }
                if (args.Parameters.Count >= 4)
                {
                    if (远程配置.result.Exists(s => (Convert.ToString(s.编号)) == args.Parameters[1]))
                    {
                        var z = 远程配置.result.Find(s => s.编号 == Convert.ToInt32(args.Parameters[1])).grid;
                        Paste(z, Convert.ToInt32(args.Parameters[2]), Convert.ToInt32(args.Parameters[3]));
                        Paste(z, args.Player.TileX, args.Player.TileY);//粘贴2遍防止家具等没放好
                        args.Player.SendInfoMessage($"[复制粘贴]远程粘贴成功！");
                    }
                    else
                    {
                        args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{args.Parameters[0]}]的建筑");
                    }
                    return;
                }
            }
            catch
            {
            }
        }
        public static List<复制粘贴配置表.图格数据> Copy(int X1, int Y1, int X2, int Y2)
        {
            try
            {
                List<复制粘贴配置表.图格数据> 复制 = new() { };
                if (X1 >= X2 && Y1 >= Y2)
                {
                    int X;
                    X = X1;
                    X1 = X2;
                    X2 = X;
                    int Y;
                    Y = Y1;
                    Y1 = Y2;
                    Y2 = Y;
                }
                if (X1 >= X2 && Y1 <= Y2)
                {
                    int X;
                    X = X1;
                    X1 = X2;
                    X2 = X;
                }
                if (X1 <= X2 && Y1 >= Y2)
                {
                    int Y;
                    Y = Y1;
                    Y1 = Y2;
                    Y2 = Y;
                }
                int Y3;
                Y3 = Y1;
                int X4 = X1;
                int Y4 = Y1;
                for (; X1 <= X2; X1++)
                {
                    Y1 = Y3;
                    for (; Y1 <= Y2; Y1++)
                    {
                        var t = Main.tile[X1, Y1];
                        byte bTileHeader = t.bTileHeader;
                        byte bTileHeader2 = t.bTileHeader2;
                        byte bTileHeader3 = t.bTileHeader3;
                        //var collisionType = t.collisionType;
                        short frameX = t.frameX;
                        short frameY = t.frameY;
                        byte liquid = t.liquid;
                        ushort sTileHeader = t.sTileHeader;
                        ushort type = t.type;
                        ushort wall = t.wall;
                        /*
                        bool active = t.active();
                        bool actuator = t.actuator();
                        int blockType = t.blockType();
                        bool bottomSlope = t.bottomSlope();
                        bool checkingLiquid = t.checkingLiquid();
                        object Clone = t.Clone();
                        byte color = t.color();
                        byte frameNumber = t.frameNumber();
                        bool fullbrightBlock = t.fullbrightBlock();
                        bool fullbrightWall = t.fullbrightWall();
                        t.halfBrick();
                        t.honey();
                        t.inActive();
                            */
                        复制.Add(new()
                        {
                            X = X1 - X4,
                            Y = Y1 - Y4,
                            bTileHeader = bTileHeader,
                            bTileHeader2 = bTileHeader2,
                            bTileHeader3 = bTileHeader3,
                            //collisionType = collisionType,
                            frameX = frameX,
                            frameY = frameY,
                            liquid = liquid,
                            sTileHeader = sTileHeader,
                            type = type,
                            wall = wall,
                            /*
                            active = active,
                            actuator = actuator,
                            blockType = blockType,
                            bottomSlope = bottomSlope,
                            checkingLiquid = checkingLiquid,
                            Clone = Clone,
                            color = color,
                            frameNumber = frameNumber,
                            fullbrightBlock = fullbrightBlock,
                            fullbrightWall = fullbrightWall,
                            */
                        }); ;
                    }
                }
                return 复制;
            }
            catch
            {
                return new() { };
            }
        }
        public static void Paste(List<复制粘贴配置表.图格数据> tg, int X1, int Y1)
        {
            try
            {
                foreach (var t in tg)
                {
                    var X = t.X + X1;
                    var Y = t.Y + Y1;
                    var z = Main.tile[X, Y];
                    z.bTileHeader = t.bTileHeader;
                    z.bTileHeader2 = t.bTileHeader2;
                    z.bTileHeader3 = t.bTileHeader3;
                    // z.collisionType = t.collisionType;
                    z.frameX = t.frameX;
                    z.frameY = t.frameY;
                    z.liquid = t.liquid;
                    z.sTileHeader = t.sTileHeader;
                    z.type = t.type;
                    z.wall = t.wall;
                    /*
                    var X = z.X;
                    var Y = z.Y;
                    var ID = z.图格ID;
                    var 子ID = z.图格子ID;
                    var yt = z.液体;
                    var q = z.墙;
                    if (yt == 0)
                    {
                        WorldGen.PlaceTile(X, Y, ID, false, false, -1, 子ID);
                        // WorldGen.PlaceDoor(X, Y, ID, 子ID);
                        //WorldGen.PlaceChest(X, Y, ID, false, 子ID);
                    }
                    else
                    {
                        WorldGen.PlaceLiquid(X, Y, (byte)ID, (byte)yt);
                    }
                    WorldGen.PlaceWall(X, Y, q);
                    var t = Main.tile[X1, Y1];
                    t.type = ;
                    */
                    TSPlayer.All.SendTileSquare(X, Y);
                    //NetMessage.SendData(20, TShock.Players, -1, null, tileX, tileY, xSize, ySize, (int)changeType);
                }
            }
            catch
            {

            }

        }
        private void 重载(CommandArgs args)
        {
            try
            {
                Reload();
                args.Player.SendErrorMessage($"[复制粘贴]重载成功！");
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[复制粘贴]配置文件读取错误");
            }
        }
        public static void Reload()
        {
            try
            {
                配置 = JsonConvert.DeserializeObject<复制粘贴配置表>(File.ReadAllText(Path.Combine(TShock.SavePath, "复制粘贴配置表.json")));
                File.WriteAllText(path, JsonConvert.SerializeObject(配置, Formatting.Indented));
            }
            catch
            {
                TSPlayer.Server.SendErrorMessage($"[复制粘贴]配置文件读取错误");
            }
        }
    }
}