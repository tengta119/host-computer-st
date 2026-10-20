namespace DeviceCommunication.Wpf.Exercises.Task048
{
    // 教练把TASK-047已完成的条件整理成一条检查路径，同时返回具体拒绝原因，不改变六字节协议。
    public class Task048Protocol
    {
        public bool IsValidFrame(byte[] frame)
        {
            return GetRejectionReason(frame) == "";
        }

        public string GetRejectionReason(byte[] frame)
        {
            if (frame == null || frame.Length != 6)
            {
                return "候选长度不是6字节。";
            }
            if (frame[0] != 0xAA)
            {
                return "候选开头不是AA。";
            }
            int rawValue = frame[3] * 256 + frame[4];
            if (rawValue > 1000)
            {
                return "原始值" + rawValue + "超过1000。";
            }
            if (frame[1] != 0x01 && frame[1] != 0x02)
            {
                return "设备编号" + frame[1].ToString("X2") + "不是01或02。";
            }
            if (frame[2] != 0x03)
            {
                return "命令" + frame[2].ToString("X2") + "不是03。";
            }
            int expected = (frame[0] + frame[1] + frame[2] + frame[3] + frame[4]) % 256;
            if (expected != frame[5])
            {
                return "校验不符，按前五字节应为" + expected.ToString("X2") + "，收到" + frame[5].ToString("X2") + "。";
            }
            return "";
        }
    }
}
