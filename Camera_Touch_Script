using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class CameraMovement : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    public static CameraMovement Inst { get; private set; }
    void Awake() => Inst = this;

    Vector3 FirstPoint;
    Vector3 SecondPoint;
    public float xAngle = 0f;
    public float yAngle = 0f;
    float xAngleTemp;

    public float angle = 16f;
    public float x_point = 0;

    public bool Login;
    public bool Move;

    GameObject Target_mob;
    RaycastHit2D hit;
    bool timer = false;
    float time = 0;
    void Update()
    {
        if (timer)
            time += Time.deltaTime;

        Mob_Control();
    }

    void Mob_Control()
    {
        if (Target_mob != null)
        {
            if (time > 0.1f)
            {
                timer = false;
                Target_mob.transform.GetComponent<Mob_play>().sky_timer = false;

                //마우스 좌표를 받아온다.
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                //마우스 좌표를 스크린 투 월드로 바꾸고 이 객체의 위치로 설정해 준다.
                Target_mob.transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + new Vector3(0, 0, 30);

                Field_Move = true;
            }
        }
        else if(time > 0.5f)
        {
            hit.transform.GetComponent<Block>().Create_Field();
            Obj_on.Inst.OnSet();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Login)
        {
            if (Story_Pointer.Inst.GameInDate("Story_Point") == "NO")
                Story_Pointer.Inst.First_Point();

            if (Story_Pointer.Inst.GameInDate("Stage") == "NO")
                PlanetManager.Inst.First_Setting();

            PlanetManager.Inst.Load_Setting();
            Player_UI.Inst.Money_Text();
            StartCoroutine(game_start());
        }
    }

    IEnumerator game_start()
    {
        yield return State_MobManager.Inst.Mob_database();
        Obj_Clear.Inst.GoTo_Screen("육성화면");
    }

    public bool Camera_Move = false;
    public bool Field_Move = false;
    public bool Plus_Cam = false;
    private int drag_time = 0;
    public void OnPointerDown(PointerEventData eventData)
    {
        drag_time = 0;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Drag"));
        if (Field_Move && Plus_Cam)
            return;

        if (hit.collider != null)
        {
            hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Mob"));
            if (hit.collider != null)
            {
                time = 0;
                timer = true;
                Target_mob = hit.collider.gameObject;
            }
        }

        hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Option"));
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            #region 메인화면
            if (hit.collider.name == "맵옵션버튼")
            {
                if (BlockManager.Inst.field_list.Count > 0)
                    Map_Option.Inst.Up_Site();
            }
            else if (hit.collider.name == "리그버튼")
                Obj_Clear.Inst.GoTo_Screen("리그화면");
            else if (hit.collider.name == "맵삭제버튼")
            {
                if (BlockManager.Inst.field_list.Count > 0)
                {
                    BlockManager.Inst.Map_Delet();
                    Map_Option.Inst.Up_Site_Set();
                }
            }
            else if (hit.collider.name == "몬스터편성버튼")
            {
                if (BlockManager.Inst.field_list.Count > 0)
                {
                    BlockManager.Inst.mob_num = hit.transform.GetComponent<Click_Select>().mob_num;

                    if (hit.transform.GetComponent<Click_Select>().character_sprite.sprite != null)
                    {
                        hit.transform.GetComponent<Click_Select>().mob_clear();
                    }
                    else
                        UI_But.Inst.Mob_Interface_list();
                }
            }
            #endregion
            #region 리그
            else if (hit.collider.name == "이전행성")
            {
                PlanetManager.Inst.planets_Setup(PlanetManager.Inst.planet_addrass - 1);
            }
            else if (hit.collider.name == "현재행성")
            {
                if (hit.collider.transform.Find("땅").GetComponent<SpriteRenderer>().color.r < 1)
                {
                    List<string> item = new();
                    List<int> amount = new();
                    item.Add("행성" + PlanetManager.Inst.planet_addrass.ToString());
                    amount.Add(1);

                    StartCoroutine(Obj_on.Inst.But_setting("골드", item, amount));
                }
                else
                    PlanetManager.Inst.VS_setting(true);
            }
            else if (hit.collider.name == "다음행성")
                PlanetManager.Inst.planets_Setup(PlanetManager.Inst.planet_addrass + 1);
            else if (hit.collider.name == "전투스킵")
                PlanetManager.Inst.Skip_TurnOnOff();
            else if (hit.collider.name == "배경")
                PlanetManager.Inst.VS_setting(false);

            /*
            else if (hit.collider.name == "노멀난이도")
                Debug.Log("노멀난이도");
            else if (hit.collider.name == "하드난이도")
                Debug.Log("하드난이도");
            */
            #endregion
        }

        hit = Physics2D.Raycast(pos, Vector2.zero, 1, LayerMask.GetMask("Block"));
        if (hit.collider != null)
        {
            Debug.Log("빔");

            if (hit.transform.GetComponent<Block>().land.color.a >= 1)
                timer = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero, 1, LayerMask.GetMask("Block"));
        if (hit.collider != null)
            hit.transform.GetComponent<Block>().Create_Field();

        

        hit = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("Option_land"));
        if (hit.collider != null)
        {
            int num = -1;
            switch (hit.collider.name)
            {
                case "초원":
                    num = 0;
                    break;
                case "사막":
                    num = 1;
                    break;
                case "바다":
                    num = 2;
                    break;
                case "용암":
                    num = 3;
                    break;
                case "빙하":
                    num = 4;
                    break;
                case "오염":
                    num = 5;
                    break;
                case "숲":
                    num = 6;
                    break;
            }

            if (Map_Option.Inst.block_sprite[num].color.r != 50 / 255f)
            {
                BlockManager.Inst.Map_Land_Change(hit.collider.name);
                Map_Option.Inst.Up_Site();
            }
        }

        if (Target_mob != null)
        {
            Target_mob.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Target_mob = null;
            Field_Move = false;
        }

        if (drag_time <= 1)
        {
            hit = Physics2D.Raycast(pos, Camera.main.transform.rotation.eulerAngles, 0f, LayerMask.GetMask("Mob"));
            if (hit.collider != null)
            {
                //콜라이더 사이즈 조정
                Plus_Cam = MobManager.Inst.image_size(hit.collider.gameObject,
                    hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x * 100,
                    hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.y * 100);
            }
            else
            {
                if (Plus_Cam)
                    Plus_Cam = MobManager.Inst.image_del();
            }
        }

        timer = false;
        time = 0;

        Camera_Move = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Move || Field_Move || Plus_Cam)
        {
            Camera_Move = true;

            BeginDrag(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        drag_time += 1;

        if (Move)
        {
            OnDrag(eventData.position);
        }
    }

    public void BeginDrag(Vector2 a_FirstPoint)
    {
        FirstPoint = a_FirstPoint;
        xAngleTemp = xAngle;
    }

    public void OnDrag(Vector2 a_SecondPoint)
    {
        if (Field_Move || Plus_Cam)
            return;

        SecondPoint = a_SecondPoint;
        xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * -90 / Screen.width;

        // 회전값을 40~85로 제한
        if (xAngle > angle)
            xAngle = angle;
        if (xAngle < -angle)
            xAngle = -angle;

        Camera.main.transform.position = new Vector3(x_point + xAngle, Camera.main.transform.position.y, -30);
    }
}
