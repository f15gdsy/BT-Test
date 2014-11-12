using UnityEngine;
using System.Collections;
using BT;

public class DoFight : BTAction {

	private Animator _animator;

	public DoFight (BTPrecondition precondition) : base (precondition) {
		
	}

	public override void Activate (Database database) {
		base.Activate (database);

		_animator = database.GetComponent<Animator>();
	}

	protected override void Enter () {
		_animator.Play("Fight");
	}

	protected override BTResult Execute ()
	{

		return BTResult.Running;
	}
}
