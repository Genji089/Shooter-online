import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.List;
import java.util.Queue;
import java.util.LinkedList;
import java.net.SocketTimeoutException;


/*
	服务端
*/
public class battleServer {
	private static String emptyMsg = "{0:{}}";
	private static String playerCtrlMsg = "";
	private static String[] playerCtrlMsgStrings = new String[2];
	private static boolean isReciveMsg = false;
	private static boolean[] isReadys = new boolean[2];
	private static boolean isBegin = false;
	private static boolean[] isReciveMsgs = new boolean[2];
	private static Queue<String> playerCtrlMsgQueue = new LinkedList<String>();
	private static long frameNum = 0;

	public static void main(String[] args) throws IOException {
		
		playerCtrlMsgStrings[0] = "";
		playerCtrlMsgStrings[1] = "";
		
		System.out.println("服务器启动了...");
		// 创建socket对象，监听指定端口
		ServerSocket ss = new ServerSocket(80);
		// 监听客户端的连接，accept方法返回连接客户端的socket对象
		
		Socket socket1 = ss.accept();
		System.out.println("first client connected.");
		Socket socket2 = ss.accept(); // 阻塞方法，等待客户端的连接
		System.out.println("second client connected.");
		
		//开启线程接收数据
		listenToClient(socket1, 1);
		listenToClient(socket2, 2);
		
		//开启线程发送数据
		sendToClient(socket1, socket2);
		//sendToOneClient(socket1);
		
		while(true) {
			//-------->这里搞不懂，不加这个延时，下面if一直进不去。明明已经ready了。update:好像是由于这个打印。才能更新if中那个isReady的状态？？<----------------
			long startTime = System.currentTimeMillis();
			while(System.currentTimeMillis() - startTime < 1000) {
			}
			System.out.println(isReadys[0]);
			System.out.println(isReadys[1]);
			if(isReadys[0] && isReadys[1]) {
				sendReadyGo(socket1, socket2);
				System.out.println("go!!");
				break;
			}
		}
		
		//判断一个客户端是否断开
		while (true) {
			if(socket1.isClosed()) {
				long startTime = System.currentTimeMillis();
				while(System.currentTimeMillis() - startTime < 5000) {
				}
				break;
			}
		}
		
		//判断是否断开,断开后，关闭服务器
//		while (true) {
//			if(socket1.isClosed()&&socket2.isClosed()) {
//				long startTime = System.currentTimeMillis();
//				while(System.currentTimeMillis() - startTime < 5000) {
//				}
//				break;
//			}
//		}
		System.out.println("关闭服务器");
		ss.close();
	}
	
	public static int byteToInt(byte[] b) { 
        int s = 0; 
        int s0 = b[0] & 0xff;// 最低位 
        int s1 = b[1] & 0xff; 
        int s2 = b[2] & 0xff; 
        int s3 = b[3] & 0xff; 
        s3 <<= 24; 
        s2 <<= 16; 
        s1 <<= 8; 
        s = s0 | s1 | s2 | s3; 
        return s; 
    }
	
	public static byte[] intToByte(int number) {  
		int temp = number;  
		byte[] b = new byte[4];  
		for (int i = 0; i < b.length; i++) {  
			b[i] = new Integer(temp & 0xff).byteValue();// 将最低位保存在最低位
			temp = temp >> 8;// 向右移8位  
		}
		return b;  
	}
	
//	public synchronized string getPlayerCtrlMsg() {
//		return playerCtrlMsg;
//	}
//	
//	public synchronized void setPlayerCtrlMsg() {
//		
//	}
	
	public synchronized static void sendClient(Socket socket) {
		try {
			if(isReciveMsgs[0] && isReciveMsgs[1]) {
				OutputStream outputStream = socket.getOutputStream();
				outputStream.write(playerCtrlMsg.getBytes());
				isReciveMsgs[0] = false;
				isReciveMsgs[1] = false;
				playerCtrlMsg = "";
			}
			
		} catch (IOException e) {
			e.printStackTrace();
			// TODO: handle exception
		}
	}
	
	public synchronized static void sendTwoClient(Socket socket1,Socket socket2) {
		try {
			String str = playerCtrlMsgQueue.poll();
			if(str != null) {
				OutputStream outputStream1 = socket1.getOutputStream();
				outputStream1.write(str.getBytes());
				OutputStream outputStream2 = socket2.getOutputStream();
				outputStream2.write(str.getBytes());
			}
			
		} catch (IOException e) {
			e.printStackTrace();
			// TODO: handle exception
		}
	}
	
