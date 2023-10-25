using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary>
    /// A reference to the Rigidbody component 
    /// </summary>
    private Rigidbody rb;
    [Tooltip("���� ���� / ���������� �󸶳� ���� �����̴���")]
    public float dodgeSpeed = 5;
    [Tooltip("���� �ڵ����� �󸶳� ������ ������ �����̴���")]
    [Range(0, 10)]
    public float rollSpeed = 5;
    
    // �߷°��ӵ������� ���� ��������, �߷°��ӵ��� �������
    // ��ġ�� ����� �� ������ ���ְ� ���� ����
    public enum MobileHorizMovement
    {
        Accelerometer,
        ScreenTouch
    }

    public MobileHorizMovement horizMovement = MobileHorizMovement.Accelerometer;


    [Header("Swipe Properties")]

    [Tooltip("���������ϸ� �÷��̾ �ָ� �����̴� �Ÿ� ���� ")]
    public float swipeMove = 2f;

    [Tooltip("�׼��� �����ϱ� ���� �÷��̾ �󸶳� �ָ� ���������ؾ� �ϴ���(1/4��ġ)")]
    public float minSwipeDistance = 0.25f;

    /// <summary>
    /// pixels�� ��ȯ�� minSwipeDistance���� ����
    /// </summary>
    private float minSwipeDistancePixels;

    /// <summary> 
    /// mobile touch events�� starting position ����
    /// </summary> 
    private Vector2 touchStart;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        minSwipeDistancePixels = minSwipeDistance * Screen.dpi;

    }

    /// <summary>
    /// FixedUpadte�� ������ �����ӿ� ȣ��Ǹ� 
    /// �ð��� ����ϴ� ��ɵ鿡 �����ϸ� �� ȿ�����̴�.
    /// </summary>
    void FixedUpdate()
    {
        // �翷���� �����̴��� Ȯ���Ѵ�.
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        // Unity �����⿡�� ���� ������ ���� ������ ���忡�� ���� ������ Ȯ���ϼ���.        
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        // �翷���� �����̴��� Ȯ���Ѵ�.
        horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        // ���콺�� ������������ �Ǵ� ����Ͽ��� ��ũ���� ������ ������
        if (Input.GetMouseButton(0))
        {
            horizontalSpeed = CalculateMovement(Input.mousePosition);
        }

        // ����ϵ���̽����� ����ǰ� �ִ��� Ȯ���Ѵ�. 
#elif UNITY_IOS || UNITY_ANDROID

             if(horizMovement == MobileHorizMovement.Accelerometer)
            {
                // Move player based on direction of the accelerometer
                horizontalSpeed = Input.acceleration.x * dodgeSpeed;
            }

            //�Է��� 0�� �̻��� ��ġ�� �����ߴ��� üũ
            if (Input.touchCount > 0)
            {
                 if (horizMovement == MobileHorizMovement.ScreenTouch)
                 {
                       //ù��° ��ġ�� �����Ѵ�. 
                       Touch touch = Input.touches[0];
                       horizontalSpeed = CalculateMovement(touch.position);
                  }
            }
#endif

        rb.AddForce(horizontalSpeed, 0, rollSpeed);

    }
    void Update()
    {
 #if UNITY_IOS || UNITY_ANDROID //����� ����̽����� ����Ǵ��� Ȯ���Ѵ�.
         if (Input.touchCount > 0) // �Է��� 0�� �̻��� ��ġ�� �����ߴ��� üũ
         {
                Touch touch = Input.touches[0]; // ù��° ��ġ�� ����
                SwipeTeleport(touch);
                ScalePlayer();
         }
 #endif
    }
        /// <summary> 
        /// �÷��̾ �������� �̵��� ��ġ�� �˾Ƴ��ϴ�.
        /// </summary> 
        /// <param name="pixelPos">�÷��̾ ��ġ/Ŭ���� ��ġ</param>
        /// <returns>x�࿡�� �̵��� ����</returns> 
     float CalculateMovement(Vector3 pixelPos)
    { 
            // 0 �� 1 scale�� ��ȯ�Ѵ�.
            var worldPos = Camera.main.ScreenToViewportPoint(pixelPos);
            float xMove = 0;

            if (worldPos.x < 0.5f) //��ũ���� �������� ���� ���
            {
                xMove = -1;
            }
            else  // //�ٸ������ ������ ��������
            {
                xMove = 1;
            }

            return xMove * dodgeSpeed;  //horizontalSpeed���� ���ο� ������ ����
     }
    void SwipeTeleport(Touch touch)
    {
            // ��ġ�� ���۵Ǿ����� Ȯ�� 
            if (touch.phase == TouchPhase.Began)
            {
                // ��ġ�� ���۵Ǿ��ٸ� touchStart ���� 
                touchStart = touch.position;
            }
            // touch�� ��������  
            else if (touch.phase == TouchPhase.Ended)
            {
                // ��ġ�� ���� ������ ����
                Vector2 touchEnd = touch.position;

                // x�࿡�� ��ġ�� ���۰� �� ������ ���̸� ����մϴ�. 
                float x = touchEnd.x - touchStart.x;

                // Swipe �Ÿ��� ���ġ �ʴٸ� �ڷ���Ʈ ���� ����.
                if (Mathf.Abs(x) < minSwipeDistancePixels)
                {
                    return;
                }
                Vector3 moveDirection;
                // x�࿡�� ������ �̵��� ��� �������� �̵�
                if (x < 0)
                {
                    moveDirection = Vector3.left;
                }
                else
                {
                    // �׷��� ������ ���������� �̵� 
                    moveDirection = Vector3.right;
                }
                RaycastHit hit;
                // �浹�Ǵ� ���� �������� �̵� 
                if (!rb.SweepTest(moveDirection, out hit, swipeMove))
                {
                    // player �̵� 
                    rb.MovePosition(rb.position + (moveDirection * swipeMove));
                }
            }
        
    }
    [Header("Scaling Properties")]

    [Tooltip("�÷��̾��� �ּ� ũ��(Unity ����).")]
    public float minScale = 0.5f;

    [Tooltip("�÷��̾��� �ִ� ũ��(Unity ����).")]
    public float maxScale = 3.0f;

    /// <summary>
    /// player�� ���� ������
    /// </summary>
    private float currentScale = 1;

    private void ScalePlayer()
    {
        // ������Ʈ�� ũ�⸦ �����Ϸ��� �� ���� ��ġ�� �ʿ��մϴ�.
        if (Input.touchCount != 2)
        {
            return;
        }
        else
        {
            //������ ��ġ�� ���� 
            Touch touch0 = Input.touches[0];
            Touch touch1 = Input.touches[1];

            // ���� �����ӿ��� �� ��ġ�� ��ġ�� ã�´�.
            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            // �� �������� ��ġ ������ �Ÿ�(�Ǵ� ũ��)�� ã���ϴ�.
            float prevTouchDeltaMag = (touch0Prev - touch1Prev).magnitude;

            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // �� ������ ������ �Ÿ� ����.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            // ������ �ӵ��� ������� ��ȭ�� �����ϰ� �����Ѵ�.
            float newScale = currentScale - (deltaMagnitudeDiff * Time.deltaTime);

            // ���ο� �������� �ùٸ� ���ֿ� �ִ��� Ȯ���Ѵ�.
            newScale = Mathf.Clamp(newScale, minScale, maxScale);

            // �÷��̾� ������ ������Ʈ
            transform.localScale = Vector3.one * newScale;

            // ���� �����ӿ� ���罺���� ����
            currentScale = newScale;

        }
    }
}