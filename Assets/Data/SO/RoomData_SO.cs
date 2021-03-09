using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataAssets/New Room Data", fileName = "Room_")]
public class RoomData_SO : ScriptableObject
{
    [Serializable]
    public class PortalInfo
    {
        public int targetRoomIndex;
        public Vector3 portalPosition;
        public float playerRotationAfterTransgress;
        public Vector3 transgressToPosition;

    }

    [Serializable]
    public class EnemiesInfo
    {
        public EnemyType type;
        public int amount;
    }

    public int roomIndex;
    public Vector3 station;
    public Vector3 playerStartPosition;
    public List<PortalInfo> portals;
    public EnemiesInfo[] enemiesInTheRoom;
}
