using UnityEngine;

namespace GKStateMachine
{
	public abstract class GKStateMachineStateBase<STATE_ID_T>
	{
		[SerializeField]
		private STATE_ID_T _stateId;
		public STATE_ID_T ID
		{
            get { return _stateId; }
            private set { _stateId = value; }
		}

        public GKStateMachineStateBase(STATE_ID_T id)
        {
            ID = id;
        }

        // 激活有限状态机时调用.
        public abstract void Enter();

        // 触发有限状态机时调用.
        // 与Enter区别为, 激活状态下, 如果再次激活,不进行Enter但或进行Trigger.
        // 适用于多重激活互斥锁参数传递.
        public abstract void Trigger(string parm, object val);

        public abstract void Exit();

        public abstract STATE_ID_T Update();

        public abstract void ResetAfterState();
    }
}
