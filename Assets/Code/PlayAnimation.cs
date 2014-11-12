using UnityEngine;
using System.Collections;
using BT;

public class PlayAnimation : BTAction {
	private string _animationName;


	public PlayAnimation (string animationName, BTPrecondition precondition = null) : base (precondition) {
		_animationName = animationName;
	}

	protected override void Enter () {
		Debug.Log(_animationName);
		Animator animator = database.GetComponent<Animator>();
		animator.Play(_animationName);
	}
}
