using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace Tool_Assembly
{
    public class ToolConfigMenu : IClickableMenu
    {
        InventoryMenu ToolInventory;
        InventoryMenu PlayerInventory;
        public long id;
        Item? grabed;
        Chest c;

        public ToolConfigMenu(long id) : base(0, 0, 0, 0, true){
            ToolInventory = new(0, 0, false, ModEntry.metaData[id]);
            PlayerInventory = new(0, 0, true, highlightMethod: (item) => { 
                return item == null || (item is Tool && item != Game1.player.ActiveItem); 
            });
            height = ToolInventory.height + PlayerInventory.height + 100;
            width = ToolInventory.width;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)pos.X;
            yPositionOnScreen = (int)pos.Y;
            ToolInventory.movePosition((int)pos.X, (int)pos.Y);
            PlayerInventory.movePosition((int)pos.X, (int)pos.Y + PlayerInventory.height + 5);
            this.id = id;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f)
            {
                myID = 9175502
            };
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            grabed = PlayerInventory.leftClick(x, y, grabed);
            grabed = ToolInventory.leftClick(x, y, grabed);
            if(ToolInventory.getInventoryPositionOfClick(x, y) == -1 && ToolInventory.isWithinBounds(x, y)
                && grabed != null && ToolInventory.actualInventory.Count < 36)
            {
                ToolInventory.actualInventory.Add(grabed);
                grabed.modData.Add("ofts.toolAss.id", id.ToString());
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
            grabed = PlayerInventory.rightClick(x, y, grabed);
            grabed = ToolInventory.rightClick(x, y, grabed);
            if (ToolInventory.getInventoryPositionOfClick(x, y) == -1 && ToolInventory.isWithinBounds(x, y) 
                && grabed != null && ToolInventory.actualInventory.Count < 36)
            {
                ToolInventory.actualInventory.Add(grabed);
                grabed.modData.Add("ofts.toolAss.id", id.ToString());
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            height = ToolInventory.height + PlayerInventory.height + 100;
            width = ToolInventory.width;
            Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)pos.X;
            yPositionOnScreen = (int)pos.Y;
            ToolInventory.movePosition((int)pos.X, (int)pos.Y);
            PlayerInventory.movePosition((int)pos.X, (int)pos.Y + PlayerInventory.height + 5);
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f)
            {
                myID = 9175502
            };
        }

        public override void draw(SpriteBatch b)
        {
            PlayerInventory.draw(b);
            ToolInventory.draw(b);
            if(grabed != null) grabed.drawInMenu(b, new Vector2(Game1.getMouseX(), Game1.getMouseY()), 1f);
            base.draw(b);
            drawMouse(b);
        }
    }
}
