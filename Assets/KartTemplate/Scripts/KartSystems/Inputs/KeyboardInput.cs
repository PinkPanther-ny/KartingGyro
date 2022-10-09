using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using System.ComponentModel;
using System.Text;
using System;
namespace KartGame.KartSystems
{
    /// <summary>
    /// A basic keyboard implementation of the IInput interface for all the input information a kart needs.
    /// </summary>
    public class KeyboardInput : MonoBehaviour, IInput
    {
        [SerializeField]
        public string device = "/dev/ttyUSB0";
        public int serialPortByteOffset = 1;
        public float fowardStartPitchAngle = 30f;
        public float fowardSepPitchAngle = 20f;
        public float fowardFullAddPitchAngle = 20f;
        
        public float turnStartRollAngle = 0f;
        public float turnSepRollAngle = 40f;
        public float turnFullAddRollAngle = 20f;
        
        SerialPort port;
        // Should read 18 but on windows there's an offset of 1 byte
        byte[] byteBuffer = new byte[20];
        float yaw_angle;
        float pitch_angle;
        float roll_angle;
        
        // Start is called before the first frame update
        void Start()
        {
	    port = new SerialPort(device, 115200, Parity.None, 8, StopBits.One);
	    port.Open();
            Thread t1 = new Thread(Update_data);
            t1.Start();
        }
        int twosComplement_hex(int hexval){
            int bits = 16;
            int val = hexval;
            if ((hexval & (1 << bits-1)) != 0){
                val -= 1 << bits;
            }
            return val;
    
        }

        // Update is called once per frame
        void Update_data()
        {
    	    while(true){
	    	    port.Read(byteBuffer, serialPortByteOffset, 18);
	    	    yaw_angle   = twosComplement_hex(byteBuffer[10] | byteBuffer[11] << 8) / 100.0f;
	    	    pitch_angle = twosComplement_hex(byteBuffer[12] | byteBuffer[13] << 8) / 100.0f;
	    	    roll_angle  = twosComplement_hex(byteBuffer[14] | byteBuffer[15] << 8) / 100.0f;


                string hex = BitConverter.ToString(byteBuffer);
                Debug.Log(hex);
                //Debug.Log((byteBuffer[0], byteBuffer[1], yaw_angle, pitch_angle, roll_angle));
            }
        }
        public float Acceleration
        {
            get { return m_Acceleration; }
        }
        public float Steering
        {
            get { return m_Steering; }
        }
        public bool BoostPressed
        {
            get { return m_BoostPressed; }
        }
        public bool FirePressed
        {
            get { return m_FirePressed; }
        }
        public bool HopPressed
        {
            get { return m_HopPressed; }
        }
        public bool HopHeld
        {
            get { return m_HopHeld; }
        }

        float m_Acceleration;
        float m_Steering;
        bool m_HopPressed;
        bool m_HopHeld;
        bool m_BoostPressed;
        bool m_FirePressed;

        bool m_FixedUpdateHappened;

        void Update ()
        {	
        
            if (Input.GetKey (KeyCode.UpArrow))
                m_Acceleration = 1f;
            else if (Input.GetKey (KeyCode.DownArrow))
                m_Acceleration = -1f;
            else
                m_Acceleration = 0f;

            if (Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow))
                m_Steering = -1f;
            else if (!Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.RightArrow))
                m_Steering = 1f;
            else
                m_Steering = 0f;
        
        
            if (pitch_angle < fowardStartPitchAngle - fowardSepPitchAngle/2)
            	
                m_Acceleration = Math.Min(1f, (fowardStartPitchAngle - fowardSepPitchAngle/2 - pitch_angle) / fowardFullAddPitchAngle );
            else if (pitch_angle > fowardStartPitchAngle + fowardSepPitchAngle/2)
                m_Acceleration = -Math.Min(1f, (pitch_angle - fowardStartPitchAngle - fowardSepPitchAngle/2) / fowardFullAddPitchAngle );
            else
                m_Acceleration = 0f;

            if (-roll_angle < turnStartRollAngle - turnSepRollAngle/2)
                m_Steering = -Math.Min(1f, (turnStartRollAngle - turnSepRollAngle/2 - (-roll_angle)) / turnFullAddRollAngle );
            else if (-roll_angle > turnStartRollAngle + turnSepRollAngle/2)
                m_Steering = Math.Min(1f, ((-roll_angle) - turnStartRollAngle - turnSepRollAngle/2) / turnFullAddRollAngle );
            else
                m_Steering = 0f;
            

            m_HopHeld = Input.GetKey (KeyCode.Space);

            if (m_FixedUpdateHappened)
            {
                m_FixedUpdateHappened = false;

                m_HopPressed = false;
                m_BoostPressed = false;
                m_FirePressed = false;
            }

            m_HopPressed |= Input.GetKeyDown (KeyCode.Space);
            m_BoostPressed |= Input.GetKeyDown (KeyCode.RightShift);
            m_FirePressed |= Input.GetKeyDown (KeyCode.RightControl);
        }

        void FixedUpdate ()
        {
            m_FixedUpdateHappened = true;
        }
    }
}
