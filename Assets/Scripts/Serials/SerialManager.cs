    #region OLD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.IO.Ports;

public delegate void DPacketDelegate(byte[] bytes);

public class SerialManager<T> : SingletonTemplate<T> where T : MonoBehaviour
{
    [Header("��� ����")]
    [Tooltip("���� ���� �������� Headr ���� �Է����ּ���.")]
    [SerializeField] protected Byte reciveHeader = 0xFA;

    [Tooltip("���� �޴� �������� ���̸� �����ּ���.(Header ����)")]
    [SerializeField] protected int reciveDataLength = 2;

    [Tooltip("���� �� �������� Headr ���� �Է����ּ���.")]
    [SerializeField] protected Byte sendHeader = (byte)0XFA;

    [Tooltip("���� �� �������� ���̸� �����ּ���.(Header ����)")]
    [SerializeField] protected int sendDataLength = 2;

    [Tooltip("��� �ӵ�")]
    [SerializeField] private int communicationSpeed = 9600;

    //[SerializeField] protected TextMeshProUGUI textMeshProUGUI;

    private SerialPort serialPort;
    private List<string> portNames;
    private string[] hasPortNames;
    private string targetPortName;
    private byte[] reciveDataArray;
    private byte[] saveDataArray;
    protected byte[] sendDataArray;

    protected DPacketDelegate sendData;
    protected DPacketDelegate reciveData;

    //private MainThreadDispatcher mtd;

    protected Queue<byte[]> mainThreadQueue = new Queue<byte[]>();
    protected readonly object queueLock = new object();

    [Tooltip("����׿�")]
    //[SerializeField] TextMeshProUGUI inputText;

    protected override void Awake()
    {
        Debug.Log("[SerialManager] Awake 시작");
        portNames = new List<string>();

        if (FindSerialPortName())
        {
            Debug.Log("[SerialManager] 포트 찾기 성공");
            InitSerialPort();
        }
        else
        {
            Debug.LogError("[SerialManager] 포트 찾기 실패 - 사용 가능한 포트가 없습니다");
            Debug.LogError("[SerialManager] 아두이노 연결을 확인하거나 다른 프로그램에서 포트를 사용 중인지 확인하세요");
        }

        reciveDataArray = new byte[reciveDataLength];
        saveDataArray = new byte[reciveDataLength];
        sendDataArray = new byte[sendDataLength];
        Debug.Log($"[SerialManager] 배열 초기화 완료 - reciveDataLength: {reciveDataLength}");
    }

    protected async virtual void Start()
    {
        Debug.Log($"[SerialManager] Start - serialPort null: {serialPort == null}, IsOpen: {serialPort?.IsOpen}");

        if (serialPort != null && serialPort.IsOpen)
        {
            Debug.Log("[SerialManager] 시리얼 포트 리스닝 시작");
            await ListeningSerialPort();
        }
        else
        {
            Debug.LogError("[SerialManager] 시리얼 포트가 열려있지 않음 - 리스닝 스킵");
        }
    }

