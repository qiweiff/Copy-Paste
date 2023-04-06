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
        public override Version Version => new(2, 0, 0, 0);
        /// 插件处理
        public 复制粘贴(Main game) : base(game)
        {
        }
        //插件启动时，用于初始化各种狗子
        //public const string path = "tshock/复制粘贴.json";
        //internal static string 复制粘贴数据表 = "tshock/复制粘贴数据表.json";
        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);//钩住游戏初始化时
            DB.Reload();
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
            Commands.ChatCommands.Add(new Command("复制粘贴", 复制指令, "复制", "fz") { });
            Commands.ChatCommands.Add(new Command("复制粘贴", 粘贴指令, "粘贴", "zt") { });
        }
        private static void 复制指令(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0)
                {
                    args.Player.SendInfoMessage($"正确指令：/复制 <1/2> --选择复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/复制 <确定/取消> <备注> --确定复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 删除 <编号> --删除复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 列表 <页码> --查看复制的区域");
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
                            int ID = DB.获取最大ID();
                            string 备注 = "";
                            if (args.Parameters.Count >= 2)
                            {
                                备注 = args.Parameters[1];
                            }
                            Task.Run(() =>
                            {
                                Copy(args.Player.TempPoints[0].X, args.Player.TempPoints[0].Y, args.Player.TempPoints[1].X, args.Player.TempPoints[1].Y, ID, 备注);
                                args.Player.SendInfoMessage($"复制成功，新建筑的编号为[c/FF3333:{ID}]！");
                            });
                        }
                        break;
                    case "取消":
                        args.Player.TempPoints[0].X = 0;
                        args.Player.TempPoints[0].Y = 0;
                        args.Player.TempPoints[1].X = 0;
                        args.Player.TempPoints[1].Y = 0;
                        args.Player.AwaitingTempPoint = 0;
                        args.Player.SendInfoMessage($"取消成功！");
                        break;
                    default:
                        args.Player.SendInfoMessage($"正确指令：/复制 <1/2> --选择复制的区域");
                        args.Player.SendInfoMessage($"正确指令：/复制 <确定/取消> <备注> --确定复制的区域");
                        args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                        args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                        args.Player.SendInfoMessage($"正确指令：/粘贴 删除 <编号> --删除复制的区域");
                        args.Player.SendInfoMessage($"正确指令：/粘贴 列表 <页码> --查看复制的区域");
                        break;
                }
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage("[复制粘贴]复制发生错误！");
                TShock.Log.ConsoleError("[复制粘贴]复制发生错误！" + ex.Message);
            }
        }
        private static void 粘贴指令(CommandArgs args)
        {
            try
            {
                if (args.Parameters.Count == 0)
                {
                    args.Player.SendInfoMessage($"正确指令：/复制 <1/2> --选择复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/复制 <确定/取消> <备注> --确定复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> --选择粘贴的建筑编号，往玩家当前位置的右下角粘贴");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 <编号> X坐标 Y坐标 --选择粘贴的建筑编号，往输入坐标的位置向右下角粘贴");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 删除 <编号> --删除复制的区域");
                    args.Player.SendInfoMessage($"正确指令：/粘贴 列表 <页码> --查看复制的区域");
                    return;
                }
                switch (args.Parameters[0])
                {
                    case "列表":
                    case "list":
                        {
                            List<建筑数据> 建筑列表 = DB.获取所有建筑备注();
                            args.Player.SendInfoMessage($"建筑列表：\n");
                            int s;
                            int y;
                            if (args.Parameters.Count <= 1)
                            {
                                y = 1;
                                s = 0;
                            }
                            else
                            {
                                if (int.TryParse(args.Parameters[1], out y) && int.TryParse(args.Parameters[1], out s))
                                {
                                    s--;
                                }
                                else
                                {
                                    args.Player.SendErrorMessage("这页没有建筑!!");
                                    return;
                                }
                            }
                            if (y <= 0)
                            {
                                args.Player.SendErrorMessage("这页没有建筑");
                                return;
                            }
                            int i = 0;
                            for (s = 10 * s; s < 建筑列表.Count; s++, i++)
                            {
                                if (i >= 10)
                                {
                                    if (建筑列表.Count >= s + 1)
                                    {
                                        args.Player.SendInfoMessage($"输入\"/复制 list {y + 1}\"查看下一页");
                                        return;
                                    }
                                    return;
                                }
                                else
                                {
                                    args.Player.SendMessage($"[[c/FF0000:{建筑列表[s].编号}.]]{建筑列表[s].备注}", 255, 255, 255);
                                }
                            }
                            args.Player.SendInfoMessage("已展示所有建筑");
                            break;
                        }
                    case "del":
                    case "删除":
                        {
                            if (!int.TryParse(args.Parameters[1], out int id))
                            {
                                args.Player.SendInfoMessage($"[复制粘贴]编号应该是数字!");
                                return;
                            }
                            if (DB.获取建筑备注(id) == null)
                            {
                                args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{id}]的建筑");
                                return;
                            }
                            DB.删除图格信息(id);
                            DB.删除建筑信息(id);
                            args.Player.SendInfoMessage($"[复制粘贴]删除成功！");
                            break;
                        }
                    default:
                        {
                            if (!int.TryParse(args.Parameters[0], out int id))
                            {
                                args.Player.SendInfoMessage($"[复制粘贴]编号应该是数字!");
                                return;
                            }
                            if (DB.获取建筑备注(id) == null)
                            {
                                args.Player.SendInfoMessage($"[复制粘贴]没有找到编号为[c/FF3333:{id}]的建筑");
                                return;
                            }
                            if (args.Parameters.Count == 1)
                            {
                                Task.Run(() =>
                                {
                                    DB.粘贴_获取图格信息(id, args.Player.TileX, args.Player.TileY);
                                    DB.粘贴_获取图格信息(id, args.Player.TileX, args.Player.TileY);//粘贴2遍防止家具等没放好
                                    更新物块();
                                    args.Player.SendInfoMessage($"[复制粘贴]粘贴成功！");
                                });
                            }
                            else if (args.Parameters.Count >= 3)
                            {
                                if (!int.TryParse(args.Parameters[1], out int x1))
                                {
                                    args.Player.SendInfoMessage($"[复制粘贴]坐标应该是数字!");
                                    return;
                                }
                                if (!int.TryParse(args.Parameters[2], out int y1))
                                {
                                    args.Player.SendInfoMessage($"[复制粘贴]坐标应该是数字!");
                                    return;
                                }
                                Task.Run(() =>
                                {
                                    DB.粘贴_获取图格信息(id, x1, y1);
                                    DB.粘贴_获取图格信息(id, x1, y1);//粘贴2遍防止家具等没放好
                                    更新物块();
                                    args.Player.SendInfoMessage($"[复制粘贴]粘贴成功！");
                                });
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage("[复制粘贴]粘贴发生错误！");
                TShock.Log.ConsoleError("[复制粘贴]粘贴发生错误！" + ex.ToString());
            }
        }
        public static void Copy(int X1, int Y1, int X2, int Y2, int ID, string 备注)
        {
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
                    short frameX = t.frameX;
                    short frameY = t.frameY;
                    byte liquid = t.liquid;
                    ushort sTileHeader = t.sTileHeader;
                    ushort type = t.type;
                    ushort wall = t.wall;
                    DB.添加图格信息(ID, new()
                    {
                        X = X1 - X4,
                        Y = Y1 - Y4,
                        bTileHeader = bTileHeader,
                        bTileHeader2 = bTileHeader2,
                        bTileHeader3 = bTileHeader3,
                        frameX = frameX,
                        frameY = frameY,
                        liquid = liquid,
                        sTileHeader = sTileHeader,
                        type = type,
                        wall = wall,
                    });
                }
            }
            DB.添加建筑信息(ID, 备注);
        }
        public static void Paste(图格数据 tg, int X1, int Y1)
        {
            var t = tg;
            var X = t.X + X1;
            var Y = t.Y + Y1;
            var z = Main.tile[X, Y];
            z.bTileHeader = t.bTileHeader;
            z.bTileHeader2 = t.bTileHeader2;
            z.bTileHeader3 = t.bTileHeader3;
            z.frameX = t.frameX;
            z.frameY = t.frameY;
            z.liquid = t.liquid;
            z.sTileHeader = t.sTileHeader;
            z.type = t.type;
            z.wall = t.wall;
            //TSPlayer.All.SendTileSquareCentered(X, Y);
        }
        public class 建筑数据
        {
            public int 编号 = 0;
            public string 备注 = "无备注";
            public List<图格数据> grid = new() { };
        }
        private static void 更新物块()
        {
            foreach (TSPlayer person in TShock.Players)
            {
                if ((person != null) && (person.Active))
                {
                    for (int i = 0; i < 255; i++)
                    {
                        for (int j = 0; j < Main.maxSectionsX; j++)
                        {
                            for (int k = 0; k < Main.maxSectionsY; k++)
                            {
                                Netplay.Clients[i].TileSections[j, k] = false;
                            }
                        }
                    }
                }
            }
        }
        public class 图格数据
        {
            public int X;
            public int Y;
            public byte bTileHeader;
            public byte bTileHeader2;
            public byte bTileHeader3;
            public short frameX;
            public short frameY;
            public byte liquid;
            public ushort sTileHeader;
            public ushort type;
            public ushort wall;
        }
    }
}