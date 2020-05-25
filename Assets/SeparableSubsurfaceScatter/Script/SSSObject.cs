using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class SSSObject : MonoBehaviour {
    private List<Vector4> kernel = new List<Vector4>(11);
    public SkinData data;
    private Renderer renderer;
    
	private void Awake () {
        renderer = GetComponent<Renderer>();
        SSSFunction.SetKernel(ref data, kernel);
    }

    private void OnWillRenderObject()
    {
        var skinRenderer = SSSCamera.currentSkr;
        if (skinRenderer.RenderCamera == null)
        {
            return;
        }
        SSSFunction.UpdateSubsurface(ref skinRenderer, ref data, ref renderer, kernel);
    }
}
