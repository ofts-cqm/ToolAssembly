using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.Inventories;

namespace Tool_Assembly
{
    public class ModEntry : Mod
    {
        public ModConfig Config = new ModConfig();
        public Dictionary<int, Inventory> metaData = new();

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
                        DisplayName = "[LocalizedText Strings\\ofts_toolass:display]",
                        Description = "[LocalizedText Strings\\ofts_toolass:descrip]",
                        Texture = "asset/texture",
                        SpriteIndex = 0,
                        MenuSpriteIndex = -1,
                        UpgradeLevel = -1,
                        ApplyUpgradeLevelToDisplayName = false,
                        CanBeLostOnDeath = false
                    };
                    datas.Add("ofts.toolAss", data);
                });
            }
        }
        
        public void ButtonPressed(object? sender, ButtonPressedEventArgs args)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu == null && 
                Game1.player.ActiveItem is Tool tool && tool.modData.TryGetValue("ofts.toolAss.id", out var id)
                && metaData.TryGetValue(int.Parse(id), out var inventory))
            {
                int indexPlayer = Game1.player.Items.IndexOf(tool);
                int indexData = inventory.IndexOf(Game1.player.ActiveItem);

                if (Config.Next.JustPressed())
                {
                    Game1.player.Items[indexPlayer] = inventory[(indexData + 1) % inventory.Count];
                }

                else if(Config.Prev.JustPressed())
                {
                    Game1.player.Items[indexPlayer] = inventory[(indexData + inventory.Count - 1) % inventory.Count];
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