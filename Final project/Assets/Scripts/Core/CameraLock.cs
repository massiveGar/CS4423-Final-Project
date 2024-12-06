using UnityEngine;

public class AspectRatioLocker : MonoBehaviour
{
    Transform playerTransform;
    public float targetAspectRatio = 16f / 9f;

    void Start() {
        if(GameController.Instance != null) {
            playerTransform = GameController.Instance.GetPlayer().transform;
        }
        
    }

    void FixedUpdate() {
        AdjustCameraViewport();
        if(playerTransform != null) {
            FollowPlayer();
        }
    }

    // Keep the screen adjusted to the target aspect ratio (16:9)
    void AdjustCameraViewport() {
        float screenAspect = (float)Screen.width / Screen.height;
        float scaleHeight = screenAspect / targetAspectRatio;

        if(scaleHeight < 1f) {
            // Add letterbox (black bars on top and bottom)
            Rect rect = Camera.main.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            Camera.main.rect = rect;
        }
        else {
            // Add pillarbox (black bars on sides)
            float scaleWidth = 1f / scaleHeight;
            Rect rect = Camera.main.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            Camera.main.rect = rect;
        }
    }

    void FollowPlayer() {
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -10);
    }
}