using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Session")]
    [SerializeField] private string sessionName = "Room_01";

    [Header("Player")]
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Pickable Box")]
    [SerializeField] private NetworkPrefabRef pickableBoxPrefab;
    [SerializeField] private Transform[] boxSpawnPoints;

    private NetworkRunner runner;
    private bool boxesSpawned;

    private readonly Dictionary<PlayerRef, NetworkObject> playerObjects = new();

    public void StartHost()
    {
        _ = StartGame(GameMode.Host);
    }

    public void StartClient()
    {
        _ = StartGame(GameMode.Client);
    }

    private async Task StartGame(GameMode mode)
    {
        if (runner != null)
            return;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        NetworkSceneManagerDefault sceneManager =
            gameObject.AddComponent<NetworkSceneManagerDefault>();

        StartGameResult result = await runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            SceneManager = sceneManager
        });

        if (!result.Ok)
        {
            Debug.LogError($"[Fusion] StartGame Failed : {result.ShutdownReason}");
            return;
        }

        Debug.Log($"[Fusion] StartGame OK : {mode}");

        if (runner.IsServer)
        {
            SpawnBoxes();
        }
    }

    private Vector3 GetSpawnPosition(PlayerRef player)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int index = player.RawEncoded % spawnPoints.Length;
            return spawnPoints[index].position;
        }

        return new Vector3(player.RawEncoded * 2f, 1f, 0f);
    }

    private void SpawnBoxes()
    {
        if (!runner.IsServer)
            return;

        if (boxesSpawned)
            return;

        boxesSpawned = true;

        if (boxSpawnPoints == null || boxSpawnPoints.Length == 0)
        {
            Debug.LogWarning("boxSpawnPoints가 비어 있습니다.");
            return;
        }

        foreach (Transform point in boxSpawnPoints)
        {
            if (point == null)
                continue;

            runner.Spawn(
                pickableBoxPrefab,
                point.position,
                point.rotation,
                null
            );
        }

        Debug.Log($"상자 {boxSpawnPoints.Length}개 생성 완료");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerNetworkInput data = new PlayerNetworkInput();

        data.Move = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        data.CameraYaw = PlayerCameraController.LocalCameraYaw;

        NetworkButtons buttons = new NetworkButtons();

        buttons.Set((int)PlayerInputButton.Fire, Input.GetMouseButton(0));
        buttons.Set((int)PlayerInputButton.Jump, Input.GetButton("Jump"));
        buttons.Set((int)PlayerInputButton.Pickup, Input.GetKey(KeyCode.E));

        data.Buttons = buttons;
        input.Set(data);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 입장 : {player}");

        if (!runner.IsServer)
            return;

        Vector3 spawnPos = GetSpawnPosition(player);

        NetworkObject playerObject = runner.Spawn(
            playerPrefab,
            spawnPos,
            Quaternion.identity,
            player
        );

        playerObjects[player] = playerObject;
        runner.SetPlayerObject(player, playerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
            return;

        if (playerObjects.TryGetValue(player, out NetworkObject obj))
        {
            runner.Despawn(obj);
            playerObjects.Remove(player);
        }

        Debug.Log($"플레이어 퇴장 : {player}");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"[Fusion] Disconnected : {reason}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Debug.Log($"[Fusion] Shutdown : {reason}");
        this.runner = null;
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}