    bool FindSerialPortName()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "UnusedPorts.txt");

        if (File.Exists(path))
        {
            string Content = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(Content))
            {
                SettingPortName(Content);
            }
        }

        hasPortNames = SerialPort.GetPortNames();

        if (hasPortNames.Length == 0)
        {
            Debug.LogError("[SerialManager] ========================================");
            Debug.LogError("[SerialManager] 포트가 전혀 감지되지 않습니다!");
            Debug.LogError("[SerialManager] 1. 아두이노가 USB에 연결되어 있는지 확인");
            Debug.LogError("[SerialManager] 2. 장치 관리자에서 COM 포트가 보이는지 확인");
            Debug.LogError("[SerialManager] 3. 아두이노 드라이버(CH340/FTDI) 설치 확인");
            Debug.LogError("[SerialManager] ========================================");
        }
        else
        {
            Debug.Log($"[SerialManager] 사용 가능한 포트: {string.Join(", ", hasPortNames)}");
        }

        if (hasPortNames.Length > 0)
        {
            foreach (var port in hasPortNames)
            {
                bool IsExcluded = false;

                if (portNames != null && portNames.Count > 0)
                {
                    foreach (string exclude in portNames)
                    {
                        if (port == exclude)
                        {
                            IsExcluded = true;
                            Debug.Log($"[SerialManager] {port} 제외됨 (UnusedPorts.txt)");
                            break;
                        }
                    }
                }

                if (!IsExcluded)
                {
                    targetPortName = port;
                    Debug.Log($"[SerialManager] {port} 선택됨");
                    return true;
                }
            }

            Debug.LogError("[SerialManager] 모든 포트가 제외 목록에 있습니다");
            return false;
        }
        else
        {
            Debug.LogError("[SerialManager] 연결된 시리얼 포트가 없습니다");
            return false;
        }
    }

    void InitSerialPort()
    {
        try
        {
            serialPort = new SerialPort(targetPortName, communicationSpeed)
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = -1 // ���� ��� (TimeoutException ����)
            };
            
            serialPort.Open();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SerialManager] 포트 열기 실패!");
            Debug.LogError($"[SerialManager] Port: {targetPortName}, Speed: {communicationSpeed}");
            Debug.LogError($"[SerialManager] Error: {e.Message}");
        }
    }

    async Task ListeningSerialPort()
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            return;
        }

        while (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            //byte[] tempBuffer = new byte[reciveDataLength];
                            int toRead = Math.Min(serialPort.BytesToRead, reciveDataArray.Length);
                            int bytesRead = serialPort.Read(reciveDataArray, 0, toRead);

                            //Debug.Log($"���� ����Ʈ: {bytesRead}, �ʿ�: {reciveDataArray.Length}");
                            //Debug.Log($"������: {BitConverter.ToString(reciveDataArray)}");

                            if (bytesRead == reciveDataArray.Length)
                            {
                                //reciveData(reciveDataArray);

                                byte[] dataCopy = new byte[reciveDataLength];
                                Array.Copy(reciveDataArray, dataCopy, reciveDataLength);

                                lock (queueLock)
                                {
                                    mainThreadQueue.Enqueue(dataCopy);
                                    //Debug.Log($"ť�� �߰���! ť ������: {mainThreadQueue.Count}");
                                }

                                //SaveData(reciveDataArray);
                            }
                            else
                            {
                                //Debug.Log("�־ʵ�?");
                                //textMeshProUGUI.text = $"���ŵ� ������ ����: {bytesRead}/{reciveDataArray.Length}";
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    });
                }
                else
                {
                    await Task.Delay(50);
                }
            }
            catch (Exception e)
            {
                break;
            }
        }
    }

    void SaveData(byte[] data)
    {
        Debug.Log("������ ����");

        if (data == null || data.Length < reciveDataLength)
        {
            //textMeshProUGUI.text = "���ŵ� ������ ����(�����Ͱ� ª���ϴ�.)";
            return;
        }

        Array.Copy(data, saveDataArray, data.Length);
        //textMeshProUGUI.text = "0";

        if (saveDataArray[0] == reciveHeader)
        {
            //textMeshProUGUI.text = "1";
            reciveData?.Invoke(saveDataArray);
            //textMeshProUGUI.text = "2";
        }
    }

    public void SendData()
    {
        //debugText.text = "���� ����Ÿ ����";

        if (serialPort != null && serialPort.IsOpen)
        {
            //debugText.text = "��Ʈ�� ���� �ƴϰ�, ��Ʈ�� ����";
            //debugText.text = $"Ĺġ {sendDataArray.Length}";
            try
            {
                //debugText.text = "����������";

                if (sendData != null)
                {
                    //debugText.text = "����Ÿ�� ���� �ƴ�";
                    sendDataArray[0] = sendHeader;
                    sendData(sendDataArray);
                }

                //debugText.text = $"����Ʈ ����ƾ�!!, {sendDataArray[1]}";
                serialPort.Write(sendDataArray, 0, sendDataArray.Length);
                //textMeshProUGUI.text = $"Sent {sendDataArray.Length} bytes: {BitConverter.ToString(sendDataArray)}";
            }
            catch (Exception e)
            {
                //debugText.text = $"Ĺġ: {e.Message}";
                //textMeshProUGUI.text = $"Failed to send data: {e.Message}";
            }
        }
        else
        {
            //textMeshProUGUI.text = "Serial port is not open or not connected.";
            //debugText.text = "����...?";
        }
    }

    void SettingPortName(string ReadFile)
    {
        if (portNames == null)
            portNames = new List<string>();

        string[] tokens = ReadFile.Split('/');
        foreach (var token in tokens)
        {
            if (!string.IsNullOrWhiteSpace(token))
                portNames.Add(token.Trim());
        }
    }
}
#endregion