using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace StoryCanvas.iOS
{
	// http://spiratesta.hatenablog.com/entry/2014/05/08/175006
	static class MemoryChecker
	{
		public const int TASK_BASIC_INFO = 4;

		public struct TimeValue
		{
			public int Seconds;
			public int Microseconds;
		}

		public struct TaskBasicInfo
		{
			public int SuspendCount;
			public uint VirtualSize;
			public uint ResidentSize;
			public TimeValue UserTime;
			public TimeValue SystemTime;
			public int Policy;
		}

		[DllImport("/usr/lib/system/libsystem_kernel.dylib", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint mach_task_self();

		[DllImport("/usr/lib/system/libsystem_kernel.dylib", CallingConvention = CallingConvention.Cdecl)]
		public static extern int task_info(uint targetTaskID, int flavor, ref TaskBasicInfo taskInfo, ref int size);

		public static TaskBasicInfo GetTaskInfo()
		{
			TaskBasicInfo taskInfo = new TaskBasicInfo();
			uint taskid = mach_task_self();
			int size = Marshal.SizeOf(typeof(TaskBasicInfo));
			task_info(taskid, TASK_BASIC_INFO, ref taskInfo, ref size);
			return taskInfo;
		}
	}
}