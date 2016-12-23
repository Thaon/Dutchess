using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouristNPC : NPCAgent
{

    #region member variables

    public bool m_bored = true;

    #endregion

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

         goal.Add(new KeyValuePair<string, object>("amused", true));

        return goal;
    }

    public override HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldState = new HashSet<KeyValuePair<string, object>>();

        /**
         * add actions like this:
         * 
         *   if (GetComponent<EatAction>() != null)
         *      {
         *          worldState.Add(new KeyValuePair<string, object>("hungry", GetComponent<EatAction>().IsHungry()));
         *      }
         */
        /**
         * check inventory if needed like this:
         * 
         * worldState.Add(new KeyValuePair<string, object>("hasIngredients", m_inventory.Contains("ingredients")));
         */
        worldState.Add(new KeyValuePair<string, object>("amused", !m_bored));

        return worldState;
    }
}
