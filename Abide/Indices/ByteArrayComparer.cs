using System.Collections.Generic;

namespace Abide
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            for (int i = 0; i < x.Length; i++) if (x[i] != y[i]) return false;
            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            unsafe
            {
                fixed (byte* src = obj)
                {
#if WIN32
                    int hash1 = (5381<<16) + 5381;
#else
                    int hash1 = 5381;
#endif
                    int hash2 = hash1;

#if WIN32
    // 32 bit machines.
                    int* pint = (int *)src;
                    int len = this.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len  -= 4;
                    }
 
                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }
#else
                    int c;
                    int i = 0;
                    byte* s = src;
                    while (i++ < obj.Length && (c = s[0]) != 0)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        c = s[1];
                        if (c == 0)
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                        s += 2;
                    }
#endif
                    return hash1 + hash2*1566083941;
                }
            }
        }
    }
}