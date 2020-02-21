using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public class TeleportHereOnStart : VRTK_SetupListener
    {
        bool teleported;

        protected override void OnSDKSetupInitialized()
        {
            if (teleported)
            {
                return;
            }

            var teleporter = FindObjectOfType<VRTK_BasicTeleport>();
            if (teleporter != null)
            {
                RaycastHit target;
                if (Physics.Raycast(transform.position, Vector3.down, out target))
                {
                    teleporter.Teleport(target.transform, target.point, transform.localRotation, true);
                    teleported = true;
                }
            }
        }
    }
}