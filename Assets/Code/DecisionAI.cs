using UnityEngine;
using System.Collections;
using BT;

public class DecisionAI : BTTree {

	public float sightForOrc;
	public float sightForGoblin;

	protected override void Init () {
		// -------Prepare--------
		// 1. Initialize parent
		base.Init ();
		
		// 2. Enable BT framework's log for debug, optional
//		BTConfiguration.ENABLE_BTACTION_LOG = true;
//		BTConfiguration.ENABLE_DATABASE_LOG = true;
		
		// 3. Create root, usually it's a priority selector
		_root = new BTParallelFlexible();

		// Set cooldown, so this BT tree will only tick every 1.5 seconds
		_root.interval = 1.5f;


		// 4. Create the nodes for reuse later
		// Preconditions
		CheckInSight checkOrcInSight = new CheckInSight(sightForOrc, Constants.GO_ORC);		// precondition
		CheckInSight checkGoblinInSight = new CheckInSight(sightForGoblin, Constants.GO_GOBLIN);	// precondition

		// Actions
		ResetData<bool> clearEscape = new ResetData<bool>(Constants.SHOULD_ESCAPE, false);
		ResetData<bool> clearFight = new ResetData<bool>(Constants.SHOULD_FIGHT, false);
		ResetData<bool> shouldEscape = new ResetData<bool>(Constants.SHOULD_ESCAPE, true, false, checkOrcInSight);
		ResetData<bool> shouldFight = new ResetData<bool>(Constants.SHOULD_FIGHT, true, false, checkGoblinInSight);


		// 5. Set initial database data
		database.SetData<bool>(Constants.SHOULD_ESCAPE, false);
		database.SetData<bool>(Constants.SHOULD_FIGHT, false);


		// -------Construct-------

		// 5.1 Clear all first
		_root.AddChild(clearEscape);
		_root.AddChild(clearFight);

		// 5.2 Decision Making
		BTPrioritySelector situationSelector = new BTPrioritySelector();
		{

			// 5.2.1 Escape Decision
			situationSelector.AddChild(shouldEscape);

			// 5.2.2 Fight Decision
			situationSelector.AddChild(shouldFight);
		}
		_root.AddChild(situationSelector);
	}
}
