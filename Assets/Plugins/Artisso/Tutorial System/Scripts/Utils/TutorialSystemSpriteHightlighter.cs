using System.Collections.Generic;
using UnityEngine;

namespace Artisso.TutorialSystem
{
    [DisallowMultipleComponent]
    public class TutorialSystemSpriteHightlighter : MonoBehaviour
    {
        [SerializeField]
        public Color outlineColor = Color.white;

        [Range(0, 10)]
        public int outlineDistance = 1;

        private SpriteRenderer spriteRenderer;
        private Material outlineMaterial;
        private List<Material> materials = new List<Material>();

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            outlineMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineSprite"));
            outlineMaterial.name = "OutlineSprite (Instance)";
            materials.Add(spriteRenderer.material);
            materials.Add(outlineMaterial);
            UpdateOutline(true);
        }

        void Update()
        {
            UpdateOutline(true);
        }

        void OnEnable()
        {
            spriteRenderer.materials = materials.ToArray();
        }

        void OnDisable()
        {
            //SetDefaultMaterials();
            UpdateOutline(false);
        }

        void OnDestroy()
        {
            // Destroy material instances
            SetDefaultMaterials();
            Destroy(outlineMaterial);
        }

        void UpdateOutline(bool outline)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", outline ? 1f : 0);
            mpb.SetColor("_OutlineColor", outlineColor);
            mpb.SetFloat("_OutlineSize", outlineDistance);
            spriteRenderer.SetPropertyBlock(mpb);
        }
        private void SetDefaultMaterials()
        {
            materials.Remove(outlineMaterial);
            spriteRenderer.materials = materials.ToArray();
        }
    }
}
