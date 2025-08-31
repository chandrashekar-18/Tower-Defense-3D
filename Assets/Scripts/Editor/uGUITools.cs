#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Editor
{
    /// <summary>
    /// Provides editor tools for manipulating uGUI RectTransforms.
    /// </summary>
    public class UGUITools : MonoBehaviour
    {
        [MenuItem("uGUI/Anchors to Corners %[")]
        private static void AnchorsToCorners()
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform rectTransform = transform as RectTransform;
                RectTransform parentRect = Selection.activeTransform.parent as RectTransform;

                if (rectTransform == null || parentRect == null) return;

                Vector2 newAnchorsMin = new Vector2(
                    rectTransform.anchorMin.x + rectTransform.offsetMin.x / parentRect.rect.width,
                    rectTransform.anchorMin.y + rectTransform.offsetMin.y / parentRect.rect.height);
                Vector2 newAnchorsMax = new Vector2(
                    rectTransform.anchorMax.x + rectTransform.offsetMax.x / parentRect.rect.width,
                    rectTransform.anchorMax.y + rectTransform.offsetMax.y / parentRect.rect.height);

                rectTransform.anchorMin = newAnchorsMin;
                rectTransform.anchorMax = newAnchorsMax;
                rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
            }
        }

        [MenuItem("uGUI/Corners to Anchors %]")]
        private static void CornersToAnchors()
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform rectTransform = transform as RectTransform;

                if (rectTransform == null) return;

                rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
            }
        }

        [MenuItem("uGUI/Mirror Horizontally Around Anchors %;")]
        private static void MirrorHorizontallyAnchors()
        {
            MirrorHorizontally(false);
        }

        [MenuItem("uGUI/Mirror Horizontally Around Parent Center %:")]
        private static void MirrorHorizontallyParent()
        {
            MirrorHorizontally(true);
        }

        private static void MirrorHorizontally(bool mirrorAnchors)
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform rectTransform = transform as RectTransform;
                RectTransform parentRect = Selection.activeTransform.parent as RectTransform;

                if (rectTransform == null || parentRect == null) return;

                if (mirrorAnchors)
                {
                    Vector2 oldAnchorMin = rectTransform.anchorMin;
                    rectTransform.anchorMin = new Vector2(1 - rectTransform.anchorMax.x, rectTransform.anchorMin.y);
                    rectTransform.anchorMax = new Vector2(1 - oldAnchorMin.x, rectTransform.anchorMax.y);
                }

                Vector2 oldOffsetMin = rectTransform.offsetMin;
                rectTransform.offsetMin = new Vector2(-rectTransform.offsetMax.x, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(-oldOffsetMin.x, rectTransform.offsetMax.y);

                rectTransform.localScale = new Vector3(-rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
            }
        }

        [MenuItem("uGUI/Mirror Vertically Around Anchors %'")]
        private static void MirrorVerticallyAnchors()
        {
            MirrorVertically(false);
        }

        [MenuItem("uGUI/Mirror Vertically Around Parent Center %\"")]
        private static void MirrorVerticallyParent()
        {
            MirrorVertically(true);
        }

        private static void MirrorVertically(bool mirrorAnchors)
        {
            foreach (Transform transform in Selection.transforms)
            {
                RectTransform rectTransform = transform as RectTransform;
                RectTransform parentRect = Selection.activeTransform.parent as RectTransform;

                if (rectTransform == null || parentRect == null) return;

                if (mirrorAnchors)
                {
                    Vector2 oldAnchorMin = rectTransform.anchorMin;
                    rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, 1 - rectTransform.anchorMax.y);
                    rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1 - oldAnchorMin.y);
                }

                Vector2 oldOffsetMin = rectTransform.offsetMin;
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, -rectTransform.offsetMax.y);
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -oldOffsetMin.y);

                rectTransform.localScale = new Vector3(rectTransform.localScale.x, -rectTransform.localScale.y, rectTransform.localScale.z);
            }
        }
    }
}
#endif