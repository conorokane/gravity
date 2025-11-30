using UnityEngine;

public class BlendShapeAnimator : MonoBehaviour
{
	SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;

	[Range(0, 1)]
	public float blendAmount = 0;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
        skinnedMesh = GetComponent<SkinnedMeshRenderer> ().sharedMesh;
    }

    void Update()
    {
		skinnedMeshRenderer.SetBlendShapeWeight(0, MathUtils.Remap(blendAmount, 0, 1, 100, 0));
		skinnedMeshRenderer.SetBlendShapeWeight(1, MathUtils.Remap(blendAmount, 0, 1, 0, 100));
    }
}
