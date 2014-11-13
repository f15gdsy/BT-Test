using UnityEngine;
using System.Collections;
using BT;

public class MoveAttackAI : BTTree {

	public float speed;
	public float sightForOrc;
	public float sightForGoblin;
	public float fightDistance;

	protected override void Init () {

		// -------Prepare--------
		// 1. Initialize parent
		base.Init ();

		// 2. Enable BT framework's log for debug, optional
		BTConfiguration.ENABLE_BTACTION_LOG = true;
//		BTConfiguration.ENABLE_DATABASE_LOG = true;

		// 3. Create root, usually it's a priority selector
		_root = new BTPrioritySelector();

		// 4. Create the nodes for reuse later
		BTParallel run = new BTParallel(BTParallel.ParallelFunction.Or);
		{
			run.AddChild(new DoRun(Constants.DESTINATION, speed));
			run.AddChild(new PlayAnimation(Constants.ANIMATION_RUN));
		}


		// -------Construct-------

		// 5.1 Escape node
		BTPreconditionBool shouldEscape = new BTPreconditionBool(Constants.SHOULD_ESCAPE, true);		// precondition

		// "Escape" serves as a parallel node
		// "Or" means the parallel node ends when any of its children ends.
		BTParallel escape = new BTParallel(BTParallel.ParallelFunction.Or, shouldEscape);
		{
			FindEscapeDestination findDestination = new FindEscapeDestination(Constants.GO_ORC, Constants.DESTINATION, sightForOrc);
			escape.AddChild(findDestination);

			escape.AddChild(run);
		}
		_root.AddChild(escape);		// Add node into root


		// 5.2 Fight node
		BTPreconditionBool shouldFight = new BTPreconditionBool(Constants.SHOULD_FIGHT, true);// precondition

		BTSequence fight = new BTSequence(shouldFight);
		{
			BTParallel parallel = new BTParallel(BTParallel.ParallelFunction.Or);
			{
				FindToTargetDestination findToTargetDestination = new FindToTargetDestination(Constants.GO_GOBLIN, Constants.DESTINATION, fightDistance * 0.9f);
				parallel.AddChild(findToTargetDestination);

				parallel.AddChild(run);		// Reuse Run
			}
			fight.AddChild(parallel);

			CheckInSight checkGoblinInFightDistance = new CheckInSight(fightDistance, Constants.GO_GOBLIN);	// precondition
			fight.AddChild(new PlayAnimation(Constants.ANIMATION_FIGHT, checkGoblinInFightDistance));
		}
		_root.AddChild(fight);


		// 5.3 Idle node
		_root.AddChild(new PlayAnimation(Constants.ANIMATION_IDLE));
	}
}
