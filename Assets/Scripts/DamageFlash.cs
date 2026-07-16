using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Wire PlayerHealth.onDamaged (Inspector) to Flash() for a red screen-flash hit indicator.
public class DamageFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.2f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.4f);

    Coroutine routine;

    public void Flash()
    {
        if (flashImage == null) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        flashImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }
}
