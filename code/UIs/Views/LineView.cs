using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Cysharp.Threading.Tasks;
using System.Threading;

public class LineView : MonoBehaviour
{
    [SerializeField] UILineRenderer lineRenderer;
    private const float startX = -400;
    private const float endX = 400;
    [SerializeField] private int N = 100;
    [SerializeField] private float amp = 200;
    [SerializeField] private int timing1 = 60;
    [SerializeField] private  int timing2 = 65;
    [SerializeField] private  float deltaAmp = 50;
    [SerializeField] private float deltaForTitle = 50;
    [SerializeField] private int intervalTime = 30;

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    void Start()
    {
        //DrawTitleLine();
    }

    public void PublicToken()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    public void CancelToken()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = null;
    }

    public void DrawTitleLine()
    {
        PublicToken();
        Adds2(cancellationTokenSource.Token).Forget();
    }

    async UniTask Adds2(CancellationToken token)
    {
        lineRenderer.Points.Clear();
        lineRenderer.SAD();
        while (true)
        {
            for (int n = 0; n < N; n++)
            {
                var nextX = startX + (endX - startX) * ((float)n / (float)N);
                var nextY = Y(nextX) + deltaForTitle;
                if (n == timing1)
                    nextY = amp + deltaForTitle;
                if (n > timing1 && n < timing2)
                    continue;
                if (n == timing2)
                    nextY = -amp + deltaForTitle;

                lineRenderer.Points.Add(new Vector2(nextX, nextY));
                lineRenderer.SAD();
                await UniTask.Delay(intervalTime,cancellationToken:token);
            }
            lineRenderer.Points.Clear();
            lineRenderer.SAD();
            if (amp <= 0)
            {
                break;
            }
        }
    }

    public void DrawGameOverLine()
    {
        PublicToken();
        Adds(cancellationTokenSource.Token).Forget();
    }

    async UniTask Adds(CancellationToken token)
    {
        lineRenderer.Points.Clear();
        lineRenderer.SAD();
        while (true) { 
            for (int n = 0; n < N; n++)
            {
                var nextX = startX + (endX - startX) * ((float)n / (float)N);
                var nextY = Y(nextX);
                if (n == timing1)
                    nextY = amp;
                if (n > timing1 && n < timing2)
                    continue;
                if (n == timing2)
                    nextY = -amp;
                lineRenderer.Points.Add(new Vector2(nextX, nextY));
                lineRenderer.SAD();
                await UniTask.Delay(intervalTime, cancellationToken: token);
            }
            lineRenderer.Points.Clear();
            lineRenderer.SAD();
            if(amp <= 0)
            {
                break;
            }
            amp = Mathf.Clamp(amp - deltaAmp, 0, amp);
        }
    }

    private float Y(float x)
    {
        return 100* Mathf.Sin(2 * Mathf.PI * 1 * x);
    }
}
