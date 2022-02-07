using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

class RaceScenarioJsonParser
{
    private string[] eventType = { "SCORE", "NOUSE_1", "NOUSE_2", "SKILL" };
    public JObject ParseFromBinary(byte[] bin)
    {
        JObject RaceScenario = new JObject();
        using (BinaryReader rawracedata = new BinaryReader(new MemoryStream(bin)))
        {           
            JObject header = new JObject();
            header.Add("maxLength", rawracedata.ReadInt32());
            header.Add("version", rawracedata.ReadInt32());
            RaceScenario.Add("header", header); //header
            RaceScenario.Add("distanceDiffMax", rawracedata.ReadSingle());
            RaceScenario.Add("horseNum", rawracedata.ReadInt32());
            RaceScenario.Add("horseFrameSize", rawracedata.ReadInt32());
            RaceScenario.Add("horseResultSize", rawracedata.ReadInt32());
            RaceScenario.Add("PaddingSize1", rawracedata.ReadInt32());
            RaceScenario.Add("frameCount", rawracedata.ReadInt32());
            RaceScenario.Add("frameSize", rawracedata.ReadInt32());
            JArray frameArray = new JArray();
            for (int i = 0; i < RaceScenario["frameCount"].ToObject<int>(); i++)
            {
                JObject frame = new JObject();
                JArray horseframeArray = new JArray();
                frame.Add("time", rawracedata.ReadSingle());
                for (int k = 0; k < RaceScenario["horseNum"].ToObject<int>(); k++)
                {
                    JObject horseFrame = new JObject();
                    horseFrame.Add("distance", rawracedata.ReadSingle());
                    horseFrame.Add("lanePosition", rawracedata.ReadUInt16());
                    horseFrame.Add("speed", rawracedata.ReadUInt16());
                    horseFrame.Add("hp", rawracedata.ReadUInt16());
                    horseFrame.Add("temptationMode", rawracedata.ReadSByte());
                    horseFrame.Add("blockFrontHorseIndex", rawracedata.ReadSByte());
                    horseframeArray.Add(horseFrame);
                }
                frame.Add("horseFrame", horseframeArray);
                frameArray.Add(frame);
            }
            RaceScenario.Add("frame", frameArray);
            RaceScenario.Add("PaddingSize2", rawracedata.ReadInt32());
            JArray horseResultArray = new JArray();
            for (int i = 0; i < RaceScenario["horseNum"].ToObject<int>(); i++)
            {
                JObject horseResult = new JObject();
                horseResult.Add("finishOrder", rawracedata.ReadInt32());
                horseResult.Add("finishTime", rawracedata.ReadSingle());
                horseResult.Add("finishDiffTime", rawracedata.ReadSingle());
                horseResult.Add("startDelayTime", rawracedata.ReadSingle());
                horseResult.Add("gutsOrder", rawracedata.ReadByte());
                horseResult.Add("wizOrder", rawracedata.ReadByte());
                horseResult.Add("lastSpurtStartDistance", rawracedata.ReadSingle());
                horseResult.Add("runningStyle", rawracedata.ReadByte());
                horseResult.Add("defeat", rawracedata.ReadInt32());
                horseResult.Add("finishTimeRaw", rawracedata.ReadSingle());
                horseResultArray.Add(horseResult);
            }
            RaceScenario.Add("horseResult", horseResultArray);
            RaceScenario.Add("PaddingSize3", rawracedata.ReadInt32());
            RaceScenario.Add("eventCount", rawracedata.ReadInt32());
            JArray eventArray = new JArray();
            for (int i = 0; i < RaceScenario["eventCount"].ToObject<int>(); i++)
            {
                JObject eventdata = new JObject();
                JObject subevent = new JObject();
                JArray paramArray = new JArray();
                eventdata.Add("eventSize", rawracedata.ReadInt16());
                subevent.Add("frameTime", rawracedata.ReadSingle());
                subevent.Add("type", eventType[rawracedata.ReadByte()]);
                subevent.Add("paramCount", rawracedata.ReadByte());
                for (int k = 0; k < subevent["paramCount"].ToObject<int>(); k++)
                {
                    paramArray.Add(rawracedata.ReadInt32());
                }
                subevent.Add("param", paramArray);
                eventdata.Add("event", subevent);
                eventArray.Add(eventdata);
            }
            RaceScenario.Add("event", eventArray);
        }
        return RaceScenario;
    }
    public byte[] WriteToBinary(JObject RaceScenario)
    {
        List<byte> bin = new List<byte>();
        bin.AddRange(BitConverter.GetBytes(RaceScenario["header"]["maxLength"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["header"]["version"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["distanceDiffMax"].ToObject<Single>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["horseNum"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["horseFrameSize"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["horseResultSize"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["PaddingSize1"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["frameCount"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes(RaceScenario["frameSize"].ToObject<Int32>()));

        foreach (var frame in RaceScenario["frame"])
        {
            bin.AddRange(BitConverter.GetBytes(frame["time"].ToObject<Single>()));
            foreach (var horseFrame in frame["horseFrame"])
            {
                bin.AddRange(BitConverter.GetBytes(horseFrame["distance"].ToObject<Single>()));
                bin.AddRange(BitConverter.GetBytes(horseFrame["lanePosition"].ToObject<UInt16>()));
                bin.AddRange(BitConverter.GetBytes(horseFrame["speed"].ToObject<UInt16>()));
                bin.AddRange(BitConverter.GetBytes(horseFrame["hp"].ToObject<UInt16>()));
                bin.Add((byte)horseFrame["temptationMode"].ToObject<sbyte>());
                bin.Add((byte)horseFrame["blockFrontHorseIndex"].ToObject<sbyte>());
            }
        }
        bin.AddRange(BitConverter.GetBytes(RaceScenario["PaddingSize2"].ToObject<Int32>()));
        foreach (var horseResult in RaceScenario["horseResult"])
        {
            bin.AddRange(BitConverter.GetBytes(horseResult["finishOrder"].ToObject<Int32>()));
            bin.AddRange(BitConverter.GetBytes(horseResult["finishTime"].ToObject<Single>()));
            bin.AddRange(BitConverter.GetBytes(horseResult["finishDiffTime"].ToObject<Single>()));
            bin.AddRange(BitConverter.GetBytes(horseResult["startDelayTime"].ToObject<Single>()));
            bin.Add(horseResult["gutsOrder"].ToObject<byte>());
            bin.Add(horseResult["wizOrder"].ToObject<byte>());
            bin.AddRange(BitConverter.GetBytes(horseResult["lastSpurtStartDistance"].ToObject<Single>()));
            bin.Add(horseResult["runningStyle"].ToObject<byte>());
            bin.AddRange(BitConverter.GetBytes(horseResult["defeat"].ToObject<Int32>()));
            bin.AddRange(BitConverter.GetBytes(horseResult["finishTimeRaw"].ToObject<Single>()));
        }
        bin.AddRange(BitConverter.GetBytes(RaceScenario["PaddingSize3"].ToObject<Int32>()));
        bin.AddRange(BitConverter.GetBytes((Int32)RaceScenario["event"].Count())); //auto calculate event count
        foreach (var eventdata in RaceScenario["event"])
        {
            int event_size = 4 + 1 + 1 + (4 * eventdata["event"]["param"].Count()); //float + byte + byte + (int32 * param count)
            bin.AddRange(BitConverter.GetBytes((Int16)event_size)); //eventSize



            bin.AddRange(BitConverter.GetBytes(eventdata["event"]["frameTime"].ToObject<Single>()));
            bin.Add((byte)Array.IndexOf(eventType, eventdata["event"]["type"].ToString()));
            bin.Add((byte)eventdata["event"]["param"].Count());
            foreach (var param in eventdata["event"]["param"])
            {
                bin.AddRange(BitConverter.GetBytes(param.ToObject<Int32>()));
            }


        }
        return bin.ToArray();
    }

}

