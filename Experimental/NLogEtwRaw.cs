//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Experimental
//{
//    class NLogEtwRaw
//    {
//        [StructLayout(LayoutKind.Sequential)]
//        static class ManifestDescriptor
//        {
//            public static readonly ushort Id = 0xFFFE;
//            public static readonly byte Version = 1;
//            public static readonly byte Channel = 0;
//            public static readonly byte Level = 0;
//            public static readonly byte Opcode = 0xFE;
//            public static readonly ushort Task = 0xFFFE;
//            public static readonly long Keywords = 0x00ffFFFFffffFFFF;
//        }


//        // Send out the ETW manifest XML out to ETW

//        bool sendManifest(byte[] rawManifest)
//        {

//            var d = new ManifestDescriptor();
//            d.Id = 0xFFFE;

//            // we don't want the manifest to show up in the event log channels so we specify as keywords 
//            // everything but the first 8 bits (reserved for the 8 channels)
//            var manifestDescr = new EventDescriptor(0xFFFE, 1, 0, 0, 0xFE, 0xFFFE, 0x00ffFFFFffffFFFF);
//            ManifestEnvelope envelope = new ManifestEnvelope();

//            envelope.Format = ManifestEnvelope.ManifestFormats.SimpleXmlFormat;
//            envelope.MajorVersion = 1;
//            envelope.MinorVersion = 0;
//            envelope.Magic = 0x5B;              // An unusual number that can be checked for consistency. 
//            int dataLeft = rawManifest.Length;
//            envelope.ChunkNumber = 0;

//            EventProvider.EventData* dataDescrs = stackalloc EventProvider.EventData[2];
//            dataDescrs[0].Ptr = (ulong)&envelope;
//            dataDescrs[0].Size = (uint)sizeof(ManifestEnvelope);
//            dataDescrs[0].Reserved = 0;

//            dataDescrs[1].Ptr = (ulong)dataPtr;
//            dataDescrs[1].Reserved = 0;

//            int chunkSize = ManifestEnvelope.MaxChunkSize;
//            TRY_AGAIN_WITH_SMALLER_CHUNK_SIZE:
//            envelope.TotalChunks = (ushort)((dataLeft + (chunkSize - 1)) / chunkSize);
//            while (dataLeft > 0)
//            {
//                dataDescrs[1].Size = (uint)Math.Min(dataLeft, chunkSize);
//                if (m_provider != null)
//                {
//                    if (!m_provider.WriteEvent(ref manifestDescr, null, null, 2, (IntPtr)dataDescrs))
//                    {
//                        // Turns out that if users set the BufferSize to something less than 64K then WriteEvent
//                        // can fail.   If we get this failure on the first chunk try again with something smaller
//                        // The smallest BufferSize is 1K so if we get to 256 (to account for envelope overhead), we can give up making it smaller. 
//                        if (EventProvider.GetLastWriteEventError() == EventProvider.WriteEventErrorCode.EventTooBig)
//                        {
//                            if (envelope.ChunkNumber == 0 && chunkSize > 256)
//                            {
//                                chunkSize = chunkSize / 2;
//                                goto TRY_AGAIN_WITH_SMALLER_CHUNK_SIZE;
//                            }
//                        }
//                        success = false;
//                        if (ThrowOnEventWriteErrors)
//                            ThrowEventSourceException();
//                        break;
//                    }
//                }
//                dataLeft -= chunkSize;
//                dataDescrs[1].Ptr += (uint)chunkSize;
//                envelope.ChunkNumber++;
//            }
//        }
//#endif
//            return success;
//        }



//}


//}

//    }
//}
