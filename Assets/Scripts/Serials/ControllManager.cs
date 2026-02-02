using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum SENDPACKET
{
    CASE0 = 0,
    CASE1 = 1
}

public class ControlManager : SerialManager<ControlManager>
{
    [Header("작성중. 수정 X")]
    [SerializeField] SENDPACKET myPacket;

    [Header("게임 로직 연결")]
    [SerializeField] private HumanToRobotTransform humanToRobotTransform;
    [SerializeField] private SelectButtonHandler selectButtonHandler;
    [SerializeField] private GameObject endBox;

    private string currentSelection = "사람"; // 현재 선택 ("사람" 또는 "로봇")

    [Header("슬라이더 조정 설정")]
    [SerializeField] private float rotationStep = 5f; // 회전 레버 1회당 변화량

    protected override void Awake()
    {
        base.Awake();
        sendData = SendPacket;
        reciveData = ReceivePacket;
    }

    protected override void Start()
    {
        base.Start();

        // 연결 상태 확인
        if (humanToRobotTransform == null)
        {
            Debug.LogError("[ControllManager] HumanToRobotTransform이 연결되지 않았습니다!");
            // 자동으로 찾기 시도
            humanToRobotTransform = FindObjectOfType<HumanToRobotTransform>();
            if (humanToRobotTransform != null)
            {
                Debug.Log("[ControllManager] HumanToRobotTransform 자동 연결 성공");
            }
        }

        if (selectButtonHandler == null)
        {
            Debug.LogError("[ControllManager] SelectButtonHandler가 연결되지 않았습니다!");
            // 자동으로 찾기 시도
            selectButtonHandler = FindObjectOfType<SelectButtonHandler>();
            if (selectButtonHandler != null)
            {
                Debug.Log("[ControllManager] SelectButtonHandler 자동 연결 성공");
            }
        }
    }

    private void Update()
    {
        // 키보드 테스트 (Input System 사용 시 주석 처리)
        // if (Input.GetKeyDown(KeyCode.Q))
        //     ReceivePacket(new byte[] { 0xFA, 0 });
        // else if (Input.GetKeyDown(KeyCode.W))
        //     ReceivePacket(new byte[] { 0xFA, 1 });
        // else if (Input.GetKeyDown(KeyCode.E))
        //     ReceivePacket(new byte[] { 0xFA, 2 });

        // 시리얼 큐에서 데이터 처리
        if (mainThreadQueue.TryDequeue(out byte[] result))
        {
            ReceivePacket(result);
        }
    }

    public void SendPacket(byte[] bytes)
    {
        switch (myPacket)
        {
            case SENDPACKET.CASE0:
                break;
            case SENDPACKET.CASE1:
                break;
        }
    }

    public void ReceivePacket(byte[] bytes)
    {
        if (endBox != null && endBox.activeSelf) return;

        Debug.Log($"[ControllManager] ========================================");
        Debug.Log($"[ControllManager] 패킷 수신 - 전체: {BitConverter.ToString(bytes)}");
        Debug.Log($"[ControllManager] Header={bytes[0]:X2}, Command={bytes[1]}");

        switch (bytes[1])
        {
            case 1:
                // 레버 상하: 사람 <-> 로봇 토글
                Debug.Log("[ControllManager] ★★★ 레버 상하 입력 - 사람/로봇 전환");
                ToggleSelection();
                break;

            case 2:
                // 버튼 클릭: 확정 처리 (3번 + 4번 키 역할)
                Debug.Log("[ControllManager] ★★★ 버튼 클릭 - 확정 처리");
                ConfirmSelection();
                break;

            case 4:
                // 회전 오른쪽: 슬라이더 증가 (로봇 쪽으로)
                Debug.Log("[ControllManager] ★★★ 회전 오른쪽 - 슬라이더 증가");
                AdjustSlider(rotationStep);
                break;

            case 8:
                // 회전 왼쪽: 슬라이더 감소 (사람 쪽으로)
                Debug.Log("[ControllManager] ★★★ 회전 왼쪽 - 슬라이더 감소");
                AdjustSlider(-rotationStep);
                break;

            default:
                Debug.LogWarning($"[ControllManager] ⚠ 알 수 없는 명령: {bytes[1]} (10진수: {bytes[1]}, 16진수: {bytes[1]:X2})");
                break;
        }

        Debug.Log($"[ControllManager] ========================================");
    }

