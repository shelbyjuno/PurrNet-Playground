using UnityEngine;
using PurrNet;

public class PlayerModelToggle : NetworkBehaviour
{
    [SerializeField] MeshRenderer[] playerMeshes;

    protected override void OnOwnerChanged(PlayerID? oldOwner, PlayerID? newOwner, bool asServer)
    {
        base.OnOwnerChanged(oldOwner, newOwner, asServer);

        foreach (var mesh in playerMeshes)
            mesh.shadowCastingMode = isOwner ? UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly : UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
