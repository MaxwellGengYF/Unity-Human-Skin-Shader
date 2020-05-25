using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class SSSFunction
{
    public static void InitSkinRender(ref SkinRender skr, Camera cam)
    {
        skr.RenderCamera = cam;
        skr.SubsurfaceBuffer = new CommandBuffer();
        skr.SubsurfaceBuffer.name = "Separable Subsurface Scatter";
        skr.SubsurfaceEffects = new Material(Shader.Find("Hidden/SeparableSubsurfaceScatter"));
        cam.clearStencilAfterLightingPass = true;
    }

    public static void DisposeSkinRender(ref SkinRender skr)
    {
        skr.RenderCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, skr.SubsurfaceBuffer);
        skr.SubsurfaceBuffer.Dispose();
    }

    public static void ClearBuffer(ref SkinRender skr)
    {
        if (skr.SubsurfaceBuffer == null)
        {
            return;
        }
        
        skr.SubsurfaceBuffer.Clear();
        skr.SubsurfaceBuffer.GetTemporaryRT(ShaderIDs._SceneColor, skr.RenderCamera.pixelWidth, skr.RenderCamera.pixelHeight, 0, FilterMode.Point, RenderTextureFormat.DefaultHDR);
    }

    public static void SetKernel(ref SkinData data, List<Vector4> kernel)
    {
        Vector3 SSSC = new Vector3(data.SubsurfaceColor.r, data.SubsurfaceColor.g, data.SubsurfaceColor.b);
        Vector3 SSSFC = new Vector3(data.SubsurfaceFalloff.r, data.SubsurfaceFalloff.g, data.SubsurfaceFalloff.b);
        SeparableSSS.CalculateKernel(kernel, 11, SSSC, SSSFC);
    }

    public static void UpdateSubsurface(ref SkinRender skr, ref SkinData data, ref Renderer renderer, List<Vector4> kernelArray)
    {
        ///SSS Buffer
        skr.SubsurfaceBuffer.SetGlobalFloat(ShaderIDs._SSSScale, data.SubsurfaceScaler);
        skr.SubsurfaceBuffer.SetGlobalVectorArray(ShaderIDs._Kernel, kernelArray);
        skr.SubsurfaceBuffer.SetGlobalFloat(ShaderIDs._RandomNumber, Random.Range(0f, 1000f));
        skr.SubsurfaceBuffer.SetGlobalTexture(ShaderIDs._MainTex, BuiltinRenderTextureType.CameraTarget);
        skr.SubsurfaceBuffer.SetRenderTarget(ShaderIDs._SceneColor, BuiltinRenderTextureType.CameraTarget);
        skr.SubsurfaceBuffer.DrawRenderer(renderer, skr.SubsurfaceEffects, 0, 0);
        skr.SubsurfaceBuffer.SetGlobalFloat(ShaderIDs._RandomNumber, Random.Range(0f, 1000f));
        skr.SubsurfaceBuffer.SetGlobalTexture(ShaderIDs._MainTex, ShaderIDs._SceneColor);
        skr.SubsurfaceBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
        skr.SubsurfaceBuffer.DrawRenderer(renderer, skr.SubsurfaceEffects, 0, 1);
    }
}


public struct SkinRender
{
    public Material SubsurfaceEffects;
    public Camera RenderCamera;
    public CommandBuffer SubsurfaceBuffer;
}

public static partial class ShaderIDs
{
    public static int _SceneColor = Shader.PropertyToID("_SceneColor");
    public static int _Kernel = Shader.PropertyToID("_Kernel");
    public static int _SSSScale = Shader.PropertyToID("_SSSScale");
    public static int _Jitter = Shader.PropertyToID("_Jitter");
    public static int _screenSize = Shader.PropertyToID("_screenSize");
    public static int _RandomNumber = Shader.PropertyToID("_RandomNumber");
}