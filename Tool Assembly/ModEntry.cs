using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.Inventories;
using StardewValley.Network;
using Netcode;
using StardewValley.GameData.BigCraftables;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.GameData.Locations;
using xTile;
using StardewValley.Objects;
using GenericModConfigMenu;
using StardewValley.Tools;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;

namespace Tool_Assembly
{
    public class ModEntry : Mod
    {
        public ModConfig Config = new ModConfig();
        public static readonly NetLongDictionary<Inventory, NetRef<Inventory>> metaData = new();
        public static readonly NetLongDictionary<int, NetInt> indices = new();
        public static readonly NetInt topIndex = new();
        public static readonly NetStringHashSet items = new() { "(O)ofts.wandCris" };

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Content.AssetRequested += loadTool;
            Helper.Events.Input.ButtonPressed += ButtonPressed;
            Helper.Events.GameLoop.SaveCreated += onSaveCreated;
            Helper.Events.GameLoop.SaveLoaded += load;
            Helper.Events.GameLoop.DayEnding += save;
            Helper.Events.GameLoop.GameLaunched += initAPI;
            Helper.Events.GameLoop.Saving += (a, b) => { Helper.Data.WriteSaveData("ofts.toolInd", topIndex.Value.ToString()); };
            Helper.Events.Specialized.LoadStageChanged += launched;
            Helper.ConsoleCommands.Add("tool", "", command);
            Config = Helper.ReadConfig<ModConfig>();
        }

        public bool isTool(Item item)
        {
            if(item is Tool) return true;
            if(item is MeleeWeapon) return true;
            if(items.Contains(item.QualifiedItemId)) return true;
            return false;
        }

