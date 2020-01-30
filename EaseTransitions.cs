using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using static Game.EaseSystem.EaseFunction;

namespace Game.EaseSystem
{
    #region Components
    // Supported Components and their Fields/Properties

    public enum ComponentTypes { Transform, SpriteRenderer, RectTransform, Image, Text }

    public enum TransformFields
    {
        PositionX, PositionY, PositionZ,
        LocalPositionX, LocalPositionY, LocalPositionZ,
        RotationX, RotationY, RotationZ, RotationW,
        LocalRotationX, LocalRotationY, LocalRotationZ, LocalRotationW,
        EulerAnglesX, EulerAnglesY, EulerAnglesZ,
        LocalEulerAnglesX, LocalEulerAnglesY, LocalEulerAnglesZ,
        LocalScaleX, LocalScaleY, LocalScaleZ
    }
    public enum SpriteRendererFields
    {
        ColorRed, ColorGreen, ColorBlue, ColorAlpha,
        SizeX,SizeY
    }
    public enum RectTransformFields
    {
        PositionX, PositionY, PositionZ,
        LocalPositionX, LocalPositionY, LocalPositionZ,
        AnchoredPositionX, AnchoredPositionY,
        AnchoredPosition3DX, AnchoredPosition3DY, AnchoredPosition3DZ,
        SizeDeltaX, SizeDeltaY,
        RotationX, RotationY, RotationZ, RotationW,
        LocalRotationX, LocalRotationY, LocalRotationZ, LocalRotationW,
        EulerAnglesX, EulerAnglesY, EulerAnglesZ,
        LocalEulerAnglesX, LocalEulerAnglesY, LocalEulerAnglesZ,
        LocalScaleX, LocalScaleY, LocalScaleZ,
        AnchorMinX, AnchorMinY,
        AnchorMaxX, AnchorMaxY,
        OffsetMinX, OffsetMinY,
        OffsetMaxX, OffsetMaxY
    }
    public enum ImageFields
    {
        ColorRed, ColorGreen, ColorBlue, ColorAlpha,
        Fill
    }
    public enum TextFields
    {
        ColorRed, ColorGreen, ColorBlue, ColorAlpha
    }
    #endregion Components


