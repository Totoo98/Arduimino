using System;
using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{

    private SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM1";
    [SerializeField] private int baudrate = 115200;

    private int _dominoType = 0;

    [SerializeField] private DominoSpawner Spawner;

    // Start is called before the first frame update
    void Start()
    {
        _serial = new SerialPort(serialPort, baudrate);
        // Guarantee that the newline is common across environments.
        _serial.NewLine = "\n";
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        Debug.Log("a");
        if (_serial.BytesToRead <= 0) return;

        var message = _serial.ReadLine();

        // Arduino sends "\r\n" with println, ReadLine() removes Environment.NewLine which will not be 
        // enough on Linux/MacOS.
        /*if (Environment.NewLine == "\n")
        {
            message = message.Trim('\r');
        }*/
        Debug.Log(message);

        if (message.StartsWith('b'))
        {
            switch (message.Trim())
            {
                case "b0":
                    /*TODO
                     Input pour selectionner rouge
                     */
                    _dominoType = 0;
                    break;
                case "b1":
                    /*TODO
                     Input pour selectionner bleu
                     */
                    _dominoType = 1;
                    break;
                case "b2":
                    /*TODO
                     Input pour selectionner vert
                     */
                    _dominoType = 2;
                    break;
                case "b3":
                    /*TODO
                     Input pour selectionner bleu
                     */
                    _dominoType = 3;
                    break;
                default:
                    break;
            }
        }
        else
        {
            int distance;
            if(int.TryParse(message, out distance))
            {
                if (distance < 5)
                {
                    Spawner.Spawn(_dominoType);
                }
            }
        }

        SetLed();
    }

    public void SetLed()
    {
        _serial.WriteLine(_dominoType.ToString());
    }

    private void OnDestroy()
    {
        _serial.Close();
    }
}
