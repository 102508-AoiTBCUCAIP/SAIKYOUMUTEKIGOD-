
using UnityEngine;

public class RacketGenerator : MonoBehaviour
{
    public Material redMaterial;
    public Material blackMaterial;
    public Material woodMaterial;

    void Start()
    {
        // ラケット本体
        CreatePart(
            "Blade",
            PrimitiveType.Cylinder,
            new Vector3(0, 0.45f, 0),
            new Vector3(0.7f, 0.025f, 0.85f),
            woodMaterial
        );

        // 赤いラバー
        CreatePart(
            "Red Rubber",
            PrimitiveType.Cylinder,
            new Vector3(0, 0.48f, 0),
            new Vector3(0.68f, 0.008f, 0.83f),
            redMaterial
        );

        // 黒いラバー
        CreatePart(
            "Black Rubber",
            PrimitiveType.Cylinder,
            new Vector3(0, 0.42f, 0),
            new Vector3(0.68f, 0.008f, 0.83f),
            blackMaterial
        );

        // 持ち手
        CreatePart(
            "Handle",
            PrimitiveType.Cube,
            new Vector3(0, 0.45f, -0.65f),
            new Vector3(0.16f, 0.09f, 0.55f),
            woodMaterial
        );
    }

    void CreatePart(
        string partName,
        PrimitiveType type,
        Vector3 position,
        Vector3 scale,
        Material material
    )
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = partName;
        part.transform.SetParent(transform, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;

        if (material != null)
        {
            part.GetComponent<Renderer>().material = material;
        }
    }
}
