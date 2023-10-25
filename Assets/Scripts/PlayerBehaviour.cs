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
    [Tooltip("공이 왼쪽 / 오른쪽으로 얼마나 빨리 움직이는지")]
    public float dodgeSpeed = 5;
    [Tooltip("공이 자동으로 얼마나 빠르게 앞으로 움직이는지")]
    [Range(0, 10)]
    public float rollSpeed = 5;
    
    // 중력가속도센서를 위한 변수선언, 중력가속도를 사용할지
    // 터치를 사용할 지 선택할 수있게 변수 선언
    public enum MobileHorizMovement
    {
        Accelerometer,
        ScreenTouch
    }

    public MobileHorizMovement horizMovement = MobileHorizMovement.Accelerometer;


    [Header("Swipe Properties")]

    [Tooltip("스와이프하면 플레이어가 멀리 움직이는 거리 지정 ")]
    public float swipeMove = 2f;

    [Tooltip("액션을 실행하기 전에 플레이어가 얼마나 멀리 스와이프해야 하는지(1/4인치)")]
    public float minSwipeDistance = 0.25f;

    /// <summary>
    /// pixels로 변환한 minSwipeDistance값을 저장
    /// </summary>
    private float minSwipeDistancePixels;

    /// <summary> 
    /// mobile touch events의 starting position 저장
    /// </summary> 
    private Vector2 touchStart;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        minSwipeDistancePixels = minSwipeDistance * Screen.dpi;

    }

    /// <summary>
    /// FixedUpadte는 일정한 프레임에 호출되며 
    /// 시간에 기반하는 기능들에 적용하면 더 효과적이다.
    /// </summary>
    void FixedUpdate()
    {
        // 양옆으로 움직이는지 확인한다.
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        // Unity 편집기에서 실행 중인지 독립 실행형 빌드에서 실행 중인지 확인하세요.        
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        // 양옆으로 움직이는지 확인한다.
        horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        // 마우스를 누르고있으면 또는 모바일에서 스크린을 누르고 있으면
        if (Input.GetMouseButton(0))
        {
            horizontalSpeed = CalculateMovement(Input.mousePosition);
        }

        // 모바일디바이스에서 실행되고 있는지 확인한다. 
#elif UNITY_IOS || UNITY_ANDROID

             if(horizMovement == MobileHorizMovement.Accelerometer)
            {
                // Move player based on direction of the accelerometer
                horizontalSpeed = Input.acceleration.x * dodgeSpeed;
            }

            //입력이 0개 이상의 터치를 감지했는지 체크
            if (Input.touchCount > 0)
            {
                 if (horizMovement == MobileHorizMovement.ScreenTouch)
                 {
                       //첫번째 터치를 감지한다. 
                       Touch touch = Input.touches[0];
                       horizontalSpeed = CalculateMovement(touch.position);
                  }
            }
#endif

        rb.AddForce(horizontalSpeed, 0, rollSpeed);

    }
    void Update()
    {
 #if UNITY_IOS || UNITY_ANDROID //모바일 디바이스에서 실행되는지 확인한다.
         if (Input.touchCount > 0) // 입력이 0개 이상의 터치를 감지했는지 체크
         {
                Touch touch = Input.touches[0]; // 첫번째 터치를 저장
                SwipeTeleport(touch);
                ScalePlayer();
         }
 #endif
    }
        /// <summary> 
        /// 플레이어를 수평으로 이동할 위치를 알아냅니다.
        /// </summary> 
        /// <param name="pixelPos">플레이어가 터치/클릭한 위치</param>
        /// <returns>x축에서 이동할 방향</returns> 
     float CalculateMovement(Vector3 pixelPos)
    { 
            // 0 과 1 scale로 변환한다.
            var worldPos = Camera.main.ScreenToViewportPoint(pixelPos);
            float xMove = 0;

            if (worldPos.x < 0.5f) //스크린의 오른쪽을 누른 경우
            {
                xMove = -1;
            }
            else  // //다른경우라면 왼쪽이 눌려진것
            {
                xMove = 1;
            }

            return xMove * dodgeSpeed;  //horizontalSpeed값을 새로운 값으로 지정
     }
    void SwipeTeleport(Touch touch)
    {
            // 터치가 시작되었는지 확인 
            if (touch.phase == TouchPhase.Began)
            {
                // 터치가 시작되었다면 touchStart 설정 
                touchStart = touch.position;
            }
            // touch가 끝났으면  
            else if (touch.phase == TouchPhase.Ended)
            {
                // 터치가 끝난 지점을 저장
                Vector2 touchEnd = touch.position;

                // x축에서 터치의 시작과 끝 사이의 차이를 계산합니다. 
                float x = touchEnd.x - touchStart.x;

                // Swipe 거리가 충분치 않다면 텔레포트 하지 않음.
                if (Mathf.Abs(x) < minSwipeDistancePixels)
                {
                    return;
                }
                Vector3 moveDirection;
                // x축에서 음수로 이동한 경우 왼쪽으로 이동
                if (x < 0)
                {
                    moveDirection = Vector3.left;
                }
                else
                {
                    // 그렇지 않으면 오른쪽으로 이동 
                    moveDirection = Vector3.right;
                }
                RaycastHit hit;
                // 충돌되는 것이 없을때만 이동 
                if (!rb.SweepTest(moveDirection, out hit, swipeMove))
                {
                    // player 이동 
                    rb.MovePosition(rb.position + (moveDirection * swipeMove));
                }
            }
        
    }
    [Header("Scaling Properties")]

    [Tooltip("플레이어의 최소 크기(Unity 단위).")]
    public float minScale = 0.5f;

    [Tooltip("플레이어의 최대 크기(Unity 단위).")]
    public float maxScale = 3.0f;

    /// <summary>
    /// player의 현재 스케일
    /// </summary>
    private float currentScale = 1;

    private void ScalePlayer()
    {
        // 오브젝트의 크기를 조정하려면 두 개의 터치가 필요합니다.
        if (Input.touchCount != 2)
        {
            return;
        }
        else
        {
            //감지된 터치를 저장 
            Touch touch0 = Input.touches[0];
            Touch touch1 = Input.touches[1];

            // 이전 프레임에서 각 터치의 위치를 찾는다.
            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            // 각 프레임의 터치 사이의 거리(또는 크기)를 찾습니다.
            float prevTouchDeltaMag = (touch0Prev - touch1Prev).magnitude;

            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // 각 프레임 사이의 거리 차이.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            // 프레임 속도에 관계없이 변화를 일정하게 유지한다.
            float newScale = currentScale - (deltaMagnitudeDiff * Time.deltaTime);

            // 새로운 스케일이 올바른 범주에 있는지 확인한다.
            newScale = Mathf.Clamp(newScale, minScale, maxScale);

            // 플레이어 스케일 업데이트
            transform.localScale = Vector3.one * newScale;

            // 다음 프레임에 현재스케일 설정
            currentScale = newScale;

        }
    }
}