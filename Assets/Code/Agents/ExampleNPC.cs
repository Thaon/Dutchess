using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleNPC : NPCAgent
{

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        /**
         * Create goals like this:
         * goal.Add(new KeyValuePair<string, object>("hungry", false));
         */

        return goal;
    }
}