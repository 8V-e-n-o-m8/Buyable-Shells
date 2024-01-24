using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LethalLib.Modules.Levels;

namespace BuyableShells
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Venom.BuyableShells";

        private const string modName = "BuyableShells";

        private const string modVersion = "0.0.1";

        private readonly Harmony harmony = new Harmony("Venom.BuyableShells");

        internal static Plugin Instance;

        public static ManualLogSource mls;

        public bool added;

        public static ConfigEntry<int> ShellPrice;

        public List<Item> AllItems => Resources.FindObjectsOfTypeAll<Item>().Concat(UnityEngine.Object.FindObjectsByType<Item>((FindObjectsInactive)1, (FindObjectsSortMode)1)).ToList();

        public Item ShotgunShell => ((IEnumerable<Item>)AllItems).FirstOrDefault((Func<Item, bool>)((Item item) => ((UnityEngine.Object)item).name == "GunAmmo"));

        private void Awake()
        {
            if ((UnityEngine.Object)(object)Instance == (UnityEngine.Object)null)
            {
                Instance = this;
            }
            harmony.PatchAll(typeof(Plugin));
            mls = BepInEx.Logging.Logger.CreateLogSource("Venom.BuyableShells");
            mls.LogInfo((object)$"{modGUID} started successfull");
            SceneManager.sceneLoaded += OnSceneLoaded;
            ShellPrice = ((BaseUnityPlugin)this).Config.Bind<int>("Settings", "Shell Price", 20, "How many credits the shotgun shell costs");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!added && ((Scene)(scene)).name == "MainMenu")
            {
                added = true;
                ShotgunShell.itemName = "Shells";
                ShotgunShell.creditsWorth = 0;
                Items.RegisterShopItem(ShotgunShell, ShellPrice.Value);
            }
        }
    }
}
