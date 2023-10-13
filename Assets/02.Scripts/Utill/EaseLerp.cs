using UnityEngine;

//(https://easings.net/ 참고)
public static class EaseLerp
{
    public static float easeOutBounce(float x)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1f / d1)
        {
            return n1 * x * x;
        }
        else if (x < 2f / d1)
        {
            x -= 1.5f / d1;
            return n1 * x * x + 0.75f;
        }
        else if (x < 2.5f / d1)
        {
            x -= 2.25f / d1;
            return n1 * x * x + 0.9375f;
        }
        else
        {
            x -= 2.625f / d1;
            return n1 * x * x + 0.984375f;
        }
    }

    /// <summary> 상승 효과 </summary>
    public static float easeOutQuint(float x)
    {
        return 1 - Mathf.Pow(1 - x, 5);
    }

    /// <summary> 점점 투명하게 만들 함수 </summary>
    public static float easeInCirc(float x)
    {
        return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
    }

    public static float easeInQuart(float x)
    {
        return x * x * x * x;
    }
}