    #region Data Containers
    // Data container for GameObjects being Transitioned
    public class TransitionProperties
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }
        public SpriteRenderer spriteRenderer { get; }
        public RectTransform rectTransform { get; }
        public Image image { get; }
        public Text text { get; }

        public Dictionary<Vector2Int, TransitionData> inTrans;

        public bool pause;


        public TransitionProperties(GameObject _gameObject)
        {
            gameObject = _gameObject;

            transform = null;
            spriteRenderer = null;
            rectTransform = null;
            image = null;
            text = null;

            if (gameObject.GetComponent<Transform>() != null)
                transform = gameObject.GetComponent<Transform>();
            if (gameObject.GetComponent<SpriteRenderer>() != null)
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (gameObject.GetComponent<RectTransform>() != null)
                rectTransform = gameObject.GetComponent<RectTransform>();
            if (gameObject.GetComponent<Image>() != null)
                image = gameObject.GetComponent<Image>();
            if (gameObject.GetComponent<Text>() != null)
                text = gameObject.GetComponent<Text>();

            inTrans = new Dictionary<Vector2Int, TransitionData>();

            pause = false;
        }


        public void SetTransition(TransformFields field, EaseFunctions ease, float duration, float start, float end) => SetTransition(new Vector2Int((int)ComponentTypes.Transform, (int)field), ease, duration, start, end);
        public void SetTransition(SpriteRendererFields field, EaseFunctions ease, float duration, float start, float end) => SetTransition(new Vector2Int((int)ComponentTypes.SpriteRenderer, (int)field), ease, duration, start, end);
        public void SetTransition(RectTransformFields field, EaseFunctions ease, float duration, float start, float end) => SetTransition(new Vector2Int((int)ComponentTypes.RectTransform, (int)field), ease, duration, start, end);
        public void SetTransition(ImageFields field, EaseFunctions ease, float duration, float start, float end) => SetTransition(new Vector2Int((int)ComponentTypes.Image, (int)field), ease, duration, start, end);
        public void SetTransition(TextFields field, EaseFunctions ease, float duration, float start, float end) => SetTransition(new Vector2Int((int)ComponentTypes.Text, (int)field), ease, duration, start, end);

        public void SetTransition(ComponentTypes component, int enumInt, EaseFunctions ease, float duration, float start, float end) => SetTransition(new Vector2Int((int)component, enumInt), ease, duration, start, end);
        public void SetTransition(Vector2Int enumInt, EaseFunctions ease, float duration, float start, float end, bool RunTransition = true)
        {
            if (inTrans.ContainsKey(enumInt))
                inTrans[enumInt] = new TransitionData(ease, duration, start, end / EaseTransitions.animationSpeed);
            else
                inTrans.Add(enumInt, new TransitionData(ease, duration, start, end / EaseTransitions.animationSpeed));

            if (!EaseTransitions.transitions.Contains(this))
                EaseTransitions.transitions.Add(this);
        }

        public void ClearAllTransitions() => inTrans.Clear();
        public void ClearTransition(ComponentTypes component, int enumInt) => ClearTransition(new Vector2Int((int)component, enumInt));
        public void ClearTransition(Vector2Int enumInt)
        {
            if (inTrans.ContainsKey(enumInt))
                inTrans.Remove(enumInt);
        }
    }

    // Data container for a Transition
    public class TransitionData
    {
        public EaseFunctions ease;
        public float duration { get; }

        public float start { get; set; }
        public float end { get; }

        public bool timed;
        public float timer;


        public TransitionData(EaseFunctions _ease, float _duration, float _start, float _end)
        {
            ease = _ease;
            duration = _duration;

            start = _start;
            end = _end;

            timed = false;
            timer = 0;
        }
    }
    #endregion Data Containers

    [ExecuteInEditMode]
    public class EaseTransitions : MonoBehaviour
    {
        #region Get Fields
        // Gets float from a supported Component's Field/Property

        public float GetField(TransitionProperties prop, ComponentTypes component, int enumInt)
        {
            switch (component)
            {
                case ComponentTypes.Transform:
                    return GetField(prop.transform, (TransformFields)enumInt);
                case ComponentTypes.SpriteRenderer:
                    return GetField(prop.spriteRenderer, (SpriteRendererFields)enumInt);
                case ComponentTypes.RectTransform:
                    return GetField(prop.rectTransform, (RectTransformFields)enumInt);
                case ComponentTypes.Image:
                    return GetField(prop.image, (ImageFields)enumInt);
                case ComponentTypes.Text:
                    return GetField(prop.text, (TextFields)enumInt);
            }
            return 0;
        }
        public float GetField(GameObject obj, ComponentTypes component, int enumInt)
        {
            switch (component)
            {
                case ComponentTypes.Transform:
                    return GetField(obj.GetComponent<Transform>(), (TransformFields)enumInt);
                case ComponentTypes.SpriteRenderer:
                    return GetField(obj.GetComponent<SpriteRenderer>(), (SpriteRendererFields)enumInt);
                case ComponentTypes.RectTransform:
                    return GetField(obj.GetComponent<RectTransform>(), (RectTransformFields)enumInt);
                case ComponentTypes.Image:
                    return GetField(obj.GetComponent<Image>(), (ImageFields)enumInt);
                case ComponentTypes.Text:
                    return GetField(obj.GetComponent<Text>(), (TextFields)enumInt);
            }
            return 0;
        }

        public float GetField(Transform transform, TransformFields field)
        {
            switch (field)
            {
                case TransformFields.PositionX:
                    return transform.position.x;
                case TransformFields.PositionY:
                    return transform.position.y;
                case TransformFields.PositionZ:
                    return transform.position.z;

                case TransformFields.LocalPositionX:
                    return transform.localPosition.x;
                case TransformFields.LocalPositionY:
                    return transform.localPosition.y;
                case TransformFields.LocalPositionZ:
                    return transform.localPosition.z;

                case TransformFields.RotationX:
                    return transform.rotation.x;
                case TransformFields.RotationY:
                    return transform.rotation.y;
                case TransformFields.RotationZ:
                    return transform.rotation.z;
                case TransformFields.RotationW:
                    return transform.rotation.w;

                case TransformFields.LocalRotationX:
                    return transform.localRotation.x;
                case TransformFields.LocalRotationY:
                    return transform.localRotation.y;
                case TransformFields.LocalRotationZ:
                    return transform.localRotation.z;
                case TransformFields.LocalRotationW:
                    return transform.localRotation.w;
                    
                case TransformFields.EulerAnglesX:
                    return transform.eulerAngles.x;
                case TransformFields.EulerAnglesY:
                    return transform.eulerAngles.y;
                case TransformFields.EulerAnglesZ:
                    return transform.eulerAngles.z;

                case TransformFields.LocalEulerAnglesX:
                    return transform.localEulerAngles.x;
                case TransformFields.LocalEulerAnglesY:
                    return transform.localEulerAngles.y;
                case TransformFields.LocalEulerAnglesZ:
                    return transform.localEulerAngles.z;

                case TransformFields.LocalScaleX:
                    return transform.localScale.x;
                case TransformFields.LocalScaleY:
                    return transform.localScale.y;
                case TransformFields.LocalScaleZ:
                    return transform.localScale.z;
            }
            return 0;
        }
        public float GetField(SpriteRenderer spriteRenderer, SpriteRendererFields field)
        {
            switch (field)
            {
                case SpriteRendererFields.ColorRed:
                    return spriteRenderer.color.r;
                case SpriteRendererFields.ColorGreen:
                    return spriteRenderer.color.g;
                case SpriteRendererFields.ColorBlue:
                    return spriteRenderer.color.b;
                case SpriteRendererFields.ColorAlpha:
                    return spriteRenderer.color.a;

                case SpriteRendererFields.SizeX:
                    return spriteRenderer.size.x;
                case SpriteRendererFields.SizeY:
                    return spriteRenderer.size.y;
            }
            return 0;
        }
        public float GetField(RectTransform rectTransform, RectTransformFields field)
        {
            switch (field)
            {
                case RectTransformFields.PositionX:
                    return rectTransform.position.x;
                case RectTransformFields.PositionY:
                    return rectTransform.position.y;
                case RectTransformFields.PositionZ:
                    return rectTransform.position.z;

                case RectTransformFields.LocalPositionX:
                    return rectTransform.localPosition.x;
                case RectTransformFields.LocalPositionY:
                    return rectTransform.localPosition.y;
                case RectTransformFields.LocalPositionZ:
                    return rectTransform.localPosition.z;

                case RectTransformFields.AnchoredPositionX:
                    return rectTransform.anchoredPosition.x;
                case RectTransformFields.AnchoredPositionY:
                    return rectTransform.anchoredPosition.y;

                case RectTransformFields.AnchoredPosition3DX:
                    return rectTransform.anchoredPosition3D.x;
                case RectTransformFields.AnchoredPosition3DY:
                    return rectTransform.anchoredPosition3D.y;
                case RectTransformFields.AnchoredPosition3DZ:
                    return rectTransform.anchoredPosition3D.z;

                case RectTransformFields.SizeDeltaX:
                    return rectTransform.sizeDelta.x;
                case RectTransformFields.SizeDeltaY:
                    return rectTransform.sizeDelta.y;

                case RectTransformFields.RotationX:
                    return rectTransform.rotation.x;
                case RectTransformFields.RotationY:
                    return rectTransform.rotation.y;
                case RectTransformFields.RotationZ:
                    return rectTransform.rotation.z;
                case RectTransformFields.RotationW:
                    return rectTransform.rotation.w;

                case RectTransformFields.LocalRotationX:
                    return rectTransform.localRotation.x;
                case RectTransformFields.LocalRotationY:
                    return rectTransform.localRotation.y;
                case RectTransformFields.LocalRotationZ:
                    return rectTransform.localRotation.z;
                case RectTransformFields.LocalRotationW:
                    return rectTransform.localRotation.w;

                case RectTransformFields.EulerAnglesX:
                    return rectTransform.eulerAngles.x;
                case RectTransformFields.EulerAnglesY:
                    return rectTransform.eulerAngles.y;
                case RectTransformFields.EulerAnglesZ:
                    return rectTransform.eulerAngles.z;

                case RectTransformFields.LocalEulerAnglesX:
                    return rectTransform.localEulerAngles.x;
                case RectTransformFields.LocalEulerAnglesY:
                    return rectTransform.localEulerAngles.y;
                case RectTransformFields.LocalEulerAnglesZ:
                    return rectTransform.localEulerAngles.z;

                case RectTransformFields.LocalScaleX:
                    return rectTransform.localScale.x;
                case RectTransformFields.LocalScaleY:
                    return rectTransform.localScale.y;
                case RectTransformFields.LocalScaleZ:
                    return rectTransform.localScale.z;

                case RectTransformFields.AnchorMinX:
                    return rectTransform.anchorMin.x;
                case RectTransformFields.AnchorMinY:
                    return rectTransform.anchorMin.y;

                case RectTransformFields.AnchorMaxX:
                    return rectTransform.anchorMax.x;
                case RectTransformFields.AnchorMaxY:
                    return rectTransform.anchorMax.y;

                case RectTransformFields.OffsetMinX:
                    return rectTransform.offsetMin.x;
                case RectTransformFields.OffsetMinY:
                    return rectTransform.offsetMin.y;

                case RectTransformFields.OffsetMaxX:
                    return rectTransform.offsetMax.x;
                case RectTransformFields.OffsetMaxY:
                    return rectTransform.offsetMax.y;
            }
            return 0;
        }
        public float GetField(Image image, ImageFields field)
        {
            switch (field)
            {
                case ImageFields.ColorRed:
                    return image.color.r;
                case ImageFields.ColorGreen:
                    return image.color.g;
                case ImageFields.ColorBlue:
                    return image.color.b;
                case ImageFields.ColorAlpha:
                    return image.color.a;

                case ImageFields.Fill:
                    return image.fillAmount;
            }
            return 0;
        }
        public float GetField(Text text, TextFields field)
        {
            switch (field)
            {
                case TextFields.ColorRed:
                    return text.color.r;
                case TextFields.ColorGreen:
                    return text.color.g;
                case TextFields.ColorBlue:
                    return text.color.b;
                case TextFields.ColorAlpha:
                    return text.color.a;
            }
            return 0;
        }
        #endregion Get Fields

        #region Set Fields
        // Sets a supported Component's Field/Property to a float

        public void SetField(TransitionProperties prop, ComponentTypes component, int enumInt, float value)
        {
            switch (component)
            {
                case ComponentTypes.Transform:
                    SetField(prop.transform, (TransformFields)enumInt, value);
                    break;
                case ComponentTypes.SpriteRenderer:
                    SetField(prop.spriteRenderer, (SpriteRendererFields)enumInt, value);
                    break;
                case ComponentTypes.RectTransform:
                    SetField(prop.rectTransform, (RectTransformFields)enumInt, value);
                    break;
                case ComponentTypes.Image:
                    SetField(prop.image, (ImageFields)enumInt, value);
                    break;
                case ComponentTypes.Text:
                    SetField(prop.text, (TextFields)enumInt, value);
                    break;
            }
        }
        public void SetField(GameObject obj, ComponentTypes component, int enumInt, float value)
        {
            switch (component)
            {
                case ComponentTypes.Transform:
                    SetField(obj.GetComponent<Transform>(), (TransformFields)enumInt, value);
                    break;
                case ComponentTypes.SpriteRenderer:
                    SetField(obj.GetComponent<SpriteRenderer>(), (SpriteRendererFields)enumInt, value);
                    break;
                case ComponentTypes.RectTransform:
                    SetField(obj.GetComponent<RectTransform>(), (RectTransformFields)enumInt, value);
                    break;
                case ComponentTypes.Image:
                    SetField(obj.GetComponent<Image>(), (ImageFields)enumInt, value);
                    break;
                case ComponentTypes.Text:
                    SetField(obj.GetComponent<Text>(), (TextFields)enumInt, value);
                    break;
            }
        }

        public void SetField(Transform transform, TransformFields field, float value)
        {
            switch (field)
            {
                case TransformFields.PositionX:
                    transform.position = new Vector3(value, transform.position.y, transform.position.z); break;
                case TransformFields.PositionY:
                    transform.position = new Vector3(transform.position.x, value, transform.position.z); break;
                case TransformFields.PositionZ:
                    transform.position = new Vector3(transform.position.x, transform.position.y, value); break;

                case TransformFields.LocalPositionX:
                    transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z); break;
                case TransformFields.LocalPositionY:
                    transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z); break;
                case TransformFields.LocalPositionZ:
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value); break;

                case TransformFields.RotationX:
                    transform.rotation = new Quaternion(value, transform.rotation.y, transform.rotation.z, transform.rotation.w); break;
                case TransformFields.RotationY:
                    transform.rotation = new Quaternion(transform.rotation.x, value, transform.rotation.z, transform.rotation.w); break;
                case TransformFields.RotationZ:
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, value, transform.rotation.w); break;
                case TransformFields.RotationW:
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, value); break;

                case TransformFields.LocalRotationX:
                    transform.localRotation = new Quaternion(value, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w); break;
                case TransformFields.LocalRotationY:
                    transform.localRotation = new Quaternion(transform.localRotation.x, value, transform.localRotation.z, transform.localRotation.w); break;
                case TransformFields.LocalRotationZ:
                    transform.localRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, value, transform.localRotation.w); break;
                case TransformFields.LocalRotationW:
                    transform.localRotation = new Quaternion(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z, value); break;

                case TransformFields.EulerAnglesX:
                    transform.eulerAngles = new Vector3(value, transform.eulerAngles.y, transform.eulerAngles.z); break;
                case TransformFields.EulerAnglesY:
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, value, transform.eulerAngles.z); break;
                case TransformFields.EulerAnglesZ:
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, value); break;

                case TransformFields.LocalEulerAnglesX:
                    transform.localEulerAngles = new Vector3(value, transform.localEulerAngles.y, transform.localEulerAngles.z); break;
                case TransformFields.LocalEulerAnglesY:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, value, transform.localEulerAngles.z); break;
                case TransformFields.LocalEulerAnglesZ:
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, value); break;

                case TransformFields.LocalScaleX:
                    transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z); break;
                case TransformFields.LocalScaleY:
                    transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z); break;
                case TransformFields.LocalScaleZ:
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, value); break;
            }
        }
        public void SetField(SpriteRenderer spriteRenderer, SpriteRendererFields field, float value)
        {
            switch (field)
            {
                case SpriteRendererFields.ColorRed:
                    spriteRenderer.color = new Color(value, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a); break;
                case SpriteRendererFields.ColorGreen:
                    spriteRenderer.color = new Color(spriteRenderer.color.r, value, spriteRenderer.color.b, spriteRenderer.color.a); break;
                case SpriteRendererFields.ColorBlue:
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, value, spriteRenderer.color.a); break;
                case SpriteRendererFields.ColorAlpha:
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, value); break;

                case SpriteRendererFields.SizeX:
                    spriteRenderer.size = new Vector3(value, spriteRenderer.size.y); break;
                case SpriteRendererFields.SizeY:
                    spriteRenderer.size = new Vector3(spriteRenderer.size.x, value); break;
            }
        }
        public void SetField(RectTransform rectTransform, RectTransformFields field, float value)
        {
            switch (field)
            {
                case RectTransformFields.PositionX:
                    rectTransform.position = new Vector3(value, rectTransform.position.y, rectTransform.position.z); break;
                case RectTransformFields.PositionY:
                    rectTransform.position = new Vector3(rectTransform.position.x, value, rectTransform.position.z); break;
                case RectTransformFields.PositionZ:
                    rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, value); break;

                case RectTransformFields.LocalPositionX:
                    rectTransform.localPosition = new Vector3(value, rectTransform.localPosition.y, rectTransform.localPosition.z); break;
                case RectTransformFields.LocalPositionY:
                    rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, value, rectTransform.localPosition.z); break;
                case RectTransformFields.LocalPositionZ:
                    rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, value); break;

                case RectTransformFields.AnchoredPositionX:
                    rectTransform.anchoredPosition = new Vector3(value, rectTransform.anchoredPosition.y); break;
                case RectTransformFields.AnchoredPositionY:
                    rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, value); break;

                case RectTransformFields.AnchoredPosition3DX:
                    rectTransform.anchoredPosition3D = new Vector3(value, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z); break;
                case RectTransformFields.AnchoredPosition3DY:
                    rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, value, rectTransform.anchoredPosition3D.z); break;
                case RectTransformFields.AnchoredPosition3DZ:
                    rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, rectTransform.anchoredPosition3D.y, value); break;

                case RectTransformFields.SizeDeltaX:
                    rectTransform.sizeDelta = new Vector3(value, rectTransform.sizeDelta.y); break;
                case RectTransformFields.SizeDeltaY:
                    rectTransform.sizeDelta = new Vector3(rectTransform.sizeDelta.x, value); break;

                case RectTransformFields.RotationX:
                    rectTransform.rotation = new Quaternion(value, rectTransform.rotation.y, rectTransform.rotation.z, rectTransform.rotation.w); break;
                case RectTransformFields.RotationY:
                    rectTransform.rotation = new Quaternion(rectTransform.rotation.x, value, rectTransform.rotation.z, rectTransform.rotation.w); break;
                case RectTransformFields.RotationZ:
                    rectTransform.rotation = new Quaternion(rectTransform.rotation.x, rectTransform.rotation.y, value, rectTransform.rotation.w); break;
                case RectTransformFields.RotationW:
                    rectTransform.rotation = new Quaternion(rectTransform.rotation.x, rectTransform.rotation.y, rectTransform.rotation.z, value); break;

                case RectTransformFields.LocalRotationX:
                    rectTransform.localRotation = new Quaternion(value, rectTransform.localRotation.y, rectTransform.localRotation.z, rectTransform.localRotation.w); break;
                case RectTransformFields.LocalRotationY:
                    rectTransform.localRotation = new Quaternion(rectTransform.localRotation.x, value, rectTransform.localRotation.z, rectTransform.localRotation.w); break;
                case RectTransformFields.LocalRotationZ:
                    rectTransform.localRotation = new Quaternion(rectTransform.localRotation.x, rectTransform.localRotation.y, value, rectTransform.localRotation.w); break;
                case RectTransformFields.LocalRotationW:
                    rectTransform.localRotation = new Quaternion(rectTransform.localRotation.x, rectTransform.localRotation.y, rectTransform.localRotation.z, value); break;

                case RectTransformFields.EulerAnglesX:
                    rectTransform.eulerAngles = new Vector3(value, rectTransform.eulerAngles.y, rectTransform.eulerAngles.z); break;
                case RectTransformFields.EulerAnglesY:
                    rectTransform.eulerAngles = new Vector3(rectTransform.eulerAngles.x, value, rectTransform.eulerAngles.z); break;
                case RectTransformFields.EulerAnglesZ:
                    rectTransform.eulerAngles = new Vector3(rectTransform.eulerAngles.x, rectTransform.eulerAngles.y, value); break;

                case RectTransformFields.LocalEulerAnglesX:
                    rectTransform.localEulerAngles = new Vector3(value, rectTransform.localEulerAngles.y, rectTransform.localEulerAngles.z); break;
                case RectTransformFields.LocalEulerAnglesY:
                    rectTransform.localEulerAngles = new Vector3(rectTransform.localEulerAngles.x, value, rectTransform.localEulerAngles.z); break;
                case RectTransformFields.LocalEulerAnglesZ:
                    rectTransform.localEulerAngles = new Vector3(rectTransform.localEulerAngles.x, rectTransform.localEulerAngles.y, value); break;

                case RectTransformFields.LocalScaleX:
                    rectTransform.localScale = new Vector3(value, rectTransform.localScale.y, rectTransform.localScale.z); break;
                case RectTransformFields.LocalScaleY:
                    rectTransform.localScale = new Vector3(rectTransform.localScale.x, value, rectTransform.localScale.z); break;
                case RectTransformFields.LocalScaleZ:
                    rectTransform.localScale = new Vector3(rectTransform.localScale.x, rectTransform.localScale.y, value); break;

                case RectTransformFields.AnchorMinX:
                    rectTransform.anchorMin = new Vector3(value, rectTransform.anchorMin.y); break;
                case RectTransformFields.AnchorMinY:
                    rectTransform.anchorMin = new Vector3(rectTransform.anchorMin.x, value); break;

                case RectTransformFields.AnchorMaxX:
                    rectTransform.anchorMax = new Vector3(value, rectTransform.anchorMax.y); break;
                case RectTransformFields.AnchorMaxY:
                    rectTransform.anchorMax = new Vector3(rectTransform.anchorMax.x, value); break;

                case RectTransformFields.OffsetMinX:
                    rectTransform.offsetMin = new Vector3(value, rectTransform.offsetMin.y); break;
                case RectTransformFields.OffsetMinY:
                    rectTransform.offsetMin = new Vector3(rectTransform.offsetMin.x, value); break;

                case RectTransformFields.OffsetMaxX:
                    rectTransform.offsetMax = new Vector3(value, rectTransform.offsetMax.y); break;
                case RectTransformFields.OffsetMaxY:
                    rectTransform.offsetMax = new Vector3(rectTransform.offsetMax.x, value); break;
            }
        }
        public void SetField(Image image, ImageFields field, float value)
        {
            switch (field)
            {
                case ImageFields.ColorRed:
                    image.color = new Color(value, image.color.g, image.color.b, image.color.a); break;
                case ImageFields.ColorGreen:
                    image.color = new Color(image.color.r, value, image.color.b, image.color.a); break;
                case ImageFields.ColorBlue:
                    image.color = new Color(image.color.r, image.color.g, value, image.color.a); break;
                case ImageFields.ColorAlpha:
                    image.color = new Color(image.color.r, image.color.g, image.color.b, value); break;

                case ImageFields.Fill:
                    image.fillAmount = value; break;
            }
        }
        public void SetField(Text text, TextFields field, float value)
        {
            switch (field)
            {
                case TextFields.ColorRed:
                    text.color = new Color(value, text.color.g, text.color.b, text.color.a); break;
                case TextFields.ColorGreen:
                    text.color = new Color(text.color.r, value, text.color.b, text.color.a); break;
                case TextFields.ColorBlue:
                    text.color = new Color(text.color.r, text.color.g, value, text.color.a); break;
                case TextFields.ColorAlpha:
                    text.color = new Color(text.color.r, text.color.g, text.color.b, value); break;
            }
        }
        #endregion Set Fields


        public static float animationSpeed = 1;     // Global speed scale of Transitions
        public static List<TransitionProperties> transitions = new List<TransitionProperties>();

        // Returns field value based on the timer
        private float EaseData(TransitionData data)
        {
            float x = Mathf.InverseLerp(0, data.duration, data.timer);
            float y = Ease(data.ease, x);

            return Mathf.LerpUnclamped(data.start, data.end, y);
        }

        // Sets timer based on where the field value is in relation to the start and end of transition
        private void SetTimer(TransitionData data, float value)
        {
            if (data.start < data.end)
            {
                if (value < data.end)
                {
                    if (value > data.start)
                    { }
                    else
                        data.start = value;
                }
                else if (value > data.end)
                {
                    if (value < data.end + (data.end - data.start))
                        data.start = data.end + (data.end - data.start);
                    else
                        data.start = value;
                }
            }
            else
            {
                if (value > data.end)
                {
                    if (value < data.start)
                    { }
                    else
                        data.start = value;
                }
                else if (value < data.end)
                {
                    if (value > data.end - (data.start - data.end))
                        data.start = data.end - (data.start - data.end);
                    else
                        data.start = value;
                }
            }

            float y = Mathf.InverseLerp(data.start, data.end, value);
            float x = EaseInverse(data.ease, y);

            data.timer = Mathf.Lerp(0, data.duration, x);
            data.timed = true;
        }

        private void Update()
        {
            if (transitions.Count == 0)
                return;

            for (int p = 0; p < transitions.Count; p++)    // Loops through all objects that need to be transitioned
            {
                TransitionProperties prop = transitions[p];

                if (prop.inTrans.Count == 0)    // Removes object if finished transitioning
                {
                    transitions.Remove(prop);
                    continue;
                }
                if (prop.pause)    // Ignores object if paused
                    continue;
                
                Dictionary<Vector2Int, TransitionData> newTrans = new Dictionary<Vector2Int, TransitionData>();
                foreach (KeyValuePair<Vector2Int, TransitionData> trans in prop.inTrans)    // Loops through all transitioning components in object
                {
                    ComponentTypes component = (ComponentTypes)trans.Key.x;
                    int enumInt = trans.Key.y;
                    TransitionData data = trans.Value;

                    if (!data.timed)    // Sets timer if component is new
                        SetTimer(data, GetField(prop, component, enumInt));

                    if (data.timer == data.duration)    // Confirms end point once transition finishes
                    {
                        SetField(prop, component, enumInt, data.end);
                        continue;
                    }

                    data.timer = Mathf.Clamp(data.timer + Time.deltaTime, 0, data.duration);    // Increment timer with deltaTime

                    SetField(prop, component, enumInt, EaseData(data));    // Sets field value based on new time
                    newTrans.Add(trans.Key, data);
                }

                prop.inTrans = newTrans;
            }
        }
    }
}