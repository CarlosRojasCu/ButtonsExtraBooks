﻿using System;
using System.Net.Mime;
using ButtonsExtraBooks.Helpers;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace ButtonsExtraBooks.Config;

public sealed class ModConfig
{
    public bool AlwaysAvailable { get; set; } = false;
    
    public bool EnableLuck { get; set; } = true;
    public int LuckPrice { get; set; } = 25000;
    public float LuckAmount { get; set; } = 0.025f;
    
    public bool EnableExtraGifts { get; set; } = true;
    public  int ExtraGiftsPrice { get; set; } = 50000;
    
    public bool EnableTreesIgnoreSeason { get; set; } = true;
    public int TreesIgnoreSeasonPrice { get; set; } = 20000;
    
    public bool EnableArtisanMachines { get; set; } = true;
    public int ArtisanMachinesPrice { get; set; } = 100000;
    public bool ArtisanMachinesGrangeMustWin { get; set; } = true;
    public int ArtisanMachinesPercentDecrease { get; set; } = 10;
    
    public bool EnableGiantCrops { get; set; } = true;
    public int GiantCropsPrice { get; set; } = 30000;
    public int GiantCropsPercent { get; set; } = 5;
    
    public bool EnablePopularity { get; set; } = true;
    public int PopularityPrice { get; set; } = 35000;

    public bool EnableBusDriving { get; set; } = true;
    public int BusDrivingPrice { get; set; } = 42500;
    
    public bool EnableQiNotebook { get; set; } = true;
    public int QiNotebookPrice { get; set; } = 10;
    public float QiNotebookPercent { get; set; } = 10f;
    
    public bool EnablePetGifts { get; set; } = true;
    public int PetGiftsPrice { get; set; } = 15000;
    
    public bool EnableCheatCodes { get; set; } = true;
    public int CheatCodesPrice { get; set; } = 65536;
    public int CheatCodesRequirement { get; set; } = 30;
    public int CheatCodesLives { get; set; } = 1;
    
    public bool EnableJunimoScrap { get; set; } = true;
    public int JunimoScrapPrice { get; set; } = 25000;
    public bool JunimoRandomItems { get; set; } = false;
    
    public bool EnableCarols { get; set; } = true;
    public int CarolsPrice { get; set; } = 10000;
    
    public bool EnableCoffee { get; set; } = true;
    public int CoffeePrice { get; set; } = 75000;
    
    public bool EnableSketchbook { get; set; } = true;
    public int SketchbookPrice { get; set; } = 30000;
    
    public bool DebugBook { get; set; } = false;

    public ModConfig()
    {
        Init();
    }

    private void Init()
    {
        ResetAllEnabled();
        ResetAllPrice();
        AlwaysAvailable = false;
        LuckAmount = 0.025f;
        ArtisanMachinesPercentDecrease = 10;
        ArtisanMachinesGrangeMustWin = true;
        GiantCropsPercent = 5;
        QiNotebookPercent = 10f;
        CheatCodesRequirement = 30;
        CheatCodesLives = 1;
        JunimoRandomItems = false;
        DebugBook = false;
    }

