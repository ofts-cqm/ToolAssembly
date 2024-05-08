using StardewValley;
using StardewValley.Inventories;
using StardewValley.Tools;

namespace Tool_Assembly
{
    public class ToolAPIBase : IToolAPI
    {
        public long assignNewInventory()
        {
            Inventory inv = new();
            inv.AddRange(new List<Item>(36));
            ModEntry.metaData.Add(ModEntry.topIndex.Value, inv);
            ModEntry.indices.Add(ModEntry.topIndex.Value++, 0);
            return ModEntry.topIndex.Value - 1;
        }

        public GenericTool createNewTool(bool assignNewInventory = false)
        {
            if (assignNewInventory)
            {
                return createToolWithId(this.assignNewInventory());
            }
            else
            {
                return ItemRegistry.Create<GenericTool>("ofts.toolAss");
            }
        } 

        public GenericTool createToolWithId(long id)
        {
            GenericTool tool = createNewTool();
            tool.modData.Add("ofts.toolAss.id", id.ToString());
            return tool;
        }

        public bool doesIDExist(long id) => ModEntry.metaData.ContainsKey(id);

        public Inventory getToolContentWithID(long id) => ModEntry.metaData[id];

        public Inventory getToolContentWithTool(Tool tool) => 
            getToolContentWithID(long.Parse(tool.modData["ofts.toolAss.id"]));
    }
}
