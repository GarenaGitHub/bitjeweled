using UnityEngine;

/// <summary>
/// Spin tile. just a demo ;)
/// </summary>
public class SpinTile : MonoBehaviour
{
    public float SpinningDegrees = 15.0f;
    public bool SpinOnX = false;
    public bool SpinOnY = true;
    public bool SpinOnZ = false;

    private float rnd_diff = 0.0f;
    private Vector3 Xaxis = new Vector3(1, 0, 0);
    private Vector3 Yaxis = new Vector3(0, 1, 0);
    private Vector3 Zaxis = new Vector3(0, 0, 1);

    private Vector3 spinningAxis = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        rnd_diff = Random.Range(2.0f, 20.0f);
        if (SpinOnX)
            spinningAxis += Xaxis;
        if (SpinOnY)
            spinningAxis += Yaxis;
        if (SpinOnZ)
            spinningAxis += Zaxis;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinningAxis, (SpinningDegrees + rnd_diff) * Time.deltaTime);
    }
}
