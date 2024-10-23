using Azul.GraphicsSettings;
using Azul.Model;
using Azul.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Azul
{
    namespace GraphicsSettings
    {
        public enum GraphicsLevel
        {
            LOW,
            MEDIUM,
            HIGH
        }
        public enum AntiAliasingLevel
        {
            NONE,
            LOW,
            MEDIUM,
            HIGH
        }
    }

    namespace Controller
    {
        public class GraphicsSettingsController : MonoBehaviour
        {
            private static readonly string GRAPHICS_FILENAME = "graphics.dat";
            [SerializeField] private UniversalRenderPipelineAsset[] qualityLevels;

            void Start()
            {
                this.LoadGraphicsConfig();
            }

            public void LoadGraphicsConfig()
            {
                GraphicsOptions graphicsOptions = FileUtils.LoadResource<GraphicsOptions>(GRAPHICS_FILENAME);
                if (null != graphicsOptions)
                {
                    this.SetQualityLevel(graphicsOptions.Level);
                    this.SetAntiAliasing(graphicsOptions.AntiAliasingLevel);
                    this.SetRenderScale(graphicsOptions.RenderScale);
                    this.SetVsync(graphicsOptions.VSync);
                }
            }

            public void SaveGraphicsConfig()
            {
                FileUtils.SaveResource(GRAPHICS_FILENAME, new GraphicsOptions
                {
                    Level = this.GetQualityLevel(),
                    AntiAliasingLevel = this.GetAntiAliasingLevel(),
                    RenderScale = this.GetRenderScale(),
                    VSync = this.GetVSync()
                });
            }

            public GraphicsLevel GetQualityLevel()
            {
                return (GraphicsLevel)QualitySettings.GetQualityLevel();
            }

            public AntiAliasingLevel GetAntiAliasingLevel()
            {
                UniversalAdditionalCameraData cameraSettings = System.Instance.GetCameraController().GetMainCamera().GetComponent<UniversalAdditionalCameraData>();
                if (cameraSettings.antialiasing == AntialiasingMode.None)
                {
                    return AntiAliasingLevel.NONE;
                }
                else
                {
                    switch (cameraSettings.antialiasingQuality)
                    {
                        case AntialiasingQuality.Low:
                            return AntiAliasingLevel.LOW;
                        case AntialiasingQuality.Medium:
                            return AntiAliasingLevel.MEDIUM;
                        case AntialiasingQuality.High:
                            return AntiAliasingLevel.HIGH;
                        default:
                            return AntiAliasingLevel.NONE;
                    }
                }
            }

            public bool GetVSync()
            {
                return QualitySettings.vSyncCount > 0;
            }

            public float GetRenderScale()
            {
                return ((UniversalRenderPipelineAsset)QualitySettings.renderPipeline).renderScale;
            }

            public void SetAntiAliasing(AntiAliasingLevel antiAliasingLevel)
            {
                UniversalAdditionalCameraData cameraSettings = System.Instance.GetCameraController().GetMainCamera().GetComponent<UniversalAdditionalCameraData>();
                switch (antiAliasingLevel)
                {
                    case AntiAliasingLevel.NONE:
                        cameraSettings.antialiasing = AntialiasingMode.None;
                        break;
                    case AntiAliasingLevel.LOW:
                        cameraSettings.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                        cameraSettings.antialiasingQuality = AntialiasingQuality.Low;
                        break;
                    case AntiAliasingLevel.MEDIUM:
                        cameraSettings.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                        cameraSettings.antialiasingQuality = AntialiasingQuality.Medium;
                        break;
                    case AntiAliasingLevel.HIGH:
                        cameraSettings.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                        cameraSettings.antialiasingQuality = AntialiasingQuality.High;
                        break;
                    default:
                        cameraSettings.antialiasing = AntialiasingMode.None;
                        break;
                }
            }

            public void SetRenderScale(float renderScale)
            {
                ((UniversalRenderPipelineAsset)QualitySettings.renderPipeline).renderScale = renderScale;
            }

            public void SetQualityLevel(GraphicsLevel qualityLevel)
            {
                float renderScale = this.GetRenderScale();
                QualitySettings.SetQualityLevel((int)qualityLevel);
                if (this.GetRenderScale() != renderScale)
                {
                    this.SetRenderScale(renderScale);
                }
            }

            public void SetVsync(bool vsync)
            {
                QualitySettings.vSyncCount = vsync ? 1 : 0;
            }
        }
    }
}
