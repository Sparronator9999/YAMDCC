namespace YAMDCC.GUI
{
    internal class Status
    {
        internal StatusCode Code;
        internal int RepeatCount;

        internal Status()
        {
            Code = StatusCode.None;
            RepeatCount = 0;
        }

        internal Status(StatusCode code, int repeatCount)
        {
            Code = code;
            RepeatCount = repeatCount;
        }
    }

    internal enum StatusCode
    {
        None = 0,
        ServiceCommandFail,
        ServiceResponseEmpty,
        ConfLoading,
        NoConfig,
        ConfApplySuccess,
        FullBlastToggleSuccess,
    }
}
