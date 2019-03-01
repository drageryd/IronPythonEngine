using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace CustomStream
{
    public class StreamStdio : Stream
    {
        private string alreadyConverted;
        private readonly List<int> list;

        //Inherited members
        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return true; } }
        public override long Length { get { return list.Count; } }
        public override long Position { get; set; }

        public StreamStdio()
        {
            Position = 0;
            alreadyConverted = "";
            list = new List<int>();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //Wait here for bytes
            if (list.Count == 0) return 0;

            //How many bytes to read
            int length = (list.Count > count) ? count : list.Count;

            //Peek list if EOF
            if (list[0] == -1) return -1;

            //Peek for non printables
            if (list[0] < 32 || list[0] > 126)
            {
                buffer[0] = (byte)list[0];
                list.RemoveAt(0);
                return 1;
            }

            //Fill buffer with chars from text
                int i;
            for (i = 0; i < length; i++)
            {
                //When next is not printable
                if (list[i] < 32 || list[i] > 126) break;
                buffer[i] = (byte)list[i];
            }
            //Remove read bytes from text
            list.RemoveRange(0, i);
            //Return number of read bytes
            return i;
        }

        public override void Flush()
        {
            //Debug.Log("Someone called Flush");
        }

        public override void SetLength(long value)
        {
            //Debug.Log("Someone called SetLength");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //Fill data list with byte values
            for (int i = offset; i < count; i++)
            {
                list.Add((int)buffer[i]);
            }
        }

        public void WriteEnd()
        {
            list.Add(-1);
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            Debug.Log("Someone called Seek");
            return 123;
        }

        public string GetText()
        {
            //return text;
            string ret = alreadyConverted;
            for (int i = ret.Length; i < list.Count; i++)
            {
                //Check if not printable
                if (list[i] >= 0 && list[i] <= 255) 
                {
                    ret += (char)list[i];
                }
                else 
                {
                    break;
                }
            }
            alreadyConverted = ret;
            return ret;
        }
    }
}
