//-------------------------------------------------------------------------------------

//              GHI Electronics, LLC

//               Copyright (c) 2010

//               All rights reserved

//-------------------------------------------------------------------------------------

/*

 * You can use this file if you agree to the following:

 *

 * 1. This header can't be changed under any condition.

 *    

 * 2. This is a free software and therefore is provided with NO warranty.

 * 

 * 3. Feel free to modify the code but we ask you to provide us with

 *	  any bugs reports so we can keep the code up to date.

 *

 * 4. This code may ONLY be used with GHI Electronics, LLC products.

 *

 * THIS SOFTWARE IS PROVIDED BY GHI ELECTRONICS, LLC ``AS IS'' AND 

 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED 

 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR 

 * A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL 

 * GHI ELECTRONICS, LLC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,

 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 

 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,

 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 

 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR ORT 

 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 

 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.  

 *

 *	Specs are subject to change without any notice

 */



using System;

using System.Runtime.CompilerServices;

using System.Threading;

using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;



namespace GHIElectronics.NETMF.FEZ

{

    public static partial class FEZ_Components

    {

        public class LED : IDisposable

        {

            OutputPort led;

            Timer timer;

            int onTime, offTime;

            bool disposed = false;

            bool timerRunning = false;

            bool blinkState;



            [MethodImpl(MethodImplOptions.Synchronized)]

            public void Dispose()

            {

                if (!disposed)

                {

                    disposed = true;

                    timerRunning = false;

                    timer.Dispose();

                    led.Dispose();

                }

            }



            public LED(FEZ_Pin.Digital pin)

            {

                led = new OutputPort((Cpu.Pin)pin, true);

                timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);

            }



            [MethodImpl(MethodImplOptions.Synchronized)]

            public void SetState(bool LED_state)

            {

                if (disposed)

                    throw new ObjectDisposedException();



                if (timerRunning)

                {

                    timer.Dispose();

                    timerRunning = false;

                }



                led.Write(LED_state);

            }



            public void TurnOn()

            {

                SetState(true);

            }

            public void ShutOff()

            {

                SetState(false);

            }



            [MethodImpl(MethodImplOptions.Synchronized)]

            public void StartBlinking(int onTime_mSec, int offTime_mSec)

            {

                if (disposed)

                    throw new ObjectDisposedException();



                this.onTime = onTime_mSec;

                this.offTime = offTime_mSec;



                timerRunning = true;

                blinkState = true;

                timer.Change(0, Timeout.Infinite);

            }



            public void StopBlinking()

            {

                StartBlinking(Timeout.Infinite, 0);

            }



            [MethodImpl(MethodImplOptions.Synchronized)]

            void Callback(object o)

            {

                if (!timerRunning)

                    return;



                if (blinkState)

                {

                    led.Write(true);

                    timer.Change(onTime, Timeout.Infinite);

                }

                else

                {

                    led.Write(false);

                    timer.Change(offTime, Timeout.Infinite);

                }



                blinkState = !blinkState;

            }

        }

    }

}

