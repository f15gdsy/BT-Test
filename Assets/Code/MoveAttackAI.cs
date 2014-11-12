using UnityEngine;
using System.Collections;
using BT;

public class MoveAttackAI : BTTree {
	private static string DESTINATION = "Destination";
	private static string ORC_NAME = "Orc";
	private static string GOBLIN_NAME = "Goblin";
	private static string RUN_ANIMATION = "Run";
	private static string FIGHT_ANIMATION = "Fight";
	private static string IDLE_ANIMATION = "Idle";

	public float speed;
	public float sightForOrc;
	public float sightForGoblin;
	public float fightDistance;

	protected override void Init () {

		// -------Prepare--------
		// 1. Initialize parent
		base.Init ();

		// 2. Enable BT framework's log for debug, optional
		// BTConfiguration.ENABLE_LOG = true;

		// 3. Create root, usually it's a priority selector
		_root = new BTPrioritySelector();

		// 4. Create the nodes for reuse later
		BTParallel run = new BTParallel(BTParallel.ParallelFunction.Or);
		{
			run.AddChild(new DoRun(DESTINATION, speed));
			run.AddChild(new PlayAnimation(RUN_ANIMATION));
		}


		// -------Construct-------

		// 3.1 Escape node
		CheckInSight checkOrcInSight = new CheckInSight(sightForOrc, ORC_NAME);		// precondition

		// "Escape" serves as a parallel node
		// "Or" means the parallel node ends when any of its children ends.
		BTParallel escape = new BTParallel(BTParallel.ParallelFunction.Or, checkOrcInSight);
		{
			FindEscapeDestination findDestination = new FindEscapeDestination(ORC_NAME, DESTINATION, sightForOrc);
			escape.AddChild(findDestination);

			escape.AddChild(run);
		}
		_root.AddChild(escape);		// Add node into root


		// 3.2 Fight node
		CheckInSight checkGoblinInSight = new CheckInSight(sightForGoblin, GOBLIN_NAME);	// precondition

		BTSequence fight = new BTSequence(checkGoblinInSight);
		{
			BTParallel parallel = new BTParallel(BTParallel.ParallelFunction.Or);
			{
				FindToTargetDestination findToTargetDestination = new FindToTargetDestination(GOBLIN_NAME, DESTINATION, fightDistance * 0.9f);
				parallel.AddChild(findToTargetDestination);

				parallel.AddChild(run);		// Reuse Run
			}
			fight.AddChild(parallel);

			CheckInSight checkGoblinInFightDistance = new CheckInSight(fightDistance, GOBLIN_NAME);	// precondition
			fight.AddChild(new PlayAnimation(FIGHT_ANIMATION, checkGoblinInFightDistance));
		}
		_root.AddChild(fight);


		// 3.3 Idle node
		_root.AddChild(new PlayAnimation(IDLE_ANIMATION));
	}
}
