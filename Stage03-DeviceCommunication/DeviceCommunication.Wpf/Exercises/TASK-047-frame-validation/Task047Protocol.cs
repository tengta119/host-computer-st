namespace DeviceCommunication.Wpf.Exercises.Task047
{
    // 这是教练为本题新建的普通类，只回答一份六字节数据是否符合TASK-047教学协议，不操作串口或窗口。
    public class Task047Protocol
    {
        public bool IsValidFrame(byte[] frame)
        {
            if (frame == null || frame.Length != 6)
            {
                return false;
            }
            // TOD 1：检查frame[0]是AA、frame[1]是01或02、frame[2]是03，两字节原始值在0至1000内，并且前五字节加和除以256的余数等于frame[5]，全部符合才返回true。
            // 任意一项不符合就返回false，先用int保存加和与原始值，具体字节和取余例子见RUN-GUIDE.md。
            if (frame[0] == 0xAA)
            {
                int rawValue = frame[3] * 256 + frame[4];
                if (rawValue > 1000)
                {
                    return false;
                }
                
                
                if ((frame[1] == 0x01 || frame[1] == 0x02) && frame[2] == 0x03)
                {
                    if ((frame[0] + frame[1] + frame[2] + frame[3] + frame[4]) % 256 == frame[5])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
