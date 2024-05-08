﻿using StardewValley.Inventories;
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
        /// Get the content inside this tool
        /// </summary>
        /// <param name="tool">the tool</param>
        /// <returns>the content of this tool as an <see cref="Inventory"/></returns>
        Inventory getToolContentWithTool(Tool tool);
        /// <summary>
        /// Create a new tool
        /// </summary>
        /// <param name="assignNewInventory">should assign a new id to this tool or leave blank</param>
        /// <returns>a new Tool Assembly</returns>
        GenericTool createNewTool(bool assignNewInventory = false);
        /// <summary>
        /// Create a new tool with specific id. 
        /// </summary>
        /// <param name="id">the id of the tool</param>
        /// <returns>a new Tool Assembly</returns>
        GenericTool createToolWithId(long id);
        /// <summary>
        /// Assign a new inventory. 
        /// </summary>
        /// <returns>the assigned id</returns>
        long assignNewInventory();
        /// <summary>
        /// return whether the specific id exists. 
        /// </summary>
        /// <param name="id">the id</param>
        /// <returns>exists for not</returns>
        bool doesIDExist(long id);
    }
}