        public void initAPI(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => Config.Prev,
                setValue: value => Config.Prev = value,
                name: () => Helper.Translation.Get("left")
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => Config.Next,
                setValue: value => Config.Next = value,
                name: () => Helper.Translation.Get("right_name")
            );
        }

        public override object? GetApi()
        {
            return new ToolAPIBase();
        }

        public void launched(object? sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage != StardewModdingAPI.Enums.LoadStage.SaveAddedLocations
                && e.NewStage != StardewModdingAPI.Enums.LoadStage.CreatedLocations) return;
            string? assetName = Helper.ModContent.GetInternalAssetName("assets/map.tmx").ToString();

            GameLocation location = new GameLocation(assetName, "aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(location);
        }

        public void save(object? sender, DayEndingEventArgs args)
        {
            if(!Context.IsMainPlayer) return;

            GameLocation location = Game1.getLocationFromName("aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart");

            for (int i = 0; i < 128; i++)
            {
                for (int j = 0; j < 128; j++)
                {
                    if(metaData.ContainsKey(i * 128 + j))
                    {
                        if (location.Objects.TryGetValue(new Vector2(i, j), out var tmp) && tmp is Chest chest)
                        {
                            chest.GetItemsForPlayer().Clear();
                            chest.GetItemsForPlayer().AddRange(metaData[i * 128 + j]);
                        }
                        else
                        {
                            Chest chest2 = new(true);
                            chest2.TileLocation = new Vector2(i, j);
                            chest2.GetItemsForPlayer().AddRange(metaData[i * 128 + j]);
                            location.Objects.Add(chest2.TileLocation, chest2);
                        }
                    }
                }
            }
        }

        public void load(object? sender, SaveLoadedEventArgs args)
        {
            if (!Context.IsMainPlayer) return;
            metaData.Clear();
            indices.Clear();

            GameLocation location = Game1.getLocationFromName("aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart");

            for (int i = 0; i < 128; i++)
                for (int j = 0; j < 128; j++)
                    if (location.Objects.TryGetValue(new Vector2(i, j), out var tmp) && tmp is Chest chest && chest.GetItemsForPlayer() is Inventory inv)
                    {
                        metaData.Add(i * 128 + j, inv);
                        indices.Add(i * 128 + j, 0);
                    }

            if(int.TryParse(Helper.Data.ReadSaveData<string>("ofts.toolInd"), out int ind))
                topIndex.Value = ind;
            else topIndex.Value = 0;
        }

        public void command(string c, string[] n)
        {
            foreach (var a in Game1.player.ActiveItem.modData)
            {
                foreach(var b in a)
                {
                    Monitor.Log(b.ToString(), LogLevel.Info);
                }
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            value = "";
            foreach (var a in Game1.player.ActiveItem.modData)
            {
                foreach (var b in a)
                {
                    if(b.Key == key)
                    {
                        value = b.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        public void onSaveCreated(object? sender, SaveCreatedEventArgs e)
        {
            Game1.player.addItemToInventory(ItemRegistry.Create("(T)ofts.toolAss"));
            Game1.player.addItemToInventory(ItemRegistry.Create("(BC)ofts.toolConfig"));
        }

        public void loadTool(object? sender, AssetRequestedEventArgs args)
        {
            if (args.NameWithoutLocale.IsEquivalentTo("Data/Tools"))
            {
                args.Edit(asset => {
                    IDictionary<string, ToolData> datas = asset.AsDictionary<string, ToolData>().Data;
                    ToolData data = new()
                    {
                        ClassName = "GenericTool",
                        Name = "toolAssembly",
                        AttachmentSlots = 0,
                        SalePrice = 1000,
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display_tool]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip_tool]",
                        Texture = "toolAss/asset/texture",
                        SpriteIndex = 0,
                        MenuSpriteIndex = 5,
                        UpgradeLevel = -1,
                        ApplyUpgradeLevelToDisplayName = false,
                        CanBeLostOnDeath = false
                    };
                    datas.Add("ofts.toolAss", data);
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, ObjectData> datas = asset.AsDictionary<string, ObjectData>().Data;
                    ObjectData data = new()
                    {
                        Name = "Wand Cristal",
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display_cris]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip_cris]",
                        Type = "Basic",
                        Category = -2,
                        Price = 100000,
                        Texture = "toolAss/asset/texture",
                        SpriteIndex = 26,
                        CanBeGivenAsGift = false,
                        ExcludeFromShippingCollection = true
                    };
                    datas.Add("ofts.wandCris", data); ;
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, BigCraftableData> datas = asset.AsDictionary<string, BigCraftableData>().Data;
                    BigCraftableData data = new()
                    {
                        Name = "ToolConfigTable",
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display_table]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip_table]",
                        Price = 100,
                        Fragility = 0,
                        IsLamp = false,
                        Texture = "toolAss/asset/texture",
                        SpriteIndex = 6,
                    };
                    datas.Add("ofts.toolConfig", data);
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                args.Edit(asset => {
                    IDictionary<string, ShopData> datas = asset.AsDictionary<string, ShopData>().Data;
                    ShopData data = datas["AdventureShop"];
                    ShopItemData wand = new()
                    {
                        Price = 2000,
                        Id = "ofts.shop.wand",
                        ItemId = "(T)ofts.toolAss",
                    };
                    data.Items.Add(wand);
                    ShopItemData cris = new()
                    {
                        Price = 200000,
                        Id = "ofts.shop.cris",
                        ItemId = "(O)ofts.wandCris",
                    };
                    data.Items.Add(cris);
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, string> datas = asset.AsDictionary<string, string>().Data;
                    datas.Add("Tool Configuation Table", "(BC)130 1 338 3 335 5 388 30/Home/ofts.toolConfig 1/true/default/");
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("toolAss/asset/texture"))
            {
                args.LoadFromModFile<Texture2D>("assets/texture", AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("toolAss/asset/map"))
            {
                args.LoadFromModFile<Map>("assets/map", AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Strings\\ofts_toolass"))
            {
                args.LoadFrom(() => {
                    return new Dictionary<string, string>()
                    {
                        { "display_table", Helper.Translation.Get("display_table") },
                        { "descrip_table", Helper.Translation.Get("descrip_table") },
                        { "display_tool", Helper.Translation.Get("display_tool") },
                        { "descrip_tool", Helper.Translation.Get("descrip_tool") },
                        { "display_cris", Helper.Translation.Get("display_cris") },
                        { "descrip_cris", Helper.Translation.Get("descrip_cris") }
                    };
                }, AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, LocationData> datas = asset.AsDictionary<string, LocationData>().Data;
                    LocationData data = new()
                    {
                        DisplayName = "aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart",
                        ExcludeFromNpcPathfinding = true,
                        CreateOnLoad = new()
                        {
                            MapPath = "toolAss/asset/map",
                            AlwaysActive = true
                        },
                        CanPlantHere = false,
                        CanHaveGreenRainSpawns = false,
                        MinDailyWeeds = 0,
                        MaxDailyWeeds = 0,
                        FirstDayWeedMultiplier = 1,
                        MinDailyForageSpawn = 0,
                        MaxDailyForageSpawn = 0,
                        ChanceForClay = 0,
                    };
                });
            }
        }
        
        public void ButtonPressed(object? sender, ButtonPressedEventArgs args)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.player.ActiveItem != null &&
                TryGetValue("ofts.toolAss.id", out string id))
            {
                if(long.TryParse(id, out long longid))
                {
                    if (metaData.TryGetValue(longid, out var inventory))
                    {
                        if (inventory.Count > 0)
                        {
                            int indexPlayer = Game1.player.Items.IndexOf(Game1.player.ActiveItem);

                            if (Config.Next.JustPressed())
                            {
                                Game1.player.Items[indexPlayer] = inventory[(indices[longid] + 1) % inventory.Count];
                                indices[longid] = (indices[longid] + 1) % inventory.Count;
                            }

                            else if (Config.Prev.JustPressed())
                            {
                                Game1.player.Items[indexPlayer] = inventory[(indices[longid] + inventory.Count - 1) % inventory.Count];
                                indices[longid] = (indices[longid] + inventory.Count - 1) % inventory.Count;
                            }
                        }
                    }
                }
            }

            bool keyPressed = false;
            foreach(var key in Game1.options.actionButton)
            {
                if (args.IsDown(key.ToSButton()))
                {
                    keyPressed = true;
                    break;
                }
            }

            if (!keyPressed) return;
            if (Game1.player.currentLocation.Objects.TryGetValue(Game1.player.GetGrabTile(), out var obj)
                && obj.QualifiedItemId == "(BC)ofts.toolConfig")
            {
                clickConfigTable();
            }
            else if (TryGetValue("ofts.toolAss.id", out string invId) && long.TryParse(invId, out long invNum) 
                && metaData.TryGetValue(invNum, out var invt) && invt.Contains(ItemRegistry.Create("ofts.wandcris")))
            {
                clickConfigTable();
            }
        }

        public int assignNewInventory(Item tool)
        {
            tool.modData.Add("ofts.toolAss.id", $"{topIndex.Value}");
            Inventory inv = new();
            inv.AddRange(new List<Item>(36));
            metaData.Add(topIndex.Value, inv);
            indices.Add(topIndex.Value++, 0);
            return topIndex.Value - 1;
        }

        public void clickConfigTable()
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                Item tool = Game1.player.ActiveItem;
                if (isTool(tool) && (
                    TryGetValue("ofts.toolAss.id", out string id) || tool.Name == "toolAssembly"))
                {
                    if (id == ""){
                        assignNewInventory(tool);
                        TryGetValue("ofts.toolAss.id", out id);
                    }

                    IInventory i = metaData[long.Parse(id)];

                    Item it = ItemRegistry.Create("ofts.toolAss");
                    it.modData.Add("ofts.toolAss.id", id);
                    Game1.player.Items[Game1.player.CurrentToolIndex] = it;
                    Game1.delayedActions.Add(new DelayedAction(10, () => 
                    { 
                        Game1.activeClickableMenu = new ItemGrabMenu(
                            inventory: i, 
                            reverseGrab: false, showReceivingMenu: true, (item) => {
                                if (item == null) return true;
                                if (i.Contains(item)) return true;
                                return isTool(item) && !item.modData.ContainsKey("ofts.toolAss.id");
                            }, behaviorOnItemSelectFunction: (a, b) => {
                                if (Game1.activeClickableMenu is not ItemGrabMenu menu)
                                    throw new InvalidOperationException("WTF Why current menu is not IGM?!!");
                                else menu.heldItem = null;
                                if (i.Count < 36) {
                                    i.Add(a);
                                    a.modData.Add("ofts.toolAss.id", Game1.player.ActiveItem.modData["ofts.toolAss.id"]);
                                    Game1.player.Items.Remove(a);
                                } 
                            }, "",
                            behaviorOnItemGrab: (a, b) => {
                                if (Game1.activeClickableMenu is not ItemGrabMenu menu) 
                                    throw new InvalidOperationException("WTF Why current menu is not IGM?!!");
                                else menu.heldItem = null;

                                if (Utility.canItemBeAddedToThisInventoryList(a, Game1.player.Items))
                                { 
                                    for(int i = 0; i < Game1.player.Items.Count; i++)
                                    {
                                        if (Game1.player.Items[i] == null)
                                        {
                                            Game1.player.Items[i] = a;
                                            break;
                                        }
                                    }
                                    i.Remove(a);
                                    i.RemoveEmptySlots();
                                    a.modData.Remove("ofts.toolAss.id");
                                } 
                            }
                        );
                    }));
                }
            }
        }
    }

    public class ModConfig
    {
        public KeybindList Next { get; set; } = KeybindList.Parse("Right");
        public KeybindList Prev { get; set; } = KeybindList.Parse("Left");
    }
}