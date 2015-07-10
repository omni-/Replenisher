using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using System.IO;

namespace Replenisher
{
    [ApiVersion(1, 19)]
    public class Replenisher : TerrariaPlugin
    {
        private static readonly int TIMEOUT = 100000;

        public Replenisher(Main game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.world.causeevents", Replen, "replen"));
        }
        public override Version Version
        {
            get { return new Version("1.0"); }
        }
        public override string Name
        {
            get { return "Replenisher"; }
        }
        public override string Author
        {
            get { return "omni"; }
        }
        public override string Description
        {
            get { return "Replenish your world's resources!"; }
        }

        public void Replen(CommandArgs args)
        {
            GenType type = GenType.ore;
            int amount = -1;
            ushort oretype = 0;
            if (args.Parameters.Count >= 2 && Enum.TryParse<GenType>(args.Parameters[0], true, out type) && int.TryParse(args.Parameters[1], out amount))
            {
                if (amount <= 0)
                {
                    args.Player.SendErrorMessage("Please enter an amount greater than zero.");
                    return;
                }
                if (type == GenType.ore)
                {
                    if (args.Parameters.Count < 3)
                    {
                        args.Player.SendErrorMessage("Please enter a valid ore type.");
                        return;
                    }
                    var obj = new Terraria.ID.TileID();
                    oretype = (ushort)obj.GetType().GetField(args.Parameters[2].FirstCharToUpper()).GetValue(obj);
                }
                int counter = 0;
                bool success;
                for (int i = 0; i < TIMEOUT; i++)
                {
                    success = false;
                    int xRandBase = WorldGen.genRand.Next(1, Main.maxTilesX);
                    int y = 0;
                    switch (type)
                    {
                        case GenType.ore:
                            y = WorldGen.genRand.Next((int)(Main.worldSurface) - 12, Main.maxTilesY);
                            if (oretype != Terraria.ID.TileID.Hellstone)
                                WorldGen.OreRunner(xRandBase, y, 2.0, amount, oretype);
                            else
                                WorldGen.OreRunner(xRandBase, WorldGen.genRand.Next((int)(Main.maxTilesY) - 200, Main.maxTilesY), 2.0, amount, oretype);
                            success = true;
                            break;
                        case GenType.chests:
                            y = WorldGen.genRand.Next((int)(Main.worldSurface) - 200, Main.maxTilesY);
                            if (WorldGen.AddBuriedChest(xRandBase, y))
                                success = true;
                            break;
                        case GenType.pots:
                            y = WorldGen.genRand.Next((int)(Main.worldSurface) - 12, Main.maxTilesY);
                            if (WorldGen.PlacePot(xRandBase, y))
                                success = true;
                            break;
                        case GenType.lifecrystals:
                            y = WorldGen.genRand.Next((int)(Main.worldSurface) - 12, Main.maxTilesY);
                            if (WorldGen.AddLifeCrystal(xRandBase, y))
                                success = true;
                            break;
                        case GenType.altars:
                            y = WorldGen.genRand.Next((int)(Main.worldSurface) - 12, Main.maxTilesY);
                            WorldGen.Place3x2(xRandBase, y, 26);
                            if (Main.tile[xRandBase, y].type == 26)
                                success = true;
                            break;
                    }
                    if(success)
                    {
                        counter++;
                        if (counter >= amount)
                        {
                            args.Player.SendInfoMessage(type.ToString().FirstCharToUpper() + " generated successfully.");// [" + xRandBase + ", " + y + "]");
                            //args.Player.Teleport(xRandBase, y);
                            return;
                        }
                    }
                }
                args.Player.SendErrorMessage("Failed to generate all the " + type.ToString() + ". Generated " + counter + " " + type.ToString() + ".");
            }
            else
            {
                args.Player.SendErrorMessage("Incorrect usage. Correct usage: /replen <ore|chests|pots|lifecrystals|altars> <amount> (oretype)");
            }
        }
    }
    public enum GenType
    {
        ore,
        chests,
        pots,
        lifecrystals,
        altars
    }
}