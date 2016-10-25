/*
 * You can use this file if you agree to the following:
 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND 
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
 */
using System;
using Microsoft.SPOT;

namespace USBizi_Ethernet
{
    public enum IR_val
    {
            /* Sn_IR values */
        Sn_IR_SEND_OK		=	0x10,		/**< complete sending */
        Sn_IR_TIMEOUT		=	0x08,		/**< assert timeout */
        Sn_IR_RECV			=	0x04,		/**< receiving data */
        Sn_IR_DISCON		=		0x02,		/**< closed socket */
        Sn_IR_CON			=		0x01,		/**< established connection */
    }
    public enum SR_val
    {
            /* Sn_SR values */
        SOCK_CLOSED				=0x00,		/**< closed */
        SOCK_INIT 				=0x13,		/**< init state */
        SOCK_LISTEN				=0x14,		/**< listen state */
        SOCK_SYNSENT	   		=0x15,		/**< connection state */
        SOCK_SYNRECV		   	=0x16,		/**< connection state */
        SOCK_ESTABLISHED		=0x17,		/**< success to connect */
        SOCK_FIN_WAIT			=0x18,		/**< closing state */
        SOCK_CLOSING		   	=0x1A,		/**< closing state */
        SOCK_TIME_WAIT			=0x1B,		/**< closing state */
        SOCK_CLOSE_WAIT			=0x1C,		/**< closing state */
        SOCK_LAST_ACK			=0x1D,		/**< closing state */
        SOCK_UDP				 =  0x22,		/**< udp socket */
        SOCK_IPRAW			   =0x32,		/**< ip raw mode socket */
        SOCK_MACRAW			   =0x42	,	/**< mac raw mode socket */
        SOCK_PPPOE				=0x5F,		/**< pppoe socket */
    }
    public enum CR_val
    {
        Sn_CR_OPEN		=0x01,		/**< initialize or open socket */
        Sn_CR_LISTEN	=	0x02,		/**< wait connection request in tcp mode(Server mode) */
        Sn_CR_CONNECT	=0x04,		/**< send connection request in tcp mode(Client mode) */
        Sn_CR_DISCON	=	0x08,		/**< send closing reqeuset in tcp mode */
        Sn_CR_CLOSE		=0x10,		/**< close socket */
        Sn_CR_SEND		=0x20,		/**< updata txbuf pointer, send data */
        Sn_CR_SEND_MAC	=0x21,		/**< send data with MAC address, so without ARP process */
        Sn_CR_SEND_KEEP	=0x22,		/**<  send keep alive message */
        Sn_CR_RECV		=0x40,		/**< update rxbuf pointer, recv data */
   }
    public enum Protocol
    {
        Sn_MR_CLOSE		=0x00,		/**< unused socket */
        Sn_MR_TCP		=0x01,		/**< TCP */
        Sn_MR_UDP		=0x02,		/**< UDP */
        Sn_MR_IPRAW 	=0x03,		/**< IP LAYER RAW SOCK */
        Sn_MR_MACRAW	=0x04,		/**< MAC LAYER RAW SOCK */
        Sn_MR_PPPOE		=0x05,		/**< PPPoE */
        Sn_MR_ND		=0x20,		/**< No Delayed Ack(TCP) flag */
        Sn_MR_MULTI		=0x80,		/**< support multicating */

    }

    class uSocket
    {
      
        static private bool[] Socket = new bool[4] { false, false, false, false };
        
        private byte _scoket_number = 100;
        static int local_port = 50000;

