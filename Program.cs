using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.SocketInterfaces;

namespace GadgeteerApp1
{
    public partial class Program
    {
        void ProgramStarted()
        {
            Thread input = new Thread(InputThread);

            Mainboard.SetMotorInversion(true, true);
            input.Start();
        }

        void InputThread()
        {
            GT.Socket socket = GT.Socket.GetSocket(5, true, null, null);
            //Mainboard.TurnOnLed(6);

            DigitalInput p7 = DigitalInputFactory.Create(socket, GT.Socket.Pin.Three, GlitchFilterMode.Off, ResistorMode.PullDown, breakout);
            //Mainboard.TurnOnLed(7);

            DigitalInput p8 = DigitalInputFactory.Create(socket, GT.Socket.Pin.Four, GlitchFilterMode.Off, ResistorMode.PullDown, breakout);
            //Mainboard.TurnOnLed(8);

            DigitalInput p9 = DigitalInputFactory.Create(socket, GT.Socket.Pin.Five, GlitchFilterMode.Off, ResistorMode.PullDown, breakout);
            //Mainboard.TurnOnLed(9);

            while (true)
            {
                int spd = 50;
                string cmd = GetCommand(p7, p8, p9);

                //Debug.Print("CMD " + cmd);
                switch (cmd)
                {
                    case "STOP":
                        Mainboard.SetMotorSpeed(0, 0);
                        break;
                    case "GO":
                        Mainboard.SetMotorSpeed(spd, spd);
                        break;
                    case "LEFT":
                        Mainboard.SetMotorSpeed(spd, -spd);
                        break;
                    case "RIGHT":
                        Mainboard.SetMotorSpeed(-spd, spd);
                        break;
                    case "BACK":
                        Mainboard.SetMotorSpeed(-spd, -spd);
                        break;
                    default:
                        Mainboard.SetMotorSpeed(0, 0);
                        break;
                }

            }
        }


        string GetCommand(DigitalInput p7, DigitalInput p8, DigitalInput p9)
        {
            bool b7 = p7.Read();
            bool b8 = p8.Read();
            bool b9 = p9.Read();

            if (b7 == false && b8 == false && b9 == true)
            {
                //Mainboard.TurnOnLed(15);
                return "GO";
            }
            else if (b7 == false && b8 == true && b9 == false)
            {
                //Mainboard.TurnOnLed(14);
                return "BACK";
            }
            else if (b7 == false && b8 == true && b9 == true)
            {
                //Mainboard.TurnOnLed(13);
                return "LEFT";
            }
            else if (b7 == true && b8 == false && b9 == false)
            {
                //Mainboard.TurnOnLed(12);
                return "RIGHT";
            }
            else
            {
                //Mainboard.TurnOnLed(11);
                return "STOP";
            }
        }

    }
}
