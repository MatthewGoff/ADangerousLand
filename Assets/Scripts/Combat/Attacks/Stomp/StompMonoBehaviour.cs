using UnityEngine;
using System.Collections;
using ADL.Core;

namespace ADL.Combat.Attacks
{
    /// <summary>
    /// MonoBehaviour for a stomp attack
    /// </summary>
    /// <remarks>
    /// See the Manager/MonoBehaviour design pattern
    /// </remarks>
    public class StompMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The intensity of radial refraction as a function of time in the ripple visual effect
        /// </summary>
        public AnimationCurve Waveform = new AnimationCurve(
            new Keyframe(0.00f, 0.50f, 0, 0),
            new Keyframe(0.05f, 1.00f, 0, 0),
            new Keyframe(0.15f, 0.10f, 0, 0),
            new Keyframe(0.25f, 0.80f, 0, 0),
            new Keyframe(0.35f, 0.30f, 0, 0),
            new Keyframe(0.45f, 0.60f, 0, 0),
            new Keyframe(0.55f, 0.40f, 0, 0),
            new Keyframe(0.65f, 0.55f, 0, 0),
            new Keyframe(0.75f, 0.46f, 0, 0),
            new Keyframe(0.85f, 0.52f, 0, 0),
            new Keyframe(0.99f, 0.50f, 0, 0)
        );

        /// <summary>
        /// An envelope on refraction and color reflection in the ripple visual effect
        /// </summary>
        public AnimationCurve Decline = new AnimationCurve(
            new Keyframe(0.0f, 1.0f, 0, 0),
            new Keyframe(0.8f, 1.0f, 0, 0),
            new Keyframe(1.0f, 0.0f, 0, 0)
        );

        /// <summary>
        /// The strength of refraction in the ripple visual effect
        /// </summary>
        [Range(0.01f, 1.0f)]
        public float RefractionStrength = 1.0f;

        /// <summary>
        /// A color added to the ripple visual effect
        /// </summary>
        public Color ReflectionColor = Color.gray;

        /// <summary>
        /// The ammount of color added to the ripple visual effect
        /// </summary>
        [Range(0.01f, 1.0f)]
        public float ReflectionStrength = 0.5f;

        /// <summary>
        /// The speed of the wave in the ripple visual effect. No idea what the units are
        /// </summary>
        [Range(0.1f, 3.0f)]
        public float WaveSpeed = 0.6f;

        /// <summary>
        /// The shader used to animate the ripple visual effect
        /// </summary>
        public Shader Shader;

        /// <summary>
        /// The manager of this MonoBehaviour
        /// </summary>
        private StompManager Manager;

        /// <summary>
        /// The material used to display the ripple visual effect
        /// </summary>
        private Material Material;

        /// <summary>
        /// Gets called when the object attached to this MonoBehaviour is instantiated
        /// </summary>
        public void Awake()
        {
            InitMaterial();
            GetComponent<SpriteRenderer>().material = Material;
            StartCoroutine("Shockwave");
        }

        /// <summary>
        /// Expand the collider for this attack as well as progress the acompanying  visual effect
        /// </summary>
        /// <returns>
        /// Required return type of a coroutine
        /// </returns>
        private IEnumerator Shockwave()
        {
            float duration = 0.5f;
            for (float i = 0f; i < duration; i += Time.deltaTime)
            {
                transform.localScale = new Vector3(i * (40.0f / 0.5f), i * (40.0f / 0.5f), 1);

                Camera camera = GameManager.Singleton.PlayerCamera.GetComponent<Camera>();
                float aspectRatio = camera.aspect;
                Vector2 screenPosition = camera.WorldToScreenPoint(transform.position);
                screenPosition = new Vector2(screenPosition.x / camera.pixelWidth, 1f - screenPosition.y / camera.pixelHeight);

                Material.SetVector("_Drop", new Vector4(screenPosition.x * aspectRatio, screenPosition.y, i / duration, 0));
                Material.SetColor("_Reflection", ReflectionColor);
                Material.SetVector("_Params1", new Vector4(aspectRatio, 1, 1 / WaveSpeed, 0));
                Material.SetVector("_Params2", new Vector4(1, 1 / aspectRatio, RefractionStrength * Decline.Evaluate(i / duration), ReflectionStrength * Decline.Evaluate(i / duration)));

                yield return null;
            }
            Manager.Expire();
            Destroy(gameObject);
        }

        /// <summary>
        /// Initialize the material used to dispaly the ripple visual effect
        /// </summary>
        private void InitMaterial()
        {
            Material = new Material(Shader);
            Material.hideFlags = HideFlags.DontSave;
            Material.SetTexture("_GradTex", CreateGradientTexture());
        }

        /// <summary>
        /// Create the texture used to produce the ripple visual effect
        /// </summary>
        /// <returns>
        /// The created texture
        /// </returns>
        private Texture2D CreateGradientTexture()
        {
            Texture2D gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
            gradTexture.wrapMode = TextureWrapMode.Clamp;
            gradTexture.filterMode = FilterMode.Bilinear;
            for (int i = 0; i < gradTexture.width; i++)
            {
                float x = 1.0f / gradTexture.width * i;
                float a = Waveform.Evaluate(x);
                gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
            }
            gradTexture.Apply();
            return gradTexture;
        }

        /// <summary>
        /// Gets called when the collider attached to the same GameObject as this MonoBehaviour collides with another collider
        /// </summary>
        /// <param name="other">
        /// The collider with which this GameObject has collided
        /// </param>
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Hitbox")
            {
                IHitboxOwner monoBehaviour = other.gameObject.transform.parent.gameObject.GetComponent<IHitboxOwner>();
                if (monoBehaviour != null)
                {
                    Manager.ResolveCollision(monoBehaviour.GetCombatantManager());
                }
            }
        }

        /// <summary>
        /// Assign this MonoBehaviour a manager
        /// </summary>
        /// <param name="manager">
        /// The manager of this MonoBehaviour
        /// </param>
        public void AssignManager(StompManager manager)
        {
            Manager = manager;
        }
    }
}