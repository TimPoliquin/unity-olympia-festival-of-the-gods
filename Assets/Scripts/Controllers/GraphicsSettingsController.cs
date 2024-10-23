using System.Collections;
using System.Collections.Generic;
using Azul.GraphicsSettings;
using Azul.Model;
using Azul.Utils;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.Rendering;
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
            MSAA_2X,
            MSAA_4X,
            MSAA_8X
        }
    }

    namespace Controller
    {
        public class GraphicsSettingsController : MonoBehaviour
        {
            [SerializeField] private UniversalRenderPipelineAsset[] qualityLevels;

            void Start()
            {
                this.LoadGraphicsConfig();
            }

            public void LoadGraphicsConfig()
            {
                GraphicsOptions graphicsOptions = FileUtils.LoadResource<GraphicsOptions>("graphics.dat");
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
                FileUtils.SaveResource("graphics.dat", new GraphicsOptions
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
                switch (QualitySettings.antiAliasing)
                {
                    case 0:
                        return AntiAliasingLevel.NONE;
                    case 2:
                        return AntiAliasingLevel.MSAA_2X;
                    case 4:
                        return AntiAliasingLevel.MSAA_4X;
                    case 8:
                        return AntiAliasingLevel.MSAA_8X;
                    default:
                        return AntiAliasingLevel.NONE;
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
                int msaa;
                switch (antiAliasingLevel)
                {
                    case AntiAliasingLevel.NONE:
                        msaa = 0;
                        break;
                    case AntiAliasingLevel.MSAA_2X:
                        msaa = 2;
                        break;
                    case AntiAliasingLevel.MSAA_4X:
                        msaa = 4;
                        break;
                    case AntiAliasingLevel.MSAA_8X:
                        msaa = 8;
                        break;
                    default:
                        msaa = 0;
                        break;
                }
                QualitySettings.antiAliasing = msaa;
            }

            public void SetRenderScale(float renderScale)
            {
                ((UniversalRenderPipelineAsset)QualitySettings.renderPipeline).renderScale = renderScale;
            }

            public void SetQualityLevel(GraphicsLevel qualityLevel)
            {
                QualitySettings.SetQualityLevel((int)qualityLevel);
            }

            public void SetVsync(bool vsync)
            {
                QualitySettings.vSyncCount = vsync ? 1 : 0;
            }
        }
    }
}
