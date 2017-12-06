using UnityEngine;
using System.Collections;
using TMPro;

public class WarpText : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    public float radius = 100;
    int maxCharachters = 14;

    void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
    }

    void Start()
    {
        Warp();
    }

    void Warp()
    {
        Vector3[] vertices;
        Matrix4x4 matrix;

        m_TextComponent.havePropertiesChanged = true;

        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        int characterCount = textInfo.characterCount;
        int minOffset = 0;
        int maxOffset = 0;
        if (characterCount < maxCharachters)
        {             
            if (characterCount % 2 != 0) //не четное
            {
                maxCharachters++;
            }
            minOffset = (maxCharachters - characterCount) / 2;
            maxOffset = maxCharachters - minOffset;
        }

        if (characterCount == 0) return;

        for (int i = minOffset; i < maxOffset; i++)
        {
            if (!textInfo.characterInfo[i - minOffset].isVisible)
                continue;
            float angle = (i * (180 / (maxCharachters - 1))) * Mathf.Deg2Rad;
            Vector3 newPos = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

            int vertexIndex = textInfo.characterInfo[i- minOffset].vertexIndex;            
            int materialIndex = textInfo.characterInfo[i - minOffset].materialReferenceIndex;
            vertices = textInfo.meshInfo[materialIndex].vertices;
            Vector3 offsetToMidBaseline = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i - minOffset].baseLine);
            
            vertices[vertexIndex + 0] += - offsetToMidBaseline;
            vertices[vertexIndex + 1] += - offsetToMidBaseline;
            vertices[vertexIndex + 2] += - offsetToMidBaseline;
            vertices[vertexIndex + 3] += - offsetToMidBaseline;

            matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90), Vector3.one);
            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

            vertices[vertexIndex + 0] += -newPos;
            vertices[vertexIndex + 1] += -newPos;
            vertices[vertexIndex + 2] += -newPos;
            vertices[vertexIndex + 3] += -newPos;
        }

        m_TextComponent.UpdateVertexData();
    }

    Vector3 Rotate(Vector3 point, float angle)
    {
        Vector3 rotated_point = Vector3.zero;
        rotated_point.x = point.x * Mathf.Cos(angle) - point.y * Mathf.Sin(angle);
        rotated_point.y = point.x * Mathf.Sin(angle) + point.y * Mathf.Cos(angle);
        return rotated_point;
    }
}