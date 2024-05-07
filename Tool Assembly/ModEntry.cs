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
using xTile.Dimensions;

namespace Tool_Assembly
{
    public class ModEntry : Mod
    {
        public ModConfig Config = new ModConfig();
        public static readonly NetLongDictionary<Inventory, NetRef<Inventory>> metaData = new();
        public static readonly NetLongDictionary<int, NetInt> indices = new();
        public static readonly NetInt topIndex = new();

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Content.AssetRequested += loadTool;
            Helper.Events.Input.ButtonPressed += ButtonPressed;
            Helper.Events.GameLoop.SaveCreated += onSaveCreated;
            Helper.Events.GameLoop.SaveLoaded += load;
            Helper.Events.GameLoop.Saving += save;
            Helper.ConsoleCommands.Add("tool", "", command);
            Config = Helper.ReadConfig<ModConfig>();
        }

        public void save(object? sender, SavingEventArgs args)
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

            Helper.Data.WriteSaveData("ofts.toolInd", topIndex.Value.ToString());
        }

        public void load(object? sender, SaveLoadedEventArgs args)
        {
            if (!Context.IsMainPlayer) return;
            metaData.Clear();
            indices.Clear();

            GameLocation location = Game1.getLocationFromName("aVeryVeryStrangeFourDimentionalSpaceThatStoresPlayersToolsStoredInToolAssemblyBecauseIDontKnowHowToSerizeTheDatasSoIDescideToLetTheGameItselfStoreTheDataForMeHaHaHaIAmSoSmart");

            for(int i = 0; i < 128; i++)
            {
                for(int j = 0; j < 128; j++)
                {
                    if (location.Objects.TryGetValue(new Vector2(i, j), out var tmp) && tmp is Chest chest && chest.GetItemsForPlayer() is Inventory inv)
                    {
                        metaData.Add(i * 128 + j, inv);
                        indices.Add(i * 128 + j, 0);
                    }
                }
            }

            if(int.TryParse(Helper.Data.ReadSaveData<string>("ofts.toolInd"), out int ind))
            {
                topIndex.Value = ind;
            }
            else
            {
                topIndex.Value = 0;
            }
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
                        { "descrip_tool", Helper.Translation.Get("descrip_tool") }
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
            //Monitor.Log($"{Game1.player.ActiveItem?.modData.ToArray()}", LogLevel.Info);
            /*if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.player.ActiveItem != null &&
                TryGetValue("ofts.toolAss.id", out string id)
                && long.TryParse(id, out long longid) && metaData.TryGetValue(longid, out var inventory)
                && inventory.Count > 1)
            {
                int indexPlayer = Game1.player.Items.IndexOf(Game1.player.ActiveItem);

                if (Config.Next.JustPressed())
                {
                    Game1.player.Items[indexPlayer] = inventory[(indices[longid] + 1) % inventory.Count];
                }

                else if(Config.Prev.JustPressed())
                {
                    Game1.player.Items[indexPlayer] = inventory[(indices[longid] + inventory.Count - 1) % inventory.Count];
                }
            }*/
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
        }

        public void assignNewInventory(Tool tool)
        {
            tool.modData.Add("ofts.toolAss.id", $"{topIndex.Value}");
            Inventory inv = new();
            inv.AddRange(new List<Item>(36));
            inv.OnSlotChanged += (inv, index, before, after) => {
                //inv.RemoveEmptySlots();
                if (before != null && before is Tool)
                {
                    before.modData.Remove("ofts.toolAss.id");
                }
                if (after != null && after is Tool)
                {
                    if (Game1.player.ActiveItem is not Tool t)
                    {
                        throw new InvalidOperationException("Inventory Changed Outside of menu, unknown id!");
                    }
                    else {
                        after.modData.Add("ofts.toolAss.id", t.modData["ofts.toolAss.id"]);
                    }
                }
            };
            metaData.Add(topIndex.Value, inv);
            indices.Add(topIndex.Value++, 0);
        }

        public void clickConfigTable()
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                if(Game1.player.ActiveItem is Tool tool && (
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
                                return item is Tool && !item.modData.ContainsKey("ofts.toolAss.id");
                            }, behaviorOnItemSelectFunction: (a, b) => {
                                if (Game1.activeClickableMenu is not ItemGrabMenu menu)
                                    throw new InvalidOperationException("WTF Why current menu is not IGM?!!");
                                else menu.heldItem = null;
                                if (i.Count < 36) {
                                    i.Add(a); 
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