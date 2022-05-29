using UnityEngine;

public class GameController : MonoBehaviour
{
    public const int RUNNING = 0;
    public const int PAUSE = 1;
    public const int LOST = 2;
    public const int WIN = 3;
    [SerializeField] public int state = 0;
}
