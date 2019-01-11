using System.Collections.Generic;
using GKStateMachine;

namespace GKToy
{
	public abstract class GKStateListMachineBase<STATE_ID_T>
	{
		List<GKStateMachineStateBase<STATE_ID_T>> _states;
		GKStateMachineStateBase<STATE_ID_T> _defaultState;
		List<GKStateMachineStateBase<STATE_ID_T>> _currentState;
        List<STATE_ID_T> _resetState;
		GKStateMachineStateBase<STATE_ID_T> _lastState;
        List<NewStateStruct> _currentAddingState;
		// 即将移除的状态：状态ID，是否正常结束（否则跳过Exit）.
        Dictionary<STATE_ID_T, bool> _currentRemovingState;
		bool _isActive;

		public GKStateListMachineBase()
		{
			_states = new List<GKStateMachineStateBase<STATE_ID_T>>();
			_currentState = new List<GKStateMachineStateBase<STATE_ID_T>>();
            _currentAddingState = new List<NewStateStruct>();
            _currentRemovingState = new Dictionary<STATE_ID_T, bool>();
            _resetState = new List<STATE_ID_T>();
            _isActive = true;
		}

		public void AddState(GKStateMachineStateBase<STATE_ID_T> state, bool asDefault)
		{
			_states.Add(state);
			if (asDefault)
			{
				_defaultState = state;
				_lastState = state;
			}
		}

		public List<GKStateMachineStateBase<STATE_ID_T>> GetCurrentState()
		{
			return _currentState;
		}

		public GKStateMachineStateBase<STATE_ID_T> GetLastState()
		{
			return _lastState;
		}

        public void GoToState(STATE_ID_T fromStateId, List<NewStateStruct> targetStateIds)
		{
			if(targetStateIds != null && targetStateIds.Count > 0)
                _currentAddingState.AddRange(targetStateIds);
			if(!_currentRemovingState.ContainsKey(fromStateId))
                _currentRemovingState.Add(fromStateId, _isActive);
		}
        public void GoToState(List<NewStateStruct> targetStateIds)
        {
            if (targetStateIds != null && targetStateIds.Count > 0)
                _currentAddingState.AddRange(targetStateIds);
        }

        public void LeaveState(STATE_ID_T stateId)
		{
			if (!_currentRemovingState.ContainsKey(stateId))
                _currentRemovingState.Add(stateId, _isActive);
		}

        public void StopAll(STATE_ID_T stopStateId)
		{
            _isActive = false;
			_currentRemovingState.Clear();
			_currentAddingState.Clear();
			if (0 != _currentState.Count)
			{
				foreach (var state in _currentState)
				{
                    _currentRemovingState.Add(state.ID, state.ID.Equals(stopStateId));
				}
			}
		}

        public void ResetState(STATE_ID_T curStateId, List<STATE_ID_T> nextStateIds)
        {
            if(!_resetState.Contains(curStateId))
            {
                _resetState.Add(curStateId);
                foreach(STATE_ID_T id in nextStateIds)
                {
                    _GetStateById(id).ResetAfterState();
                }
            }
        }

        public void ClearResetList()
        {
            _resetState.Clear();
        }

        public void Update()
		{
			if (_states.Count == 0)
				return;

			if (_currentState.Count == 0)
			{
				if (_defaultState == null)
					return;
                _currentAddingState.Add(new NewStateStruct(_defaultState.ID, "", null));
				_defaultState = null;
			}
			if (0 != _currentAddingState.Count)
			{
				foreach (var state in _currentAddingState)
				{
                    _AddCurrentState(state.state, state.parm, state.value);
				}
				_currentAddingState.Clear();
			}
			if (0 != _currentRemovingState.Count)
			{
				foreach (var state in _currentRemovingState)
				{
					_RemoveCurrentState(state.Key, state.Value);
				}
				_currentRemovingState.Clear();
			}
            if (_isActive && 0 != _currentState.Count)
			{
				foreach (var state in _currentState)
				{
					state.Update();
				}
			}
		}

		public GKStateMachineStateBase<STATE_ID_T> _GetStateById(STATE_ID_T id)
		{
			foreach (GKStateMachineStateBase<STATE_ID_T> state in _states)
			{
				if (id.Equals(state.ID))
					return state;
			}

			return null;
		}

        void _AddCurrentState(STATE_ID_T stateId, string parm, object val)
		{
			GKStateMachineStateBase<STATE_ID_T> state = _GetStateById(stateId);
            if (null == state)
                return;

            state.Trigger(parm, val);
			if (!_currentState.Contains(state))
			{
				_currentState.Add(state);
                state.Enter();
			}
		}

		void _RemoveCurrentState(STATE_ID_T stateId, bool isNormal)
		{
			GKStateMachineStateBase<STATE_ID_T> state = _GetStateById(stateId);
			if (state != null && _currentState.Contains(state))
			{
				if(isNormal)
					state.Exit();
				_lastState = state;
				_currentState.Remove(state);
			}
		}

        // 新状态结构, 增加字符窜参数.
        public class NewStateStruct
        {
            public NewStateStruct(STATE_ID_T s, string p, object v)
            {
                state = s;
                parm = p;
                value = v;
            }

            public STATE_ID_T state;
            public string parm;
            public object value;
        }
	}
}
