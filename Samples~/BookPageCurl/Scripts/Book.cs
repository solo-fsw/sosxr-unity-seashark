//The implementation is based on this article:http://rbarraza.com/html5-canvas-pageflip/
//Enhanced with improved Input System integration

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum FlipMode
{
    RightToLeft,
    LeftToRight
}


[ExecuteInEditMode]
public class Book : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField] private RectTransform BookPanel;
    public Sprite background;
    public Sprite[] bookPages;
    public bool interactable = true;

    public bool enableShadowEffect = true;

    //represent the index of the sprite shown in the right page
    public int currentPage;
    public Image ClippingPlane;
    public Image NextPageClip;
    public Image Shadow;
    public Image ShadowLTR;
    public Image Left;
    public Image LeftNext;
    public Image Right;
    public Image RightNext;
    public UnityEvent OnFlip;

    [Tooltip("Input action for pointer position (e.g. <Pointer>/position or <Mouse>/position)")] [SerializeField]
    private InputActionProperty m_mousePosition;

    [Tooltip("Input action for mouse click (e.g. <Mouse>/leftButton)")] [SerializeField]
    private InputActionProperty m_mouseClick;

    private float radius1, radius2;

    //Spine Bottom
    private Vector3 _spineBottom;

    //Spine Top
    private Vector3 _spineTop;

    //corner of the page
    private Vector3 _cornerPage;

    //Edge Bottom Right
    //Edge Bottom Left
    //follow point
    private Vector3 _followPoint;
    private bool pageDragging;

    private bool isMousePressed;

    //current flip mode
    private FlipMode _flipMode;
    private Coroutine _currentCoroutine;
    private Vector2 _screenPoint;

    public int TotalPageCount => bookPages.Length;
    public Vector3 EndBottomLeft { get; private set; }

    public Vector3 EndBottomRight { get; private set; }

    public float Height => BookPanel.rect.height;


    private void OnEnable()
    {
        EnableInputActions();
    }


    private void OnDisable()
    {
        DisableInputActions();
    }


    private void EnableInputActions()
    {
        m_mousePosition.action?.Enable();

        if (m_mouseClick.action != null)
        {
            m_mouseClick.action.Enable();
            m_mouseClick.action.performed += OnMousePressed;
            m_mouseClick.action.canceled += OnMouseReleased;
        }
    }


    private void DisableInputActions()
    {
        m_mousePosition.action?.Disable();

        if (m_mouseClick.action != null)
        {
            m_mouseClick.action.performed -= OnMousePressed;
            m_mouseClick.action.canceled -= OnMouseReleased;
            m_mouseClick.action.Disable();
        }
    }


    private void OnMousePressed(InputAction.CallbackContext context)
    {
        if (!interactable)
        {
            return;
        }

        isMousePressed = true;
        var mousePos = GetMousePosition();
        var localPos = transformPoint(mousePos);

        // Check which side was clicked and start dragging if valid
        if (localPos.x > 0 && currentPage < bookPages.Length)
        {
            OnMouseDragRightPage();
        }
        else if (localPos.x < 0 && currentPage > 0)
        {
            OnMouseDragLeftPage();
        }
    }


    private void OnMouseReleased(InputAction.CallbackContext context)
    {
        if (isMousePressed)
        {
            isMousePressed = false;
            OnMouseRelease();
        }
    }


    private Vector2 GetMousePosition()
    {
        if (m_mousePosition.action != null)
        {
            return m_mousePosition.action.ReadValue<Vector2>();
        }

        return Input.mousePosition; // Fallback
    }


    private void Start()
    {
        if (!canvas)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (!canvas)
        {
            Debug.LogError("Book should be a child to canvas");
        }

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        UpdateSprites();
        CalcCurlCriticalPoints();

        var pageWidth = BookPanel.rect.width / 2.0f;
        var pageHeight = BookPanel.rect.height;
        NextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageHeight * 2);

        ClippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

        //hypotenous (diagonal) page length
        var hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        var shadowPageHeight = pageWidth / 2 + hyp;

        Shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        Shadow.rectTransform.pivot = new Vector2(1, pageWidth / 2 / shadowPageHeight);

        ShadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        ShadowLTR.rectTransform.pivot = new Vector2(0, pageWidth / 2 / shadowPageHeight);
    }


    private void CalcCurlCriticalPoints()
    {
        _spineBottom = new Vector3(0, -BookPanel.rect.height / 2);
        EndBottomRight = new Vector3(BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
        EndBottomLeft = new Vector3(-BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
        _spineTop = new Vector3(0, BookPanel.rect.height / 2);
        radius1 = Vector2.Distance(_spineBottom, EndBottomRight);
        var pageWidth = BookPanel.rect.width / 2.0f;
        var pageHeight = BookPanel.rect.height;
        radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
    }


    public Vector3 transformPoint(Vector3 mouseScreenPos)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            var mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseWorldPos);

            return localPos;
        }

        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            var ray = Camera.main.ScreenPointToRay(mouseScreenPos);
            var globalEBR = transform.TransformPoint(EndBottomRight);
            var globalEBL = transform.TransformPoint(EndBottomLeft);
            var globalSt = transform.TransformPoint(_spineTop);
            var p = new Plane(globalEBR, globalEBL, globalSt);
            float distance;
            p.Raycast(ray, out distance);
            Vector2 localPos = BookPanel.InverseTransformPoint(ray.GetPoint(distance));

            return localPos;
        }
        else
        {
            //Screen Space Overlay
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseScreenPos);

            return localPos;
        }
    }


    private void Update()
    {
        _screenPoint = GetMousePosition();

        if (pageDragging && interactable)
        {
            UpdateBook();
        }
    }


    public void UpdateBook()
    {
        _followPoint = Vector3.Lerp(_followPoint, transformPoint(_screenPoint), Time.deltaTime * 10);

        if (_flipMode == FlipMode.RightToLeft)
        {
            UpdateBookRTLToPoint(_followPoint);
        }
        else
        {
            UpdateBookLTRToPoint(_followPoint);
        }
    }


    public void UpdateBookLTRToPoint(Vector3 followLocation)
    {
        _flipMode = FlipMode.LeftToRight;
        _followPoint = followLocation;
        ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
        ShadowLTR.transform.localPosition = new Vector3(0, 0, 0);
        ShadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);
        Left.transform.SetParent(ClippingPlane.transform, true);

        Right.transform.SetParent(BookPanel.transform, true);
        Right.transform.localEulerAngles = Vector3.zero;
        LeftNext.transform.SetParent(BookPanel.transform, true);

        _cornerPage = Calc_C_Position(followLocation);
        Vector3 t1;
        var clipAngle = CalcClipAngle(_cornerPage, EndBottomLeft, out t1);
        //0 < T0_T1_Angle < 180
        clipAngle = (clipAngle + 180) % 180;

        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        //page position and angle
        Left.transform.position = BookPanel.TransformPoint(_cornerPage);
        var C_T1_dy = t1.y - _cornerPage.y;
        var C_T1_dx = t1.x - _cornerPage.x;
        var C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Left.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - 90 - clipAngle);

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        LeftNext.transform.SetParent(NextPageClip.transform, true);
        Right.transform.SetParent(ClippingPlane.transform, true);
        Right.transform.SetAsFirstSibling();

        ShadowLTR.rectTransform.SetParent(Left.rectTransform, true);
    }


    public void UpdateBookRTLToPoint(Vector3 followLocation)
    {
        _flipMode = FlipMode.RightToLeft;
        _followPoint = followLocation;
        Shadow.transform.SetParent(ClippingPlane.transform, true);
        Shadow.transform.localPosition = Vector3.zero;
        Shadow.transform.localEulerAngles = Vector3.zero;
        Right.transform.SetParent(ClippingPlane.transform, true);

        Left.transform.SetParent(BookPanel.transform, true);
        Left.transform.localEulerAngles = Vector3.zero;
        RightNext.transform.SetParent(BookPanel.transform, true);
        _cornerPage = Calc_C_Position(followLocation);
        Vector3 t1;
        var clipAngle = CalcClipAngle(_cornerPage, EndBottomRight, out t1);

        if (clipAngle > -90)
        {
            clipAngle += 180;
        }

        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        //page position and angle
        Right.transform.position = BookPanel.TransformPoint(_cornerPage);
        var C_T1_dy = t1.y - _cornerPage.y;
        var C_T1_dx = t1.x - _cornerPage.x;
        var C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Right.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - (clipAngle + 90));

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        RightNext.transform.SetParent(NextPageClip.transform, true);
        Left.transform.SetParent(ClippingPlane.transform, true);
        Left.transform.SetAsFirstSibling();

        Shadow.rectTransform.SetParent(Right.rectTransform, true);
    }


    private float CalcClipAngle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
    {
        var t0 = (c + bookCorner) / 2;
        var T0_CORNER_dy = bookCorner.y - t0.y;
        var T0_CORNER_dx = bookCorner.x - t0.x;
        var T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
        var T0_T1_Angle = 90 - T0_CORNER_Angle;

        var T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
        T1_X = normalizeT1X(T1_X, bookCorner, _spineBottom);
        t1 = new Vector3(T1_X, _spineBottom.y, 0);

        //clipping plane angle=T0_T1_Angle
        var T0_T1_dy = t1.y - t0.y;
        var T0_T1_dx = t1.x - t0.x;
        T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;

        return T0_T1_Angle;
    }


    private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
    {
        return (t1 > sb.x && sb.x > corner.x) || (t1 < sb.x && sb.x < corner.x) ? sb.x : t1;
    }


    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 c;
        _followPoint = followLocation;
        var F_SB_dy = _followPoint.y - _spineBottom.y;
        var F_SB_dx = _followPoint.x - _spineBottom.x;
        var F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
        var r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle), radius1 * Mathf.Sin(F_SB_Angle), 0) + _spineBottom;

        var F_SB_distance = Vector2.Distance(_followPoint, _spineBottom);

        if (F_SB_distance < radius1)
        {
            c = _followPoint;
        }
        else
        {
            c = r1;
        }

        var F_ST_dy = c.y - _spineTop.y;
        var F_ST_dx = c.x - _spineTop.x;
        var F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);

        var r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle), radius2 * Mathf.Sin(F_ST_Angle), 0) + _spineTop;

        var C_ST_distance = Vector2.Distance(c, _spineTop);

        if (C_ST_distance > radius2)
        {
            c = r2;
        }

        return c;
    }


    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Length)
        {
            return;
        }

        pageDragging = true;
        _flipMode = FlipMode.RightToLeft;
        _followPoint = point;

        NextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(0, 0);
        Left.transform.position = RightNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        Left.sprite = currentPage < bookPages.Length ? bookPages[currentPage] : background;
        Left.transform.SetAsFirstSibling();

        Right.gameObject.SetActive(true);
        Right.transform.position = RightNext.transform.position;
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        Right.sprite = currentPage < bookPages.Length - 1 ? bookPages[currentPage + 1] : background;

        RightNext.sprite = currentPage < bookPages.Length - 2 ? bookPages[currentPage + 2] : background;

        LeftNext.transform.SetAsFirstSibling();

        if (enableShadowEffect)
        {
            Shadow.gameObject.SetActive(true);
        }

        UpdateBookRTLToPoint(_followPoint);
    }


    public void OnMouseDragRightPage()
    {
        if (interactable)
        {
            DragRightPageToPoint(transformPoint(_screenPoint));
        }
    }


    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0)
        {
            return;
        }

        pageDragging = true;
        _flipMode = FlipMode.LeftToRight;
        _followPoint = point;

        NextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

        Right.gameObject.SetActive(true);
        Right.transform.position = LeftNext.transform.position;
        Right.sprite = bookPages[currentPage - 1];
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        Right.transform.SetAsFirstSibling();

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(1, 0);
        Left.transform.position = LeftNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        Left.sprite = currentPage >= 2 ? bookPages[currentPage - 2] : background;

        LeftNext.sprite = currentPage >= 3 ? bookPages[currentPage - 3] : background;

        RightNext.transform.SetAsFirstSibling();

        if (enableShadowEffect)
        {
            ShadowLTR.gameObject.SetActive(true);
        }

        UpdateBookLTRToPoint(_followPoint);
    }


    public void OnMouseDragLeftPage()
    {
        if (interactable)
        {
            DragLeftPageToPoint(transformPoint(_screenPoint));
        }
    }


    public void OnMouseRelease()
    {
        if (interactable)
        {
            ReleasePage();
        }
    }


    public void ReleasePage()
    {
        if (pageDragging)
        {
            pageDragging = false;
            var distanceToLeft = Vector2.Distance(_cornerPage, EndBottomLeft);
            var distanceToRight = Vector2.Distance(_cornerPage, EndBottomRight);

            if (distanceToRight < distanceToLeft && _flipMode == FlipMode.RightToLeft)
            {
                TweenBack();
            }
            else if (distanceToRight > distanceToLeft && _flipMode == FlipMode.LeftToRight)
            {
                TweenBack();
            }
            else
            {
                TweenForward();
            }
        }
    }


    private void UpdateSprites()
    {
        LeftNext.sprite = currentPage > 0 && currentPage <= bookPages.Length ? bookPages[currentPage - 1] : background;
        RightNext.sprite = currentPage >= 0 && currentPage < bookPages.Length ? bookPages[currentPage] : background;
    }


    public void TweenForward()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        if (_flipMode == FlipMode.RightToLeft)
        {
            _currentCoroutine = StartCoroutine(TweenTo(EndBottomLeft, 0.15f, () => { Flip(); }));
        }
        else
        {
            _currentCoroutine = StartCoroutine(TweenTo(EndBottomRight, 0.15f, () => { Flip(); }));
        }
    }


    private void Flip()
    {
        if (_flipMode == FlipMode.RightToLeft)
        {
            currentPage += 2;
        }
        else
        {
            currentPage -= 2;
        }

        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.transform.SetParent(BookPanel.transform, true);
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        Right.transform.SetParent(BookPanel.transform, true);
        RightNext.transform.SetParent(BookPanel.transform, true);
        UpdateSprites();
        Shadow.gameObject.SetActive(false);
        ShadowLTR.gameObject.SetActive(false);

        OnFlip?.Invoke();
    }


    public void TweenBack()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        if (_flipMode == FlipMode.RightToLeft)
        {
            _currentCoroutine = StartCoroutine(TweenTo(EndBottomRight, 0.15f,
                () =>
                {
                    UpdateSprites();
                    RightNext.transform.SetParent(BookPanel.transform);
                    Right.transform.SetParent(BookPanel.transform);

                    Left.gameObject.SetActive(false);
                    Right.gameObject.SetActive(false);
                    pageDragging = false;
                }
            ));
        }
        else
        {
            _currentCoroutine = StartCoroutine(TweenTo(EndBottomLeft, 0.15f,
                () =>
                {
                    UpdateSprites();

                    LeftNext.transform.SetParent(BookPanel.transform);
                    Left.transform.SetParent(BookPanel.transform);

                    Left.gameObject.SetActive(false);
                    Right.gameObject.SetActive(false);
                    pageDragging = false;
                }
            ));
        }
    }


    public IEnumerator TweenTo(Vector3 to, float duration, Action onFinish)
    {
        var steps = (int)(duration / 0.025f);
        var displacement = (to - _followPoint) / steps;

        for (var i = 0; i < steps - 1; i++)
        {
            if (_flipMode == FlipMode.RightToLeft)
            {
                UpdateBookRTLToPoint(_followPoint + displacement);
            }
            else
            {
                UpdateBookLTRToPoint(_followPoint + displacement);
            }

            yield return new WaitForSeconds(0.025f);
        }

        onFinish?.Invoke();
    }


    // Public methods for external control
    public void NextPage()
    {
        if (currentPage < bookPages.Length - 1 && !pageDragging)
        {
            OnMouseDragRightPage();
            StartCoroutine(DelayedFlip(0.1f, true));
        }
    }


    public void PreviousPage()
    {
        if (currentPage > 0 && !pageDragging)
        {
            OnMouseDragLeftPage();
            StartCoroutine(DelayedFlip(0.1f, true));
        }
    }


    private IEnumerator DelayedFlip(float delay, bool forward)
    {
        yield return new WaitForSeconds(delay);

        if (forward)
        {
            TweenForward();
        }
        else
        {
            TweenBack();
        }
    }
}