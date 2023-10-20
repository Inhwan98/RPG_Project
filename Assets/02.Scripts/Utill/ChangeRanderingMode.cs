using System.Collections;
using UnityEngine;

public static class ChangeRanderingMode
{

    /// <summary> 랜더링모드 Opaque(불투명) 으로 변경 </summary>
    public static void Opaque(ref Material mat)
    {
        mat.SetFloat("_Mode", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }

    /// <summary> 랜더링모드 Fade로 변경 </summary>
    public static void Fade(ref Material mat)
    {
        mat.SetFloat("_Mode", 2);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    /// <summary> 활성화, 비활성화 여부에 따른 현재 오브젝트의 불투명도 </summary>
    public static IEnumerator ActiveAlphaValue(Material mat, bool value)
    {
        Fade(ref mat); //쉐이더 랜더링모드를 Fade로 변경

        Color color = mat.color;

        float time = 0.0f;
        float t = 0.0f;
        float destTime = 2.0f; //2초에 걸쳐서

        if (value == true)
        {
            color.a = 0;
            while (color.a != 1)
            {
                if (time < destTime)
                {
                    time += Time.deltaTime;
                    t = time / destTime;
                }

                color.a = Mathf.Lerp(0, 1, t);

                mat.color = color;
                yield return null;
            }
            Opaque(ref mat);
        }
        else
        {
            color.a = 1;
            while (color.a != 0)
            {
                if (time < destTime)
                {
                    time += Time.deltaTime;
                    t = time / destTime;
                }

                color.a = Mathf.Lerp(1, 0, t);

                mat.color = color;
                yield return null;
            }
        }
    }
}