        /// <summary>
        /// This Socket function initialize the channel in perticular mode, and set the port and wait for W5100 done it.
        /// </summary>
        /// <param name="s">for socket number</param>
        /// <param name="protocol">for socket protocol</param>
        /// <param name="port">the source port for the socket</param>
        /// <param name="flag">the option for the socket</param>
        public uSocket(byte s, Protocol protocol, UInt16 port, byte flag)
        {
            
            if(Socket[s] == true)
                throw new Exception("Socket"+ s.ToString()+" is in use");
                        
            if ((protocol == Protocol.Sn_MR_TCP) || (protocol == Protocol.Sn_MR_UDP) || (protocol == Protocol.Sn_MR_IPRAW) || (protocol == Protocol.Sn_MR_MACRAW) )
	        {

                 _scoket_number = s;
                this.Close();//why do we close it?
                
		        W5100.IINCHIP_WRITE(s,W5100.SocketRegisters.MR,(byte)((byte)protocol | flag));
		        if (port != 0) 
                {
                    W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.PORT0, (byte)((port & 0xff00) >> 8));
                    W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.PORT0 + 1, (byte)(port & 0x00ff));
		        }
                else 
                {
			        local_port++; // if don't set the source port, set local_port number.
                    W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.PORT0, (byte)((local_port & 0xff00) >> 8));
                    W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.PORT0 + 1, (byte)(local_port & 0x00ff));
		        }
                W5100.IINCHIP_WRITE(s,W5100.SocketRegisters.CR, (byte)CR_val.Sn_CR_OPEN); // run sockinit Sn_CR

		        /* +20071122[chungs]:wait to process the command... */
                while (W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0) ;
		        /* ------- */
                _scoket_number = s;
                Socket[s] = true;

	        }
	        else
	        {
		        throw new Exception("Failed");
	        }

            Debug.Print("Sn_SR = " + W5100.IINCHIP_READ(s, W5100.SocketRegisters.SR).ToString() + " Protocol = " + W5100.IINCHIP_READ(s, W5100.SocketRegisters.MR).ToString());
        }
        /// <summary>
        /// This function close the socket
        /// </summary>
        public void Close()
        {
            byte s = _scoket_number;
            W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte)CR_val.Sn_CR_CLOSE);

	        /* +20071122[chungs]:wait to process the command... */
            while (W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0) ;
	        /* ------- */

	        /* +2008.01 [hwkim]: clear interrupt */	
	        //#ifdef __DEF_IINCHIP_INT__
              /* m2008.01 [bj] : all clear */
	          //     putISR(s, 0x00);
	        //#else
              /* m2008.01 [bj] : all clear */
            W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.IR, 0xFF);
	        //#endif
            _scoket_number = 100;
            Socket[s] = false;

        }


        
        /// <summary>
        /// This function established  the connection for the channel in passive (server) mode. This function waits for the request from the peer.
        /// </summary>
        public bool Listen()
        {
            byte s = _scoket_number;
	        
	        if (W5100.IINCHIP_READ(s,W5100.SocketRegisters.SR) == (byte)SR_val.SOCK_INIT)
	        {
                W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR, (byte)CR_val.Sn_CR_LISTEN);
		        /* +20071122[chungs]:wait to process the command... */
                while (W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0) ;
		        /* ------- */
		        return true;
	        }
	        else
	        {
                return false;
		        //new Exception("Fail[invalid ip,port]\r\n");
	        }
        }


        /// <summary>
        /// This function established  the connection for the channel in Active (client) mode. 
        /// This function waits for the untill the connection is established.
        /// </summary>
        /// <param name="addr"> Destination IP address</param>
        /// <param name="port"> Destination Port</param>
        public void Connect(byte[] addr, UInt16 port)
        {
	        byte s = _scoket_number;
	        if(
		        ((addr[0] == 0xFF) && (addr[1] == 0xFF) && (addr[2] == 0xFF) && (addr[3] == 0xFF)) ||
	 	        ((addr[0] == 0x00) && (addr[1] == 0x00) && (addr[2] == 0x00) && (addr[3] == 0x00)) ||
	 	        (port == 0x00) ) 
 	        {
 		       throw  new Exception("Fail[invalid ip,port]\r\n");
        
	        }
	        else
	        {
		        // set destination IP
                W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.DIPR0, addr[0]);
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DIPR0 + 1), addr[1]);
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DIPR0 + 2), addr[2]);
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DIPR0 + 3), addr[3]);
                W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.DPORT0, (byte)((port & 0xff00) >> 8));
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DPORT0 + 1), (byte)(port & 0x00ff));
                W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR, (byte)CR_val.Sn_CR_CONNECT);
              /* m2008.01 [bj] :  wait for completion */
		        while ( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0) ;
                
                while (W5100.IINCHIP_READ(s, W5100.SocketRegisters.SR)!=(byte)SR_val.SOCK_ESTABLISHED );
		        
		        
	        }
        }
        
        /// <summary>
        /// This function used for disconnect the socket
        /// </summary>
        public void Disconnect()
        {
            byte s = _scoket_number;
	        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte)CR_val.Sn_CR_DISCON);

	        /* +20071122[chungs]:wait to process the command... */
	        while( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0 );
	        /* ------- */
        }
     
        /// <summary>
        ///   This function used to send the data in TCP mode
        /// </summary>
        /// <param name="buf">a pointer to data</param>
        /// <param name="len">the data size to be send</param>
        /// <returns>returns 1 if succeeded</returns>
        public int Send(	      
	        byte [] buf,
	        int len
	        )
        {
        byte s = _scoket_number;
	        byte status=0;
	        int freesize=0;
            int ret;
        

           if (len > W5100.GetTxMAX(s))
               ret = W5100.GetTxMAX(s); // check size not to exceed MAX size.
           else 
               ret = len;

           // if freebuf is available, start.
	        do 
	        {
		        freesize = W5100.Get_TX_FSR(s);
		        status = W5100.IINCHIP_READ(s, W5100.SocketRegisters.SR);
		        if ((status != (byte)SR_val.SOCK_ESTABLISHED) && (status != (byte)SR_val.SOCK_CLOSE_WAIT))
		        {
			        ret = 0; 
			        break;
		        }
                Debug.Print("socket " + s.ToString() + "  freesize(" + freesize.ToString() + " empty or error");
        
	        } while (freesize < ret);

              // copy data
	        W5100.SendDataProc(s, buf, ret);
	        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte)CR_val.Sn_CR_SEND);

	        /* +20071122[chungs]:wait to process the command... */
	        while( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0);
	        /* ------- */

        /* +2008.01 bj */	
        //#ifdef __DEF_IINCHIP_INT__
	      //  while ( (getISR(s) & Sn_IR_SEND_OK) != Sn_IR_SEND_OK ) 
        //#else
	        while ( (W5100.IINCHIP_READ(s, W5100.SocketRegisters.IR) & (byte)IR_val.Sn_IR_SEND_OK) != (byte)IR_val.Sn_IR_SEND_OK ) 
        //#endif
	        {
		        /* m2008.01 [bj] : reduce code */
		        if ( W5100.IINCHIP_READ(s, W5100.SocketRegisters.SR) == (byte)SR_val.SOCK_CLOSED )
		        {
        
			        Debug.Print("SOCK_CLOSED.\r\n");
        
			        Close();
			        return 0;
		        }
  	        }
        /* +2008.01 bj */	
        //#ifdef __DEF_IINCHIP_INT__
  	      //  putISR(s, getISR(s) & (~Sn_IR_SEND_OK));
        //#else
	        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.IR, (byte)IR_val.Sn_IR_SEND_OK);
        //#endif
  	        return ret;
        }



        public int AvilableBytes
        {
            get
            {
                return W5100.getSn_RX_RSR(_scoket_number);
            }
        }
        
           
        

        /// <summary>
        ///   This function is an application I/F function which is used to receive the data in TCP mode.
        ///   It continues to wait for data as much as the application wants to receive.
        /// </summary>
        /// <param name="buf">a pointer to copy the data to be received</param>
        /// <param name="len">the data size to be read</param>
        /// <returns>received data size for success else -1.</returns>
        public int recv(
	        byte [] buf,
	        int len
	        )
        {
            byte s = _scoket_number;
            int ret=0;
	        

	        if ( len > 0 )
	        {
		        W5100.RecvDataProc(s, buf, len);
		        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte)CR_val.Sn_CR_RECV);

		        /* +20071122[chungs]:wait to process the command... */
		        while( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) !=0) ;
		        /* ------- */
		        ret = len;
	        }
	        return ret;
        }


        /// <summary>
        /// This function is an application I/F function which is used to send the data for other then TCP mode. 
        /// Unlike TCP transmission, The peer's destination address and the port is needed.
        /// </summary>
        /// <param name="buf">a pointer to the data</param>
        /// <param name="len">the data size to send</param>
        /// <param name="addr">the peer's Destination IP address </param>
        /// <param name="port">the peer's destination port number </param>
        /// <returns>This function return send data size for success else -1</returns>
        public int Sendto(
	        byte [] buf,
	        int len,
	        byte[] addr,
	        UInt16 port
	        )
        {
        //	uint8 status=0;
        //	uint8 isr=0;
            byte s = _scoket_number;
	        int ret=0;
        	
           if (len > W5100.GetTxMAX(s)) ret = W5100.GetTxMAX(s); // check size not to exceed MAX size.
           else ret = len;

	        if
		        (
		 	        ((addr[0] == 0x00) && (addr[1] == 0x00) && (addr[2] == 0x00) && (addr[3] == 0x00)) ||
		 	        ((port == 0x00)) ||(len == 0)
		        ) 
 	        {
 	           /* +2008.01 [bj] : added return value */
 	      
        //#ifdef __DEF_IINCHIP_DBG__
	      //  printf("%d Fail[%.2x.%.2x.%.2x.%.2x, %.d, %d]\r\n",s, addr[0], addr[1], addr[2], addr[3] , port, len);
	       throw new Exception("Fail[invalid ip,port]\r\n");
        //#endif
                
	        }
	        else
	        {
		        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.DIPR0,addr[0]);
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DIPR0 + 1), addr[1]);
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DIPR0 + 2), addr[2]);
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DIPR0 + 3), addr[3]);
                W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.DPORT0, (byte)((port & 0xff00) >> 8));
                W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.DPORT0 + 1), (byte)(port & 0x00ff));

  		        // copy data
  		        W5100.SendDataProc(s, buf, ret);
		        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte)CR_val.Sn_CR_SEND);

		        /* +20071122[chungs]:wait to process the command... */
		        while( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0) ;
		        /* ------- */
        		
               while ( (W5100.IINCHIP_READ(s, W5100.SocketRegisters.IR) & (byte)IR_val.Sn_IR_SEND_OK) != (byte)IR_val.Sn_IR_SEND_OK ) 
               {
                  if ((W5100.IINCHIP_READ(s, W5100.SocketRegisters.IR) & (byte)IR_val.Sn_IR_TIMEOUT) != 0)
        //#endif
			        {
        //#ifdef __DEF_IINCHIP_DBG__
				        Debug.Print("send fail.\r\n");
        //#endif
        /* +2008.01 [bj]: clear interrupt */
        //#ifdef __DEF_IINCHIP_INT__
         //	        putISR(s, getISR(s) & ~(Sn_IR_SEND_OK | Sn_IR_TIMEOUT));  /* clear SEND_OK & TIMEOUT */
        //#else
         	        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.IR, (byte)(IR_val.Sn_IR_SEND_OK | IR_val.Sn_IR_TIMEOUT)); /* clear SEND_OK & TIMEOUT */
        //#endif
			        return 0;
			        }
		        }

        /* +2008.01 bj */	
        //#ifdef __DEF_IINCHIP_INT__
     	  //      putISR(s, getISR(s) & (~Sn_IR_SEND_OK));
        //#else
	           W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.IR, (byte) IR_val.Sn_IR_SEND_OK);
        //#endif

	        }
	        return ret;
        }


        private byte[] head = new byte[8];
        /// <summary>
        /// This function is an application I/F function which is used to receive the data in other then
        /// TCP mode. This function is used to receive UDP, IP_RAW and MAC_RAW mode, and handle the header as well. 
        /// </summary>
        /// <param name="buf">a pointer to the copy data to be received</param>
        /// <param name="len">the data size to read</param>
        /// <param name="addr">a pointer to store the peer's IP address</param>
        /// <param name="port">a pointer to store the peer's port number.</param>
        /// <returns>received data size for success else -1</returns>
        public int recvfrom(
	        byte [] buf, 
	        int len, 
	        byte [] addr,
	        UInt16 []port
	        )
        {
            byte s = _scoket_number;
	        
	        int data_len=0;
	        int ptr=0;
        
	        if ( len > 0 )
	        {
   	        ptr = W5100.IINCHIP_READ(s, W5100.SocketRegisters.RX_RD0);
   	        ptr = ((ptr & 0x00ff) << 8) + W5100.IINCHIP_READ(s, W5100.SocketRegisters.RX_RD0 + 1);
        //#ifdef __DEF_IINCHIP_DBG__
   	        //Debug.Print("ISR_RX: rd_ptr : %.4x\r\n", ptr);
        //#endif
   	        switch (W5100.IINCHIP_READ(s, W5100.SocketRegisters.MR) & 0x07)
   	        {
   	        case (int)Protocol.Sn_MR_UDP :
   			        W5100.ReadData(s, ptr, head, 0x08);
   			        ptr += 8;
   			        // read peer's IP address, port number.
    			        addr[0] = head[0];
   			        addr[1] = head[1];
   			        addr[2] = head[2];
   			        addr[3] = head[3];
   			        port[0] = head[4];
   			        port[0] = (UInt16)((port[0] << 8) + head[5]);
   			        data_len = head[6];
   			        data_len = (data_len << 8) + head[7];
           			
        	        Debug.Print("UDP msg arrived\r\n");
   		
                    W5100.ReadData(s, ptr, buf, data_len); // data copy.
			        ptr += data_len;

			        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.RX_RD0,(byte)((ptr & 0xff00) >> 8));
			        W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.RX_RD0 + 1),(byte)(ptr & 0x00ff));
   			        break;
           
   	        case (int)Protocol.Sn_MR_IPRAW :
   			        W5100.ReadData(s, ptr, head, 0x06);
   			        ptr += 6;
           
   			        addr[0] = head[0];
   			        addr[1] = head[1];
   			        addr[2] = head[2];
   			        addr[3] = head[3];
   			        data_len = head[4];
   			        data_len = (data_len << 8) + head[5];
           	
        	        Debug.Print("IP RAW msg arrived\r\n");
   			        W5100.ReadData(s, ptr, buf, data_len); // data copy.
			        ptr += data_len;

			        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.RX_RD0,(byte)((ptr & 0xff00) >> 8));
			        W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.RX_RD0 + 1),(byte)(ptr & 0x00ff));
   			        break;
   	        case (int)Protocol.Sn_MR_MACRAW :
   			        W5100.ReadData(s,ptr,head,2);
   			        ptr+=2;
   			        data_len = head[0];
   			        data_len = (data_len<<8) + head[1] - 2;

   			        W5100.ReadData(s, ptr,buf,data_len);
   			        ptr += data_len;
   			        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.RX_RD0,(byte)((ptr & 0xff00) >> 8));
   			        W5100.IINCHIP_WRITE(s, (W5100.SocketRegisters.RX_RD0 + 1),(byte)(ptr & 0x00ff));
           			
        	        Debug.Print("MAC RAW msg arrived\r\n");
			        //printf("dest mac=%.2X.%.2X.%.2X.%.2X.%.2X.%.2X\r\n",buf[0],buf[1],buf[2],buf[3],buf[4],buf[5]);
			        //printf("src  mac=%.2X.%.2X.%.2X.%.2X.%.2X.%.2X\r\n",buf[6],buf[7],buf[8],buf[9],buf[10],buf[11]);
			        //printf("type    =%.2X%.2X\r\n",buf[12],buf[13]); 
        	        break;

   	        default :
   			        break;
   	        }
		        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte)CR_val.Sn_CR_RECV);

		        /* +20071122[chungs]:wait to process the command... */
		        while( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) != 0) ;
		        /* ------- */
	        }
        
 	        return data_len;
        }


        public int SendIGMP(byte[] buf, int len)
        {
            byte s = _scoket_number;
	        byte status=0;
        //	uint8 isr=0;
	        int ret=0;
        	
           if (len > W5100.GetTxMAX(s)) ret = W5100.GetTxMAX(s); // check size not to exceed MAX size.
           else ret = len;

	        if	(ret != 0) 
 	        {
		        // copy data
		        W5100.SendDataProc(s, buf, ret);
		        W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.CR,(byte) CR_val.Sn_CR_SEND);
        /* +2008.01 bj */	
		        while( W5100.IINCHIP_READ(s, W5100.SocketRegisters.CR) !=0)  ;
        /* ------- */
        		
               while ( (W5100.IINCHIP_READ(s, W5100.SocketRegisters.IR) & (byte)IR_val.Sn_IR_SEND_OK) != (byte)IR_val.Sn_IR_SEND_OK ) 
                {
			        status = W5100.IINCHIP_READ(s, W5100.SocketRegisters.SR);
                  if ((W5100.IINCHIP_READ(s, W5100.SocketRegisters.IR) & (byte)IR_val.Sn_IR_TIMEOUT) != 0)
        	        {
        		        Debug.Print("igmpsend fail.\r\n");
        	           Close();
					   
				        return 0;
			        }
		        }

               W5100.IINCHIP_WRITE(s, W5100.SocketRegisters.IR, (byte)IR_val.Sn_IR_SEND_OK);
           }
	        return ret;
        }


    }
}
