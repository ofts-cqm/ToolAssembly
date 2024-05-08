using StardewValley.Inventories;
using StardewValley;
using StardewValley.Tools;

namespace Tool_Assembly
{
    public interface IToolAPI
    {
        /// <summary>
        /// Get the specific <see cref="Inventory"/> that the id represent
        /// </summary>
        /// <param name="id">an id. If exists, can be found in Item.modData with key "ofts.toolAss.id"</param>
        /// <returns>the content of this id as an <see cref="Inventory"/></returns>
        Inventory getToolContentWithID(long id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        Inventory getToolContentWithTool(Tool tool);
        GenericTool createNewTool(bool assignNewInventory = false);
        GenericTool createToolWithId(long id);
        long assignNewInventory();
        bool doesIDExist(long id);
    }
}
