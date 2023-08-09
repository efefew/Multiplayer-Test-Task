using System.Collections.Generic;

using Mirror;

using UnityEngine;

public class SpawnPointsCreator : NetworkBehaviour
{
    public List<Transform> spawnPoints;
    private void Awake() => NetworkManager.startPositions = spawnPoints;
}
