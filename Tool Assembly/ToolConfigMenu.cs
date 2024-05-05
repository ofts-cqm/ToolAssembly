using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Tool_Assembly
{
    public class ToolConfigMenu : IClickableMenu
    {
        InventoryMenu ToolInventory;
        InventoryMenu PlayerInventory;
        Item? grabed;

        public ToolConfigMenu(long id) : base(0, 0, 0, 0, true){
            ToolInventory = new(0, 0, false, ModEntry.metaData[id]);
            PlayerInventory = new(0, 0, false, highlightMethod: (item) => { return item == null || item is Tool; });
            height = ToolInventory.height + PlayerInventory.height + 50;
            width = ToolInventory.width;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(height, width);
            xPositionOnScreen = (int)pos.X;
            yPositionOnScreen = (int)pos.Y;
            ToolInventory.xPositionOnScreen = xPositionOnScreen;
            ToolInventory.yPositionOnScreen = yPositionOnScreen;
            PlayerInventory.xPositionOnScreen = xPositionOnScreen;
            PlayerInventory.yPositionOnScreen = yPositionOnScreen + PlayerInventory.height + 50;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            grabed = PlayerInventory.leftClick(x, y, grabed);
            grabed = ToolInventory.leftClick(x, y, grabed);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
            grabed = PlayerInventory.rightClick(x, y, grabed);
            grabed = ToolInventory.rightClick(x, y, grabed);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            height = ToolInventory.height + PlayerInventory.height + 50;
            width = ToolInventory.width;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(height, width);
            xPositionOnScreen = (int)pos.X;
            yPositionOnScreen = (int)pos.Y;
            ToolInventory.xPositionOnScreen = xPositionOnScreen;
            ToolInventory.yPositionOnScreen = yPositionOnScreen;
            PlayerInventory.xPositionOnScreen = xPositionOnScreen;
            PlayerInventory.yPositionOnScreen = yPositionOnScreen + PlayerInventory.height + 50;
        }
    }
}
