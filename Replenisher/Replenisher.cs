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
    [ApiVersion(1, 18)]
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
            if (args.Parameters.Count >= 2 && Enum.TryParse<GenType>(args.Parameters[0], true, out type) && int.TryParse(args.Parameters[1], out amount))
            {
                if (amount <= 0)
                {
                    args.Player.SendErrorMessage("Please enter an amount greater than zero.");
                    return;
                }
                switch(type)
                {
                    case GenType.ore:
                        break;
                    case GenType.chests:
                        int[] NonHMChestItems = new int[] { 997, 49, 50, 53, 54, 55, 975, 930 };
                        for (int i = 0; i < TIMEOUT; i++)
                        {
                            int item = NonHMChestItems[WorldGen.genRand.Next(0, NonHMChestItems.Length + 1)];
                            //WorldGen.AddBuriedChest()
                        }
                            break;
                    case GenType.pots:
                        int potcounter = 0;
                        for (int i = 0; i < TIMEOUT; i++)
                        {
                            if (WorldGen.PlacePot(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(Main.worldSurface) - 12, Main.maxTilesY)))
                            {
                                potcounter++;
                                if (potcounter >= amount)
                                {
                                    args.Player.SendInfoMessage("Pots generated successfully.");
                                    return;
                                }
                            }
                        }
                        args.Player.SendErrorMessage("Failed to generate all the pots. Generated " + potcounter + " pots.");
                            break;
                    case GenType.lifecrystals:
                        break;
                    case GenType.altars:
                        break;
                }
            }
            else
            {
                args.Player.SendErrorMessage("Incorrect usage. Correct usage: /replen <ore|chests|pots|lifecrystals|altars> <amount>");
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