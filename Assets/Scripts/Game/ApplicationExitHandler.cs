using UnityEngine;
namespace Game
{
    public interface IApplicationExitHandler
    {
        void Exit();
    }
    
    public sealed class ApplicationExitHandler : IApplicationExitHandler
    {
        public void Exit() 
        {
            if (IsEditor()) 
                StopEditorPlayMode();
            else 
                QuitApplication();
        }

        private bool IsEditor()
        {
#if UNITY_EDITOR
            return true;
#endif
            return false;
        }

        private void StopEditorPlayMode()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void QuitApplication() => Application.Quit();
    }
}