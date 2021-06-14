using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ballManager : MonoBehaviour {
    public hitDirection ballHitDirectionStrength;

    public ballMovement ballMoveType;

    public enum ballMovement {
        NORMAL,
        CURVE,
        SIN,
        TWITCH
    }

    public enum hitDirection {
        NORMAL,
        EXTREME
    }

}