	public static void sendReadyGo(Socket socket1,Socket socket2) {
		try {
			OutputStream outputStream1 = socket1.getOutputStream();
			outputStream1.write("1".getBytes());
			OutputStream outputStream2 = socket2.getOutputStream();
			outputStream2.write("2".getBytes());
			System.out.println("send go");
		} catch (IOException e) {
			e.printStackTrace();
			// TODO: handle exception
		}
	}
	
	public synchronized static void setPlayerCtrlMsg(String str, int i) {
		playerCtrlMsgStrings[i] = str;
		isReciveMsgs[i] = true;
		if(isReciveMsgs[0]&&isReciveMsgs[1]) {
			if(frameNum == 0) {
				long startTime = System.currentTimeMillis();
				while(System.currentTimeMillis() - startTime < 20) {
				}
			}
			playerCtrlMsg = playerCtrlMsgStrings[0] + playerCtrlMsgStrings[1];
			playerCtrlMsgQueue.offer(playerCtrlMsg);
			frameNum++;
			isReciveMsgs[0] = false;
			isReciveMsgs[1] = false;
			playerCtrlMsg = "";
			System.out.println("frameNum now : "+frameNum);
		}
	}
	
	public static void listenToClient(Socket socket,int socketNum) {
		Thread thread = new Thread(new Runnable() {
			@Override
			public void run() {
				// TODO Auto-generated method stub
				try {
					String str = "";
					boolean isDiscard = false;
					// 读取客户端发送过来的数据
					while (!socket.isClosed()) {
						//接收ready消息，和第一帧的操作
						if(!isReadys[socketNum - 1] || frameNum == 0) {
							InputStream in = socket.getInputStream(); 
							byte[] buf = new byte[1024];
							int bufLen = buf.length;
							int len = in.read(buf); // 一次性最多读取1024个字节数据，该方法会返回实际读取到的数据长度
							str = new String(buf, 0, len);
							if(str.equals("ready")) {
								isReadys[socketNum - 1] = true;
								System.out.println("client "+socketNum+" ready.");
							}else {
								setPlayerCtrlMsg(str, socketNum - 1);
							}
						}
						//第一帧之后的操作，固定时间间隔接收
						
						if(frameNum != 0) {
							long startTime = System.currentTimeMillis();
							try {
								
								socket.setSoTimeout(60);//60ms超时值
								InputStream in = socket.getInputStream(); 
								byte[] buf = new byte[1024];
								int bufLen = buf.length;
								int len = in.read(buf); 
								str = new String(buf, 0, len);
								if(isDiscard) {
									isDiscard = false;
									len = in.read(buf);
									str = new String(buf, 0, len);
								}
								while(System.currentTimeMillis() - startTime < 60) {
								}
								//System.out.println("the chazhi time:"+(endTime - startTime));
							}catch (SocketTimeoutException e) {
								System.out.print("client "+ socketNum +" is late!");
								isDiscard = true;
								//System.out.println("the chazhi time:"+(endTime - startTime));
								str = emptyMsg;
								// TODO: handle exception
							}
							setPlayerCtrlMsg(str, socketNum - 1);
						}
					}
				} catch (Exception e) {
					e.printStackTrace();
					// TODO: handle exception
				}
			}
		});
		thread.start();
	}
	
	public static void sendToClient(Socket socket1, Socket socket2) {
		Thread thread = new Thread(new Runnable() {
			@Override
			public void run() {
				// TODO Auto-generated method stub
				try {
					while(!socket1.isClosed() && !socket2.isClosed()) {
						
						sendTwoClient(socket1, socket2);
//						long startTime = System.currentTimeMillis();
//						while(System.currentTimeMillis() - startTime < 3000) {
//						}
					}
				} catch (Exception e) {
					e.printStackTrace();
					// TODO: handle exception
				}
			}
		});
		thread.start();
	}
	
	public static void sendToOneClient(Socket socket) {
		Thread thread = new Thread(new Runnable() {
			@Override
			public void run() {
				// TODO Auto-generated method stub
				try {
					while(!socket.isClosed()) {
						sendClient(socket);
//						long startTime = System.currentTimeMillis();
//						while(System.currentTimeMillis() - startTime < 3000) {
//						}
					}
				} catch (Exception e) {
					e.printStackTrace();
					// TODO: handle exception
				}
			}
		});
		thread.start();
	}
	
//	public static Boolean isServerClose(Socket socket){ 
//		try{ 
//			socket.sendUrgentData(0xFF);//发送1个字节的紧急数据，默认情况下，服务器端没有开启紧急数据处理，不影响正常通信 
//		    return false; 
//		}catch(Exception se){ 
//		    return true; 
//		} 
//	}
}
