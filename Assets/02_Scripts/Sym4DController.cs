using UnityEngine;
using Sym4D;
using Sym = Sym4D.Sym4DEmulator;
using System.Collections;

public class Sym4DController : MonoBehaviour
{
    // 포트번호 (COM포트)
    [SerializeField] private int xPort; // 의자 컨트롤 포트
    [SerializeField] private int wPort; // 팬 포트

    // 조이스틱 변수
    private float prevJoyX, prevJoyY;
    private float currJoyX, currJoyY;

    private WaitForSeconds ws = new WaitForSeconds(0.5f);

    IEnumerator Start()
    {
        StartCoroutine(InitSym4D());

        yield return ws;

        StartCoroutine(TestDevice());
    }

    void Update()
    {
        currJoyX = Input.GetAxis("Horizontal");
        currJoyY = Input.GetAxis("Vertical");

        if (currJoyX != prevJoyX || currJoyY != prevJoyY)
        {
            StartCoroutine(SetMotion((int)currJoyX * 10, (int)currJoyY * 10));
            prevJoyX = currJoyX;
            prevJoyY = currJoyY;
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            Sym.Sym4D_W_SendMosionData(100);
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button12))
        {
            Sym.Sym4D_W_SendMosionData(0);
        }
    }

    void OnDestroy()
    {
        Sym.Sym4D_X_EndContents();
        Sym.Sym4D_W_EndContents();
    }

    IEnumerator TestDevice()
    {
        // Roll
        StartCoroutine(SetMotion(-10, 0));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(10, 0));
        yield return new WaitForSeconds(2.0f);

        // Pitch
        StartCoroutine(SetMotion(0, -10));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(0, 10));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(10, 10));
        yield return new WaitForSeconds(2.0f);

        Sym.Sym4D_W_SendMosionData(100);
        yield return new WaitForSeconds(3.0f);
        Sym.Sym4D_W_SendMosionData(0);
    }

    // 모션 실행
    IEnumerator SetMotion(int roll, int pitch)
    {
        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;
        Sym.Sym4D_X_SendMosionData(roll * 10, pitch * 10);
        yield return ws;
    }

    // Sym4D 초기화
    IEnumerator InitSym4D()
    {
        // 포트 추출
        xPort = Sym.Sym4D_X_Find();
        yield return ws;

        wPort = Sym.Sym4D_W_Find();
        yield return ws;

        // 의자와 연결 포트를 오픈
        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;

        // 의자의 허용 각도 (0 ~ 100) (-10도 ~ +10도)
        Sym.Sym4D_X_SetConfig(100, 100);
        yield return ws;

        // 팬 연결
        Sym.Sym4D_W_StartContents(wPort);
        yield return ws;

        // 풍량 설정
        Sym.Sym4D_W_SetConfig(100);
        yield return ws;
    }
}
