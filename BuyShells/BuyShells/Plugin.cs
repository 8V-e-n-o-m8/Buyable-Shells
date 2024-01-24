using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Неймспейс */
namespace BuyableShells
{
    /* Наследуемся от BaseUnityPlugin - передаем обязательные параметры */
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        /* Уникальный идентификатор мода */
        private const string modGUID = "Venom.BuyableShells";

        /* Название мода */
        private const string modName = "BuyableShells";

        /* Версия мода */
        private const string modVersion = "1.0.0";

        /* Readnonly бибилиотека Harmony */
        private readonly Harmony harmony = new Harmony(modGUID);

        /* Инстанс текущего класса */
        internal static Plugin Instance;

        /* Переменная для логов в консоль */
        public static ManualLogSource mls;

        /* bool переменная - идентификатор загрузки патронов в магаз */
        public bool added;

        /* Переменная конфига */
        public static ConfigEntry<int> ShellPrice;

        /* Коллекция всех предметов */
        public List<Item> AllItems => Resources.FindObjectsOfTypeAll<Item>().Concat(UnityEngine.Object.FindObjectsByType<Item>((FindObjectsInactive)1, (FindObjectsSortMode)1)).ToList();

        /* Переменная с объектом патронов для шотгана */
        public Item ShotgunShell => ((IEnumerable<Item>)AllItems).FirstOrDefault((Func<Item, bool>)((Item item) => ((UnityEngine.Object)item).name == "GunAmmo"));

        /* Инициализирующий метод - поднимаем мод */
        private void Awake()
        {
            /* Если наш мод еще не проинициализирован */
            if ((UnityEngine.Object)(object)Instance == (UnityEngine.Object)null)
            {
                /* Сетапим инстанс - переменной текущего класса */
                Instance = this;
            }
            
            /* Патчим все классы - типа Plugin */
            harmony.PatchAll(typeof(Plugin));
            
            /* Указываем для какого плагина будет логировать */
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            
            /* Проверяем - если еще не добавлены патроны в магаз - добавляем */
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            /* Добавляем в конфиг патроны, указываем название и цену */
            ShellPrice = ((BaseUnityPlugin)this).Config.Bind<int>("Settings", "Shell Price", 20, "How many credits the shotgun shell costs");
            
            /* После всех проверок - указываем что мод стартанул */
            mls.LogInfo((object)$"{modName} started successfull");
        }
        
        /* Метод регистрирует патроны для дробовика в магазине */
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            /* Если патроны еще не добавлены и мы в главном меню */
            if (!added && ((Scene)(scene)).name == "MainMenu")
            {
                /* Сетапим флаг что патроны добавлены */
                added = true;
                
                /* Прописываем им имя для магаза */
                ShotgunShell.itemName = "Shells";
                
                /* Старторая цена - 0 (Предположительно) */
                ShotgunShell.creditsWorth = 0;
                
                /* Регистрируем патроны в магазине */
                Items.RegisterShopItem(ShotgunShell, ShellPrice.Value);
            }
        }
    }
}