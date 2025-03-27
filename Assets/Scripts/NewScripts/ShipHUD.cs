using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipGUD : MonoBehaviour
{
    [SerializeField]
    float updateRate;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color lockColor;
    [SerializeField]
    List<GameObject> helpDialogs;
    [SerializeField]
    Compass compass;
    [SerializeField]
    PitchLadder pitchLadder;
    [SerializeField]
    Bar throttleBar;
    [SerializeField]
    Transform hudCenter;
    [SerializeField]
    Transform velocityMarker;
    [SerializeField]
    Text airspeed;
    [SerializeField]
    Text aoaIndicator;
    [SerializeField]
    Text gforceIndicator;
    [SerializeField]
    Text altitude;
    [SerializeField]
    Bar healthBar;
    [SerializeField]
    Text healthText;
    [SerializeField]
    Transform targetBox;
    [SerializeField]
    Text targetName;
    [SerializeField]
    Text targetRange;
    [SerializeField]
    Transform missileLock;
    [SerializeField]
    Transform reticle;
    [SerializeField]
    RectTransform reticleLine;
    [SerializeField]
    RectTransform targetArrow;
    [SerializeField]
    RectTransform missileArrow;
    [SerializeField]
    float targetArrowThreshold;
    [SerializeField]
    float missileArrowThreshold;
    [SerializeField]
    float cannonRange;
    [SerializeField]
    float bulletSpeed;
    [SerializeField]
    GameObject aiMessage;

    [SerializeField]
    List<Graphic> missileWarningGraphics;

    // Data for the plane and target simulation
    public Transform planeTransform;
    public Transform targetTransform;
    public Vector3 planeVelocity;
    public Vector3 targetVelocity;
    public bool missileLocked;
    public bool missileTracking;
    public string targetNameText = "Target Name";
    public float planeHealth = 100f;
    public float maxHealth = 100f;

    Camera camera;
    Transform cameraTransform;

    GameObject hudCenterGO;
    GameObject velocityMarkerGO;
    GameObject targetBoxGO;
    Image targetBoxImage;
    GameObject missileLockGO;
    Image missileLockImage;
    GameObject reticleGO;
    GameObject targetArrowGO;
    GameObject missileArrowGO;

    float lastUpdateTime;

    const float metersToKnots = 1.94384f;
    const float metersToFeet = 3.28084f;

    void Start()
    {
        hudCenterGO = hudCenter.gameObject;
        velocityMarkerGO = velocityMarker.gameObject;
        targetBoxGO = targetBox.gameObject;
        targetBoxImage = targetBox.GetComponent<Image>();
        missileLockGO = missileLock.gameObject;
        missileLockImage = missileLock.GetComponent<Image>();
        reticleGO = reticle.gameObject;
        targetArrowGO = targetArrow.gameObject;
        missileArrowGO = missileArrow.gameObject;
    }

    public void SetCamera(Camera camera)
    {
        this.camera = camera;

        if (camera == null)
        {
            cameraTransform = null;
        }
        else
        {
            cameraTransform = camera.GetComponent<Transform>();
        }

        if (compass != null)
        {
            compass.SetCamera(camera);
        }

        if (pitchLadder != null)
        {
            pitchLadder.SetCamera(camera);
        }
    }

    void UpdateVelocityMarker()
    {
        var velocity = planeVelocity;

        if (velocity.sqrMagnitude > 1)
        {
            var hudPos = TransformToHUDSpace(cameraTransform.position + velocity);

            if (hudPos.z > 0)
            {
                velocityMarkerGO.SetActive(true);
                velocityMarker.localPosition = new Vector3(hudPos.x, hudPos.y, 0);
            }
            else
            {
                velocityMarkerGO.SetActive(false);
            }
        }
    }

    void UpdateAirspeed()
    {
        var speed = planeVelocity.z * metersToKnots;
        airspeed.text = string.Format("{0:0}", speed);
    }

    void UpdateAltitude()
    {
        var altitude = planeTransform.position.y * metersToFeet;
        this.altitude.text = string.Format("{0:0}", altitude);
    }

    Vector3 TransformToHUDSpace(Vector3 worldSpace)
    {
        var screenSpace = camera.WorldToScreenPoint(worldSpace);
        return screenSpace - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }

    void UpdateHealth()
    {
        healthBar.SetValue(planeHealth / maxHealth);
        healthText.text = string.Format("{0:0}", planeHealth);
    }

    void UpdateWeapons()
    {
        if (targetTransform == null)
        {
            targetBoxGO.SetActive(false);
            missileLockGO.SetActive(false);
            return;
        }

        // Update target box, missile lock
        var targetDistance = Vector3.Distance(planeTransform.position, targetTransform.position);
        var targetPos = TransformToHUDSpace(targetTransform.position);
        var missileLockPos = missileLocked ? targetPos : TransformToHUDSpace(planeTransform.position + (targetTransform.position - planeTransform.position).normalized * targetDistance);

        if (targetPos.z > 0)
        {
            targetBoxGO.SetActive(true);
            targetBox.localPosition = new Vector3(targetPos.x, targetPos.y, 0);
        }
        else
        {
            targetBoxGO.SetActive(false);
        }

        if (missileTracking && missileLockPos.z > 0)
        {
            missileLockGO.SetActive(true);
            missileLock.localPosition = new Vector3(missileLockPos.x, missileLockPos.y, 0);
        }
        else
        {
            missileLockGO.SetActive(false);
        }

        if (missileLocked)
        {
            targetBoxImage.color = lockColor;
            targetName.color = lockColor;
            targetRange.color = lockColor;
            missileLockImage.color = lockColor;
        }
        else
        {
            targetBoxImage.color = normalColor;
            targetName.color = normalColor;
            targetRange.color = normalColor;
            missileLockImage.color = normalColor;
        }

        targetName.text = targetNameText;
        targetRange.text = string.Format("{0:0 m}", targetDistance);

        // Update target arrow
        var targetDir = (targetTransform.position - planeTransform.position).normalized;
        var targetAngle = Vector3.Angle(cameraTransform.forward, targetDir);

        if (targetAngle > targetArrowThreshold)
        {
            targetArrowGO.SetActive(true);
            // Add 180 degrees if target is behind camera
            float flip = targetPos.z > 0 ? 0 : 180;
            targetArrow.localEulerAngles = new Vector3(0, 0, flip + Vector2.SignedAngle(Vector2.up, new Vector2(targetPos.x, targetPos.y)));
        }
        else
        {
            targetArrowGO.SetActive(false);
        }

        // Update target lead (reticle)
        var leadPos = planeTransform.position + (targetTransform.position - planeTransform.position).normalized * targetDistance;
        var reticlePos = TransformToHUDSpace(leadPos);

        if (reticlePos.z > 0 && targetDistance <= cannonRange)
        {
            reticleGO.SetActive(true);
            reticle.localPosition = new Vector3(reticlePos.x, reticlePos.y, 0);
        }
        else
        {
            reticleGO.SetActive(false);
        }
    }

    void UpdateWarnings()
    {
        // Missile warning (simulate incoming missile detection)
        missileArrowGO.SetActive(false);  // Simulate missile warning display off
    }

    void LateUpdate()
    {
        if (camera == null) return;

        throttleBar.SetValue(1);  // Placeholder throttle value, replace with actual value

        if (missileLocked)
        {
            missileWarningGraphics.ForEach(g => g.color = lockColor);
        }

        UpdateVelocityMarker();
        UpdateHealth();
        UpdateWeapons();
        UpdateWarnings();

        if (Time.time > lastUpdateTime + (1f / updateRate))
        {
            lastUpdateTime = Time.time;
            UpdateAirspeed();
            UpdateAltitude();
        }
    }
}
