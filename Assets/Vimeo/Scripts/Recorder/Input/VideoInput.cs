#if UNITY_2018_1_OR_NEWER
#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Recorder.Input;

namespace Vimeo.Recorder
{
    public class VideoInput : MonoBehaviour
    {
        [HideInInspector] public VimeoRecorder recorder;

        bool m_ModifiedResolution;

        public int outputWidth { get; protected set; }
        public int outputHeight { get; protected set; }
        
        public virtual Texture2D GetFrame() { return new Texture2D(1, 1); }
        public virtual void StartFrame() { }
        public virtual void EndFrame() { }

        public virtual void BeginRecording() 
        { 
            Screen.orientation = ScreenOrientation.Landscape;
            
            int w, h;
            GameViewSize.GetGameRenderSize(out w, out h);

#if UNITY_EDITOR
            switch (recorder.defaultResolution)
            {
                case Resolution.Window:
                {
                    outputWidth  = (w + 1) & ~1;
                    outputHeight = (h + 1) & ~1;
                    break;
                }

                default:
                {
                    outputHeight = (int)recorder.defaultResolution;
                    outputWidth  = (int)(outputHeight * AspectRatioHelper.GetRealAR(recorder.defaultAspectRatio));

                    outputWidth  = (outputWidth + 1) & ~1;
                    outputHeight = (outputHeight + 1) & ~1;
                    break;
                }
            }
         
            //Debug.Log("Screen resolution: " + w + "x" + h);

            if (w != outputWidth || h != outputHeight)
            {
                Debug.Log("Setting window size to: " + outputWidth + "x" + outputHeight);
                var size = GameViewSize.SetCustomSize(outputWidth, outputHeight) ?? GameViewSize.AddSize(outputWidth, outputHeight);
                if (GameViewSize.m_ModifiedResolutionCount == 0) {
                    GameViewSize.BackupCurrentSize();
                }
                else {
                    if (size != GameViewSize.currentSize) {
                        Debug.LogError("Requestion a resultion change while a recorder's input has already requested one! Undefined behaviour.");
                    }
                }

                GameViewSize.m_ModifiedResolutionCount++;
                m_ModifiedResolution = true;
                GameViewSize.SelectSize(size);
            }
#endif            
        }

        public virtual void EndRecording() 
        { 
#if UNITY_EDITOR
            if (m_ModifiedResolution)
            {
                GameViewSize.m_ModifiedResolutionCount--;
                if (GameViewSize.m_ModifiedResolutionCount == 0) {
                    GameViewSize.RestoreSize();
                }
            }
#endif            
        }
    }
}

#endif
#endif