package DwgProps;
import java.io.*;

public class getdwgprops {
	private static String propname[]= {"title","argument","author","keywords","comments","lastuser","unknown","hyperlink"};
	
	// Read a single byte
	private static byte readbyte(DataInputStream f) {
		try {
			return f.readByte();
		} catch(IOException e) {
			return -1;			
		}
	}
	
	// Read 'count' bytes
	private static byte[] readbytes(DataInputStream f,int count) {
		byte[] buf=new byte[count];
		for(int i=0;i<count;i++) buf[i]=readbyte(f);
		return buf;
	}
	
	// Read a string of 'count' bytes
	private static String readstring(DataInputStream f,int count) {
		return new String(readbytes(f,count));
	}
	
	// Read a wide char string of 'n' wide chars
	private static String readstringW(DataInputStream f,int count) {
		char[] buf=new char[count];
		for(int i=0;i<count;i++) {
			buf[i]=(char)readshort(f);
		}
		return new String(buf);
	}

	// read a short integer (big endian)
	private static short readshort(DataInputStream f) {
		try {
			return (short)(((int)f.readByte()&0xff)+0x100*((int)f.readByte()&0xff));
		} catch (IOException e) {
			return -1;
		}
	}
	
	// 4 bytes to long integer (big endian)
	private static long bytes2long(byte b0,byte b1,byte b2,byte b3) {
		int lowshort = (((int)b0&0xff)+0x100*((int)b1&0xff));
		int highshort =(((int)b2&0xff)+0x100*((int)b3&0xff));
		return ((long)lowshort&0xffff)+(((long)highshort&0xffff)<<16);
	}
	
	// read a variable length string 
	private static String readllstring(DataInputStream f){
		int len=readshort(f);
		return len>0?readstring(f, len):"";
	}
	
	//	 read a variable length string (wide char)
	private static String readllstringW(DataInputStream f){
		int len=readshort(f);
		return len>0?readstringW(f,len):"";
	}

	// get properties (acad 2004)
	private static void get2004props(DataInputStream f,String[] propArray) {
		for(int i=0;i<propArray.length;i++) {
			String tmp=readllstring(f);
			propArray[i]=tmp.substring(0,tmp.length()-1);
		}
	}

	// get properties (acad 2007 - acad2010)
	private static void get2007or2010props(DataInputStream f,String[] propArray) {		
		for(int i=0;i<propArray.length;i++) {
			String tmp=readllstringW(f);
			propArray[i]=tmp.substring(0,tmp.length()-1);
		}		
	}
	private static void skipToPropertyInfoSection(DataInputStream f, byte[] header) {
		// Property info area pointer (long integer) is @offset 0x20 (big endian)
		long propertyInfoPointer=bytes2long(header[0x20],header[0x21],header[0x22],header[0x23]);
		
		// skip to find propertyInfo section start (propertyInfoPointer less current position)
		long bytesToSkip=propertyInfoPointer - 0x80;
		while(bytesToSkip>0x7fff) {
			readbytes(f,0x7fff);
			bytesToSkip-=0x7fff;
		}
		readbytes(f,(int)bytesToSkip);
	}
	
	//get file properties (any version)
	public static void getprops(DataInputStream f) {
		String props[]=new String[propname.length];
		String version;
		byte[] header=new byte[128];
		header=readbytes(f,128);
		
		// Version string @offset 0
		version=new String(header).substring(0,6);
		
		if (version.equals("AC1018")) {
			skipToPropertyInfoSection(f, header);
			get2004props(f,props);
		} else if (version.equals("AC1021") || version.equals("AC1024")) {
			skipToPropertyInfoSection(f, header);
			get2007or2010props(f,props);
		}		
		// Output props
		System.out.println("<?xml version='1.0'?>");
		System.out.println("<dwgprops>");
		for(int i=0;i<props.length;i++) {
			if(props[i]!=null) {
				System.out.print("\t<" + propname[i] + ">");
				System.out.print(props[i]);
				System.out.println("</" + propname[i] + ">");
			}
		}
		System.out.println("</dwgprops>");
	}
	/**
	 * @param args
	 */
	public static void main(String[] args) {
		if(args.length!=1) {
			System.err.println("Usage: getdwgprops file_name");
			System.exit(-1);
		}
		FileInputStream in = null;
		try {
            in = new FileInputStream(args[0]);
            DataInputStream f=new DataInputStream(in);            
            getprops(f);
            in.close();    
        }
		catch(IOException e) {
			System.err.println("Cannot open file:" + e.getMessage());
			System.exit(-2);
		}
	}

}