    /// <summary>
    /// 사람 <-> 로봇 선택 토글
    /// </summary>
    private void ToggleSelection()
    {
        if (currentSelection == "사람")
        {
            currentSelection = "로봇";
            selectButtonHandler?.OnClickSelectButtonDown(); // 로봇 선택
        }
        else
        {
            currentSelection = "사람";
            selectButtonHandler?.OnClickSelectButtonUp(); // 사람 선택
        }

        Debug.Log($"[ControllManager] 현재 선택: {currentSelection}");
    }

    /// <summary>
    /// 슬라이더 값 조정 (회전 레버)
    /// </summary>
    private void AdjustSlider(float delta)
    {
        if (humanToRobotTransform != null)
        {
            humanToRobotTransform.transformValue = Mathf.Clamp(
                humanToRobotTransform.transformValue + delta,
                0f,
                100f
            );
            Debug.Log($"[ControllManager] 슬라이더 값: {humanToRobotTransform.transformValue}");
        }
    }

    /// <summary>
    /// 확정 처리 (3번 + 4번 키 한 번에 실행)
    /// </summary>
    private void ConfirmSelection()
    {
        // 3번 키 역할: 확정 배경 표시
        selectButtonHandler?.OnClickSelectCheck();
        Debug.Log("[ControllManager] 확정 배경 표시 (3번 키 역할)");

        // 4번 키 역할: 최종 결과 표시 (선택에 따라 커스텀 메시지)
        ShowCustomResultMessage();
        Debug.Log("[ControllManager] 최종 결과 표시 (4번 키 역할)");
    }

    /// <summary>
    /// 선택에 따른 커스텀 결과 메시지 표시
    /// </summary>
    private void ShowCustomResultMessage()
    {
        if (humanToRobotTransform == null || selectButtonHandler == null) return;

        // 게이지 값 = 항상 로봇 비율
        float sliderValue = humanToRobotTransform.transformValue;
        int robotPercent = (int)sliderValue;
        int humanPercent = 100 - robotPercent;

        // 결과 메시지 직접 설정
        var endBox = selectButtonHandler.GetType().GetField("_endBox",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(selectButtonHandler);
        var endText = selectButtonHandler.GetType().GetField("_endText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(selectButtonHandler);
        var gameInit = selectButtonHandler.GetType().GetField("_gameInit",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(selectButtonHandler);

        if (endBox != null && endText != null)
        {
            ((UnityEngine.GameObject)endBox).SetActive(true);
            ((TMPro.TextMeshProUGUI)endText).text =
                $"당신은\n사람 {humanPercent}% 기계 {robotPercent}%를\n{currentSelection} 이라고 선택했습니다.";

            if (gameInit != null)
            {
                ((AllGameInit)gameInit).StartTimer();
            }

            Debug.Log($"[ControllManager] 결과 표시: 사람 {humanPercent}% 기계 {robotPercent}%를 {currentSelection} 선택");
        }
    }

    /// <summary>
    /// 초기화: 게이지 0%, 사람 선택으로 리셋
    /// </summary>
    public void ResetAll()
    {
        // 게이지 0으로 초기화
        if (humanToRobotTransform != null)
        {
            humanToRobotTransform.transformValue = 0f;
            Debug.Log("[ControllManager] 게이지 0%로 초기화");
        }

        // 선택 "사람"으로 초기화
        currentSelection = "사람";
        selectButtonHandler?.OnClickSelectButtonUp();
        Debug.Log("[ControllManager] 선택 '사람'으로 초기화");

        // SelectButtonHandler의 allInit도 호출
        selectButtonHandler?.allInit();
    }
}
