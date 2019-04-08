using UnityEditor;
using UnityEngine;

namespace GKUI
{
    /// <summary>
    /// Define a popup window that return a result.
    /// Base class for IModal call implementation.
    /// </summary>
    public abstract class GKUIModalWindow : EditorWindow
    {
        public const float TITLEBAR = 18;

        protected IModal _owner;

        protected string _title = "ModalWindow";

        protected WindowResult _result = WindowResult.None;

        public WindowResult Result
        {
            get { return _result; }
        }

        protected virtual void _OnLostFocus()
        {
            this.Focus();
        }

        protected virtual void _Cancel()
        {
            this.Focus();
        }

        protected virtual void _Ok()
        {
            _result = WindowResult.Ok;

            if (_owner != null)
                _owner._ModalClosed(this);

            Close();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label(_title);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            Rect content = new Rect(0, TITLEBAR, position.width, position.height - TITLEBAR);
            _Draw(content);
        }

        protected abstract void _Draw(Rect region);
    }

    /// <summary>
    /// This EditorWindow can recieve and send Modal inputs.
    /// </summary>
    public interface IModal
    {
        /// <summary>
        /// Called when the Modal shortcut is pressed.
        /// The implementation should call Create if the condition are right.
        /// </summary>
        void _ModalRequest(bool shift);

        /// <summary>
        /// Called when the associated modal is closed.
        /// </summary>
        void _ModalClosed(GKUIModalWindow window);
    }

    public enum WindowResult
    {
        None,
        Ok,
        Cancel,
        Invalid,
        LostFocus
    }
}
