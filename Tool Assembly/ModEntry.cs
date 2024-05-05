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
using Microsoft.VisualBasic;

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
            Config = Helper.ReadConfig<ModConfig>();
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
                        SpriteIndex = 21,
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
                args.LoadFromModFile<Dictionary<string, string>>("assets/translations", AssetLoadPriority.Low);
            }
        }
        
        public void ButtonPressed(object? sender, ButtonPressedEventArgs args)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && 
                Game1.player.ActiveItem is Tool tool && tool.modData.TryGetValue("ofts.toolAss.id", out var id)
                && long.TryParse(id, out var longid) && metaData.TryGetValue(longid, out var inventory))
            {
                int indexPlayer = Game1.player.Items.IndexOf(tool);

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

            if (Game1.player.currentLocation.Objects.TryGetValue(Game1.player.GetGrabTile(), out var obj)
                && obj.QualifiedItemId == "(BC)ofts.toolConfig")
            {
                clickConfigTable();
            }
        }

        public void assignNewInventory(Tool tool)
        {
            tool.modData.Add("ofts.toolAss.id", $"{topIndex.Value++}");
            Inventory inv = new Inventory() { tool };
            inv.OnSlotChanged += (a, b, c, d) => { a.RemoveEmptySlots(); };
            metaData.Add(topIndex.Value, inv);
            indices.Add(topIndex.Value, 0);
        }

        public void clickConfigTable()
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                if(Game1.player.ActiveItem is Tool tool && (tool.Name == "toolAssembly" || 
                    tool.modData.ContainsKey("ofts.toolAss.id")))
                {
                    if (!tool.modData.ContainsKey("ofts.toolAss.id")){
                        assignNewInventory(tool);
                    }

                    Game1.activeClickableMenu = new ToolConfigMenu(long.Parse(tool.modData["ofts.toolAss.id"]));
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