using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void SetUp(Transform originalRootBone)
    {
        MatchAllChieldTransform(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, transform.position, 10f);

    }

    private void MatchAllChieldTransform(Transform root, Transform clone)
    {
        foreach (Transform chield in root)
        {
            Transform cloneChield = clone.Find(chield.name);
            if(cloneChield != null)
            {
                cloneChield.position = chield.position;
                cloneChield.rotation = chield.rotation;

                MatchAllChieldTransform(chield, cloneChield);

            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform chield  in root)
        {
            if(chield.TryGetComponent<Rigidbody>(out Rigidbody chieldRigidbody) )
            {
                chieldRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            ApplyExplosionToRagdoll(chield, explosionForce, explosionPosition, explosionRange);
        }
    }
}
