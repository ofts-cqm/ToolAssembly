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
using StardewValley.Menus;
using StardewValley.Tools;

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
            Helper.ConsoleCommands.Add("tool", "", command);
            Config = Helper.ReadConfig<ModConfig>();
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
                if (a.TryGetValue(key, out value)) return true;
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
                        MenuSpriteIndex = -1,
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
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipies"))
            {
                args.Edit(asset =>
                {
                    IDictionary<string, string> datas = asset.AsDictionary<string, string>().Data;
                    datas.Add("Tool Configuation Table", "166 1 338 10 335 5/Home/ofts.toolConfig 1/true/default/");
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("toolAss/asset/texture"))
            {
                args.LoadFromModFile<Texture2D>("assets/texture", AssetLoadPriority.Low);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Strings\\ofts_toolass"))
            {
                args.LoadFromModFile<Dictionary<string, string>>($"i18n/{Helper.Translation.Locale}", AssetLoadPriority.Low);
            }
        }
        
        public void ButtonPressed(object? sender, ButtonPressedEventArgs args)
        {
            Monitor.Log($"{Game1.player.ActiveItem?.modData.ToArray()}", LogLevel.Info);
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && Game1.player.ActiveItem != null &&
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
            //if (Game1.player.currentLocation.Objects.TryGetValue(t, out var obj)
                //&& obj.QualifiedItemId == "(BC)ofts.toolConfig")
            //{
                clickConfigTable();
            //}
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
                if(Game1.player.ActiveItem is GenericTool tool && (tool.Name == "toolAssembly" || 
                    tool.modData.ContainsKey("ofts.toolAss.id")))
                {
                    if (!tool.modData.ContainsKey("ofts.toolAss.id")){
                        assignNewInventory(tool);
                    }

                    Inventory i = metaData[long.Parse(tool.modData["ofts.toolAss.id"])];

                    Game1.activeClickableMenu = new ItemGrabMenu(
                        inventory: i, 
                        reverseGrab: false, showReceivingMenu: true, (item) => {
                            if (item == null) return true;
                            if (i.Contains(item)) return true;
                            return item is Tool && !item.modData.ContainsKey("ofts.toolAss.id");
                        }, behaviorOnItemSelectFunction: (a, b) => {
                            (Game1.activeClickableMenu as ItemGrabMenu).heldItem = null;
                            if (i.Count < 36) {
                                i.Add(a); 
                                Game1.player.Items.Remove(a);
                            } 
                        }, "",
                        behaviorOnItemGrab: (a, b) => {
                            (Game1.activeClickableMenu as ItemGrabMenu).heldItem = null;
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
                            } 
                        });
                    //Game1.activeClickableMenu = new ToolConfigMenu(long.Parse(tool.modData["ofts.toolAss.id"]));
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