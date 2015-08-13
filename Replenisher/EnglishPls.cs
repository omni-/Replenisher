using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace EnglishPls
{
    [ApiVersion(1, 21)]
    public class EnglishPls : TerrariaPlugin
    {
        public Config cfg = new Config();

        public EnglishPls(Main game)
            : base(game)
        {
        }
        public override Version Version
        {
            get { return new Version("1.0"); }
        }
        public override string Name
        {
            get { return "EnglishPls"; }
        }
        public override string Author
        {
            get { return "omni"; }
        }
        public override string Description
        {
            get { return "Only allow English names."; }
        }

        public override void Initialize()
        {
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            Commands.ChatCommands.Add(new Command("tshock.cfg.reload", ConfigReload, "epreload"));
            if (!ReadConfig())
                TShock.Log.ConsoleError("Error in config file.");
        }

        #region config
        private void CreateConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "EnglishPlsConfig.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        cfg = new Config();
                        var configString = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
                cfg = new Config();
            }
        }

        private bool ReadConfig()
        {
            string filepath = Path.Combine(TShock.SavePath, "EnglishPlsConfig.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            cfg = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    TShock.Log.ConsoleError("EnglishPls config not found. Creating new one...");
                    CreateConfig();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
            }
            return false;
        }

        private void ConfigReload(CommandArgs args)
        {
            if (ReadConfig())
                args.Player.SendSuccessMessage("EnglishPls config reloaded.");
            else
                args.Player.SendErrorMessage("Error reading config. Check log for details.");
            return;
        }
        #endregion

        private void OnJoin(JoinEventArgs e)
        {
            var plr = TShock.Players[e.Who];
            if (!IsValid(plr.Name))
                TShock.Utils.Kick(plr, "Please use valid English for your character name.", false, true);
        }
        private bool IsValid(string s)
        {
            string cset = "", 
                   legalA = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 
                   legalS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-=!@#$%^&*()_+`~[]\\{}|;':\",./<>?";

            if (cfg.CustomCharset != "null")
                cset = cfg.AllowSymbols ? legalS : legalA;
            else
                cset = cfg.CustomCharset;

            foreach(char c in s)
                if (!cset.Contains(c))
                    return false;
            return true;
        }
    }
    public class Config
    {
        public bool AllowSymbols = true;
        public string CustomCharset = "null";
    }
}