    public void ResetAllEnabled()
    {
        foreach (var property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(bool) && property.Name.StartsWith("Enable"))
            {
                property.SetValue(this, true);
            }
        }
    }

    public void ResetAllPrice()
    {
        LuckPrice = 25000;
        ExtraGiftsPrice = 50000;
        TreesIgnoreSeasonPrice = 20000;
        ArtisanMachinesPrice = 100000;
        GiantCropsPrice = 30000;
        PopularityPrice = 35000;
        BusDrivingPrice = 42500;
        QiNotebookPrice = 10;
        PetGiftsPrice = 15000;
        CheatCodesPrice = 65536;
        JunimoScrapPrice = 25000;
        CarolsPrice = 10000;
        CoffeePrice = 75000;
        SketchbookPrice = 30000;
    }
    
    public bool GetPowerEnabled(string powerName)
    {
        return (bool)GetType().GetProperty($"Enable{powerName}")?.GetValue(this)!;
    }
    
    public int GetBookPrice(string bookName)
    {
        return (int)GetType().GetProperty($"{bookName}Price")?.GetValue(this)!;
    }

    public string GetLanguageCode()
    {
        return LocalizedContentManager.CurrentLanguageCode.ToString() != "mod" ? LocalizedContentManager.CurrentLanguageCode.ToString() : LocalizedContentManager.CurrentModLanguage.LanguageCode;
    }
    
    public Func<string> TryGetI18n(string key)
    {
        return () =>
        {
            Log.Info($"Trying to get i18n for {key} and language code {GetLanguageCode()}");
            if (ModEntry.ContentPackI18n.TryGetValue(GetLanguageCode(), out var i18nStrings))
            {
                Log.Info("Found language code");
                if (i18nStrings.TryGetValue(key, out var i18nString))
                {
                    return i18nString;
                }
            }
            
            return "Missing translation key!";
        };
    }

    public void SetupConfig(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper, Harmony harmony)
    {
        configMenu.Register(
            mod: ModManifest,
            reset: Init,
            save: () => Helper.WriteConfig(this)
        );
        SetupPages(configMenu, ModManifest, Helper, harmony);
        SetupGeneral(configMenu, ModManifest, Helper, harmony);
        SetupEnableDisable(configMenu, ModManifest);
        SetupPrices(configMenu, ModManifest);
        SetupAdjustments(configMenu, ModManifest, harmony);
        SetupDebug(configMenu, ModManifest, Helper, harmony);
    }
    
    private static void SetupPages(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.General",
            text: i18n.Config_SectionTitle_General
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.EnableDisable",
            text: i18n.Config_SectionTitle_EnableDisable
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Prices",
            text: i18n.Config_SectionTitle_Prices
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Adjustments",
            text: i18n.Config_SectionTitle_Adjustments
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Debug",
            text: i18n.Config_SectionTitle_Debug
        );
    }
    
    private void SetupGeneral(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.General",
            pageTitle: i18n.Config_SectionTitle_General
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_General_AlwaysAvailable_Name,
            tooltip: i18n.Config_General_AlwaysAvailable_Description,
            getValue: () => AlwaysAvailable,
            setValue: value => AlwaysAvailable = value
        );
    }

    private void SetupEnableDisable(IGenericModConfigMenuApi configMenu, IManifest ModManifest)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.EnableDisable",
            pageTitle: i18n.Config_SectionTitle_EnableDisable
        );
        foreach (var property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(bool) && property.Name.StartsWith("Enable"))
            {
                string bookName = property.Name.Substring(6);
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => i18n.GetByKey($"Config.Books.{bookName}.Title"),
                    tooltip: i18n.Config_General_Enabled_Description,
                    getValue: () => (bool)property.GetValue(this)!,
                    setValue: value => property.SetValue(this, value)
                );
            }
        }
    }
    
    private void SetupPrices(IGenericModConfigMenuApi configMenu, IManifest ModManifest)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Prices",
            pageTitle: i18n.Config_SectionTitle_Prices
        );
        foreach (var property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(int) && property.Name.EndsWith("Price"))
            {
                string bookName = property.Name.Substring(0, property.Name.Length - 5);
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => i18n.GetByKey($"Config.Books.{bookName}.Title")!,
                    tooltip: i18n.Config_General_Price_Description,
                    getValue: () => (int)property.GetValue(this)!,
                    setValue: value => property.SetValue(this, value)
                );
            }
        }
    }

    private void SetupAdjustments(IGenericModConfigMenuApi configMenu, IManifest ModManifest, Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Adjustments",
            pageTitle: i18n.Config_SectionTitle_Adjustments
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: TryGetI18n("Luck.Book.Name")
            // text: i18n.Config_Books_Luck_Title
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_LuckBonus_Name,
            tooltip: i18n.Config_Books_LuckBonus_Description,
            getValue: () => LuckAmount,
            setValue: value => LuckAmount = value
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: i18n.Config_Books_ArtisanMachines_Title
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_ArtisanMachinesGrangeMustWin_Name,
            tooltip: i18n.Config_Books_ArtisanMachinesGrangeMustWin_Description,
            getValue: () => ArtisanMachinesGrangeMustWin,
            setValue: value => ArtisanMachinesGrangeMustWin = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_ArtisanMachinesBonus_Name,
            tooltip: i18n.Config_Books_ArtisanMachinesBonus_Description,
            getValue: () => ArtisanMachinesPercentDecrease,
            setValue: (value) =>
            {
                ArtisanMachinesPercentDecrease = value;
                harmony.UnpatchAll(ModManifest.UniqueID);
                harmony.PatchAll();
            },
            min: 1,
            max: 50,
            interval: 1,
            formatValue: (value) => $"{value}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: i18n.Config_Books_GiantCrops_Title
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_GiantCropsBonus_Name,
            tooltip: i18n.Config_Books_GiantCropsBonus_Description,
            getValue: () => GiantCropsPercent,
            setValue: value => GiantCropsPercent = value,
            min: 2,
            max: 100,
            interval: 1,
            formatValue: value => $"{value}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: i18n.Config_Books_QiNotebook_Title
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_QiNotebookBonus_Name,
            tooltip: i18n.Config_Books_QiNotebookBonus_Description,
            getValue: () => QiNotebookPercent,
            setValue: value => QiNotebookPercent = value,
            min: 1,
            max: 100,
            interval: 1,
            formatValue: value => $"{value / 10}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: i18n.Config_Books_CheatCodes_Title
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_CheatCodesRequirement_Name,
            tooltip: i18n.Config_Books_CheatCodesRequirement_Description,
            getValue: () => CheatCodesRequirement,
            setValue: value => CheatCodesRequirement = value,
            min: 1,
            max: 100,
            interval: 1
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: i18n.Config_Books_CheatCodesBonus_Name,
            tooltip: i18n.Config_Books_CheatCodesBonus_Description,
            getValue: () => CheatCodesLives,
            setValue: value => CheatCodesLives = value,
            min: 1,
            max: 100,
            interval: 1
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: i18n.Config_Books_JunimoScrapbook_Title
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Books_JunimoScrapbookRandom_Name,
            tooltip: i18n.Config_Books_JunimoScrapbookRandom_Description,
            getValue: () => JunimoRandomItems,
            setValue: value => JunimoRandomItems = value
        );
    }
    
    private void SetupDebug(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Debug",
            pageTitle: i18n.Config_SectionTitle_Debug
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: i18n.Config_Debug_EnableDebugBook_Name,
            tooltip: i18n.Config_Debug_EnableDebugBook_Description,
            getValue: () => DebugBook,
            setValue: value => DebugBook = value
        );
    }
}