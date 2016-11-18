using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

class CardinalDirection {
    public static readonly Vector2 Up = new Vector2(0, 1);
    public static readonly Vector2 Down = new Vector2(0, -1);
    public static readonly Vector2 Right = new Vector2(1, 0);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 UpRight = new Vector2(1, 1);
    public static readonly Vector2 UpLeft = new Vector2(-1, 1);
    public static readonly Vector2 DownRight = new Vector2(1, -1);
    public static readonly Vector2 DownLeft = new Vector2(-1, -1);
}

public enum Swipe {
    None,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
};

public enum Tap {
    None,
    Single,
    Double
};

public class SwipeManager : MonoBehaviour {
    #region Inspector Variables

    [Tooltip("Min swipe distance (inches) to register as swipe")]
    [SerializeField]
    float minSwipeLength = 0.5f;

    [Tooltip("Whether to detect eight or four cardinal directions")]
    [SerializeField]
    bool useEightDirections = false;

    #endregion

    const float eightDirAngle = 0.906f;
    const float fourDirAngle = 0.5f;
    const float defaultDPI = 72f;
    const float dpcmFactor = 2.54f;

    static Dictionary<Swipe, Vector2> cardinalDirections = new Dictionary<Swipe, Vector2>()
    {
        { Swipe.Up,         CardinalDirection.Up         },
        { Swipe.Down,         CardinalDirection.Down         },
        { Swipe.Right,         CardinalDirection.Right     },
        { Swipe.Left,         CardinalDirection.Left         },
        { Swipe.UpRight,     CardinalDirection.UpRight     },
        { Swipe.UpLeft,     CardinalDirection.UpLeft     },
        { Swipe.DownRight,     CardinalDirection.DownRight },
        { Swipe.DownLeft,     CardinalDirection.DownLeft     }
    };

    public delegate void OnSwipeDetectedHandler(Swipe swipeDirection);

    static OnSwipeDetectedHandler _OnSwipeDetected;
    public static event OnSwipeDetectedHandler OnSwipeDetected {
        add {
            _OnSwipeDetected += value;
            autoDetectSwipes = true;
        }
        remove {
            _OnSwipeDetected -= value;
        }
    }

    static float dpcm;
    static bool autoDetectSwipes;
    static Swipe swipeDirection;
    static Tap tap;
    static Vector2 firstPressPos;
    static Vector2 secondPressPos;
    static SwipeManager instance;
    static float touchDuration;
    static Touch touch;


    void Awake() {
        instance = this;
        float dpi = (Screen.dpi == 0) ? defaultDPI : Screen.dpi;
        dpcm = dpi / dpcmFactor;
    }

    void Update() {
        if (autoDetectSwipes) {
            DetectSwipe();
        }
    }

    /// <summary>
    /// Attempts to detect the current swipe direction.
    /// Should be called over multiple frames in an Update-like loop.
    /// </summary>
    static void DetectSwipe() {
        if (GetTouchInput() || GetMouseInput()) {
            Vector2 currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            float swipeCm = currentSwipe.magnitude / dpcm;

            // Make sure it was a legit swipe, not a tap
            if (swipeCm < instance.minSwipeLength) {
                swipeDirection = Swipe.None;
                touchDuration += Time.deltaTime;

                if (touch.phase == TouchPhase.Ended && touchDuration < 0.2f) // making sure it only check the touch once && it was a short touch/tap and not a dragging.
                    instance.StartCoroutine("singleOrDouble");
                return;
            }
            else {
                tap = Tap.None;
                touchDuration = 0.0f;
            }

            swipeDirection = GetSwipeDirByTouch(currentSwipe);

            if (_OnSwipeDetected != null) {
                _OnSwipeDetected(swipeDirection);
            }
        } else {
            swipeDirection = Swipe.None;
        }
    }

    public static bool IsSingleTap() { return IsTap(Tap.Single);  }
    public static bool IsDoubleTap() { return IsTap(Tap.Double); }
    public static bool IsSwipingRight() { return IsSwipingDirection(Swipe.Right); }
    public static bool IsSwipingLeft() { return IsSwipingDirection(Swipe.Left); }
    public static bool IsSwipingUp() { return IsSwipingDirection(Swipe.Up); }
    public static bool IsSwipingDown() { return IsSwipingDirection(Swipe.Down); }
    public static bool IsSwipingDownLeft() { return IsSwipingDirection(Swipe.DownLeft); }
    public static bool IsSwipingDownRight() { return IsSwipingDirection(Swipe.DownRight); }
    public static bool IsSwipingUpLeft() { return IsSwipingDirection(Swipe.UpLeft); }
    public static bool IsSwipingUpRight() { return IsSwipingDirection(Swipe.UpRight); }

    #region Helper Functions

    static bool GetTouchInput() {
        if (Input.touches.Length > 0) {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                firstPressPos = new Vector2(touch.position.x, touch.position.y);
            }

            if (touch.phase == TouchPhase.Ended) {
                secondPressPos = new Vector2(touch.position.x, touch.position.y);
                return true;
            }
        }

        return false;
    }

    IEnumerator singleOrDouble() {
        yield return new WaitForSeconds(0.2f);
        if (touch.tapCount == 1)
            tap = Tap.Single;
        else if (touch.tapCount == 2) {
            //this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
            StopCoroutine("singleOrDouble");
            tap = Tap.Double;
        }
    }

    static bool GetMouseInput() {
        if (Input.GetMouseButtonDown(0)) {
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        } else if (Input.GetMouseButtonUp(0)) {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            return true;
        }

        return false;
    }

    static bool IsDirection(Vector2 direction, Vector2 cardinalDirection) {
        var angle = instance.useEightDirections ? eightDirAngle : fourDirAngle;
        return Vector2.Dot(direction, cardinalDirection) > angle;
    }

    static Swipe GetSwipeDirByTouch(Vector2 currentSwipe) {
        currentSwipe.Normalize();
        var swipeDir = cardinalDirections.FirstOrDefault(dir => IsDirection(currentSwipe, dir.Value));
        return swipeDir.Key;
    }

    static bool IsSwipingDirection(Swipe swipeDir) {
        DetectSwipe();
        return swipeDirection == swipeDir;
    }

    static bool IsTap(Tap tapType) {
        DetectSwipe();
        return tap == tapType;
    }

    #endregion
}