using UnityEngine;

[CreateAssetMenu(menuName = "Create SkinData", fileName = "SkinData", order = 0)]
public class SkinData : ScriptableObject
{
	[Range(0, 50)]
	public float SubsurfaceScaler = 1f;
	public Color SubsurfaceColor = new Color(1f, 0.446f, 0.174f, 0f);
	public Color SubsurfaceFalloff = new Color(1f, 0.5497051f, 0.1462264f, 0f);
}